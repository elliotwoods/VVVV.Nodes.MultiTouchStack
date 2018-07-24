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
		public IConstraint Constraint = null;
		public Vector2D Minimum;
		public Vector2D Maximum;

		public bool CheckConstraint(Matrix4x4 transform, CheckConstraintArguments checkConstraintArguments)
		{
			if (this.Constraint != null)
			{
				if (!this.Constraint.CheckConstraint(transform, checkConstraintArguments))
				{
					return false;
				}
			}

			Vector3D rotation, scale, translation;
			Matrix4x4Utils.Decompose(transform
				, out scale
				, out rotation
				, out translation);

			var translation2 = new Vector2D(translation.x, translation.y);

			var result = translation2.x >= this.Minimum.x
				&& translation2.y >= this.Minimum.y
				&& translation2.x <= this.Maximum.x
				&& translation2.y <= this.Maximum.y;
			if (this.Constraint != null)
			{
				result &= this.Constraint.CheckConstraint(transform, checkConstraintArguments);
			}
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
				Constraint = this.FInConstraint[0],
				Minimum = this.FInMinimum[0],
				Maximum = this.FInMaximum[0]
			};
		}
	}
}
