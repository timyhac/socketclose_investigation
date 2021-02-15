using Example;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetFramework
{
    class Program
    {
        static void Main(string[] args)
        {
            var method = new MethodThatThrows();

            try
            {
                var result = method.Run();
                Console.WriteLine($"Result = {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.GetType()} {ex.Message}");
            }

            Console.WriteLine(method.Log);
            Console.ReadKey();
        }
    }
}
