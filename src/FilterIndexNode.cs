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
	[PluginInfo(Name = "Filter", Category = "MultiTouchStack", Version = "Index", Author = "elliotwoods")]
	#endregion PluginInfo
	public class FilterIndexNode : IPluginEvaluate
	{
		#region fields & pins
		[Input("Input")]
		public ISpread<Slide> FInSlides;
		
		[Input("Index")]
		public ISpread<int> FInIndex;

		[Output("Output")]
		public ISpread<Slide> FOutSlides;

		[Import()]
		public ILogger FLogger;
		#endregion fields & pins

		//called when data for any output pin is requested
		public void Evaluate(int SpreadMax)
		{
			FOutSlides.SliceCount = 0;

			var indexList = new List<int>(FInIndex);

			foreach(var slide in FInSlides)
			{
				if (slide != null)
				{
					if (indexList.Contains(slide.Index))
					{
						FOutSlides.Add(slide);
					}
				}
			}
		}
	}
}
