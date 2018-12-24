namespace expression {
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System;
    using Sprache;

    class Executor {
        public static void Execute(SourceFile sourceFile) {
            var preprocessed = Preprocessor.ProcessDirective(sourceFile);
            Logger.LogInfo("preprocessed");
            var commentLess = CommentProcessor.DeleteComments(preprocessed);
            Logger.LogInfo("comment processed");
            var program = ExprParser.MainParser.Parse(commentLess);
            Logger.LogInfo("parsed");
            CheckUndefinedVar(program);
            Logger.LogInfo("varable checked");
            Logger.LogInfo("ready to execute");
            var value = program.Calculate();
            Logger.LogInfo($"Program Ended with return value: {value}");
        }

        public static void CheckUndefinedVar(Expr program) {
            try {
                UndefinedVariableChecker.Check(program);
            } catch (VariableUndefinedException ex) {
                Logger.LogError($"undefined variable {ex.Variable} detected in");
                ex.PartialExpressions.Select(x => x + "\n---------------------------------------------").ToList().ForEach(Logger.LogError);
                Environment.Exit(1);
            }
        }
    }
}
