using System.Collections.Generic;

namespace PorphyStruct.Chemistry.Data
{
    public abstract class AbstractAtomDataPointProvider : IAtomDataPointProvider
    {
        public abstract DataType DataType { get; }
        public virtual IEnumerable<AtomDataPoint> DataPoints { get; set; }

        public bool Normalized { get; set; }
        public bool Inverted { get; set; }
        public double Factor { get; set; }

        public virtual int Priority => int.MaxValue;

        public virtual void Invert() { return; }

        public virtual void Normalize() { return; }
    }
}
