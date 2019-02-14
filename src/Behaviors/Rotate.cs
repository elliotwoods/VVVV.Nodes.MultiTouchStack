using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVVV.Utils.VMath;

namespace VVVV.Nodes.MultiTouchStack.Behaviors
{
	public class Rotate : IBehavior
	{
		public override Matrix4x4 Perform(Matrix4x4 transform, PerformArguments performArguments)
		{
			var cursorCount = performArguments.Cursors.Count();

			// We split the cursors in half, the half with the largest movement are the rotation, the half with the lowest movement are the pivot
			var cursorsSortedByMovement = performArguments.Cursors.OrderBy(cursor => cursor.Movement.LengthSquared).ToList();

			var cursorCountForPivot = (int) Math.Ceiling((double) cursorCount / 2);
			var cursorsForPivot = cursorsSortedByMovement.GetRange(0, cursorCountForPivot);
			var cursorsForRotation = cursorsSortedByMovement.GetRange(cursorCountForPivot, cursorCount - cursorCountForPivot);

			var pivot = Utils.MeanPosition(cursorsForPivot);

			var newTransform = transform;

			if (cursorsForRotation.Count > 0)
			{
				// Take the total rotation
				double totalRotation = 0.0;
				foreach (var cursor in cursorsForRotation)
				{
					var currentCursorPositionVSCOM = cursor.Position - pivot;
					var previousCursorPositionVSCOM = currentCursorPositionVSCOM - cursor.Movement;
					var rotationFromCursor = Math.Atan2(currentCursorPositionVSCOM.y, currentCursorPositionVSCOM.x)
						- Math.Atan2(previousCursorPositionVSCOM.y, previousCursorPositionVSCOM.x);
					totalRotation += rotationFromCursor;
				}

				var averageRotation = totalRotation / cursorsForRotation.Count();

				newTransform = newTransform
					* VMath.Translate(-pivot.x, -pivot.y, 0.0)
					* VMath.RotateZ(averageRotation)
					* VMath.Translate(pivot.x, pivot.y, 0.0);
			}
			
			if(cursorsForPivot.Count > 0)
			{
				// Perform translation with the movement of the pivot points
				newTransform = TranslateNode.PrincipalBehavior.Perform(newTransform, new PerformArguments
				{
					Cursors = cursorsForPivot
					// We don't pass the Validate function since we dont need to test inside
				});
			}

			performArguments.ActionsApplied.Add(ActionsApplied.Translate);
			performArguments.ActionsApplied.Add(ActionsApplied.Rotate);

			return newTransform;
		}
	}
}
