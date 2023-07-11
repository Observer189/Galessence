using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fuzzy
{
    public class Conclusion: Statement
    {
        private double weight;

        public Conclusion(Variable v, FuzzySetInterface fsi, double w): base(v, fsi)
        {
            this.weight = w;
        }

        public double GetWeight() { return weight; }

    }
}
