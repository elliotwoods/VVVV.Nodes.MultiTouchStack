using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVVV.Core.Logging;
using VVVV.Utils.VMath;
using VVVV.PluginInterfaces.V2;

namespace VVVV.Nodes.MultiTouchStack.Constraints
{
	public class HitTest : IConstraint
	{
		public IConstraint UpstreamConstraint = null;
		public IHitTestFunction HitTestFunction = null;
		public int Resolution;

		public bool CheckConstraint(Behaviors.ValidateFunctionArguments validateFunctionArguments, CheckConstraintArguments checkConstraintArguments)
		{
			if (this.UpstreamConstraint != null)
			{
				if (!this.UpstreamConstraint.CheckConstraint(validateFunctionArguments, checkConstraintArguments))
				{
					return false;
				}
			}

			if (this.HitTestFunction != null)
			{
				var localCoordinates = new List<Vector2D>();
				var worldCoordinates = new List<Vector2D>();

				// Build local coordinates
				for (int i = 0; i < this.Resolution; i++)
				{
					for (int j = 0; j < this.Resolution; j++)
					{
						localCoordinates.Add(new Vector2D(
							((double)i / (double)(Resolution - 1)) - 0.5,
							((double)j / (double)(Resolution - 1)) - 0.5
							));
					}
				}

				// Build world coordinates
				foreach (var localCoordinate in localCoordinates)
				{
					// Ignore local coordinate which doesn't pass slide hit test
					if (checkConstraintArguments.Slide.DragHitTest(localCoordinate))
					{
						var localCoordinate3 = new Vector3D(localCoordinate);
						var worldCoordinate3 = validateFunctionArguments.Transform * localCoordinate3;
						var worldCoordinate = new Vector2D(worldCoordinate3.x, worldCoordinate3.y);
						worldCoordinates.Add(worldCoordinate);
					}
				}

				// Check if any world coordinates fail hit test
				foreach (var worldCoordinate in worldCoordinates)
				{
					var worldCoordinateInHitTest = worldCoordinate;

					if(this.HitTestFunction is HitTestImageFileNode.Function)
					{
						worldCoordinateInHitTest /= 2.0;
					}

					if (!this.HitTestFunction.TestHit(worldCoordinateInHitTest)) // Full screen hit test rather than default quad size
					{
						return false;
					}
				}
			}

			return true;
		}
	}

	[PluginInfo(Name = "HitTest", Category = "MultiTouchStack", Version = "Constraints", Author = "elliotwoods")]
	public class HitTestNode : IPluginEvaluate
	{
		#region fields & pins
		[Input("Constraint", IsSingle = true)]
		ISpread<IConstraint> FInConstraint;

		[Input("Hit Test Function", IsSingle = true)]
		ISpread<IHitTestFunction> FInHitTestFunction;

		[Input("Resolution", IsSingle = true, DefaultValue = 5)]
		ISpread<int> FInResolution;

		[Output("Output")]
		public ISpread<IConstraint> FOutput;
		#endregion fields & pins


		public void Evaluate(int SpreadMax)
		{
			FOutput[0] = new HitTest
			{
				UpstreamConstraint = this.FInConstraint[0],
				HitTestFunction = this.FInHitTestFunction[0],
				Resolution = this.FInResolution[0]
			};
		}
	}
}
