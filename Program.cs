namespace expression {
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System;
    using Sprache;

    class Program {
        static void Main(string[] arg) {
            // TODO: Add argParser;
            Logger.Criteria = Logger.ErrorLevel.Error;

            if (arg.Length == 0) {
                Console.WriteLine("No file specified.");
                Console.WriteLine("Tests are run.");
                TestSuits.Test();
                return;
            }

            var sourceFile = new SourceFile(arg[0]);
            Executor.Execute(sourceFile);
        }
    }
}
