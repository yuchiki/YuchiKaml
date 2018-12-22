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
            var program = ExprParser.MainParser.Parse(commentLess);
            Console.WriteLine("parsed");
            CheckUndefinedVar(program);
            Console.WriteLine("checked");
            var value = program.Calculate();
            Console.WriteLine($"return value is: {value}");
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
