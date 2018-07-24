using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVVV.PluginInterfaces.V2.NonGeneric;
using VVVV.Utils.VMath;

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

		public static Vector2D MeanPosition(IEnumerable<Cursor> cursors)
		{
			var totalPosition = new Vector2D(0.0, 0.0);
			foreach (var cursor in cursors)
			{
				totalPosition += cursor.Position;
			}
			return totalPosition / cursors.Count();
		}
	}
}
