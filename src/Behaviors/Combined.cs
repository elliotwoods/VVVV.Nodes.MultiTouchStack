using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVVV.Utils.VMath;

namespace VVVV.Nodes.MultiTouchStack.Behaviors
{
	public class Combined : IBehavior
	{
		public IBehavior Principal = null;
		public IBehavior AlsoTry = null;
		public IBehavior Fallback = null;

		public override bool PerformAndValidate(PerformArguments performArguments, Matrix4x4 priorTransform, out Matrix4x4 newTransform)
		{
			bool success = false;
			newTransform = priorTransform;

			if (this.Principal != null)
			{
				success |= this.Principal.PerformAndValidate(performArguments, newTransform, out newTransform);
			}

			if (this.AlsoTry != null)
			{
				success |= this.AlsoTry.PerformAndValidate(performArguments, newTransform, out newTransform);
			}

			if (!success)
			{
				// Neither the principal or the AlsoTry succeeded, try the fall-back action
				if (this.Fallback != null)
				{
					success |= this.Fallback.PerformAndValidate(performArguments, newTransform, out newTransform);
				}
			}

			return success;
		}
	}
}
