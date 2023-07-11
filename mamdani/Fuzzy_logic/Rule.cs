using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fuzzy_logic
{
    public class Rule
    {
        private List<Conclusion> conclusions;
        private List<Condition> conditions;
        public Rule(List<Condition> conditions, List<Conclusion> conclusions)
        {
            this.conditions = conditions;
            this.conclusions = conclusions;
        }

        public List<Conclusion> GetConclusions()
        {
            return conclusions;
        }

        public List<Condition> GetConditions()
        {
            return conditions;
        }
    }
}
