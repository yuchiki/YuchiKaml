namespace expression {
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.IO;
    using System.Linq;
    using System;

    using BuiltInFunction = System.Func<Value, Value>;
    using BuiltInFunctionPair =
        System.Collections.Generic.KeyValuePair<string, System.Func<Value, Value>>;

    public static class BuiltInFunctions {
        public static ImmutableDictionary<string, BuiltInFunction> BuiltIns =
            new [] {
                new BuiltInFunctionPair("read_char", ReadChar),
                    new BuiltInFunctionPair("read_line", ReadLine),
                    new BuiltInFunctionPair("put_string", PutString),
                    new BuiltInFunctionPair("chars_of_string", CharsOfString),
                    new BuiltInFunctionPair("concat_strings", ConcatStrings)

            }.ToImmutableDictionary();

        private static Value ReadChar(Value _) => new VString(((char) Console.Read()).ToString());
        private static Value ReadLine(Value _) => new VString(Console.ReadLine());
        private static Value PutString(Value s) {
            Console.Write(((VString) s).Value);
            return new VUnit();
        }

        // TODO: Implement it.
        private static Value CharsOfString(Value v) =>
            throw new NotImplementedException();
        // TODO: Implement it.
        private static Value ConcatStrings(Value v) =>
            throw new NotImplementedException();

    }
}
