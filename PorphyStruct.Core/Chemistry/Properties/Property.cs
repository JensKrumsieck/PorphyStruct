namespace PorphyStruct.Chemistry.Properties
{
    public class Property
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public Property(string name, string value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// for json deserialization
        /// </summary>
        public Property() { }
    }
}
