using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVVV.Utils.VMath;

namespace VVVV.Nodes.MultiTouchStack.Behaviors
{
	public class Rotate : Behavior
	{
		public override bool Perform(PerformArguments performArguments, Matrix4x4 oldTransform, ref Matrix4x4 newTransform)
		{
			var cursorCount = performArguments.Cursors.Count();

			if(cursorCount < 2)
			{
				return false;
			}

			performArguments.ActionsApplied.Add(ActionsApplied.Rotate);
			performArguments.ActionsApplied.Add(ActionsApplied.Translate);

			// We split the cursors in half, the half with the largest movement are the rotation, the half with the lowest movement are the pivot
			var cursorsSortedByMovement = performArguments.Cursors.OrderBy(cursor => cursor.Movement.LengthSquared).ToList();

			var cursorCountForPivot = (int) Math.Ceiling((double) cursorCount / 2);
			var cursorsForPivot = cursorsSortedByMovement.GetRange(0, cursorCountForPivot);
			var cursorsForRotation = cursorsSortedByMovement.GetRange(cursorCountForPivot, cursorCount - cursorCountForPivot);

			var pivot = Utils.MeanPosition(cursorsForPivot);

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

			newTransform = oldTransform
				* VMath.Translate(-pivot.x, -pivot.y, 0.0)
				* VMath.RotateZ(averageRotation)
				* VMath.Translate(pivot.x, pivot.y, 0.0);
			
			if(cursorsForPivot.Count > 0)
			{
				var totalTranslation = new Vector2D(0.0, 0.0);
				foreach (var cursor in cursorsForPivot)
				{
					totalTranslation += cursor.Movement;
				}
				var averageTranslation = totalTranslation / cursorsForPivot.Count();

				newTransform = newTransform * VMath.Translate(new Vector3D(averageTranslation));
			}

			return true;
		}
	}
}
