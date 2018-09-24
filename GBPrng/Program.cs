using System;
using System.Linq;
using System.Diagnostics;

namespace GBPrng
{
    public class Program
    {
        private const int Iterations = 1000000;

        public static void Main()
        {
            //ushort seed = 0xbabe;
            var seed = (ushort)DateTime.Now.Ticks;
            var rng = new LinearFeedbackShiftRegisterPrng(seed);

            var resultCounts = Enumerable.Range(0, 256)
                .Select(i => new { Key = (byte)i, Value = 0 })
                .ToDictionary(k => k.Key, v => v.Value);

            for (int i = 0; i < Iterations; i++)
            {
                byte result = rng.Generate();
                resultCounts[result]++;

                //print the first few iterations for comparing runs w/ same seed
                if (i < 10) Debug.WriteLine(result);
            }

            Console.WriteLine($"Distribution of random numbers generated over {Iterations:n0} iterations:{Environment.NewLine}");
            Console.WriteLine("  # | Occurrences");
            Console.WriteLine("----+------------");
            foreach (var kvp in resultCounts)
            {
                Console.WriteLine($"{kvp.Key,3} | {kvp.Value:n0}");
            }
        }
    }
}
