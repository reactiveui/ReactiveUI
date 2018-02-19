using System;
using System.Collections.Generic;
using System.Text;

namespace ReactiveUI.Subjects
{
    public class TwoWayConstraint<Source, Target>
    {
        public Func<Target, Source> ConvertTo;
        public Func<Source, Target> ConvertFrom;
        public TwoWayConstraint() { }
        public TwoWayConstraint(Func<Target, Source> convertTo, Func<Source, Target> convertFrom)
        {
            ConvertTo = convertTo;
            ConvertFrom = convertFrom;
        }
    }

    public class InvalidConstraint<Source, Target> : TwoWayConstraint<Source, Target>
    {
        public InvalidConstraint(Func<Source, Target> fn):
            base( minorRadius => throw new Exception("Invalid"), fn)
        {
        }
    }

    public static class Constraint
    {
        public static TwoWayConstraint<double, decimal> Round(int precision)
        {
            return new TwoWayConstraint<double, decimal>()
            {
                ConvertTo = Convert.ToDouble,
                ConvertFrom = dbl => Math.Round(Convert.ToDecimal(dbl), precision)
            };
        }
        public static TwoWayConstraint<int, int> Add(int amount){
            return new TwoWayConstraint<int,int>()
            {
                ConvertFrom = v => v + amount, 
                ConvertTo = v => v - amount
            };
        }

        public static TwoWayConstraint<int, int> Subtract(int amount){
            return Add(-amount);
        }

        public static TwoWayConstraint<double, double> Add(double amount){
            return new TwoWayConstraint<double,double>()
            {
                ConvertFrom = v => v + amount, 
                ConvertTo = v => v - amount
            };
        }

        public static TwoWayConstraint<double, double> Subtract(double amount){
            return Add(-amount);
        }

        public static TwoWayConstraint<double, double> Multiply(double multiplier)
        {
            return new TwoWayConstraint<double, double>()
            {
                ConvertFrom = v => v * multiplier, 
                ConvertTo = v => v / multiplier
            };
        }

    }
}
