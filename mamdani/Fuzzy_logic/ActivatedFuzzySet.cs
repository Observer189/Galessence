using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fuzzy_logic
{
    public class ActivatedFuzzySet: FuzzySet
    {
        private double truthDegree;

        public ActivatedFuzzySet(double _truthDegree)
        {
            this.truthDegree = _truthDegree;
        }
        private double GetActivatedValue(double x)
        {
           
            return Math.Min(base.GetValue(x), truthDegree); 
        }

        public void SetTruthDegree(double td)
        {
            truthDegree = td;
        }

        public double GetValue(double v)
        {
            return v;
        }
    }
}
