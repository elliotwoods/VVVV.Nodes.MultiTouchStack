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
	[PluginInfo(Name = "HitCallback", Category = "MultiTouchStack Split", Author = "elliotwoods")]
	#endregion PluginInfo
	public class HitCallbackSplitNode : IPluginEvaluate
	{
		#region fields & pins
		[Input("Input")]
		public ISpread<HitCallback> FInHitCallbacks;

		[Input("Filter Action Type", IsSingle = true, DefaultEnumEntry = "Any")]
		public ISpread<HitCallback.CursorActionType> FInFilterActionType;

		[Output("Event Name")]
		public ISpread<string> FOutEventName;

		[Output("Cursor")]
		public ISpread<Cursor> FOutCursor;

		[Output("Action Type")]
		public ISpread<HitCallback.CursorActionType> FOutActionType;

		[Import()]
		public ILogger FLogger;
		#endregion fields & pins

		//called when data for any output pin is requested
		public void Evaluate(int SpreadMax)
		{
			FOutEventName.SliceCount = 0;
			FOutCursor.SliceCount = 0;
			FOutActionType.SliceCount = 0;

			for (int i = 0; i < SpreadMax; i++)
			{
				var hitCallback = FInHitCallbacks[i];
				if(hitCallback == null)
				{
					continue;
				} else
				{
					if(FInFilterActionType[0].HasFlag(hitCallback.ActionType))
					{
						FOutEventName.Add(hitCallback.Event.Name);
						FOutCursor.Add(hitCallback.Cursor);
						FOutActionType.Add(hitCallback.ActionType);
					}
				}

			}
		}
	}
}
