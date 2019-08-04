using OxyPlot;
using OxyPlot.Axes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PorphyStruct.Oxy.Override
{
	public abstract class AlternativeAxis : Axis
	{
		/// <summary>
		/// Renders the axis on the specified render context.
		/// </summary>
		/// <param name="rc">The render context.</param>
		/// <param name="pass">The pass.</param>
		/// <see cref="Axis.Render(IRenderContext, int)"/>
		public override void Render(IRenderContext rc, int pass)
		{
			var r = new AlternativeAxisRenderer(rc, this.PlotModel);
			r.Render(this, pass);
		}
		
	}
}
