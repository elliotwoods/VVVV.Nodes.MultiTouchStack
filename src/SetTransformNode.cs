#region usings
using System;
using System.ComponentModel.Composition;

using VVVV.PluginInterfaces.V1;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VColor;
using VVVV.Utils.VMath;

using VVVV.Core.Logging;
#endregion usings

namespace VVVV.Nodes.MultiTouchStack
{	
	#region PluginInfo
	[PluginInfo(Name = "SetTransform", Category = "MultiTouchStack", Author = "elliotwoods", AutoEvaluate = true)]
	#endregion PluginInfo
	public class SetTransformNode : IPluginEvaluate
	{
		#region fields & pins
		[Input("Input")]
		public ISpread<Slide> FInSlides;
		
		[Input("Transform")]
		public ISpread<Matrix4x4> FInTransform;
		
		[Input("Set", IsBang=true)]
		public ISpread<bool> FInSet;
		
		[Output("Success")]
		public ISpread<bool> FOutSuccess;

		[Import()]
		public ILogger FLogger;
		#endregion fields & pins

		//called when data for any output pin is requested
		public void Evaluate(int SpreadMax)
		{
			
		}
	}
}
