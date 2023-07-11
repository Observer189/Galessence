using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fuzzy
{
    public interface FuzzySetInterface
    {
     
        double GetValue(double v);

        double GetLeftBase();
        double GetRightBase();
        double GetHeight();
    }
}
