using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fuzzy_logic
{
    public class Conclusion: Statement
    {
        private double weight;
        public Conclusion(Variable variable, FuzzySet term, double weight) : base(variable, term) { this.weight = weight; }
        public double GetWeight()
        {
            return weight;
        }

        public void SetWeight(double w)
        {
            weight = w;
        }
    }
}
