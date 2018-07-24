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
		public IDiffSpread<bool> FInTapBringsToFront;

		[Output("Output")]
		public ISpread<Settings> FOutput;

		[Import()]
		public ILogger FLogger;

		Settings FSettings = new Settings();
		#endregion fields & pins

		//called when data for any output pin is requested
		public void Evaluate(int SpreadMax)
		{
			if(FInTapBringsToFront.IsChanged)
			{
				var settings = new Settings
				{
					TapBringsToFront = FInTapBringsToFront[0]
				};
				FOutput[0] = settings;
			}
		}
	}
}
