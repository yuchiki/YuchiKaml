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
                    new BuiltInFunctionPair("print", Print),
                    new BuiltInFunctionPair("num_of_string", IntOfString),
                    new BuiltInFunctionPair("string_of_num", StringOfInt),
                    new BuiltInFunctionPair("chars_of_string", CharsOfString),
                    new BuiltInFunctionPair("concat_strings", ConcatStrings)
            }.ToImmutableDictionary();

        private static Value ReadChar(Value _) => new VString(((char) Console.Read()).ToString());
        private static Value ReadLine(Value _) => new VString(Console.ReadLine());
        private static Value Print(Value s) {
            Console.Write(s);
            return new VUnit();
        }

        // TODO: Implement it.
        private static Value CharsOfString(Value v) =>
            ((VString) v).Value.Reverse()
            .Aggregate(new CString("nil"), (Expr acc, char c) => new Abs("b", new If(new Var("b"), new CString(c.ToString()), acc)))
            .Calculate();

        private static Value ConcatStrings(Value v) {
            var str = "";
            Expr e = new Abs(((Closure) v).Variable, ((Closure) v).Body);
            while (!e.Equals(new CString("nil"))) {
                var ifExpr = (If) (((Abs) e).Body);
                str += ((CString) ifExpr.Left).Value;
                e = ifExpr.Right;
            }
            return new VString(str);
        }

        private static Value IntOfString(Value v) =>
            new VInt(int.Parse(((VString) v).Value));

        private static Value StringOfInt(Value v) =>
            new VString(((VInt) v).Value.ToString());

    }
}
