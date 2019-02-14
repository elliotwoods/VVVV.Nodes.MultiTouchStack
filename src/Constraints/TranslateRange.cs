using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VMath;

namespace VVVV.Nodes.MultiTouchStack.Constraints
{
	class TranslateRange : IConstraint
	{
		public IConstraint UpstreamConstraint = null;
		public Vector2D Minimum;
		public Vector2D Maximum;

		public bool CheckConstraint(Behaviors.ValidateFunctionArguments validateFunctionArguments, CheckConstraintArguments checkConstraintArguments)
		{
			if (this.UpstreamConstraint != null)
			{
				if (!this.UpstreamConstraint.CheckConstraint(validateFunctionArguments, checkConstraintArguments))
				{
					return false;
				}
			}

			if (!validateFunctionArguments.ActionsApplied.Contains(Behaviors.ActionsApplied.Translate))
			{
				// Ignore if no translate applied
				return true;
			}

			Vector3D rotation, scale, translation;
			Matrix4x4Utils.Decompose(validateFunctionArguments.Transform
				, out scale
				, out rotation
				, out translation);

			var translation2 = new Vector2D(translation.x, translation.y);

			var result = translation2.x >= this.Minimum.x
				&& translation2.y >= this.Minimum.y
				&& translation2.x <= this.Maximum.x
				&& translation2.y <= this.Maximum.y;
			
			return result;
		}
	}

	[PluginInfo(Name = "TranslateRange", Category = "MultiTouchStack", Version = "Constraints", Author = "elliotwoods")]
	public class TranslateRangeNode : IPluginEvaluate
	{
		#region fields & pins
		[Input("Constraint", IsSingle = true)]
		ISpread<IConstraint> FInConstraint;

		[Input("Minimum", DefaultValues = new double[] { -1.0, -1.0 })]
		ISpread<Vector2D> FInMinimum;

		[Input("Maximum", DefaultValues = new double[] { 1.0, 1.0 })]
		ISpread<Vector2D> FInMaximum;

		[Output("Output")]
		public ISpread<IConstraint> FOutput;
		#endregion fields & pins


		public void Evaluate(int SpreadMax)
		{
			FOutput[0] = new TranslateRange
			{
				UpstreamConstraint = this.FInConstraint[0],
				Minimum = this.FInMinimum[0],
				Maximum = this.FInMaximum[0]
			};
		}
	}
}
