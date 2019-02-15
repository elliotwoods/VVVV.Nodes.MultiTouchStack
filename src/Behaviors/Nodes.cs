using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVVV.PluginInterfaces.V2;

namespace VVVV.Nodes.MultiTouchStack.Behaviors
{
	public class BehaviorNode<BehaviorType> : IPluginEvaluate where BehaviorType : IBehavior, new()
	{
		public static BehaviorType PrincipalBehavior = new BehaviorType();

		#region fields & pins
		[Input("Fall Back", IsSingle = true)]
		public ISpread<IBehavior> FInFallBack;

		[Output("Output")]
		public ISpread<IBehavior> FOutput;

		Settings FSettings = new Settings();
		#endregion fields & pins

		public void Evaluate(int SpreadMax)
		{
			if(FInFallBack[0] != null)
			{
				FOutput[0] = new Combined
				{
					Principal = BehaviorNode<BehaviorType>.PrincipalBehavior,
					Fallback = FInFallBack[0]
				};
			}
			else
			{
				FOutput[0] = BehaviorNode<BehaviorType>.PrincipalBehavior;
			}
		}
	}

	[PluginInfo(Name = "FullMultitouch", Category = "MultiTouchStack", Version = "Behaviors", Author = "elliotwoods")]
	public class FullMultitouchNode : BehaviorNode<FullMultitouch> { }

	[PluginInfo(Name = "Scale", Category = "MultiTouchStack", Version = "Behaviors", Author = "elliotwoods")]
	public class ScaleNode : BehaviorNode<Scale> { }

	[PluginInfo(Name = "Rotate", Category = "MultiTouchStack", Version = "Behaviors", Author = "elliotwoods")]
	public class RotateNode : BehaviorNode<Rotate> { }

	[PluginInfo(Name = "Translate", Category = "MultiTouchStack", Version = "Behaviors", Author = "elliotwoods")]
	public class TranslateNode : BehaviorNode<Translate> { }

	[PluginInfo(Name = "TranslateX", Category = "MultiTouchStack", Version = "Behaviors", Author = "elliotwoods")]
	public class TranslateXNode : BehaviorNode<TranslateX> { }

	[PluginInfo(Name = "TranslateY", Category = "MultiTouchStack", Version = "Behaviors", Author = "elliotwoods")]
	public class TranslateYNode : BehaviorNode<TranslateY> { }
}
