namespace YuchikiML {
    using System.Collections.Generic;
    using System.Linq;
    using System;

    using Environment = System.Collections.Immutable.ImmutableDictionary<string, Value>;

    public static class ExprExtensions {
        static Value Calculate(this Expr e, Environment env) {
            Logger.LogTrace(e.GetType().ToString());
            var v = Calculate_(e, env);
            return v;
        }

        public static Value Calculate_(this Expr e, Environment env) {

            // NOTE: I want to use switch expression if C# 8.0 get released.
            switch (e) {
                case Unit u:
                    return new VUnit();
                case CString cs:
                    return new VString(cs.Value);
                case CInt ci:
                    return new VInt(ci.Value);
                case CBool cb:
                    return new VBool(cb.Value);
                case Var v:
                    return BuiltInFunctions.BuiltIns.ContainsKey(v.Name) ? new BuiltInClosure(BuiltInFunctions.BuiltIns[v.Name])
                        : env[v.Name];
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
                    return BinEqCalculate(op.Left, op.Right, env);
                case Neq op:
                    return BinNeqCalculate(op.Left, op.Right, env);
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
                        return b.ExprBody.Calculate(env.Update(b.Variable, b.VarBody.Calculate(env)));
                    }
                case Abs abs:
                    return new Closure(env, abs.Variable, abs.Body);
                case LetRec letRec:
                    {
                        var Closure = new Closure(env, letRec.Argument, letRec.VarBody);
                        Closure.Env = Closure.Env.Update(letRec.Function, Closure);
                        var newEnv = env.Update(letRec.Function, Closure);
                        return letRec.ExprBody.Calculate(newEnv);
                    }
                case Not n:
                    {
                        var b = n.Body.EvalTo<VBool>(env);
                        return new VBool(!(b.Value));
                    }
                case App app:
                    {
                        var evaluatedValue = app.Left.Calculate(env);
                        var right = app.Right.Calculate(env);
                        switch (evaluatedValue) {
                            case Closure left:
                                {
                                    return left.Body.Calculate(left.Env.Update(left.Variable, right));
                                }
                            case BuiltInClosure left:
                                {
                                    return left.Function(right);
                                }
                            default:
                                Logger.LogError($"left:{app.Left}");
                                Logger.LogError($"left:{evaluatedValue}");
                                throw new ArgumentException();
                        }
                    }
                case If ifExpr:
                    {
                        var condition = ifExpr.Condition.EvalTo<VBool>(env);
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

        public static Value Calculate(this Expr e) => e.Calculate(Environment.Empty);

        static VInt BinOpCalculate(Expr left, Expr right, Func<int, int, int> f, Environment env) {
            var l = left.EvalTo<VInt>(env);
            var r = right.EvalTo<VInt>(env);
            return new VInt(f(l.Value, r.Value));
        }

        static VBool BinCompOpCalculate(Expr left, Expr right, Func<int, int, bool> f, Environment env) {
            var l = left.EvalTo<VInt>(env);
            var r = right.EvalTo<VInt>(env);
            return new VBool(f(l.Value, r.Value));
        }

        static VBool BinEqCalculate(Expr left, Expr right, Environment env) {
            var l = left.Calculate(env);
            var r = right.Calculate(env);
            return new VBool(l.Equals(r));
        }

        static VBool BinNeqCalculate(Expr left, Expr right, Environment env) {
            var l = left.Calculate(env);
            var r = right.Calculate(env);
            return new VBool(!l.Equals(r));
        }

        static VBool BinBoolOpCalculate(Expr left, Expr right, Func<bool, bool, bool> f, Environment env) {
            var l = left.EvalTo<VBool>(env);
            var r = right.EvalTo<VBool>(env);
            return new VBool(f(l.Value, r.Value));
        }

        static T EvalTo<T>(this Expr e, Environment env) where T : Value {
            var v = e.Calculate(env);
            if (!(v is T)) throw new InvalidCastException($"expected {e} => {v} has type {typeof(T).FullName}, but result is {v.GetType()}");
            return (T) v;
        }
    }
}
