﻿using OxyPlot;
using OxyPlot.Annotations;

namespace PorphyStruct.Plot
{
    public class BondAnnotation : ArrowAnnotation
    {
        public BondAnnotation(AtomDataPoint a1, AtomDataPoint a2)
        {
            StartPoint = a1.ToDataPoint();
            EndPoint = a2.ToDataPoint();
            HeadLength = 0;
            Color = OxyColor.Parse(Settings.Instance.BondColor);
            StrokeThickness = Settings.Instance.BondThickness;
            Layer = AnnotationLayer.BelowSeries;
            Tag = $"{a1.Atom.Title} - {a2.Atom.Title} ({a1}, {a2})";
        }
    }
}
