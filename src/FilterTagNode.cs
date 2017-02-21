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
	public enum StringFilter {
		Matches,
		Contains,
		MatchesAll,
		ContainsAll
	}
	
	#region PluginInfo
	[PluginInfo(Name = "Filter", Category = "MultiTouchStack", Version = "Tag", Author = "elliotwoods")]
	#endregion PluginInfo
	public class FilterTagNode : IPluginEvaluate
	{
		#region fields & pins
		[Input("Input")]
		public ISpread<Slide> FInSlides;
		
		[Input("Key")]
		public ISpread<String> FInKey;
		
		[Input("Comparison")]
		public ISpread<StringFilter> FInComparison;

		[Output("Output")]
		public ISpread<Slide> FOutSlides;

		[Import()]
		public ILogger FLogger;
		#endregion fields & pins

		//called when data for any output pin is requested
		public void Evaluate(int SpreadMax)
		{
			
		}
	}
}
