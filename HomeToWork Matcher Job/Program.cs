using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using data.Common;

namespace HomeToWork_Matcher_Job
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.Out.WriteLine("Avvio routine calculator!");

            var watch = System.Diagnostics.Stopwatch.StartNew();
            RoutineCalculator.execute();
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            Console.Out.WriteLine($"Ultimato in {elapsedMs/1000} secondi");


        }
    }
}
