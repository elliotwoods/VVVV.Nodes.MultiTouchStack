using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VMath;

namespace VVVV.Nodes.MultiTouchStack.Constraints
{
	class ScaleRange : IConstraint
	{
		public IConstraint UpstreamConstraint = null;
		public double Minimum;
		public double Maximum;

		public bool CheckConstraint(Behaviors.ValidateFunctionArguments validateFunctionArguments, CheckConstraintArguments checkConstraintArguments)
		{
			if (this.UpstreamConstraint != null)
			{
				if (!this.UpstreamConstraint.CheckConstraint(validateFunctionArguments, checkConstraintArguments))
				{
					return false;
				}
			}

			if (!validateFunctionArguments.ActionsApplied.Contains(Behaviors.ActionsApplied.Scale))
			{
				// Ignore if no scale applied
				return true;
			}

			Vector3D rotation, scale, translation;
			Matrix4x4Utils.Decompose(validateFunctionArguments.Transform
				, out scale
				, out rotation
				, out translation);

			return Math.Min(scale.x, scale.y) >= this.Minimum && Math.Max(scale.x, scale.y) <= this.Maximum;
		}
	}


	[PluginInfo(Name = "ScaleRange", Category = "MultiTouchStack", Version = "Constraints", Author = "elliotwoods")]
	public class ScaleRangeNode : IPluginEvaluate
	{
		#region fields & pins
		[Input("Constraint", IsSingle = true)]
		ISpread<IConstraint> FInConstraint;

		[Input("Minimum", IsSingle = true, DefaultValue = 0.05)]
		ISpread<double> FInMinimum;

		[Input("Maximum", IsSingle = true, DefaultValue = 10)]
		ISpread<double> FInMaximum;

		[Output("Output")]
		public ISpread<IConstraint> FOutput;
		#endregion fields & pins


		public void Evaluate(int SpreadMax)
		{
			FOutput[0] = new ScaleRange
			{
				UpstreamConstraint = this.FInConstraint[0],
				Minimum = this.FInMinimum[0],
				Maximum = this.FInMaximum[0]
			};
		}
	}
}
