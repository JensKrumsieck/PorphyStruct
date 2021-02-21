namespace PorphyStruct.Core.Analysis.Properties
{
    public class KeyValueProperty
    {
        public virtual string Key { get; set; }
        public virtual double Value { get; set; }
        public virtual string Unit { get; set; }

        public override string ToString() => $"{Key}: {Value:N3} {Unit}";
    }
}
