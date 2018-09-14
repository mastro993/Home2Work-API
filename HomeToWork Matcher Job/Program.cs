using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using data.Common;
using data.Repositories;

namespace HomeToWork_Matcher_Job
{
    class Program
    {


        static void Main(string[] args)
        {
            // Repositories
            var userRepo = new UserRepository();
            var locationRepo = new LocationRepository();

            // Routine calculator creation
            var routineCalculator = new RoutineCalculator(locationRepo);

            // Logging
            Console.Out.WriteLine("Avvio routine calculator!");
            var watch = System.Diagnostics.Stopwatch.StartNew();

            // Routine calculator execution
            foreach (var user in userRepo.GetAll())
            {
                var userRoutines = routineCalculator.GetRoutines(user);
            }

            // Logging
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.Out.WriteLine($"Ultimato in {elapsedMs / 1000.0} secondi");

            // End of program
        }
    }
}