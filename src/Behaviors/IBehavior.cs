using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVVV.Utils.VMath;

namespace VVVV.Nodes.MultiTouchStack.Behaviors
{
	public class PerformArguments
	{
		public IEnumerable<Cursor> Cursors;
		public Func<Matrix4x4, bool> Validate = null;
	}

	public class IBehavior
	{
		virtual public bool PerformAndTest(PerformArguments performArguments, Matrix4x4 priorTransform, out Matrix4x4 newTransform)
		{
			try
			{
				newTransform = this.Perform(priorTransform, performArguments.Cursors);
				if (performArguments.Validate == null)
				{
					return true;
				}
				else
				{
					return performArguments.Validate(newTransform);
				}
			}
			catch
			{
				//this action could not be performed
				newTransform = priorTransform;
				return false;
			}
		}

		virtual public Matrix4x4 Perform(Matrix4x4 transform, IEnumerable<Cursor> cursors)
		{
			//default is no action
			return transform;
		}
	}
}
