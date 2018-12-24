namespace YuchikiML {
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System;
    using Sprache;

    public static class Preprocessor {
        public static char dq => '"';
        public static string ProcessDirective(SourceFile source) {
            var locallyIncluded = new Regex("#include\"([\\w/.]+)\"")
                .Replace(source.Code, s => ProcessDirective(new SourceFile(source.DirectoryPath + "/" + s.Groups[1].Value)));

            var FullyIncluded = new Regex("#include<([\\w/.]+)>")
                .Replace(locallyIncluded, s => ProcessDirective(new SourceFile(Environment.CurrentDirectory + "/Library/" + s.Groups[1].Value)));
            return FullyIncluded;
        }
    }
}
