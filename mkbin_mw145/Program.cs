using System;
using System.IO;
using System.Reflection;

namespace mkbin_mw145
{
    class Program
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                PrinterCommon aPrinter = new MW145(args[0], args[1]);
                aPrinter.Exec();
            }
            else
            {
                Console.Out.WriteLine("Usage: {0} inputFile outputFile", Path.GetFileName(Assembly.GetExecutingAssembly().Location));
            }
        }
    }
}
