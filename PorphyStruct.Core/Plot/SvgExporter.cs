using OxyPlot;
using OxyPlot.SkiaSharp;
using SkiaSharp;
using System.IO;

namespace PorphyStruct.Core.Plot
{
    /// <summary>
    /// Inspired by https://github.com/oxyplot/oxyplot/blob/develop/Source/OxyPlot.SkiaSharp/SvgExporter.cs
    /// </summary>
    public class SvgExporter : IExporter
    {
        /// <summary>
        /// Gets or sets the height (in user units) of the output area.
        /// </summary>
        public float Height { get; set; }

        /// <summary>
        /// Gets or sets the width (in user units) of the output area.
        /// </summary>
        public float Width { get; set; }

        /// <summary>
        /// Gets or sets the DPI.
        /// </summary>
        public float Dpi { get; set; } = 96;

        /// <inheritdoc/>
        public void Export(IPlotModel model, Stream stream)
        {
            using var skStream = new SKManagedWStream(stream);
            using var canvas = SKSvgCanvas.Create(new SKRect(0, 0, Width, Height), skStream);

            if (!model.Background.IsInvisible())
            {
                canvas.Clear(model.Background.ToSKColor());
            }

            // SVG export does not work with UseTextShaping=true. However SVG does text shaping by itself anyway, so we can just disable it
            using var context = new SkiaRenderContext
            {
                RenderTarget = RenderTarget.VectorGraphic,
                SkCanvas = canvas,
                UseTextShaping = false,
                DpiScale = Dpi / 96
            };
            //use fake dpi to scale, looks much better!
            model.Update(true);
            model.Render(context, new OxyRect(0, 0, Width / context.DpiScale, Height / context.DpiScale));
        }
    }
}
