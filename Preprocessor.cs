namespace expression {
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System;
    using Sprache;

    public static class Preprocessor {
        public static char dq => '"';
        public static string ProcessDirective(SourceFile source) =>
            new Regex("#include\"([\\w/.]+)\"")
            .Replace(source.Code, s => ProcessDirective(new SourceFile(source.DirectoryPath + "/" + s.Groups[1].Value)));
    }
}
