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
	[PluginInfo(Name = "GetCursors", Category = "MultiTouchStack", Author = "elliotwoods")]
	#endregion PluginInfo
	public class GetCursorsNode : IPluginEvaluate
	{
		#region fields & pins
		[Input("World", IsSingle = true)]
		public Pin<World> FInWorld;

		[Output("Cursors")]
		public ISpread<Cursor> FOutCursors;

		[Import()]
		public ILogger FLogger;
		#endregion fields & pins

		//called when data for any output pin is requested
		public void Evaluate(int SpreadMax)
		{
			if (!this.FInWorld.PluginIO.IsConnected)
			{
				FOutCursors.SliceCount = 0;
				return;
			}
			else
			{
				var world = FInWorld[0];
				FOutCursors.AssignFrom(world.Cursors.Values);
			}
		}
	}
}
