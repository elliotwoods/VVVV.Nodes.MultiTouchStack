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
	[PluginInfo(Name = "HitEvent", Category = "MultiTouchStack Join", Author = "elliotwoods")]
	#endregion PluginInfo
	public class HitEventJoinNode : IPluginEvaluate
	{
		#region fields & pins
		[Input("Hit Test Function")]
		public IDiffSpread<IHitTestFunction> FInHitTestFunction;

		[Input("Event Name")]
		public IDiffSpread<string> FInEventName;

		[Output("Output")]
		public ISpread<HitEvent> FOutput;
		
		[Import()]
		public ILogger FLogger;
		#endregion fields & pins

		//called when data for any output pin is requested
		public void Evaluate(int SpreadMax)
		{
			if(FInHitTestFunction.IsChanged || FInEventName.IsChanged)
			{
				FOutput.SliceCount = SpreadMax;

				for (int i = 0; i < SpreadMax; i++)
				{
					var hitTestFunction = FInHitTestFunction[i];
					var eventName = FInEventName[i];

					//check if we need to rebuild this output
					if (FOutput[i] != null)
					{
						var output = FOutput[i];
						if (output.HitTestFunction == hitTestFunction
							&& output.Name == eventName)
						{
							//already constructed, save some time
							continue;
						}
					}

					//build it
					FOutput[i] = new HitEvent
					{
						HitTestFunction = hitTestFunction,
						Name = eventName
					};
				}
			}
		}
	}
}
