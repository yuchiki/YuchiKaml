namespace YuchikiML {
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System;
    using CommandLine;

    class Program {
        static void Main(string[] args) {
            // TODO: Add argParser;
            Logger.Criteria = Logger.ErrorLevel.Error;

            Parser.Default.ParseArguments<CommandLineOptions>(args).WithParsed<CommandLineOptions>(
                opt => {
                    ValidateOpt(opt);
                    Console.WriteLine(opt.ErrorLevel);
                    Logger.Criteria = opt.ErrorLevel;
                    if (opt.Verbose) Logger.Criteria = Logger.ErrorLevel.Trace;
                    if (opt.IsTestMode) {
                        TestSuits.Test();
                        return;
                    }
                    Executor.Execute(new SourceFile(opt.FileName));
                }
            );
        }

        static void ValidateOpt(CommandLineOptions opt) {
            if (opt.FileName != null && opt.IsTestMode) {
                Console.Error.WriteLine(" File name cannot be designated in test mode.");
                Environment.Exit(1);
            } else if (opt.FileName == null && !opt.IsTestMode) {
                Console.Error.WriteLine(" File not designated.");
                Console.Error.WriteLine("--help to see help.");
                Environment.Exit(1);
            }
        }

    }
}
