namespace expression {
    using System.Collections.Generic;
    using System.Linq;
    using System;

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
}
