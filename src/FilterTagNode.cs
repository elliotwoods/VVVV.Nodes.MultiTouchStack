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
	public enum KeyFilter {
		ContainsAll,
		ContainsAny,
		Matches,
		DoesntContainAny
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
		public ISpread<String> FInKeys;
		
		[Input("Filter", IsSingle = true)]
		public ISpread<KeyFilter> FInKeyFilter;

		[Output("Output")]
		public ISpread<Slide> FOutSlides;

		[Import()]
		public ILogger FLogger;
		#endregion fields & pins

		bool Filter(Slide slide)
		{
			switch (FInKeyFilter[0])
			{
				case KeyFilter.ContainsAny:
					{
						foreach (var key in this.FInKeys)
						{
							if(slide.Tags.Contains(key))
							{
								return true;
							}
						}
						return false;
					}
				case KeyFilter.ContainsAll:
					{
						foreach (var key in this.FInKeys)
						{
							if (!slide.Tags.Contains(key))
							{
								return false;
							}
						}
						return true;
					}
				case KeyFilter.Matches:
					{
						return Enumerable.SequenceEqual(slide.Tags.OrderBy(tag => tag)
							, this.FInKeys.OrderBy(tag => tag));
					}
				case KeyFilter.DoesntContainAny:
					{
						foreach (var key in this.FInKeys)
						{
							if (slide.Tags.Contains(key))
							{
								return false;
							}
						}
						return true;
					}
				default:
					return false;
			}
		}

		//called when data for any output pin is requested
		public void Evaluate(int SpreadMax)
		{
			FOutSlides.SliceCount = 0;
			foreach(var slide in FInSlides)
			{
				if(slide != null)
				{
					if (Filter(slide))
					{
						FOutSlides.Add(slide);
					}
				}
			}
		}
	}
}
