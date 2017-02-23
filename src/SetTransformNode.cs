#region usings
using System;
using System.ComponentModel.Composition;

using VVVV.PluginInterfaces.V1;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VColor;
using VVVV.Utils.VMath;

using VVVV.Core.Logging;
using System.Linq;
#endregion usings

namespace VVVV.Nodes.MultiTouchStack
{	
	#region PluginInfo
	[PluginInfo(Name = "SetTransform", Category = "MultiTouchStack", Author = "elliotwoods", AutoEvaluate = true)]
	#endregion PluginInfo
	public class SetTransformNode : IPluginEvaluate
	{
		#region fields & pins
		[Input("Input", AutoValidate = false)]
		public ISpread<Slide> FInSlides;
		
		[Input("Transform", AutoValidate = false)]
		public ISpread<Matrix4x4> FInTransform;
		
		[Input("Set", IsBang=true)]
		public ISpread<bool> FInSet;
		
		[Import()]
		public ILogger FLogger;
		#endregion fields & pins

		//called when data for any output pin is requested
		public void Evaluate(int SpreadMax)
		{
			//check if set is called at all (if not, quit early)
			if (!FInSet.Any(value => value == true))
			{
				return;
			}

			//update inputs
			{
				FInSlides.Sync();
				FInTransform.Sync();
				SpreadMax = Utils.SpreadMax(FInSlides, FInTransform, FInSet);
			}

			for (int i=0; i<SpreadMax; i++)
			{
				if(FInSet[i])
				{
					var slide = FInSlides[i];
					if(slide != null)
					{
						slide.Transform = FInTransform[i];
					}
				}
			}
		}
	}
}
