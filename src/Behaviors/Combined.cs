using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVVV.Utils.VMath;

namespace VVVV.Nodes.MultiTouchStack.Behaviors
{
	public class Combined : IBehavior
	{
		public IBehavior Principal = null;
		public IBehavior Fallback = null;

		public bool PerformAndValidate(PerformArguments performArguments, Matrix4x4 priorTransform, ref Matrix4x4 newTransform)
		{
			bool success = false;

			if (this.Principal != null)
			{
				newTransform = new Matrix4x4();
				success = this.Principal.PerformAndValidate(performArguments, priorTransform, ref newTransform);
			}
			
			if (!success)
			{
				// Neither the principal or the AlsoTry succeeded, try the fall-back action
				if (this.Fallback != null)
				{
					newTransform = new Matrix4x4();
					success = this.Fallback.PerformAndValidate(performArguments, priorTransform, ref newTransform);
				}
			}

			return success;
		}
	}
}
