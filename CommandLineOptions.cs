namespace YuchikiML {
    using CommandLine;

    class CommandLineOptions {
        [Option("verbose", Required = false, HelpText = "Run this interpreter in verbose mode.")]
        public bool Verbose { get; set; }

        [Option('l', "level", Required = false, HelpText = "Designate Trace|Info|Warn|Error. Default is Error.")]
        public Logger.ErrorLevel ErrorLevel { get; set; }

        [Option("test", Required = false, HelpText = "Run test")]
        public bool IsTestMode { get; set; }

        [Value(0, MetaName = "FileName", HelpText = "Designate filename")]
        public string FileName { get; set; }
    }
}
