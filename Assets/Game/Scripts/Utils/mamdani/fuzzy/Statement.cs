using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fuzzy
{
    public class Statement
    {
        private FuzzySetInterface term;

        private Variable variable;

        public Statement(Variable _variable, FuzzySetInterface fsi)
        {
            this.variable = _variable;
            this.term = fsi;
        }
        public FuzzySetInterface GetTerm()
        {
            return term;
        }

        public Variable GetVariable()
        {
            return variable;
        }

        public void SetTerm(FuzzySetInterface fz)
        {
            term = fz;
        }

        public void SetVariable(Variable v)
        {
            variable = v;
        }
    }
}
