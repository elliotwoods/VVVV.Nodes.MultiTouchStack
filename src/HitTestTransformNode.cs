#region usings
using System;
using System.ComponentModel.Composition;

using VVVV.PluginInterfaces.V1;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VColor;
using VVVV.Utils.VMath;

using VVVV.Core.Logging;
#endregion usings

namespace VVVV.Nodes.MultiTouchStack
{
	#region PluginInfo
	[PluginInfo(Name = "HitTest", Category = "MultiTouchStack", Tags = "delegate", Version = "Transform", Author = "elliotwoods")]
	#endregion PluginInfo
	public class HitTestTransformNode : IPluginEvaluate
	{
		public enum Shape
		{
			Quad,
			Circle
		}

		public class Function : IHitTestFunction
		{
			public Matrix4x4 Transform;
			public Shape TestShape;

			public bool TestHit(Vector2D localCoordinates)
			{
				if(TestShape == Shape.Quad)
				{
					return Math.Abs(localCoordinates.x) <= 0.5
						&& Math.Abs(localCoordinates.y) <= 0.5;
				}
				else if(TestShape == Shape.Circle)
				{
					return localCoordinates.LengthSquared < 0.25;
				} else
				{
					//default
					return false;
				}
			}
		}

		#region fields & pins
		[Input("Transform")]
		public ISpread<Matrix4x4> FInTransform;

		[Input("Shape")]
		public ISpread<Shape> FInShape;

		[Output("Output")]
		public ISpread<IHitTestFunction> FOutput;

		[Import()]
		public ILogger FLogger;
		#endregion fields & pins

		//called when data for any output pin is requested
		public void Evaluate(int SpreadMax)
		{
			FOutput.SliceCount = SpreadMax;
			for (int i = 0; i < SpreadMax; i++)
			{
				FOutput[i] = new Function
				{
					Transform = FInTransform[i],
					TestShape = FInShape[i]
				};
			}
		}
	}
}
