using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fuzzy
{
    public class ActivatedFuzzySet: FuzzySetInterface
    {
        private FuzzySetInterface fuzzySet;

        private double truthDegree;

        public ActivatedFuzzySet(FuzzySetInterface fuzzySet, double truthDegree)
        {
            this.fuzzySet = fuzzySet;
            this.truthDegree = truthDegree;
        }

        private double GetActivatedValue(double x)
        {
            return Math.Min(fuzzySet.GetValue(x), truthDegree);
        }
        public double GetValue(double x)
        {
            return GetActivatedValue(x);
        }
        public double GetLeftBase()
        {
            return fuzzySet.GetLeftBase();
        }

        public double GetRightBase()
        {
            return fuzzySet.GetRightBase();
        }

        public double GetHeight()
        {
            return fuzzySet.GetHeight();
        }

    }
}
