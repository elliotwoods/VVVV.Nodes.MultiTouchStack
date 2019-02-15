using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVVV.Utils.VMath;

namespace VVVV.Nodes.MultiTouchStack.Behaviors
{
	public class TranslateY : Behavior
	{
		public override bool Perform(PerformArguments performArguments, Matrix4x4 oldTransform, ref Matrix4x4 newTransform)
		{
			if (performArguments.Cursors.Count() < 1)
			{
				// Cannot perform Behaviour
				return false;
			}

			performArguments.ActionsApplied.Add(ActionsApplied.Translate);

			var totalTranslation = new Vector2D(0.0, 0.0);
			foreach (var cursor in performArguments.Cursors)
			{
				totalTranslation += cursor.Movement;
			}
			var averageTranslation = totalTranslation / performArguments.Cursors.Count();

			newTransform = oldTransform * VMath.Translate(0.0, averageTranslation.y, 0.0);

			return true;
		}
	}
}
