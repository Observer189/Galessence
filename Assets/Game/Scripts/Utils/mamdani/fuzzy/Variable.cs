using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fuzzy
{
    public class Variable
    {
        private int id;

        public Variable(int _id)
        {
            this.id = _id;
        }

        public int GetId() { return id; }
    }
}
