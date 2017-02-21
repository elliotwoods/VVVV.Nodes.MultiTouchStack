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
	[PluginInfo(Name = "AddSlide", Category = "MultiTouchStack", Version = "Transform", Author = "elliotwoods", AutoEvaluate=true)]
	#endregion PluginInfo
	public class AddSlideNode : IPluginEvaluate
	{
		#region fields & pins
		[Input("World", IsSingle=true)]
		public ISpread<World> FInWorld;
		
		[Input("Position")]
		public ISpread<Vector2D> FInPosition;
		
		[Input("Rotation")]
		public ISpread<Double> FInRotation;
		
		[Input("Scale", DefaultValues = new double[] { 1.0, 1.0 })]
		public ISpread<Vector2D> FInScale;
		
		[Input("Tags")]
		public ISpread<ISpread<String>> FInTags;
		
		[Input("Minimum Scale", DefaultValue = 0.1)]
		public ISpread<Double> FInMinimumScale;
		
		[Input("Maximum Scale", DefaultValue = 100.0)]
		public ISpread<Double> FInMaximumScale;
		
		[Input("Drag Hit Test")]
		public ISpread<IHitTestFunction> FInDragHitTestFunction;
		
		[Input("Hit Events")]
		public ISpread<ISpread<HitEvent>> FInHitEvents;
		
		[Input("Add", IsBang=true)]
		public ISpread<bool> FInAdd;
		
		[Output("Success", IsBang = true)]
		public ISpread<bool> FOutSuccess;

		[Import()]
		public ILogger FLogger;
		#endregion fields & pins

		//called when data for any output pin is requested
		public void Evaluate(int SpreadMax)
		{
			var world = FInWorld[0];
			if(world == null)
			{
				return;
			}

			for(int i=0; i<SpreadMax; i++)
			{
				if (FInAdd[i])
				{
					//construct the new slide
					var newSlide = new Slide
					{
						Index = world.NextAvailableIndex,
						Transform = VMath.Scale(FInScale[i].x, FInScale[i].y, 1.0)
							* VMath.RotateZ(FInRotation[i])
							* VMath.Translate(FInPosition[i].x, FInPosition[i].y, 0.0),
						Tags = new List<String>(FInTags[i]),
						MinimumScale = FInMinimumScale[i],
						MaximumScale = FInMaximumScale[i],
						DragHitTestFunction = FInDragHitTestFunction[i],
						HitEvents = new List<HitEvent>(FInHitEvents[i]),
						AttachedCursors = new List<Cursor>()
					};

					//clear out any null HitEvents (e.g. if spread was empty)
					newSlide.HitEvents.RemoveAll(hitEvent => hitEvent == null);
					
					//add the new slide to the world
					world.Slides.Add(newSlide);
				}
			}
		}
	}
}
