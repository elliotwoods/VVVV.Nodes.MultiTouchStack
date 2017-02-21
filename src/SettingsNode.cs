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
	[PluginInfo(Name = "Settings", Category = "MultiTouchStack", Tags = "", Author = "elliotwoods")]
	#endregion PluginInfo
	public class SettingsNode : IPluginEvaluate
	{
		#region fields & pins
		[Input("Tap Brings To Front", IsSingle=true, DefaultBoolean=true)]
		public ISpread<bool> FInTapBringsToFront;

		[Input("Canvas Can Be Transformed", IsSingle=true, DefaultBoolean=false)]
		public ISpread<bool> FInCanvasCanBeTransformed;

		[Input("Canvas Size", IsSingle=true, DefaultValues = new double[]{1.0, 1.0})]
		public ISpread<Vector2D> FInCanvasSize;

		[Output("Output")]
		public ISpread<Settings> FOutput;

		[Import()]
		public ILogger FLogger;
		#endregion fields & pins

		//called when data for any output pin is requested
		public void Evaluate(int SpreadMax)
		{
			
		}
	}
}
