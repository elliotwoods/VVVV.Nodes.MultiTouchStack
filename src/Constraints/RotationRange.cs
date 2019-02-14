using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VMath;

namespace VVVV.Nodes.MultiTouchStack.Constraints
{
	class RotationRange : IConstraint
	{
		public IConstraint UpstreamConstraint = null;
		public double Minimum;
		public double Maximum;

		public bool CheckConstraint(Behaviors.ValidateFunctionArguments validateFunctionArguments, CheckConstraintArguments checkConstraintArguments)
		{
			if(this.UpstreamConstraint != null)
			{
				if (!this.UpstreamConstraint.CheckConstraint(validateFunctionArguments, checkConstraintArguments))
				{
					return false;
				}
			}

			if (!validateFunctionArguments.ActionsApplied.Contains(Behaviors.ActionsApplied.Rotate))
			{
				// Ignore if no rotation applied
				return true;
			}

			Vector3D rotation, scale, translation;
			Matrix4x4Utils.Decompose(validateFunctionArguments.Transform
				, out scale
				, out rotation
				, out translation);

			return rotation.z / (Math.PI * 2.0) >= this.Minimum && rotation.z / (Math.PI * 2.0) <= this.Maximum;
		}
	}


	[PluginInfo(Name = "RotationRange", Category = "MultiTouchStack", Version = "Constraints", Author = "elliotwoods")]
	public class RotationRangeNode : IPluginEvaluate
	{
		#region fields & pins
		[Input("Constraint", IsSingle = true)]
		ISpread<IConstraint> FInConstraint;

		[Input("Minimum", IsSingle = true, DefaultValue = -0.5)]
		ISpread<double> FInMinimum;

		[Input("Maximum", IsSingle = true, DefaultValue = 0.5)]
		ISpread<double> FInMaximum;

		[Output("Output")]
		public ISpread<IConstraint> FOutput;
		#endregion fields & pins


		public void Evaluate(int SpreadMax)
		{
			FOutput[0] = new RotationRange
			{
				UpstreamConstraint = this.FInConstraint[0],
				Minimum = this.FInMinimum[0],
				Maximum = this.FInMaximum[0]
			};
		}
	}
}
