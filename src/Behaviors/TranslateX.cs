using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVVV.Utils.VMath;

namespace VVVV.Nodes.MultiTouchStack.Behaviors
{
	public class TranslateX : IBehavior
	{
		public override Matrix4x4 Perform(Matrix4x4 transform, PerformArguments performArguments)
		{
			if (performArguments.Cursors.Count() < 1)
			{
				// Perform no action
				throw new Exception();
			}

			performArguments.ActionsApplied.Add(ActionsApplied.Translate);

			var totalTranslation = new Vector2D(0.0, 0.0);
			foreach (var cursor in performArguments.Cursors)
			{
				totalTranslation += cursor.Movement;
			}
			var averageTranslation = totalTranslation / performArguments.Cursors.Count();

			return transform * VMath.Translate(averageTranslation.x, 0.0, 0.0);
		}
	}
}
