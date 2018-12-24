namespace expression {
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System;

    public static class Logger {
        public enum ErrorLevel {
            Error = 10,
            Warn = 20,
            Info = 30
        }

        public static ErrorLevel Criteria = ErrorLevel.Info;

        public static void LogInfo(string str) {
            if (ErrorLevel.Info <= Criteria) Console.Error.WriteLine(str);
        }
        public static void LogWarn(string str) {
            if (ErrorLevel.Warn <= Criteria) Console.Error.WriteLine(str);
        }

        public static void LogError(string str) {
            if (ErrorLevel.Error <= Criteria) Console.Error.WriteLine(str);
        }
    }
}
