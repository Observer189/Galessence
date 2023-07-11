using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fuzzy_logic
{
    public class Variable
    {
        private int id;
        public HashSet<FuzzySet> terms;
        public Variable(int _id, HashSet<FuzzySet> _terms)
        {
            this.id = _id;
            this.terms = _terms;
        }
        public int GetId()
        {
            return id;
        }

        public void SetId(int id)
        {
            this.id = id;
        }


    }
}
