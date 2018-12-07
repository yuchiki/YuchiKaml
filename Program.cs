﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace expression {
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
                    new CInt(10)
                ));

            ShowExp(e2);
        }

        static void ShowExp(Expr e) => Console.WriteLine($"{e} ==> {e.Calculate()}");

    }

    /*
        expression e ::= n | x | e + e | e * e | e - e | e / e | let x = e in e | \x -> e | e e
            | b | e && e | e || e | !e
            | e == e | e != e | e <= e | e < e | e >= e | e > e
            | if e then e else e
            | let rec f x = e in e  // it is allowed only in function form to exist unevaluated expression as a part of the value.
     */

    public abstract class Value {}

    public class VInt : Value {
        public int Value { get; }
        public VInt(int value) => Value = value;
        public override string ToString() => $"{Value}";
    }
    public class VBool : Value {
        public bool Value { get; }
        public VBool(bool value) => Value = value;
        public override string ToString() => $"{Value}";
    }

    public class Closure : Value {
        public Environment Env { get; }
        public string Variable { get; }
        public Expr Body { get; }
        public Closure(Environment env, string variable, Expr body) => (Env, Variable, Body) = (env, variable, body);

        public override string ToString() => $"\\{Variable} -> {Body}";
    }

    public class RecClosure : Value {
        public Environment Env { get; }
        public string Variable { get; }
        public Expr Body { get; }
        public RecClosure(Environment env, string variable, Expr body) => (Env, Variable, Body) = (env, variable, body);
    }

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

    class And : BinOperator { public And(Expr left, Expr right) : base("&", left, right) {} }
    class Or : BinOperator { public Or(Expr left, Expr right) : base("|", left, right) {} }

    class Eq : BinOperator { public Eq(Expr left, Expr right) : base("==", left, right) {} }
    class Neq : BinOperator { public Neq(Expr left, Expr right) : base("!=", left, right) {} }
    class Gt : BinOperator { public Gt(Expr left, Expr right) : base(">", left, right) {} }
    class Lt : BinOperator { public Lt(Expr left, Expr right) : base("<", left, right) {} }
    class Geq : BinOperator { public Geq(Expr left, Expr right) : base(">=", left, right) {} }
    class Leq : BinOperator { public Leq(Expr left, Expr right) : base("<=", left, right) {} }

    class Not : Expr {
        public Expr Body { get; }
        public Not(Expr body) => Body = body;

        public override string ToString() => $"!{Body}";
    }

    class Bind : Expr {
        public string Variable { get; }
        public Expr VarBody { get; }
        public Expr ExprBody { get; }
        public Bind(string variable, Expr varBody, Expr exprBody) => (Variable, VarBody, ExprBody) = (variable, varBody, exprBody);

        public override string ToString() => $"let {Variable} = {VarBody} in {ExprBody}";
    }

    class LetRec : Expr {
        public string Function { get; }
        public string Argument { get; }
        public Expr VarBody { get; }
        public Expr ExprBody { get; }
        public LetRec(string function, string argument, Expr varBody, Expr exprBody) => (Function, Argument, VarBody, ExprBody) = (function, argument, varBody, exprBody);

        public override string ToString() => $"let {Function} {Argument} = {VarBody} in {ExprBody}";
    }

    class Abs : Expr {
        public string Variable { get; }
        public Expr Body { get; }
        public Abs(string variable, Expr body) => (Variable, Body) = (variable, body);

        public override string ToString() => $"\\{Variable} -> {Body}";
    }

    class App : Expr {
        public Expr Left { get; }
        public Expr Right { get; }
        public App(Expr left, Expr right) => (Left, Right) = (left, right);
        public override string ToString() => $"({Left}) ({Right})";
    }

    class If : Expr {
        public Expr Condition { get; }
        public Expr Left { get; }
        public Expr Right { get; }
        public If(Expr condition, Expr left, Expr right) => (Condition, Left, Right) = (condition, left, right);
        public override string ToString() => $"if {Condition} then {Left} else {Right}";
    }

    class CBool : Expr {
        public bool Value { get; }
        public CBool(bool value) => Value = value;
        public override string ToString() => $"{Value}";
    }

    public static class ExprExtensions {
        static Value Calculate(this Expr e, Environment env) {
            // Console.WriteLine($"env:{env}");
            // Console.WriteLine($"{e}");
            var v = Calculate_(e, env);
            // Console.WriteLine($"{e} ==> {v}");
            return v;
        }

        public static Value Calculate_(this Expr e, Environment env) {

            // NOTE: I want to use switch expression if C# 8.0 get released.
            switch (e) {
                case CInt ci:
                    return new VInt(ci.Value);
                case CBool cb:
                    return new VBool(cb.Value);
                case Var v:
                    return env[v.Name];
                case Add a:
                    return BinOpCalculate(a.Left, a.Right, (x, y) => x + y, env);
                case Mul a:
                    return BinOpCalculate(a.Left, a.Right, (x, y) => x * y, env);
                case Sub a:
                    return BinOpCalculate(a.Left, a.Right, (x, y) => x - y, env);
                case Div a:
                    return BinOpCalculate(a.Left, a.Right, (x, y) => x / y, env);
                case And and:
                    return BinBoolOpCalculate(and.Left, and.Right, (x, y) => x & y, env);
                case Or op:
                    return BinBoolOpCalculate(op.Left, op.Right, (x, y) => x | y, env);
                case Eq op:
                    return BinCompOpCalculate(op.Left, op.Right, (x, y) => x == y, env);
                case Neq op:
                    return BinCompOpCalculate(op.Left, op.Right, (x, y) => x != y, env);
                case Gt op:
                    return BinCompOpCalculate(op.Left, op.Right, (x, y) => x > y, env);
                case Lt op:
                    return BinCompOpCalculate(op.Left, op.Right, (x, y) => x < y, env);
                case Geq op:
                    return BinCompOpCalculate(op.Left, op.Right, (x, y) => x >= y, env);
                case Leq op:
                    return BinCompOpCalculate(op.Left, op.Right, (x, y) => x <= y, env);
                case Bind b:
                    {
                        var newEnv = env.AddAndCopy(b.Variable, b.VarBody.Calculate(env));
                        return b.ExprBody.Calculate(newEnv);
                    }
                case Abs abs:
                    return new Closure(env, abs.Variable, abs.Body);
                case LetRec letRec:
                    {
                        var Closure = new Closure(env, letRec.Argument, letRec.VarBody);
                        Closure.Env[letRec.Function] = Closure;
                        var newEnv = env.AddAndCopy(letRec.Function, Closure);
                        return letRec.ExprBody.Calculate(newEnv);
                    }
                case Not n:
                    {
                        var b = (VBool) n.Body.Calculate(env);
                        return new VBool(!(b.Value));
                    }
                case App app:
                    {
                        var left = (Closure) app.Left.Calculate(env);
                        var right = app.Right.Calculate(env);
                        return left.Body.Calculate(left.Env.AddAndCopy(left.Variable, right));
                    }
                case If ifExpr:
                    {
                        var condition = (VBool) ifExpr.Condition.Calculate(env);
                        if (condition.Value) {
                            return ifExpr.Left.Calculate(env);
                        } else {
                            return ifExpr.Right.Calculate(env);
                        }
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static Value Calculate(this Expr e) => e.Calculate(new Environment());

        static VInt BinOpCalculate(Expr left, Expr right, Func<int, int, int> f, Environment env) {
            var l = (VInt) left.Calculate(env);
            var r = (VInt) right.Calculate(env);
            return new VInt(f(l.Value, r.Value));
        }

        static VBool BinCompOpCalculate(Expr left, Expr right, Func<int, int, bool> f, Environment env) {
            var l = (VInt) left.Calculate(env);
            var r = (VInt) right.Calculate(env);
            return new VBool(f(l.Value, r.Value));
        }
        static VBool BinBoolOpCalculate(Expr left, Expr right, Func<bool, bool, bool> f, Environment env) {
            var l = (VBool) left.Calculate(env);
            var r = (VBool) right.Calculate(env);
            return new VBool(f(l.Value, r.Value));
        }

    }

    public static class EnvironmentExtension {
        // NOTE: it may be better to use some persistent data structures instead of Dictionary
        public static Environment AddAndCopy(this Environment env, string variable, Value value) {
            var newEnv = new Environment(env);
            newEnv[variable] = value;
            return newEnv;
        }
    }

    public class Environment : Dictionary<string, Value> {
        public Environment() : base() {}
        public Environment(Environment env) : base(env) {}
        public override string ToString() {
            var s = "{ ";
            foreach (var key in Keys) {
                s += $"{key}: {this[key]}, ";
            }
            s += "}";
            return s;
        }
    }
}
