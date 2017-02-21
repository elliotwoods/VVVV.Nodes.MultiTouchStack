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
	[PluginInfo(Name = "HitResult", Category = "MultiTouchStack", Author = "elliotwoods")]
	#endregion PluginInfo
	public class HitResultNode : IPluginEvaluate
	{
		#region fields & pins
		[Input("Slides")]
		public ISpread<Slide> FInSlides;
		
		[Output("Hit Rest Results")]
		public ISpread<ISpread<string>> FOutHitTestResults;

		[Import()]
		public ILogger FLogger;
		#endregion fields & pins

		//called when data for any output pin is requested
		public void Evaluate(int SpreadMax)
		{
			
		}
	}
}
