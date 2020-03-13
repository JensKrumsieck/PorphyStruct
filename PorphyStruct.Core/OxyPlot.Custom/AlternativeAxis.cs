using OxyPlot;
using OxyPlot.Axes;

namespace PorphyStruct.OxyPlotOverride
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
            var r = new AlternativeAxisRenderer(rc, PlotModel);
            r.Render(this, pass);
        }

    }
}
