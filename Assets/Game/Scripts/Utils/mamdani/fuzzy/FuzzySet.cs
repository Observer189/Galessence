using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace fuzzy
{
    public  class FuzzySet: FuzzySetInterface
    {

        private List<double> trapezoidIntervals;

        public FuzzySet(List<double> intervals)
        {
            trapezoidIntervals = intervals.Select(i => (double)i).ToList();
        }

        public double GetLeftBase()
        {
            return trapezoidIntervals[0];
        }

        public double GetRightBase()
        {
            return trapezoidIntervals[3];
        }

        public double GetHeight()
        {
            return Math.Max(trapezoidIntervals[1] - trapezoidIntervals[0], trapezoidIntervals[3] - trapezoidIntervals[2]);
        }

        //?? пока трапезоида 
        public double GetValue(double value)
        {
            if (value < trapezoidIntervals[0])
            {
                if (Mathf.Abs((float)(trapezoidIntervals[0] - trapezoidIntervals[1])) < 0.0001)
                {
                    return 1;
                }

                return 0;
            }
            else if (trapezoidIntervals[0] < value && trapezoidIntervals[1] > value)
            {
                return (value - trapezoidIntervals[0]) / (trapezoidIntervals[1] - trapezoidIntervals[0]);
            }
            else if (trapezoidIntervals[1] <= value && trapezoidIntervals[2] >= value)
            {
                return 1;
            }
            else if (trapezoidIntervals[2] < value)
            {
                if (Mathf.Abs((float)(trapezoidIntervals[2] - trapezoidIntervals[3])) < 0.0001)
                {
                    return 1;
                }
                else if (trapezoidIntervals[3] > value)
                {
                    return (trapezoidIntervals[3] - value) / (trapezoidIntervals[3] - trapezoidIntervals[2]);
                }
            }
            else
            {
                return 0;
            }

            return 0;
        }
    }


}

