using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVVV.Utils.VMath;

namespace VVVV.Nodes.MultiTouchStack.Behaviors
{
	public class Translate : IBehavior
	{
		public override Matrix4x4 Perform(Matrix4x4 transform, IEnumerable<Cursor> cursors)
		{
			if (cursors.Count() < 1)
			{
				// Perform no action
				throw new Exception();
			}

			var totalTranslation = new Vector2D(0.0, 0.0);
			foreach(var cursor in cursors)
			{
				totalTranslation += cursor.Movement;
			}
			var averageTranslation = totalTranslation / cursors.Count();

			return transform * VMath.Translate(new Vector3D(averageTranslation));
		}
	}
}
