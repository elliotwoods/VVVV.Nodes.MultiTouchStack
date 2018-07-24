using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVVV.Utils.VMath;

namespace VVVV.Nodes.MultiTouchStack.Constraints
{
	public class CheckConstraintArguments
	{
		public Slide Slide = null;
	}

	public interface IConstraint
	{
		bool CheckConstraint(Matrix4x4 transform, CheckConstraintArguments checkConstraintArguments);
	}
}
