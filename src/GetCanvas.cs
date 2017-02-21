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
	[PluginInfo(Name = "GetCanvas", Category = "MultiTouchStack", Author = "elliotwoods")]
	#endregion PluginInfo
	public class GetCanvasNode : IPluginEvaluate
	{
		#region fields & pins
		[Input("World", IsSingle = true)]
		public ISpread<World> FInWorld;

		[Output("Canvas Transform")]
		public ISpread<Matrix4x4> FOutCanvasTransform;

		[Output("Cursors On Canvas")]
		public ISpread<Cursor> FOutCursorsOnCanvas;

		[Import()]
		public ILogger FLogger;
		#endregion fields & pins

		//called when data for any output pin is requested
		public void Evaluate(int SpreadMax)
		{
			if (FInWorld[0] == null)
			{
				FOutCanvasTransform[0] = VMath.IdentityMatrix;
				FOutCursorsOnCanvas.SliceCount = 0;
				return;
			}
			else
			{
				var world = FInWorld[0];
				FOutCanvasTransform[0] = world.CanvasTransform;
				FOutCursorsOnCanvas.AssignFrom(world.CursorsOnCanvas);
			}
		}
	}
}
