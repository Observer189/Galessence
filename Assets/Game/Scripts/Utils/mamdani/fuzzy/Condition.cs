using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fuzzy
{
    public class Condition: Statement
    {
        public Condition(Variable v, FuzzySetInterface fsi) : base(v, fsi) { }
    }
}
