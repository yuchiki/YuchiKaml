namespace YuchikiML {
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System;
    using Sprache;

    public struct SourceFile {
        public string DirectoryPath { get; }
        public string Code { get; }
        public SourceFile(String Name) =>
            (DirectoryPath, Code) = (Path.GetDirectoryName(Name), File.ReadAllText(Name));
    }
}
