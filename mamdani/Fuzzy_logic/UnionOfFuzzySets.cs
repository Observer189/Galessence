using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fuzzy_logic
{
    public class UnionOfFuzzySets: FuzzySetIface
    {
        private List<FuzzySet> fuzzySets;
        public UnionOfFuzzySets(List<FuzzySet> _fuzzySets)
        {
            this.fuzzySets = _fuzzySets;
        }

        public void AddFuzzySet(FuzzySet fs)
        {
            this.fuzzySets.Add(fs);
        }

        private double GetMaxValue(double x)
        {
            double result = 0.0;

            foreach (FuzzySet fuzzySet in fuzzySets)
            {
                result = Math.Max(result, fuzzySet.GetValue(x));
            }

            return result;
        }

        public double GetValue(double v)
        {
            return v;
        }

    }

}
