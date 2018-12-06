using System;
using System.Collections.Generic;
using System.Linq;

namespace expression {
    using Environment = Dictionary<string, Expr>;
    class Program {
        static void Main(string[] args) {
            var e =
                new Bind(
                    "x",
                    new Add(
                        new CInt(3),
                        new CInt(5)),
                    new Add(
                        new CInt(2),
                        new Mul(
                            new Var("x"),
                            new CInt(5))));
            Console.WriteLine($"{e} ==> {e.Calculate()}");
        }
    }

    /*
        expression e ::= n | x | e + e | e * e | e - e | e / e | let x = e in e
     */
    public abstract class Expr {}

    abstract class BinOperator : Expr {
        string Symbol { get; }
        public Expr Left { get; }
        public Expr Right { get; }
        public BinOperator(string symbol, Expr left, Expr right) => (Symbol, Left, Right) = (symbol, left, right);
        public override string ToString() => $"({Left}) {Symbol} ({Right})";
    }

    class CInt : Expr {
        public int Value { get; }
        public CInt(int value) => Value = value;
        public override string ToString() => $"{Value}";
    }

    class Var : Expr {
        public string Name { get; }
        public Var(string name) => Name = name;
        public override string ToString() => Name;
    }

    class Add : BinOperator { public Add(Expr left, Expr right) : base("+", left, right) {} }
    class Mul : BinOperator { public Mul(Expr left, Expr right) : base("*", left, right) {} }
    class Sub : BinOperator { public Sub(Expr left, Expr right) : base("-", left, right) {} }
    class Div : BinOperator { public Div(Expr left, Expr right) : base("/", left, right) {} }

    class Bind : Expr {
        public string Variable { get; }
        public Expr VarBody { get; }
        public Expr ExprBody { get; }
        public Bind(string variable, Expr varBody, Expr exprBody) => (Variable, VarBody, ExprBody) = (variable, varBody, exprBody);

        public override string ToString() => $"let {Variable} = {VarBody} in {ExprBody}";
    }

    public static class ExprExtensions {
        public static Expr Calculate(this Expr e, Environment env) {
            // NOTE: I want to use switch expression if C# 8.0 get released.
            switch (e) {
                case CInt ci:
                    return ci;
                case Var v:
                    return env[v.Name];
                case Add a:
                    {
                        CInt left = (CInt) a.Left.Calculate(env);
                        CInt right = (CInt) a.Right.Calculate(env);
                        return new CInt(left.Value + right.Value);
                    }
                case Mul a:
                    {
                        CInt left = (CInt) a.Left.Calculate(env);
                        CInt right = (CInt) a.Right.Calculate(env);
                        return new CInt(left.Value + right.Value);
                    }
                case Sub a:
                    {
                        CInt left = (CInt) a.Left.Calculate(env);
                        CInt right = (CInt) a.Right.Calculate(env);
                        return new CInt(left.Value + right.Value);
                    }
                case Div a:
                    {
                        CInt left = (CInt) a.Left.Calculate(env);
                        CInt right = (CInt) a.Right.Calculate(env);
                        return new CInt(left.Value + right.Value);
                    }
                case Bind b:
                    // NOTE: it may be better to use some persistent data structures instead of Dictionary
                    var newEnv = new Environment(env);
                    newEnv[b.Variable] = b.VarBody.Calculate(env);
                    return b.ExprBody.Calculate(newEnv);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static Expr Calculate(this Expr e) => e.Calculate(new Environment());
    }
}
