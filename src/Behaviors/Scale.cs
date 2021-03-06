﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVVV.Utils.VMath;

namespace VVVV.Nodes.MultiTouchStack.Behaviors
{
	public class Scale : Behavior
	{
		public override bool Perform(PerformArguments performArguments, Matrix4x4 oldTransform, ref Matrix4x4 newTransform)
		{
			var cursorCount = performArguments.Cursors.Count();

			if (cursorCount < 2)
			{
				// Cannot perform Behaviour
				return false;
			}

			performArguments.ActionsApplied.Add(ActionsApplied.Translate);
			performArguments.ActionsApplied.Add(ActionsApplied.Scale);

			// We split the cursors in half, the half with the largest movement are the rotation, the half with the lowest movement are the pivot
			var cursorsSortedByMovement = performArguments.Cursors.OrderBy(cursor => cursor.Movement.LengthSquared).ToList();

			var cursorCountForPivot = cursorCount / 2;
			var cursorsForPivot = cursorsSortedByMovement.GetRange(0, cursorCountForPivot);
			var cursorsForAction = cursorsSortedByMovement.GetRange(cursorCountForPivot, cursorCount - cursorCountForPivot);

			var pivot = Utils.MeanPosition(cursorsForPivot);

			// Take the total rotation
			double totalScale = 1.0;
			foreach (var cursor in cursorsForAction)
			{
				var currentCursorPositionVSCOM = cursor.Position - pivot;
				var previousCursorPositionVSCOM = currentCursorPositionVSCOM - cursor.Movement;
				var scaleFromCursor = currentCursorPositionVSCOM.Length / previousCursorPositionVSCOM.Length;
				totalScale *= scaleFromCursor;
			}

			var averageScale = Math.Pow(totalScale, 1 / cursorsForAction.Count());

			newTransform = oldTransform
				* VMath.Translate(-pivot.x, -pivot.y, 0.0)
				* VMath.Scale(totalScale, totalScale, 1.0)
				* VMath.Translate(pivot.x, pivot.y, 0.0);

			return true;
		}
	}
}
