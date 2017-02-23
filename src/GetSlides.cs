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
	[PluginInfo(Name = "GetSlides", Category = "MultiTouchStack", Author = "elliotwoods")]
	#endregion PluginInfo
	public class GetSlidesNode : IPluginEvaluate
	{
		#region fields & pins
		[Input("World", IsSingle = true)]
		public Pin<World> FInWorld;

		[Output("Slides")]
		public ISpread<Slide> FOutSlides;
		
		[Import()]
		public ILogger FLogger;
		#endregion fields & pins

		//called when data for any output pin is requested
		public void Evaluate(int SpreadMax)
		{
			if (!this.FInWorld.PluginIO.IsConnected)
			{
				FOutSlides.SliceCount = 0;
				return;
			}
			else
			{
				FOutSlides.AssignFrom(FInWorld[0].Slides);
			}
		}
	}
}
