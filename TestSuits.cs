namespace expression {
    using System.Collections.Generic;
    using System.Linq;
    using System;
    using Sprache;

    static class TestSuits {

        public static void Test() {
            Console.WriteLine("EVALUATION TEST");
            EvaluationTest();
            Console.WriteLine("");
            Console.WriteLine("PARSING TEST");
            ParsingTest();

        }

        public static void ParsingTest() {
            ExprParser.MainParser.Parse(" 12 ").Cast<CInt>().Value.ShouldBeEqual(12, "int");
            ExprParser.MainParser.Parse(" true ").Cast<CBool>().Value.ShouldBeEqual(true, "true");
            ExprParser.MainParser.Parse(" false ").Cast<CBool>().Value.ShouldBeEqual(false, "false");
            ExprParser.MainParser.Parse(" foo").Cast<Var>().Name.ShouldBeEqual("foo", "var");
            ExprParser.MainParser.Parse(" ( 12 ) ").Cast<CInt>().Value.ShouldBeEqual(12, "paren");
            ExprParser.MainParser.Parse(" x y z ").Cast<App>().Left.Cast<App>().Left.Cast<Var>().Name.ShouldBeEqual("x", "app1");
            ExprParser.MainParser.Parse(" x y z ").Cast<App>().Left.Cast<App>().Right.Cast<Var>().Name.ShouldBeEqual("y", "app2");
            ExprParser.MainParser.Parse(" x y z ").Cast<App>().Right.Cast<Var>().Name.ShouldBeEqual("z", "app3");

            ExprParser.MainParser.Parse(" !x ").Cast<Not>().Body.Cast<Var>().Name.ShouldBeEqual("x", "not");
            ExprParser.MainParser.Parse(" !!x ").Cast<Not>().Body.Cast<Not>().Body.Cast<Var>().Name.ShouldBeEqual("x", "notnot");
            ExprParser.MainParser.Parse(" ! ! x ").Cast<Not>().Body.Cast<Not>().Body.Cast<Var>().Name.ShouldBeEqual("x", "not not");

            ExprParser.MainParser.Parse("2*3").Cast<Mul>().Left.Cast<CInt>().Value.ShouldBeEqual(2, "nummulnum");
            ExprParser.MainParser.Parse("2 * 3").Cast<Mul>().Left.Cast<CInt>().Value.ShouldBeEqual(2, "num mul num");
            ExprParser.MainParser.Parse("2 * 3 / 4").Cast<Div>().Right.Cast<CInt>().Value.ShouldBeEqual(4, "mul1");
            ExprParser.MainParser.Parse("2 * 3 / 4").Cast<Div>().Left.Cast<Mul>().Left.Cast<CInt>().Value.ShouldBeEqual(2, "mul2");
            ExprParser.MainParser.Parse("2 * 3 / 4").Cast<Div>().Left.Cast<Mul>().Right.Cast<CInt>().Value.ShouldBeEqual(3, "mul3");

        }

        public static void EvaluationTest() {
            new CInt(2).Test("cInt", new VInt(2));
            new CBool(true).Test("cBool true", new VBool(true));
            new CBool(false).Test("cBool false", new VBool(false));
            new Add(new CInt(3), new CInt(5)).Test("add", new VInt(8));
            new Sub(new CInt(5), new CInt(3)).Test("sub", new VInt(2));
            new Mul(new CInt(3), new CInt(5)).Test("mul", new VInt(15));
            new Div(new CInt(13), new CInt(5)).Test("div", new VInt(2));
            new And(new CBool(false), new CBool(false)).Test("and f f", new VBool(false));
            new And(new CBool(false), new CBool(true)).Test("and f t", new VBool(false));
            new And(new CBool(true), new CBool(false)).Test("and t f", new VBool(false));
            new And(new CBool(true), new CBool(true)).Test("and t t", new VBool(true));
            new Or(new CBool(false), new CBool(false)).Test("or f f", new VBool(false));
            new Or(new CBool(false), new CBool(true)).Test("or f t", new VBool(true));
            new Or(new CBool(true), new CBool(false)).Test("or t f", new VBool(true));
            new Or(new CBool(true), new CBool(true)).Test("or t t", new VBool(true));
            new Not(new CBool(false)).Test("not f", new VBool(true));
            new Not(new CBool(true)).Test("not t", new VBool(false));
            new Eq(new CInt(2), new CInt(1)).Test("eq when gt", new VBool(false));
            new Eq(new CInt(2), new CInt(2)).Test("eq when eq", new VBool(true));
            new Eq(new CInt(2), new CInt(3)).Test("eq when lt", new VBool(false));
            new Neq(new CInt(2), new CInt(1)).Test("neq when gt", new VBool(true));
            new Neq(new CInt(2), new CInt(2)).Test("neq when eq", new VBool(false));
            new Neq(new CInt(2), new CInt(3)).Test("neq when lt", new VBool(true));
            new Gt(new CInt(2), new CInt(1)).Test("gt when gt", new VBool(true));
            new Gt(new CInt(2), new CInt(2)).Test("gt when eq", new VBool(false));
            new Gt(new CInt(2), new CInt(3)).Test("gt when lt", new VBool(false));
            new Lt(new CInt(2), new CInt(1)).Test("lt when gt", new VBool(false));
            new Lt(new CInt(2), new CInt(2)).Test("lt when eq", new VBool(false));
            new Lt(new CInt(2), new CInt(3)).Test("lt when lt", new VBool(true));
            new Geq(new CInt(2), new CInt(1)).Test("geq when gt", new VBool(true));
            new Geq(new CInt(2), new CInt(2)).Test("geq when eq", new VBool(true));
            new Geq(new CInt(2), new CInt(3)).Test("geq when lt", new VBool(false));
            new Leq(new CInt(2), new CInt(1)).Test("leq when gt", new VBool(false));
            new Leq(new CInt(2), new CInt(2)).Test("leq when eq", new VBool(true));
            new Leq(new CInt(2), new CInt(3)).Test("leq when lt", new VBool(true));
            new If(new CBool(true), new CInt(1), new CInt(2)).Test("if t", new VInt(1));
            new If(new CBool(false), new CInt(1), new CInt(2)).Test("if f", new VInt(2));
            new Bind("myVar", new CInt(10), new Var("myVar")).Test("bind and var", new VInt(10));
            new App(new Abs("x", new Add(new Var("x"), new CInt(10))), new CInt(2)).Test("abs and app", new VInt(12));
            new LetRec(
                    "sum", "n",
                    new If(
                        new Eq(new Var("n"), new CInt(0)),
                        new CInt(0),
                        new Add(
                            new Var("n"),
                            new App(
                                new Var("sum"),
                                new Sub(new Var("n"), new CInt(1))))),
                    new App(
                        new Var("sum"),
                        new CInt(10)))
                .Test("letRec (recSum)", new VInt(55));
        }
        static void ShowExp(Expr e) => Console.WriteLine($" {e} ==> {e.Calculate()}");
    }

    static class EvalExtentionsForTest {
        public static void Test(this Expr e, string name, Value expected) => expression.Test.TestEval(name, e, expected);
    }
}
