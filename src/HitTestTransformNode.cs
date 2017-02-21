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
	[PluginInfo(Name = "HitTest", Category = "MultiTouchStack", Tags = "delegate", Version = "Transform", Author = "elliotwoods")]
	#endregion PluginInfo
	public class HitTestTransformNode : IPluginEvaluate
	{
		#region fields & pins
		[Input("Transform")]
		public ISpread<Matrix4x4> FInTransform;
		
		[Output("Output")]
		public ISpread<IHitTestFunction> FOutput;

		[Import()]
		public ILogger FLogger;
		#endregion fields & pins

		//called when data for any output pin is requested
		public void Evaluate(int SpreadMax)
		{
			
		}
	}
}
