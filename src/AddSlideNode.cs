#region usings
using System;
using System.ComponentModel.Composition;

using VVVV.PluginInterfaces.V1;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VColor;
using VVVV.Utils.VMath;

using VVVV.Core.Logging;
using System.Collections.Generic;
using System.Linq;
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
		public Pin<World> FInWorld;
		
		[Input("Position", AutoValidate = false)]
		public ISpread<Vector2D> FInPosition;
		
		[Input("Rotation", AutoValidate = false)]
		public ISpread<Double> FInRotation;
		
		[Input("Scale", AutoValidate = false, DefaultValues = new double[] { 1.0, 1.0 })]
		public ISpread<Vector2D> FInScale;
		
		[Input("Tags", AutoValidate = false)]
		public ISpread<ISpread<String>> FInTags;
		
		[Input("Behavior", AutoValidate = false)]
		public ISpread<Behaviors.IBehavior> FInBehavior;

		[Input("Constraint", AutoValidate = false)]
		public ISpread<Constraints.IConstraint> FInConstraint;

		[Input("Maximum Scale", AutoValidate = false, DefaultValue = 100.0)]
		public ISpread<Double> FInMaximumScale;
		
		[Input("Drag Hit Test", AutoValidate = false)]
		public ISpread<IHitTestFunction> FInDragHitTestFunction;
		
		[Input("Hit Events", AutoValidate = false)]
		public ISpread<ISpread<HitEvent>> FInHitEvents;
		
		[Input("Add", IsBang=true)]
		public ISpread<bool> FInAdd;
		
		[Import()]
		public ILogger FLogger;
		#endregion fields & pins

		//called when data for any output pin is requested
		public void Evaluate(int SpreadMax)
		{
			if(!FInWorld.PluginIO.IsConnected)
			{
				return;
			}

			//check if add is called at all (if not, quit early)
			if(!FInAdd.Any(add => add == true))
			{
				return;
			}

			//evaluate the inputs
			{
				FInPosition.Sync();
				FInRotation.Sync();
				FInScale.Sync();
				FInTags.Sync();
				FInBehavior.Sync();
				FInConstraint.Sync();
				FInDragHitTestFunction.Sync();
				FInHitEvents.Sync();

				SpreadMax = Utils.SpreadMax(FInPosition
					, FInRotation
					, FInScale
					, FInTags
					, FInBehavior
					, FInConstraint
					, FInDragHitTestFunction
					, FInHitEvents
					, FInAdd);
			}

			var world = FInWorld[0];

			for(int i=0; i<SpreadMax; i++)
			{
				if (FInAdd[i])
				{
					//construct the new slide
					var newSlide = new Slide(world)
					{
						Index = world.NextAvailableIndex,
						Transform = VMath.Scale(FInScale[i].x, FInScale[i].y, 1.0)
							* VMath.RotateZ(FInRotation[i])
							* VMath.Translate(FInPosition[i].x, FInPosition[i].y, 0.0),
						Tags = new List<String>(FInTags[i]),
						Behaviour = FInBehavior[i],
						Constraint = FInConstraint[i],
						DragHitTestFunction = FInDragHitTestFunction[i],
						HitEvents = new List<HitEvent>(FInHitEvents[i])
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
