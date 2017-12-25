using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using static System.Console;

namespace OopSoftwareMetrics
{
    class Program
    {
        private const string ResultString = "From assembly {0} we get this metrics:\n" +
                                            "DIT: {1}\n" +
                                            "NOC: {2}\n" +
                                            "MHF: {3:0.0000}\n" +
                                            "AHF: {4:0.0000}\n" +
                                            "MIF: {5:0.0000}\n" +
                                            "AIF: {6:0.0000}\n" +
                                            "POF: {7:0.0000}\n";

        static void Main()
        {
            string input;
            input = ReadLine();
            var x = Stopwatch.StartNew();
            Metrics Metrics;
            try
            {
                if (input != null && !String.IsNullOrWhiteSpace(input))
                {
                    Metrics = MetricsAnalizer.GetAssemblyMetrics(Assembly.LoadFrom(input));
                    OutputMetrics(input, Metrics);
                }
                else
                {
                    Metrics = MetricsAnalizer.GetAssemblyMetrics(Assembly.LoadFrom("../../assemblies/BouncyCastle.Crypto.dll"));
                    OutputMetrics("BouncyCastle.Crypto.dll", Metrics);
                }
            }
            catch (Exception e)
            {
                WriteLine(e.Message);
            }

            x.Stop();
            WriteLine($"Counted by that time - {x.ElapsedMilliseconds} ms");
        }

        private static void OutputMetrics(string input, Metrics metrics)
        {
            WriteLine(ResultString, Path.GetFileName(input), metrics.DIT,
                    metrics.NOC, metrics.MHF,
                    metrics.AHF, metrics.MIF,
                    metrics.AIF, metrics.POF);
        }
    }
}
