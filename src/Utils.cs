using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVVV.PluginInterfaces.V2.NonGeneric;

namespace VVVV.Nodes.MultiTouchStack
{
	class Utils
	{
		public static int SpreadMax(params ISpread[] pins)
		{
			int maxSliceCount = 0;
			foreach(var pin in pins)
			{
				if(pin.SliceCount == 0)
				{
					return 0;
				}
				if(pin.SliceCount > maxSliceCount)
				{
					maxSliceCount = pin.SliceCount;
				}
			}
			return maxSliceCount;
		}
	}
}
