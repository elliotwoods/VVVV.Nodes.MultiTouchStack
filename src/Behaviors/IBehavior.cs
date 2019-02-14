using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVVV.Utils.VMath;

namespace VVVV.Nodes.MultiTouchStack.Behaviors
{
	public enum ActionsApplied
	{
		Translate,
		Rotate,
		Scale
	}

	public class ValidateFunctionArguments
	{
		public Matrix4x4 Transform;
		public HashSet<ActionsApplied> ActionsApplied;
	}

	public class PerformArguments
	{
		public IEnumerable<Cursor> Cursors;
		public Func<ValidateFunctionArguments, bool> ValidateFunction = null;
		public HashSet<ActionsApplied> ActionsApplied; // As you perform you should update this enum
	}

	public class IBehavior
	{
		virtual public bool PerformAndValidate(PerformArguments performArguments, Matrix4x4 priorTransform, out Matrix4x4 newTransform)
		{
			// For PerformAndValidate the call tree goes:
			// IBehaviour-override->Combined-calls->Principal-usessuperclass->IBehaviour
			// Then inside here we do Perform and do a Validate

			try
			{
				// Reset the HashSet
				performArguments.ActionsApplied = new HashSet<ActionsApplied>();

				newTransform = this.Perform(priorTransform, performArguments);
				if (performArguments.ValidateFunction == null)
				{
					return true;
				}
				else
				{
					return performArguments.ValidateFunction(new ValidateFunctionArguments {
						Transform = newTransform,
						ActionsApplied = performArguments.ActionsApplied
					});
				}
			}
			catch
			{
				//this action could not be performed
				newTransform = priorTransform;
				return false;
			}
		}

		virtual public Matrix4x4 Perform(Matrix4x4 transform, PerformArguments performArguments)
		{
			//default is no action
			return transform;
		}
	}
}
