using System;
using System.Diagnostics;
using System.IO;
using static System.Console;

namespace OopSoftwareMetrics
{
    class Program
    {
        //        static void Main()
        //        {
        //            string input = @"public class Aon<T,K> : IFoo, Gen, INoo<T>
        //{
        //	int d;
        //}

        //public class Aon<T,K> : Gen<T,K>, IGen, INoo<T>
        //{
        //  private class Foo
        //  {
        //  }
        //	int d;
        //}

        //public class Aon<T,K>
        //{
        //	int d;
        //}";

        //            string pattern = @"class\s*(\w+)(?:<\s*\w+(?:\s*,\s*\w+)*>)?(?:\s*:\s*(\w+))?";
        //            Regex regex = new Regex(pattern);

        //            foreach (Match match in regex.Matches(input))
        //            {
        //                Console.WriteLine(match.Groups[0].Value);
        //                var x = String.IsNullOrWhiteSpace(match.Groups[2].Value);
        //                Console.WriteLine(match.Groups[2].Value);

        //                Console.WriteLine(match.Value);
        //            }
        //        }

        private const string ResultString = "From module {0} we get this metrics:\n" +
                                            "Number of code lines: {1}\n" +
                                            "Number of empty code lines: {2}\n" +
                                            "Number of physical code lines: {3}\n" +
                                            "Number of logical code lines: {4}\n" +
                                            "Number of comment lines: {5}\n" +
                                            "Comment percantage: {6:0.0}%\n";

        static void Main()
        {
            string input;
            var metricsAnalizer = new MetricsAnalizer();

            while ((input = ReadLine()) != null && input != "exit")
            {
                try
                {
                    var x = Stopwatch.StartNew();
                    metricsAnalizer.GetModuleMetrics(input);
                    x.Stop();
                    WriteLine(x.ElapsedMilliseconds);
                }
                catch (Exception e)
                {
                    WriteLine(e);
                    continue;
                }

                OutPutMetrics(input, metricsAnalizer);
            }
        }

        private static void OutPutMetrics(string input, MetricsAnalizer metricsAnalizer)
        {
            //WriteLine(ResultString, Path.GetDirectoryName(input), metricsAnalizer.CodeLinesCount,
            //    metricsAnalizer.EmptyCodeLinesCount, metricsAnalizer.PhysicalCodeLinesCount,
            //    metricsAnalizer.LogicalCodeLinesCount, metricsAnalizer.CommentCodeLinesCount,
            //    metricsAnalizer.CommentPercent);
        }
    }
}
