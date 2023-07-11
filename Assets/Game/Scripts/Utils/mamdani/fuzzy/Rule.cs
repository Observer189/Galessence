using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fuzzy
{
    public class Rule
    {
        private List<Condition> conditions;

        private List<Conclusion> conclusions;

        public Rule(List<Condition> conditions, List<Conclusion> conclusions)
        {
            this.conditions = conditions;
            this.conclusions = conclusions;
        }

        public List<Condition> GetConditions()
        {
            return conditions;
        }

        public List<Conclusion> GetConclusions()
        {
            return conclusions;
        }

    }
}
