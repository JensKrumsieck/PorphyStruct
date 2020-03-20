using PorphyStruct.Chemistry.Properties;
using PorphyStruct.Simulations;
using PorphyStruct.Util;
using System.Collections.Generic;

namespace PorphyStruct.Chemistry.Data
{
    public class SimulationData : AbstractPropertyProvider, IAtomDataPointProvider
    {
        public DataType DataType => DataType.Simulation;

        public IEnumerable<AtomDataPoint> DataPoints { get; set; }

        public bool Normalized { get; set; }
        public bool Inverted { get; set; }
        public double Factor { get; set; }

        /// <summary>
        /// The parameters when simulation is finalized
        /// </summary>
        public IEnumerable<SimParam> SimulationParameters { get; set; }

        public override PropertyType Type => PropertyType.Simulation;

        public SimulationData(IEnumerable<AtomDataPoint> dataPoints) => DataPoints = dataPoints;

        public override IEnumerable<Property> CalculateProperties()
        {
            foreach (SimParam param in SimulationParameters)
            {
                yield return new Property(param.Title + " (percentage)", (param.Best * 100).ToString("N2") + " %");
                yield return new Property(param.Title + " (absolute)", (param.Best * DataPoints.MeanDisplacement()).ToString("N4") + " Å");
            }
            yield return new Property("OOP-Distortion (Sim)", DataPoints.MeanDisplacement().ToString("G6") + " Å");
        }

        public void Normalize()
        {
            if (!Normalized)
            {
                Factor = DataPoints.GetNormalizationFactor();
                DataPoints = DataPoints.Normalize();
            }
            else DataPoints = DataPoints.Factor(1 / Factor);

            Normalized = !Normalized;
        }

        public void Invert()
        {
            DataPoints = DataPoints.Invert();
            Inverted = !Inverted;
        }

        public int Priority => 1;

        /** DANGER ZONE**/
        //unused...
        public override IList<string[]> Selectors => null;
    }
}
