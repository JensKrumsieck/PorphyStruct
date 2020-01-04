using HelixToolkit.Wpf;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using PorphyStruct.Oxy.Override;
using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace PorphyStruct.Chemistry
{
    public static class MacrocyclePainter
    {
        /// <summary>
        /// Paintmode Enum
        /// </summary>
        public enum PaintMode { Exp, Sim, Diff, Com1, Com2 };

        /// <summary>
        /// Paints the Macrocycle object
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="cycle"></param>
        /// <param name="mode"></param>
        public static void Paint(PlotModel pm, Macrocycle cycle, PaintMode mode)
        {
            foreach (var dp in cycle.dataPoints) AssignValue(dp, mode);
            //read marker type
            MarkerType mType = MarkerType.Circle;
            if (mode == PaintMode.Exp) mType = Properties.Settings.Default.markerType;
            if (mode == PaintMode.Sim || mode == PaintMode.Diff) mType = Properties.Settings.Default.simMarkerType;
            if (mode == PaintMode.Com1) mType = Properties.Settings.Default.com1MarkerType;
            if (mode == PaintMode.Com2) mType = Properties.Settings.Default.com2MarkerType;

            //build series
            ScatterSeries series = new ScatterSeries()
            {
                MarkerType = mType,
                ItemsSource = cycle.dataPoints,
                ColorAxisKey = Properties.Settings.Default.singleColor ? null : "colors",
                Title = mode.ToString()
            };
            if (Properties.Settings.Default.singleColor)
                series.MarkerFill = Atom.modesSingleColor[(int)mode];
            //add series

            if(!Properties.Settings.Default.singleColor)  pm.Axes.Add(ColorAxis(cycle.dataPoints));
            else series.MarkerFill = Atom.modesSingleColor[(int) mode];

            pm.Series.Add(series);

            //draw bonds			
            foreach (OxyPlot.Annotations.ArrowAnnotation a in cycle.DrawBonds((int)mode)) pm.Annotations.Add(a);
        }

        /// <summary>
        /// Assigns Value to an AtomDataPoint
        /// </summary>
        /// <param name="dp"></param>
        /// <param name="value"></param>
        private static void AssignValue(AtomDataPoint dp, PaintMode mode) => dp.Value = dp.atom.Type == "C" ? 1000d * (int)mode : dp.X + (1000 * ((int)mode));

        /// <summary>
        /// Builds RangeColorAxis
        /// </summary>
        /// <returns></returns>
        public static AtomRangeColorAxis ColorAxis(IEnumerable<AtomDataPoint> dataPoints) => new AtomRangeColorAxis(dataPoints)
        {
            Key = "colors",
            Position = AxisPosition.Bottom,
            IsAxisVisible = false
        };


        /// <summary>
        /// Paint the Molecule in 3D
        /// </summary>
        /// <param name="cycle"></param>
        /// <param name="selected"></param>
        /// <returns></returns>
        public static Model3DGroup Paint3D(Macrocycle cycle, Atom selected = null)
        {
            Model3DGroup group = new Model3DGroup();
            //loop atoms
            foreach (var atom in cycle.Atoms)
            {
                var builder = new MeshBuilder(true, true);
                builder.AddSphere(atom.ToPoint3D(), atom.AtomRadius / 2);
                Brush brush = atom.Brush;
                if (selected != null && atom == (selected)) brush = Brushes.LightGoldenrodYellow;
                group.Children.Add(new GeometryModel3D(builder.ToMesh(), MaterialHelper.CreateMaterial(brush)));
            }
            //loop bonds
            foreach (var a1 in cycle.Atoms)
            {
                foreach (var a2 in cycle.Atoms)
                {
                    var builder = new MeshBuilder(true, true);
                    if (a1.BondTo(a2))
                        builder.AddCylinder(a1.ToPoint3D(), a2.ToPoint3D(), cycle.IsValidBond(a1,a2) ? 0.2 : 0.075, 10);//add only to selection if both are macrocycle marked
                    group.Children.Add(new GeometryModel3D(builder.ToMesh(), cycle.IsValidBond(a1, a2) ? Materials.Blue : Materials.Gray));
                }
            }
            return group;
        }
    }
}
