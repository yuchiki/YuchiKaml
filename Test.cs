namespace expression {
    using System.Collections.Generic;
    using System.Linq;
    using System;

    static class Test {
        public static void TestEval(string name, Expr e, Value expected) {
            Console.Write($"Test \"{name}\": ");
            var result = e.Calculate();
            if (expected.Equals(result)) {
                Console.WriteLine("OK");
            } else {
                Console.WriteLine("NG");
                Console.WriteLine($"Expression:{e}, Expected:{expected}, Result:{result}");
                System.Environment.Exit(1);
            }
        }
    }
}
