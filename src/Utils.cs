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

		/// <summary>
		/// Perform a multitouch transform to an existing transform using a set of cursors
		/// </summary>
		/// <param name="transform">Transform to be affected</param>
		/// <param name="cursors">List of cursors which may have moved</param>
		/// <returns>True if transform was changed</returns>
		public static bool MultitouchTransform(ref Matrix4x4 transform, List<Cursor> cursors, double minimumScale, double maximumScale)
		{
			if (cursors.Count == 1)
			{
				var cursor0 = cursors[0];
				transform = transform * VMath.Translate(new Vector3D(cursor0.Movement));
				return true;
			}
			else if (cursors.Count >= 2)
			{
				//choose the cursors with the most movement
				var cursorsSortedByMovement = cursors.OrderByDescending(cursor => cursor.Movement.LengthSquared).ToList();

				var cursor0Now = cursorsSortedByMovement[0].Position;
				var cursor1Now = cursorsSortedByMovement[1].Position;
				var cursor0Previous = cursorsSortedByMovement[0].Position - cursorsSortedByMovement[0].Movement;
				var cursor1Previous = cursorsSortedByMovement[1].Position - cursorsSortedByMovement[1].Movement;

				double num1 = (double)cursor0Previous.x;
				double num2 = (double)cursor0Previous.y;
				double num3 = (double)cursor1Previous.x;
				double num4 = (double)cursor1Previous.y;
				double num5 = (double)cursor0Now.x;
				double num6 = (double)cursor0Now.y;
				double num7 = (double)cursor1Now.x;
				double num8 = (double)cursor1Now.y;
				double num9 = num3 - num1;
				double num10 = num4 - num2;
				double num11 = num7 - num5;
				double num12 = num8 - num6;
				Math.Sqrt((num11 * num11 + num12 * num12) / (num9 * num9 + num10 * num10));
				double num13 = (num9 * num11 + num10 * num12) / (num9 * num9 + num10 * num10);
				double num14 = (num10 * num11 - num9 * num12) / (num9 * num9 + num10 * num10);
				double num15 = num5 - (num13 * num1 + num14 * num2);
				double num16 = num6 - (-num14 * num1 + num13 * num2);

				var newTransform = transform * new Matrix4x4(num13, -num14, 0.0, 0.0, num14, num13, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, num15, num16, 0.0, 1.0);

				//check if newTransform doesn't break the scale rules before applying it
				{
					var scale = Math.Sqrt(newTransform[0, 0] * newTransform[0, 0] + newTransform[0, 1] * newTransform[0, 1])
						+ Math.Sqrt(newTransform[1, 0] * newTransform[1, 0] + newTransform[1, 1] * newTransform[1, 1]);
					scale /= 2.0f;

					if (scale > maximumScale)
					{
						var scaleCorrection = maximumScale / scale;
						transform = VMath.Scale(scaleCorrection, scaleCorrection, scaleCorrection) * transform;
					} else if(scale < minimumScale)
					{
						var scaleCorrection = minimumScale / scale;
						transform = VMath.Scale(scaleCorrection, scaleCorrection, scaleCorrection) * transform;
					} else
					{
						transform = newTransform;
					}
				}

				return true;
			}

			return false;
		}
	}
}
