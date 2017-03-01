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
	[PluginInfo(Name = "Slide", Category = "MultiTouchStack Split", Author = "elliotwoods")]
	#endregion PluginInfo
	public class SlideSplitNode : IPluginEvaluate
	{
		#region fields & pins
		[Input("Input")]
		public ISpread<Slide> FInSlides;

		[Output("Index")]
		public ISpread<int> FOutIndex;
		
		[Output("Transform")]
		public ISpread<Matrix4x4> FOutTransform;
		
		[Output("Tags")]
		public ISpread<ISpread<String>> FOutTags;

		[Output("Minimum Scale", Visibility = PinVisibility.OnlyInspector)]
		public ISpread<Double> FOutMinimumScale;

		[Output("Maximum Scale", Visibility = PinVisibility.OnlyInspector)]
		public ISpread<Double> FOutMaximumScale;

		[Output("Cursors Attached")]
		public ISpread<ISpread<Cursor>> FOutCursorsAttached;

		[Output("Hit Callbacks")]
		public ISpread<ISpread<HitCallback>> FOutHitCallbacks;

		[Import()]
		public ILogger FLogger;
		#endregion fields & pins

		//called when data for any output pin is requested
		public void Evaluate(int SpreadMax)
		{
			if(FInSlides.SliceCount == 0 || FInSlides[0] == null)
			{
				SpreadMax = 0;
			}

			FOutIndex.SliceCount = SpreadMax;
			FOutTransform.SliceCount = SpreadMax;
			FOutMinimumScale.SliceCount = SpreadMax;
			FOutMaximumScale.SliceCount = SpreadMax;
			FOutTags.SliceCount = SpreadMax;
			FOutCursorsAttached.SliceCount = SpreadMax;
			FOutHitCallbacks.SliceCount = SpreadMax;

			for (int i = 0; i < SpreadMax; i++)
			{
				var slide = FInSlides[i];

				//apply a z-order transform
				var transform = slide.Transform;
				transform = slide.TransformWithZOrder;

				FOutIndex[i] = slide.Index;
				FOutTransform[i] = transform;
				FOutMinimumScale[i] = slide.MinimumScale;
				FOutMaximumScale[i] = slide.MaximumScale;
				FOutTags[i].AssignFrom(slide.Tags);
				FOutCursorsAttached[i].AssignFrom(slide.AttachedCursors);
				FOutHitCallbacks[i].AssignFrom(slide.HitCallbacksThisFrame);
			}
		}
	}
}
