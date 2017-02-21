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
	public enum TestType {
		Alpha,
		Luminance
	}
	
	#region PluginInfo
	[PluginInfo(Name = "HitTest", Category = "MultiTouchStack", Tags = "delegate", Version = "ImageFile", Author = "elliotwoods")]
	#endregion PluginInfo
	public class HitTestImageFileNode : IPluginEvaluate
	{
		#region fields & pins
		[Input("Filename", StringType = StringType.Filename, FileMask = "PNG File (*.png)|*.png| JPEG File (*.jpg)|*.jpg;*.jpeg|BMP File (*.bmp)|*.bmp")]
		public ISpread<string> FInFilename;
		
		[Input("Test")]
		public ISpread<TestType> FInTestType;

		[Input("Threshold", DefaultValue = 0.5)]
		public ISpread<Double> FInThreshold;
		
		[Output("Output")]
		public ISpread<IHitTestFunction> FOutput;

		[Import()]
		public ILogger FLogger;
		#endregion fields & pins

		//called when data for any output pin is requested
		public void Evaluate(int SpreadMax)
		{
			
		}
	}
}
