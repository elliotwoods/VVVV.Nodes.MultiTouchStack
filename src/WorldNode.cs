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
		public ISpread<Settings> FInSettings;

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
				this.FWorld = new World();
			}
			//
			//--



			//--
			//Clear frame state
			//--
			//
			this.FWorld.HitEventsFiredThisFrame.Clear();
			this.FWorld.CursorsAddedThisFrame.Clear();
			this.FWorld.CursorsLostThisFrame.Clear();
			//
			//--
			
			
			
			//--
			//Cursor input
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
						var cursor = new Cursor
						{
							Index = cursorIndex,
							Position = cursorPosition,
							Movement = new Vector2D()
						};

						cursorsForNextFrame.Add(cursorIndex, cursor);
						this.FWorld.CursorsAddedThisFrame.Add(cursor);

						//check hits (in reverse z-order)
						for(int iSlide = FWorld.Slides.Count - 1; iSlide >= 0; iSlide--)
						{
							var slide = this.FWorld.Slides[iSlide];

							//get cursor in local normalised slide coords
							Vector2D cursorInSlide;
							{
								var cursorInSlide3 = VMath.Inverse(slide.Transform) * cursorPosition;
								cursorInSlide = new Vector2D(cursorInSlide3.x, cursorInSlide3.y);
							}

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

									if(hitEvent.CursorAction == HitEvent.CursorActionType.Down)
									{
										this.FWorld.HitEventsFiredThisFrame.Add(hitEvent);
									}
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
								//move to back of stack
								this.FWorld.Slides.Remove(slide);
								this.FWorld.Slides.Add(slide);

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
							this.FWorld.CursorsLostThisFrame.Add(cursorFromPreviousFrame.Value);
						}
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
				foreach(var slideToRemoveThisFrame in FWorld.SlidesToRemoveThisFrame)
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
