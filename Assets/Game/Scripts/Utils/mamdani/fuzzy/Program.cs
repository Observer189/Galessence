using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fuzzy
{

    public class Test
    {

        public static List<int> Cold = new List<int> { 0, 0, 10, 15 };
        public static List<int> Cool = new List<int> { 10, 15, 20, 25 };
        public static List<int> Normal = new List<int> { 20, 25, 27, 30 };
        public static List<int> Warm = new List<int> { 27, 30, 35, 40 };
        public static List<int> Hot = new List<int> { 35, 40, 50, 50 };

      
        public static List<int> High = new List<int> {0, 0, 20, 30 };// кондиционер выпускает горячий воздух
        public static List<int> Medium = new List<int> { 20, 30, 60, 70 };// кондиционер выпускает ?теплый?(средний) воздух
        public static List<int> Low = new List<int> { 60, 70, 100, 100 };// кондиционер выпускает холодный воздух


    }
    

    public class Program
    {
       

        static void Main(string[] args)
        {
           /* List<Rule> rules = new List<Rule>();

            rules.Add(new Rule(new List<Condition>() { new Condition(new Variable(0),
                new FuzzySet(Test.Cold))}, 
                new List<Conclusion>() { new Conclusion(new Variable(0),
                new FuzzySet(Test.High), 1.0)}));

            rules.Add(new Rule(new List<Condition>() { new Condition(new Variable(0),
                new FuzzySet(Test.Cool))},
                new List<Conclusion>() { new Conclusion(new Variable(0),
                new FuzzySet(Test.Medium), 1.0)}));

            rules.Add(new Rule(new List<Condition>() { new Condition(new Variable(0),
                new FuzzySet(Test.Normal))},
                new List<Conclusion>() { new Conclusion(new Variable(0),
                new FuzzySet(Test.Low),1.0)}));
             
            rules.Add(new Rule(new List<Condition>() { new Condition(new Variable(0),
                new FuzzySet(Test.Warm))},
                new List<Conclusion>() { new Conclusion(new Variable(0),
                new FuzzySet(Test.Medium), 1.0)}));

            rules.Add(new Rule(new List<Condition>() { new Condition(new Variable(0),
                new FuzzySet(Test.Hot))},
                new List<Conclusion>() { new Conclusion(new Variable(0),
                new FuzzySet(Test.Low), 1.0)}));


            double[] inputData = new double[] { 36 };
            Mamdani mamdani = new Mamdani(rules, inputData);
            List<double> result = mamdani.execute();


            if (result[0] >= 70)
            {
                Console.WriteLine("Температура: " + inputData[0] + "\n" + result[0] + "\n=>Система кондиционирования работает на режиме холодный воздух");
            }
            else if (result[0] >= 30)
            {
                Console.WriteLine("Температура: " + inputData[0] + "\n" + result[0] + "\n=>Система кондиционирования работает на среднем режиме.");
            }
            else
            {
                Console.WriteLine("Температура: " + inputData[0] + "\n" + +result[0] + "\n=>Система кондиционирования работает на режиме горячий воздух.");
            }

            inputData = new double[] { 41 };
            mamdani = new Mamdani(rules, inputData);
            result = mamdani.execute();

            if (result[0] >= 70)
            {
                Console.WriteLine("Температура: " + inputData[0] + "\n" + result[0] + "\n=>Система кондиционирования работает на режиме холодный воздух");
            }
            else if (result[0] >= 30)
            {
                Console.WriteLine("Температура: " + inputData[0] + "\n" + result[0] + "\n=>Система кондиционирования работает на среднем режиме.");
            }
            else
            {
                Console.WriteLine("Температура: " + inputData[0] + "\n" + +result[0] + "\n=>Система кондиционирования работает на режиме горячий воздух.");
            }

            Console.ReadLine();*/

        }
    }
}
