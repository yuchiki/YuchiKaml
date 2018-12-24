namespace expression {
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System;
    using Sprache;

    class Executor {
        public static void Execute(SourceFile sourceFile) {
            var preprocessed = Preprocessor.ProcessDirective(sourceFile);
            Console.WriteLine("preprocessed");
            var commentLess = CommentProcessor.DeleteComments(preprocessed);
            Console.WriteLine("comment processed");
            var program = Parse(commentLess);
            Console.WriteLine("parsed");
            CheckUndefinedVar(program);
            Console.WriteLine("varable checked");
            Console.WriteLine("ready to execute");
            var value = program.Calculate();
            Console.WriteLine($"Program Ended with return value: {value}");
        }

        public static Expr Parse(String commentLess) {
            try {
                return ExprParser.MainParser.Parse(commentLess);
            } catch (Sprache.ParseException ex) {
                Console.WriteLine("caught");
                Console.WriteLine(ex.Message);
                foreach (var item in ex.Data.Keys) {
                    Console.WriteLine($"{item} :-> {ex.Data[item]}");
                }
                throw;
            }
        }

        public static void CheckUndefinedVar(Expr program) {
            try {
                UndefinedVariableChecker.Check(program);
            } catch (VariableUndefinedException ex) {
                Console.Error.WriteLine($"undefined variable {ex.Variable} detected in");
                ex.PartialExpressions.Select(x => x + "\n---------------------------------------------").ToList().ForEach(Console.WriteLine);
                Environment.Exit(1);
            }
        }
    }
}
