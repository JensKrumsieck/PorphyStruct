using OxyPlot;
using OxyPlot.Axes;
using System;
using System.Reflection;

namespace PorphyStruct.Core.Plot
{
    public sealed class HorizontalAndVerticalAxisRenderer : OxyPlot.Axes.HorizontalAndVerticalAxisRenderer
    {
        public HorizontalAndVerticalAxisRenderer(IRenderContext rc, PlotModel plot) : base(rc, plot) { }

        protected override void RenderAxisTitle(Axis axis, double titlePosition)
        {
            if (string.IsNullOrEmpty(axis.ActualTitle)) return;
            var isHorizontal = axis.IsHorizontal();
            OxySize? maxSize = null;

            if (axis.ClipTitle)
            {
                var screenLength = isHorizontal ? Math.Abs(axis.ScreenMax.X - axis.ScreenMin.X) : Math.Abs(axis.ScreenMax.Y - axis.ScreenMin.Y);
                maxSize = new OxySize(screenLength * axis.TitleClippingLength, double.MaxValue);
            }
            var angle = (axis is LinearAxis linearAxis) ? linearAxis.TitleAngle : -90d;
            var halign = HorizontalAlignment.Center;
            var valign = VerticalAlignment.Top;
            var lpt = GetAxisTitlePositionAndAlignment(axis, titlePosition, ref angle, ref halign, ref valign);

            RenderContext.DrawMathText(lpt, axis.ActualTitle,
                ReflectionHack<OxyColor>("ActualTitleColor", axis),
                ReflectionHack<string>("ActualTitleFont", axis),
                ReflectionHack<double>("ActualTitleFontSize", axis),
                ReflectionHack<double>("ActualTitleFontWeight", axis),
                angle, halign, valign, maxSize);
        }

        /// <summary>
        /// Unfortunately the actual properties are internal
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        private static T ReflectionHack<T>(string propertyName, Axis owner)
        {
            var pInfo = typeof(Axis).GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic);
            return (T)pInfo?.GetValue(owner);
        }
    }
}
