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
	[PluginInfo(Name = "Cursor", Category = "MultiTouchStack Split", Author = "elliotwoods")]
	#endregion PluginInfo
	public class CursorSplitNode : IPluginEvaluate
	{
		#region fields & pins
		[Input("Input")]
		public ISpread<Cursor> FInCursors;
		
		[Output("Index", Order = 0)]
		public ISpread<int> FOutIndex;
		
		[Output("Position")]
		public ISpread<Vector2D> FOutPosition;

		[Output("Movement")]
		public ISpread<Vector2D> FOutMovement;

		[Output("Lifetime", DimensionNames = new string[] { "s" })]
		public ISpread<Double> FOutLifetime;

		[Output("Assigned Slide")]
		public ISpread<ISpread<Slide>> FOutAssignedSlide;

		[Import()]
		public ILogger FLogger;
		#endregion fields & pins

		//called when data for any output pin is requested
		public void Evaluate(int SpreadMax)
		{
			if(FInCursors.SliceCount == 0 || FInCursors[0] == null)
			{
				SpreadMax = 0;
			}

			FOutIndex.SliceCount = SpreadMax;
			FOutPosition.SliceCount = SpreadMax;
			FOutMovement.SliceCount = SpreadMax;
			FOutLifetime.SliceCount = SpreadMax;
			FOutAssignedSlide.SliceCount = SpreadMax;

			for(int i=0; i < SpreadMax; i++)
			{
				var cursor = FInCursors[i];
				FOutIndex[i] = cursor.Index;
				FOutPosition[i] = cursor.Position;
				FOutMovement[i] = cursor.Movement;
				FOutLifetime[i] = cursor.LifeTime;

				if(cursor.AssignedSlide == null)
				{
					FOutAssignedSlide[i].SliceCount = 0;
				}
				else
				{
					FOutAssignedSlide[i].SliceCount = 1;
					FOutAssignedSlide[i][0] = cursor.AssignedSlide;
				}
			}
		}
	}
}
