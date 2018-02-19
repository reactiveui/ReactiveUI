using System;
using System.Collections.Generic;
using System.Text;

namespace ReactiveUI.Subjects
{
    public class TwoWayConverter<Source, Target>
    {
        public Func<Target, Source> ConvertTo;
        public Func<Source, Target> ConvertFrom;
        public TwoWayConverter() { }
        public TwoWayConverter(Func<Target, Source> convertTo, Func<Source, Target> convertFrom)
        {
            ConvertTo = convertTo;
            ConvertFrom = convertFrom;
        }
    }

    public class InvalidConverter<Source, Target> : TwoWayConverter<Source, Target>
    {
        public InvalidConverter(Func<Source, Target> fn):
            base( minorRadius => throw new Exception("Invalid"), fn)
        {
        }
    }

    public static class Converter
    {
        public static TwoWayConverter<double, decimal> Round(int precision)
        {
            return new TwoWayConverter<double, decimal>()
            {
                ConvertTo = Convert.ToDouble,
                ConvertFrom = dbl => Math.Round(Convert.ToDecimal(dbl), precision)
            };
        }
        public static TwoWayConverter<int, int> Add(int amount){
            return new TwoWayConverter<int,int>()
            {
                ConvertFrom = v => v + amount, 
                ConvertTo = v => v - amount
            };
        }

        public static TwoWayConverter<int, int> Subtract(int amount){
            return Add(-amount);
        }
        public static TwoWayConverter<double, double> Subtract(double amount){
            return Add(-amount);
        }


        public static TwoWayConverter<double, double> Multiply(double multiplier)
        {
            return new TwoWayConverter<double, double>()
            {
                ConvertFrom = v => v * multiplier, 
                ConvertTo = v => v / multiplier
            };
        }


        public static TwoWayConverter<double, double> Add(double amount){
            return new TwoWayConverter<double,double>()
            {
                ConvertFrom = v => v + amount, 
                ConvertTo = v => v - amount
            };
        }


    }
}
