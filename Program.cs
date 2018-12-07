namespace expression {
    using System.Collections.Generic;
    using System.Linq;
    using System;

    class Program {
        static void Main(string[] args) {
            var sumBody =
                new If(
                    new Eq(new Var("n"), new CInt(0)),
                    new CInt(0),
                    new Add(
                        new Var("n"),
                        new App(
                            new Var("sum"),
                            new Sub(new Var("n"), new CInt(1)))));
            var e2 = new LetRec(
                "sum", "n", sumBody,
                new App(
                    new Var("sum"),
                    new CInt(10)));
            ShowExp(e2);
            TestEval("recursive sum", e2, new VInt(55));
        }

        public static void TestEval(string name, Expr e, Value expected) {
            Console.Write($"Test \"{name}\": ");
            var result = e.Calculate();
            if (expected.Equals(result)) {
                Console.WriteLine("OK");
            } else {
                Console.WriteLine("NG");
                Console.WriteLine($"Expression:{e}, Expected:{expected}, Result:{result}");
                throw new ArgumentException();
            }
        }

        static void ShowExp(Expr e) => Console.WriteLine($" {e} ==> {e.Calculate()}");
    }
}
