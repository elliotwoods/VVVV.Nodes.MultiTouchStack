using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVVV.Utils.VMath;

namespace VVVV.Nodes.MultiTouchStack.Behaviors
{
	public class FullMultitouch : Behavior
	{
		public override bool Perform(PerformArguments performArguments, Matrix4x4 oldTransform, ref Matrix4x4 newTransform)
		{
			var cursorCount = performArguments.Cursors.Count();

			if (cursorCount < 1)
			{
				// We should never arrive here
				return false;
			}
			if (cursorCount < 2)
			{
				// revert to translate only
				return TranslateNode.PrincipalBehavior.Perform(performArguments, oldTransform, ref newTransform);
			}

			performArguments.ActionsApplied.Add(ActionsApplied.Translate);
			performArguments.ActionsApplied.Add(ActionsApplied.Rotate);
			performArguments.ActionsApplied.Add(ActionsApplied.Scale);

			//choose the cursors with the most movement
			var cursorsSortedByMovement = performArguments.Cursors.OrderByDescending(cursor => cursor.Movement.LengthSquared).ToList();

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

			newTransform = oldTransform * new Matrix4x4(num13, -num14, 0.0, 0.0, num14, num13, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, num15, num16, 0.0, 1.0);

			return true;
		}
	}
}
