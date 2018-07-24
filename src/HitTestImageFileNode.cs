#region usings
using System;
using System.ComponentModel.Composition;

using VVVV.PluginInterfaces.V1;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VColor;
using VVVV.Utils.VMath;

using VVVV.Core.Logging;
using System.Drawing;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;
#endregion usings

namespace VVVV.Nodes.MultiTouchStack
{ 
	#region PluginInfo
	[PluginInfo(Name = "HitTest", Category = "MultiTouchStack", Tags = "delegate", Version = "ImageFile", Author = "elliotwoods")]
	#endregion PluginInfo
	public class HitTestImageFileNode : IPluginEvaluate
	{
		public enum TestType
		{
			Alpha,
			Luminance
		}

		public class Function : IHitTestFunction
		{
			public static Dictionary<string, WeakReference<Bitmap>> GImageCache = new Dictionary<string, WeakReference<Bitmap>>();

			public string Filename { get; private set; }
			public TestType TestType { get; private set; }
			public Double Threshold { get; private set; }
			public Bitmap Bitmap { get; private set; }

			public Function(string filename, TestType testType, Double threshold)
			{
				//constructor
				this.TestType = testType;
				this.Threshold = threshold;
				this.Filename = filename;

				//get the bitmap from the cache if available
				if (GImageCache.ContainsKey(filename))
				{
					//we have an entry in the cache
					Bitmap bitmap;
					if (GImageCache[filename].TryGetTarget(out bitmap))
					{
						//the entry is alive so use it
						this.Bitmap = bitmap;
					}
					else
					{
						//the entry is stale so remove it
						GImageCache.Remove(filename);
					}
				}

				//load the image if it wasn't loaded from the cache
				if (this.Bitmap == null)
				{
					//open and close file
					using (var fs = new System.IO.FileStream(filename, System.IO.FileMode.Open))
					{
						this.Bitmap = new Bitmap(fs);
					}
					GImageCache.Add(filename, new WeakReference<Bitmap>(this.Bitmap));
				}
			}

			public bool TestHit(Vector2D localCoordinates)
			{
				try
				{
					var bitmap = this.Bitmap;

					var x = (int)((0.5 + localCoordinates.x) * bitmap.Width);
					var y = (int)((0.5 - localCoordinates.y) * bitmap.Height);

					//check if outside bounds
					if (x < 0
						|| x >= bitmap.Width
						|| y < 0
						|| y >= bitmap.Height)
					{
						return false;
					}

					var color = bitmap.GetPixel(x, y);
					switch (this.TestType)
					{
						case TestType.Alpha:
							return (Double)color.A / (Double)Byte.MaxValue >= this.Threshold;
						case TestType.Luminance:
							{
								var brightness = color.GetBrightness();
								return brightness >= (float)this.Threshold;
							}
						default:
							return false;
					}
				}
				catch
				{
					//return solid quad (default personality)
					return localCoordinates.x >= -0.5
						&& localCoordinates.x <= +0.5
						&& localCoordinates.y >= -0.5
						&& localCoordinates.y <= +0.5;
				}
			}
		}

		#region fields & pins
		[Input("Filename", StringType = StringType.Filename, FileMask = "Image File (*.bmp;*.gif;*.exif;*.jpg;*.jpeg;*.png;*.tif;*.tiff)|*.bmp;*.gif;*.exif;*.jpg;*.jpeg;*.png;*.tif;*.tiff|PNG File (*.png)|*.png| JPEG File (*.jpg)|*.jpg;*.jpeg|BMP File (*.bmp)|*.bmp")]
		public IDiffSpread<string> FInFilename;
		
		[Input("Test")]
		public IDiffSpread<TestType> FInTestType;

		[Input("Threshold", DefaultValue = 0.5, MinValue = 0.0, MaxValue = 1.0)]
		public IDiffSpread<Double> FInThreshold;
		
		[Output("Output")]
		public ISpread<IHitTestFunction> FOutput;

		[Output("Error")]
		public ISpread<string> FOutError;

		[Output("Success")]
		public ISpread<bool> FOutSuccess;

		[Import()]
		public ILogger FLogger;
		#endregion fields & pins

		//called when data for any output pin is requested
		public void Evaluate(int SpreadMax)
		{
			if(FInFilename.IsChanged
				|| FInTestType.IsChanged
				|| FInThreshold.IsChanged)
			{
				FOutput.SliceCount = SpreadMax;
				FOutError.SliceCount = SpreadMax;
				FOutSuccess.SliceCount = SpreadMax;
				for (int i = 0; i < SpreadMax; i++)
				{
					try
					{
						FOutput[i] = new Function(FInFilename[i], FInTestType[i], FInThreshold[i]);
						FOutError[i] = "OK";
						FOutSuccess[i] = true;
					}
					catch (Exception e)
					{
						FOutError[i] = e.Message;
						FOutSuccess[i] = false;
					}
				}
			}
		}
	}
}
