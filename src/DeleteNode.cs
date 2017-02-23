#region usings
using System;
using System.ComponentModel.Composition;

using VVVV.PluginInterfaces.V1;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VColor;
using VVVV.Utils.VMath;

using VVVV.Core.Logging;
using System.Linq;
#endregion usings

namespace VVVV.Nodes.MultiTouchStack
{	
	#region PluginInfo
	[PluginInfo(Name = "Delete", Category = "MultiTouchStack", Author = "elliotwoods", AutoEvaluate = true)]
	#endregion PluginInfo
	public class DeleteNode : IPluginEvaluate
	{
		#region fields & pins
		[Input("Input", AutoValidate = false)]
		public ISpread<Slide> FInSlides;
		
		[Input("Delete", IsBang=true)]
		public ISpread<bool> FInDelete;

		[Import()]
		public ILogger FLogger;
		#endregion fields & pins

		//called when data for any output pin is requested
		public void Evaluate(int SpreadMax)
		{
			//check if delete is called at all (if not, quit early)
			if (!FInDelete.Any(value => value == true))
			{
				return;
			}

			//update inputs
			{
				FInSlides.Sync();
				SpreadMax = Utils.SpreadMax(FInSlides, FInDelete);
			}


			for (int i=0; i<SpreadMax; i++)
			{
				if(FInDelete[i])
				{
					var slide = FInSlides[i];
					if(slide != null)
					{
						var world = slide.World.Target as World;
						if(world != null)
						{
							world.Slides.Remove(slide);
						}
					}
				}
			}
		}
	}
}
