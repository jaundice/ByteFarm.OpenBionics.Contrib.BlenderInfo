using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByteFarm.OpenBionics.Contrib.BlenderInfo
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!args.Any() || !args.Last().EndsWith(".blend", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("No file path passed");
                return;
            }

            var reader = new BlenderReader(args.Last());

            foreach (var blenderToken in reader.ReadStructure())
            {
                Console.WriteLine();
                Console.WriteLine("==== File Block ====");
                Console.WriteLine();
                Console.WriteLine(blenderToken.ToString());
            }
        }
    }
}
