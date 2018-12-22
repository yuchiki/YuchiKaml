namespace expression {
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System;
    using Sprache;

    class Executor {
        public static void Execute(SourceFile sourceFile) {
            var preprocessed = Preprocessor.ProcessDirective(sourceFile);
            var commentLess = CommentProcessor.DeleteComments(preprocessed);
            Console.WriteLine(commentLess);
            var program = ExprParser.MainParser.Parse(commentLess);
            CheckUndefinedVar(program);
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
