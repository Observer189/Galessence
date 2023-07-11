using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace fuzzy
{
    public class Mamdani
    {
        private List<Rule> rules;
        private double[] inputData;

        public Mamdani(List<Rule> rules, double[] inputData)
        {
            this.rules = rules;
            this.inputData = inputData;
        }

        public List<double> execute()
        {
            List<double> fuzzificated = Fuzzification(inputData);
            List<double> aggregated = Aggregation(fuzzificated);
            List<ActivatedFuzzySet> activated = Activation(aggregated);
            List<UnionOfFuzzySets> accumulated = Accumulation(activated);
            return Defuzzification(accumulated);
        }


        private List<double> Fuzzification(double[] inputData)
        {
            List<double> b = new List<double>();
            foreach (Rule rule in rules)
            {
                foreach (Condition condition in rule.GetConditions())
                {
                    int j = condition.GetVariable().GetId();
                    FuzzySetInterface term = condition.GetTerm();
                    b.Add(term.GetValue(inputData[j]));
                }
            }
            return b;
        }

        private List<double> Aggregation(List<double> b)
        {
            int i = 0;
            int j = 0;
            List<double> c = new List<double>();
            foreach (Rule rule in rules)
            {
                double truthOfConditions = 1.0;
                foreach (Condition condition in rule.GetConditions())
                {
                    truthOfConditions = Math.Min(truthOfConditions, b[i]);
                    i++;
                }
                c.Add(truthOfConditions);
            }
            return c;
        }

        private List<ActivatedFuzzySet> Activation(List<double> c)
        {
            int i = 0;
            List<ActivatedFuzzySet> activatedFuzzySets = new List<ActivatedFuzzySet>();
            foreach (Rule rule in rules)
            {
                foreach (Conclusion conclusion in rule.GetConclusions())
                {
                    activatedFuzzySets.Add(new ActivatedFuzzySet(
                            conclusion.GetTerm(), c[i] * conclusion.GetWeight()
                    ));
                }
                i++;
            }
            return activatedFuzzySets;
        }


        private List<UnionOfFuzzySets> Accumulation(List<ActivatedFuzzySet> activatedFuzzySets)
        {
            Dictionary<int, UnionOfFuzzySets> unionsOfFuzzySets = new Dictionary<int, UnionOfFuzzySets>();
            int i = 0;
            foreach (Rule rule in rules)
            {
                foreach (Conclusion conclusion in rule.GetConclusions())
                {
                    int index = conclusion.GetVariable().GetId();
                    if (!unionsOfFuzzySets.ContainsKey(index))
                    {
                        unionsOfFuzzySets.Add(index, new UnionOfFuzzySets());
                    }
                    unionsOfFuzzySets[index].AddFuzzySet(activatedFuzzySets[i]);
                    i++;
                }
            }
            return new List<UnionOfFuzzySets>(unionsOfFuzzySets.Values);
        }

        private List<double> Defuzzification(List<UnionOfFuzzySets> unionsOfFuzzySets)
        {
            List<double> y = new List<double>();
            foreach (UnionOfFuzzySets unionOfFuzzySets in unionsOfFuzzySets)
            {
                double i1 = Integral(unionOfFuzzySets, true);
                double i2 = Integral(unionOfFuzzySets, false);
                //Debug.Log($"i1 = {i1}, i2 = {i2}");
                y.Add(i1 / i2);
            }
            return y;
        }

        private double Integral(FuzzySetInterface fuzzySet, bool b)
        {
            double leftBase = fuzzySet.GetLeftBase();
            double rightBase = fuzzySet.GetRightBase();
            double height = fuzzySet.GetHeight();

            int numSubintervals = 10; // ���������� �������������
            double subintervalWidth = (rightBase - leftBase) / numSubintervals; // ������ ������� ������������

            double integral = 0.0;

            Func<double, double> function;
            if (b)
            {
                function = delegate (double x) { return x * fuzzySet.GetValue(x); };
            }
            else
            {
                function = delegate (double x) { return fuzzySet.GetValue(x); };
            }

            for (int i = 0; i < numSubintervals; i++)
            {
                double subintervalLeft = leftBase + i * subintervalWidth;
                double subintervalRight = subintervalLeft + subintervalWidth;

                double subintervalIntegral = Integrate(subintervalLeft, subintervalRight, function);
                integral += subintervalIntegral;
            }

            return integral;
        }


        public static double Integrate(double a, double b, Func<double, double> function)
        {
            double h = (function(a) + function(b)) / 2; // ������� �������� ������� �� ����� � ������ ��������
            double baseWidth = b - a; // ������ ��������� ��������

            return h * baseWidth; // ������� ��������
        }

    }
}