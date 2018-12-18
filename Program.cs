namespace expression {
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System;
    using Sprache;

    class Program {
        static void Main(string[] arg) {
            if (arg.Length == 0) {
                Console.WriteLine("No file specified.");
                Console.WriteLine("Tests are run.");
                TestSuits.Test();
                return;
            }

            var text = File.ReadAllText(arg[0]);
            var program = ExprParser.MainParser.Parse(text);
            var value = program.Calculate();
            Console.WriteLine($"return value is: {value}");
        }
    }
}
