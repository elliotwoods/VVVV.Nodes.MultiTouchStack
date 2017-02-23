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

		public class Function : IHitTestFunction, IDisposable
		{
			class ImageStore
			{
				public int ReferenceCount = 0;
				public Bitmap Bitmap
				{
					get
					{
						if(!this.FImageLoader.IsCompleted)
						{
							return null;
						} else
						{
							return this.FBitmap;
						}
					}
				}

				Bitmap FBitmap;
				Task FImageLoader;

				public ImageStore(string filename)
				{
					this.ReferenceCount = 1;

					//asynchronously load the image
					FImageLoader = Task.Run(() =>
					{
						using (var fs = new System.IO.FileStream(filename, System.IO.FileMode.Open))
						{
							this.FBitmap = new Bitmap(fs);
						}
					});
				}
			}

			static Dictionary<string, ImageStore> GImageStores = new Dictionary<string, ImageStore>();

			public string Filename { get; private set; }
			public TestType TestType { get; private set; }
			public Double Threshold { get; private set; }

			public Function(string filename, TestType testType, Double threshold)
			{
				//constructor
				this.TestType = testType;
				this.Threshold = threshold;
				this.Filename = filename;

				if(!GImageStores.ContainsKey(filename))
				{
					GImageStores.Add(filename, new ImageStore(filename));
				} else
				{
					GImageStores[filename].ReferenceCount++;
				}
			}

			public bool TestHit(Vector2D localCoordinates)
			{
				try
				{
					var bitmap = GImageStores[this.Filename].Bitmap;

					if (bitmap == null)
					{
						//return solid quad (default personality)
						return localCoordinates.x >= -0.5
							&& localCoordinates.x <= +0.5
							&& localCoordinates.y >= -0.5
							&& localCoordinates.y <= +0.5;
					}

					var x = (int) ((0.5 + localCoordinates.x) * bitmap.Width);
					var y = (int) ((0.5 - localCoordinates.y) * bitmap.Height);

					//check if outside bounds
					if(x < 0
						|| x >= bitmap.Width
						|| y < 0
						|| y >= bitmap.Height)
					{
						return false;
					}

					var color = bitmap.GetPixel(x, y);
					switch(this.TestType)
					{
						case TestType.Alpha:
							return (Double)color.A / (Double)Byte.MaxValue >= this.Threshold;
						case TestType.Luminance:
							return color.GetBrightness() >= (float) this.Threshold;
						default:
							return false;
					}
				}
				catch
				{
					return false;
				}
			}

			#region IDisposable Support
			private bool disposedValue = false; // To detect redundant calls

			protected virtual void Dispose(bool disposing)
			{
				if (!disposedValue)
				{
					if(this.Filename != null
						&& GImageStores.ContainsKey(this.Filename))
					{
						//remove reference to this image file in store
						GImageStores[this.Filename].ReferenceCount--;

						if (GImageStores[this.Filename].ReferenceCount <= 0)
						{
							GImageStores.Remove(this.Filename);
						}
					}
					

					disposedValue = true;
				}
			}

			~Function()
			{
				this.Dispose();
			}

			public void Dispose()
			{
				Dispose(true);
			}
			#endregion
		}

		#region fields & pins
		[Input("Filename", StringType = StringType.Filename, FileMask = "PNG File (*.png)|*.png| JPEG File (*.jpg)|*.jpg;*.jpeg|BMP File (*.bmp)|*.bmp")]
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
