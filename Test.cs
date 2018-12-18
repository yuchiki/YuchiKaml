namespace expression {
    using System.Collections.Generic;
    using System.Linq;
    using System;

    static class Test {
        public static void TestEval(string name, Expr e, Value expected) {
            Console.Write($"Test {name,-20}: ");
            var result = e.Calculate();
            if (expected.Equals(result)) {
                Console.WriteLine("OK");
            } else {
                Console.WriteLine("NG");
                Console.WriteLine($"Expression:{e}, Expected:{expected} of {expected.GetType()}, Result:{result} of {result.GetType()}");
                System.Environment.Exit(1);
            }
        }

        public static void ShouldBeEqual<T>(this T t, Object expected, string name) {
            Console.Write($"Test {name, -20}: ");
            if (expected.Equals(t)) {
                Console.WriteLine("OK");
            } else {
                Console.WriteLine("NG");
                Console.WriteLine($"Expected:{expected} of {expected.GetType()}, Result:{t} of {t.GetType()}");
                System.Environment.Exit(1);
            }
        }
    }
}
