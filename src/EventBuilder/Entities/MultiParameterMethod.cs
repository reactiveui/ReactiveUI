namespace EventBuilder.Entities
{
    public class MultiParameterMethod
    {
        public string Name { get; set; }
        public string ParameterList { get; set; } // "FooType foo, BarType bar, BazType baz"
        public string ParameterTypeList { get; set; } // "FooType, BarType, BazType"
        public string ParameterNameList { get; set; } // "foo, bar, baz"
    }
}