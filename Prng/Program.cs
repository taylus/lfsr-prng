using System;
using System.Linq;

namespace Prng
{
    public class Program
    {
        public static void Main()
        {
            ushort seed = 0xbabe;
            //var seed = (ushort)DateTime.Now.Ticks;

            PrintRun(seed, iterations: 20);
            //PrintAnalysis(seed, iterations: 1_000_000);
        }

        /// <summary>
        /// Prints the given # of random numbers generated using the given seed.
        /// </summary>
        private static void PrintRun(ushort seed, int iterations)
        {
            var rng = new LinearFeedbackShiftRegisterPrng(seed);

            Console.WriteLine($"Seed: {seed:x4}");
            for (int i = 0; i < iterations; i++)
            {
                byte result = rng.Generate();
                Console.WriteLine($"Random number: {result:x2}");
            }
        }

        /// <summary>
        /// Prints the distribution of generating the given number of random
        /// numbers using the given seed.
        /// </summary>
        private static void PrintAnalysis(ushort seed, int iterations)
        {
            var rng = new LinearFeedbackShiftRegisterPrng(seed);

            var resultCounts = Enumerable.Range(0, 256)
                .Select(i => new { Key = (byte)i, Value = 0 })
                .ToDictionary(k => k.Key, v => v.Value);

            Console.WriteLine($"Seed: {seed:x4}");
            for (int i = 0; i < iterations; i++)
            {
                byte result = rng.Generate();
                resultCounts[result]++;
            }

            Console.WriteLine($"Distribution of random numbers generated over {iterations:n0} iterations:{Environment.NewLine}");
            Console.WriteLine(" # | Occurrences");
            Console.WriteLine("---+------------");
            foreach (var kvp in resultCounts)
            {
                Console.WriteLine($"{kvp.Key:x2} | {kvp.Value:n0}");
            }
        }
    }
}
