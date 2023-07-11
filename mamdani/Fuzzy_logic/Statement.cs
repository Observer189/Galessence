using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fuzzy_logic
{
    public class Statement
    {
        private FuzzySet term;

        private Variable variable;

        public Statement(FuzzySet _term, Variable _variable)
        {
            this.term = _term;
            this.variable = _variable;
        }
        public FuzzySet GetTerm()
        {
            return term;
        }

        public Variable GetVariable()
        {
            return variable;
        }

        public void SetTerm(FuzzySet fz)
        {
            term = fz;
        }

        public void SetVariable(Variable v)
        {
            variable = v;
        }
    }
}
