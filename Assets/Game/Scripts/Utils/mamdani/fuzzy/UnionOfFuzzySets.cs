using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fuzzy
{
    public class UnionOfFuzzySets: FuzzySetInterface
    {
        private List<FuzzySetInterface> fuzzySets = new List<FuzzySetInterface>();

        public UnionOfFuzzySets() { }

        public void AddFuzzySet(FuzzySetInterface fuzzySet)
        {
            fuzzySets.Add(fuzzySet);
        }

        private double getMaxValue(double x)
        {
            double result = 0.0;

            foreach (FuzzySetInterface fuzzySet in fuzzySets)
            {
                result = Math.Max(result, fuzzySet.GetValue(x));
            }

            return result;

           
        }

        public double GetValue(double value)
        {
            return getMaxValue(value);
        }
        public double GetLeftBase()
        {
            double result = double.MaxValue;

            foreach (FuzzySetInterface fuzzySet in fuzzySets)
            {
                result = Math.Min(result, fuzzySet.GetLeftBase());
            }

            return result;
        }

        public double GetRightBase()
        {
            double result = double.MinValue;

            foreach (FuzzySetInterface fuzzySet in fuzzySets)
            {
                result = Math.Max(result, fuzzySet.GetRightBase());
            }

            return result;
        }

        public double GetHeight()
        {
            double result = 0.0;

            foreach (FuzzySetInterface fuzzySet in fuzzySets)
            {
                result = Math.Max(result, fuzzySet.GetHeight());
            }

            return result;
        }

    }
}
