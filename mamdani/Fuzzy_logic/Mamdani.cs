using Fuzzy_logic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace Fuzzy_logic
{
    public class Mamdani
    {
        private int numberOfConclusions;
        private int numberOfConditions;
        private int numberOfInputVariables;
        private int numberOfOutputVariables;
        private int numberOfRules;
        private List<Rule> rules;
        public Mamdani(int _numbOfConc, int _numbOfCond, int _numbOfInp, int _numbOfOut, int _numbOfRul, List<Rule> _rules)
        {
            this.numberOfConclusions = _numbOfConc;
            this.numberOfConditions = _numbOfCond;
            this.numberOfInputVariables = _numbOfInp;
            this.numberOfOutputVariables = _numbOfOut;
            this.numberOfRules = _numbOfRul;
            this.rules = _rules;
        }

        private List<UnionOfFuzzySets> Accumulation(List<ActivatedFuzzySet> activatedFuzzySets)
        {

            List<UnionOfFuzzySets> unionsOfFuzzySets = new List<UnionOfFuzzySets>(numberOfOutputVariables);

            foreach (Rule rule in rules) {
                foreach (Conclusion conclusion in rule.GetConclusions()) {
                    int id = conclusion.GetVariable().GetId();
                    unionsOfFuzzySets[id].AddFuzzySet(activatedFuzzySets[id]); 
                } 
            }

            return unionsOfFuzzySets;
        }

        private List<ActivatedFuzzySet> Activation(double[] c)
        {
            
            int i = 0; 
            List<ActivatedFuzzySet> activatedFuzzySets = new List<ActivatedFuzzySet>(); 
            double[] d = new double[numberOfConclusions];

            foreach (Rule rule in rules)
            {
                foreach (Conclusion conclusion in rule.GetConclusions()) { 
                    d[i] = c[i] * conclusion.GetWeight(); 
                    ActivatedFuzzySet activatedFuzzySet = (ActivatedFuzzySet)conclusion.GetTerm(); 
                    activatedFuzzySet.SetTruthDegree(d[i]); 
                    activatedFuzzySets.Add(activatedFuzzySet); i++; }
            }

            return activatedFuzzySets;

        }

        private double[] Aggregation(double[] b)
        {
            

            int i = 0; int j = 0; double[] c = new double[numberOfInputVariables];

            foreach (Rule rule in rules)
            {
                double truthOfConditions = 1.0;

                foreach (Condition condition in rule.GetConditions())
                {
                    truthOfConditions = Math.Min(truthOfConditions, b[i]);
                    i++;
                }

                c[j] = truthOfConditions;
                j++;
            }

            return c;
        }

        private double[] Defuzzification(List<UnionOfFuzzySets> unionsOfFuzzySets)
        {
            double[] y = new double[numberOfOutputVariables];

            for (int i = 0; i < numberOfOutputVariables; i++) {
                double i1 = Integral(unionsOfFuzzySets[i], true); 
                double i2 = Integral(unionsOfFuzzySets[i], false);
                y[i] = i1 / i2;
            }

            return y;
        }

        public double[] execute(double[] v, List<Rule> r)
        {
            double[] fuzzificated = Fuzzification(v);
            double[] aggregated = Aggregation(fuzzificated);
            List<ActivatedFuzzySet> activated = Activation(aggregated);
            List<UnionOfFuzzySets> accumulated = Accumulation(activated);
            return Defuzzification(accumulated);
        }

        private double[] Fuzzification(double[] inputData)
        {
            int i = 0;
            double[] b = new double[numberOfConditions];
            foreach (Rule rule in rules)
            {
                foreach (Condition condition in rule.GetConditions())
                {
                    int j = condition.GetVariable().GetId();
                    FuzzySet term = condition.GetTerm();
                    b[i] = term.GetValue(inputData[j]);
                    i++;
                }
            }

            return b;
        }

      
        private double Integral(FuzzySetIface fuzzySet, bool b)
        {
            //Func<double, double> function = b ? (x => x * fuzzySet.GetValue(x)) : fuzzySet.GetValue;

            Func<double, double> function;
            if (b)
            {
                function = delegate (double x) { return x * fuzzySet.GetValue(x); };
            }
            else
            {
                function = delegate (double x) { return fuzzySet.GetValue(x); };
            }
            return Integrate(0, 100, function);
        }


        public static double Integrate(double a, double b, Func<double, double> function)
        {
        int N = 10000;
        double h = (b - a) / (N - 1);

        double sum = (1.0 / 3.0) * (function(a) + function(b));

        for (int i = 1; i < N - 1; i += 2)
        {
            double x = a + h * i;
            sum += (4.0 / 3.0) * function(x);
        }

        for (int i = 2; i < N - 1; i += 2)
        {
            double x = a + h * i;
            sum += (2.0 / 3.0) * function(x);
        }

        return sum * h;
        }
       

    }

   
}
