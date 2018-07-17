using System;
using System.Linq;
using BurnOutSharp;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (string arg in args)
            {
                Console.WriteLine(String.Join("\r\n", ProtectionFind.Scan(arg).Select(kvp => kvp.Key + ": " + kvp.Value)));
            }

            Console.ReadLine();
        }
    }
}
