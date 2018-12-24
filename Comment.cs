namespace YuchikiML {
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System;
    using Sprache;

    public static class CommentProcessor {
        public static string DeleteComments(string source) {
            var builder = new StringBuilder();
            var first = 0;
            var current = 0;
            while (current < source.Length - 1) {
                if (source[current] == '/' && source[current + 1] == '/') {
                    builder.Append(source, first, current - first);
                    current += 2;
                    while (source[current] != '\n') current++;
                    builder.Append('\n');
                    current++;
                    first = current;
                } else if (source[current] == '(' && source[current + 1] == '*') {
                    builder.Append(source, first, current - first);
                    current += 2;
                    while (source[current] != '*' || source[current + 1] != ')') current++;
                    current += 2;
                    first = current;
                } else {
                    current++;
                }
            }
            builder.Append(source, first, current - first);
            return builder.ToString();
        }
    }
}
