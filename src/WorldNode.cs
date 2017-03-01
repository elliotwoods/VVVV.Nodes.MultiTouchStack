#region usings
using System;
using System.ComponentModel.Composition;

using VVVV.PluginInterfaces.V1;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VColor;
using VVVV.Utils.VMath;

using VVVV.Core.Logging;
using System.Collections.Generic;
#endregion usings

namespace VVVV.Nodes.MultiTouchStack
{
	#region PluginInfo
	[PluginInfo(Name = "World", Category = "MultiTouchStack", Help = "The world of slides in the MultiTouch Stack", Tags = "", Author = "elliotwoods", AutoEvaluate = true)]
	#endregion PluginInfo
	public class WorldNode : IPluginEvaluate, IDisposable
	{
		#region fields & pins
		[Input("Cursor Index")]
		public ISpread<int> FInCursorIndex;

		[Input("Cursor Position")]
		public ISpread<Vector2D> FInCursorPosition;
		
		[Input("Settings")]
		public IDiffSpread<Settings> FInSettings;

		[Input("Reset", IsBang = true)]
		public ISpread<bool> FInReset;

		[Output("World")]
		public ISpread<World> FOutWorld;
		
		[Import()]
		public ILogger FLogger;
		#endregion fields & pins

		World FWorld = new World();

		//called when data for any output pin is requested
		public void Evaluate(int SpreadMax)
		{
			//--
			//Reset
			//--
			//
			if(FInReset[0])
			{
				FWorld = new World();
				if(FInSettings[0] != null)
				{
					FWorld.Settings = FInSettings[0];
				}
			}
			//
			//--



			//--
			//Settings
			//--
			//
			if(FInSettings.IsChanged)
			{
				var settings = FInSettings[0];
				if(settings != null)
				{
					FWorld.Settings = settings;
				}
			}
			//
			//--



			//--
			//Clear frame state
			//--
			//
			FWorld.CursorsAddedThisFrame.Clear();
			FWorld.CursorsLostThisFrame.Clear();
			
			foreach(var slide in FWorld.Slides)
			{
				slide.HitCallbacksThisFrame.Clear();
			}
			//
			//--
			
			
			
			//--
			//Handle cursors added and lost
			//--
			//
			{
				int cursorCount = Math.Max(FInCursorIndex.SliceCount
					, FInCursorPosition.SliceCount);
				if(FInCursorIndex.SliceCount == 0
					|| FInCursorPosition.SliceCount == 0)
				{
					cursorCount = 0;
				}

				var cursorsFromPreviousFrame = FWorld.Cursors;
				var cursorsForNextFrame = new Dictionary<int, Cursor>();

				//for each cursor input
				for (int i=0; i<cursorCount; i++)
				{
					int cursorIndex = FInCursorIndex[i];
					Vector2D cursorPosition = FInCursorPosition[i];

					if(this.FWorld.Cursors.ContainsKey(cursorIndex))
					{
						//pre-existing cursor
						var cursor = this.FWorld.Cursors[cursorIndex];
						cursor.Movement = cursorPosition - cursor.Position;
						cursor.Position = cursorPosition;

						cursorsForNextFrame.Add(cursorIndex, cursor);
					} else
					{
						//new cursor
						var cursor = new Cursor(FWorld)
						{
							Index = cursorIndex,
							Position = cursorPosition,
							Movement = new Vector2D()
						};

						cursorsForNextFrame.Add(cursorIndex, cursor);
						FWorld.CursorsAddedThisFrame.Add(cursor);

						//check hits (in reverse z-order)
						for(int iSlide = FWorld.Slides.Count - 1; iSlide >= 0; iSlide--)
						{
							var slide = FWorld.Slides[iSlide];

							//get cursor in local normalised slide coords
							var cursorInSlide = slide.CanvasToSlide(cursorPosition);

							bool thisSlideHasHit = false;

							//check event hits
							for (int iHitEvent = slide.HitEvents.Count - 1; iHitEvent >= 0; iHitEvent--)
							{
								var hitEvent = slide.HitEvents[iHitEvent];
								if (hitEvent.HitTestFunction.TestHit(cursorInSlide))
								{
									//event was hit
									hitEvent.AttachedCursors.Add(cursor);
									cursor.AssignedSlide = slide;
									cursor.AssignedHitEvent = hitEvent;

									thisSlideHasHit = true;
									break;
								}
							}

							//check drag hit (if no event was hit)
							if (!thisSlideHasHit)
							{
								if(slide.DragHitTest(cursorInSlide))
								{
									//drag area was hit
									slide.AttachedCursors.Add(cursor);
									cursor.AssignedSlide = slide;
									thisSlideHasHit = true;
								}
							}

							if (thisSlideHasHit)
							{
								if(FWorld.Settings.TapBringsToFront)
								{
									FWorld.Slides.Remove(slide);
									FWorld.Slides.Add(slide);
								}

								//break out of loop
								break;
							}
						}
					}
				}

				this.FWorld.Cursors = cursorsForNextFrame;

				//check for any cursors lost
				{
					foreach(var cursorFromPreviousFrame in cursorsFromPreviousFrame)
					{
						if(!cursorsForNextFrame.ContainsKey(cursorFromPreviousFrame.Key))
						{
							FWorld.CursorsLostThisFrame.Add(cursorFromPreviousFrame.Value);
						}
					}
				}
			}
			//
			//--



			//--
			//Handle events
			//--
			//
			{
				//Cursor down
				foreach(var cursor in FWorld.CursorsAddedThisFrame)
				{
					var hitEvent = cursor.AssignedHitEvent;
					if(hitEvent != null)
					{
						var hitCallback = new HitCallback
						{
							Event = hitEvent,
							Cursor = cursor,
							ActionType = HitCallback.CursorActionType.Down
						};
						cursor.AssignedSlide.HitCallbacksThisFrame.Add(hitCallback);
					}
				}

				//Cursor release
				foreach (var cursor in FWorld.CursorsLostThisFrame)
				{
					var hitEvent = cursor.AssignedHitEvent;
					var slide = cursor.AssignedSlide;

					if (hitEvent != null)
					{
						bool isHit = hitEvent.HitTestFunction.TestHit(slide.CanvasToSlide(cursor.Position));

						var hitCallback = new HitCallback
						{
							Event = hitEvent,
							Cursor = cursor,
							ActionType = isHit ? HitCallback.CursorActionType.ReleaseInside : HitCallback.CursorActionType.ReleaseOutside
						};
						cursor.AssignedSlide.HitCallbacksThisFrame.Add(hitCallback);
					}
				}
			}
			//
			//--



			//--
			//Handle dead cursors, slides, events
			//--
			//

			//dead slides
			{
				foreach (var slideToRemoveThisFrame in FWorld.SlidesToRemoveThisFrame)
				{
					FWorld.Slides.Remove(slideToRemoveThisFrame);
				}
			}

			//clean out dead weak references
			{
				foreach(var slide in FWorld.Slides)
				{
					slide.AttachedCursors.RemoveAll(Cursor => this.FWorld.CursorsLostThisFrame.Contains(Cursor));
					
					foreach(var hitEvent in slide.HitEvents)
					{
						hitEvent.AttachedCursors.RemoveAll(Cursor => this.FWorld.CursorsLostThisFrame.Contains(Cursor));
					}
				}
			}
			//
			//--



			//--
			//Update function
			//--
			//
			foreach(var slide in FWorld.Slides)
			{
				//apply motion from attached cursors
				slide.Update();
			}
			//
			//--



			//--
			//Output
			//--
			//
			this.FOutWorld[0] = FWorld;
			//
			//--
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					this.FWorld.Dispose();
				}

				disposedValue = true;
			}
		}

		public void Dispose()
		{
			Dispose(true);
		}
		#endregion
	}
}
