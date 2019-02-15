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

	public interface IBehavior
	{
		bool PerformAndValidate(PerformArguments performArguments, Matrix4x4 priorTransform, ref Matrix4x4 newTransform);
	}

	public abstract class Behavior : IBehavior
	{
		public bool PerformAndValidate(PerformArguments performArguments, Matrix4x4 priorTransform, ref Matrix4x4 newTransform)
		{
			// For PerformAndValidate the call tree goes:
			// IBehavior-override->Combined-calls->Principal-usessuperclass->IBehavior
			// Then inside here we do Perform and do a Validate

			// Reset the HashSet
			performArguments.ActionsApplied = new HashSet<ActionsApplied>();

			if (!this.Perform(performArguments, priorTransform, ref newTransform))
			{
				// Failed to perform (e.g. not enough cursors)
				return false;
			}
			else
			{
				// Performed
				if (performArguments.ValidateFunction == null)
				{
					// No Constraints required
					return true;
				}
				else
				{
					// Check if passed Constraints
					var success = performArguments.ValidateFunction(new ValidateFunctionArguments
					{
						Transform = newTransform,
						ActionsApplied = performArguments.ActionsApplied
					});
					return success;
				}
			}
		}

		abstract public bool Perform(PerformArguments performArguments, Matrix4x4 transform, ref Matrix4x4 newTransform);
	}
}
