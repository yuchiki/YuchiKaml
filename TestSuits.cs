namespace expression {
    using System.Collections.Generic;
    using System.Linq;
    using System;
    using Sprache;

    static class TestSuits {

        public static void Test() {
            Console.WriteLine("EVALUATION TEST");
            EvaluationTest();
            Console.WriteLine();
            Console.WriteLine("PARSING TEST");
            ParsingTest();
            Console.WriteLine();
            Console.WriteLine("ProgramTest");
            ProgramTest();

        }

        public static void ProgramTest() {
            var comp1 = " 1 + 2 *3 - 4 * 5 / 6";
            var letTest = "let x = 2 in let y = 3 in x + y";
            var recSumProgram = "let rec sum n = if n <= 0 then 0 else n + sum (n - 1) in sum 10";
            var gcdProgram = @"
                let rec gcd m n =
                    if m > n then gcd (m-n) n
                    else if m < n then gcd m (n-m)
                    else m
                    in gcd 120 45";

            var listProgram = $@"
let makeTuple a b condition = if condition then a else b in
let fst t = t true in
let snd t = t false in

let Nil = {'"'}nil{'"'} in
let isNil l = l == Nil in
let Cons x xs =
    makeTuple x xs in

let rec map f l =
    if isNil l then Nil
    else
        let car = fst l in
        let cdr = snd l in
        Cons (f car) (map f cdr) in

let rec fold i f l =
    if isNil l then i
    else
        let car = fst l in
        let cdr = snd l in
        fold (f i car) f cdr in

let rec filter f l =
    if isNil l then Nil
    else
        let car = fst l in
        let cdr = snd l in
        if f car then Cons car (filter f cdr)
        else filter f cdr in

let rec range minValue maxValue =
    if minValue > maxValue then Nil
    else Cons minValue (range (minValue + 1) maxValue) in

let isEven x = x / 2 * 2 == x in
let list = range 0 10 in
let list = filter isEven list in
let list = map (\x -> x * x) list in
fold 0 (\x -> \y -> x + y) list";

            ExprParser.MainParser.Parse(comp1).Calculate().ShouldBeEqual(new VInt(4), "numeral expression");
            ExprParser.MainParser.Parse(letTest).Calculate().ShouldBeEqual(new VInt(5), "let");
            ExprParser.MainParser.Parse(recSumProgram).Calculate().ShouldBeEqual(new VInt(55), "let rec and recursion");
            ExprParser.MainParser.Parse(gcdProgram).Calculate().ShouldBeEqual(new VInt(15), "multi let rec: gcd");
            ExprParser.MainParser.Parse(listProgram).Calculate().ShouldBeEqual(new VInt(220), "long example: list");
        }

        public static void ParsingTest() {
            Func<string, Expr> parse = ExprParser.MainParser.Parse;

            parse(" () ").ShouldBeEqual(new Unit(), "unit");
            parse(" \" str1_ ng \" ").Cast<CString>().Value.ShouldBeEqual(" str1_ ng ", "string");
            parse(" 12 ").Cast<CInt>().Value.ShouldBeEqual(12, "int");
            parse(" true ").Cast<CBool>().Value.ShouldBeEqual(true, "true");
            parse(" false ").Cast<CBool>().Value.ShouldBeEqual(false, "false");
            parse(" foo").Cast<Var>().Name.ShouldBeEqual("foo", "var");
            parse(" ( 12 ) ").Cast<CInt>().Value.ShouldBeEqual(12, "paren");
            parse(" x y z ").Cast<App>().Left.Cast<App>().Left.Cast<Var>().Name.ShouldBeEqual("x", "app1");
            parse(" x y z ").Cast<App>().Left.Cast<App>().Right.Cast<Var>().Name.ShouldBeEqual("y", "app2");
            parse(" x y z ").Cast<App>().Right.Cast<Var>().Name.ShouldBeEqual("z", "app3");

            parse(" !x ").Cast<Not>().Body.Cast<Var>().Name.ShouldBeEqual("x", "not");
            parse(" !!x ").Cast<Not>().Body.Cast<Not>().Body.Cast<Var>().Name.ShouldBeEqual("x", "notnot");
            parse(" ! ! x ").Cast<Not>().Body.Cast<Not>().Body.Cast<Var>().Name.ShouldBeEqual("x", "not not");

            parse("2*3").Cast<Mul>().Left.Cast<CInt>().Value.ShouldBeEqual(2, "nummulnum");
            parse("2 * 3").Cast<Mul>().Left.Cast<CInt>().Value.ShouldBeEqual(2, "num mul num");
            parse("2 * 3 / 4").Cast<Div>().Right.Cast<CInt>().Value.ShouldBeEqual(4, "mul1");
            parse("2 * 3 / 4").Cast<Div>().Left.Cast<Mul>().Left.Cast<CInt>().Value.ShouldBeEqual(2, "mul2");
            parse("2 * 3 / 4").Cast<Div>().Left.Cast<Mul>().Right.Cast<CInt>().Value.ShouldBeEqual(3, "mul3");

            parse("let f a b = 1 in 2").Cast<Bind>().Variable.ShouldBeEqual("f", "let1");
            parse("let f a b = 1 in 2").Cast<Bind>().VarBody.Cast<Abs>().Variable.ShouldBeEqual("a", "let2");
            parse("let f a b = 1 in 2").Cast<Bind>().VarBody.Cast<Abs>().Body.Cast<Abs>().Variable.ShouldBeEqual("b", "let3");
            parse("let f a b = 1 in 2").Cast<Bind>().VarBody.Cast<Abs>().Body.Cast<Abs>().Body.Cast<CInt>().Value.ShouldBeEqual(1, "let4");
            parse("let f a b = 1 in 2").Cast<Bind>().ExprBody.Cast<CInt>().Value.ShouldBeEqual(2, "let5");

        }

        public static void EvaluationTest() {
            new Unit().Test("unit", new VUnit());
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
