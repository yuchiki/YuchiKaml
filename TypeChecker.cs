namespace YuchikiML {
    using System.Collections.Immutable;
    using System.Linq;

    //                                                <string, TypeScheme> <- will be modified later.
    using TEnv = System.Collections.Generic.Dictionary<string, Type>;
    using TypeEquations = System.Collections.Immutable.ImmutableList<TypeEquation>;

    using System;
    public static class TypeChecker {
        public static ImmutableHashSet<TVar> FTV(Type t) {
            switch (t) {
                case TVar tv:
                    return ImmutableHashSet<TVar>.Empty.Add(tv);
                case TVarLiteral _:
                    return ImmutableHashSet<TVar>.Empty;
                case TFun tf:
                    return FTV(tf.Left).Union(FTV(tf.Right));
                default:
                    throw new InvalidOperationException($"unreachable here.:{t}");
            }
        }

        public static ImmutableHashSet<TVar> FTV(TypeScheme ts) =>
            FTV(ts.SchemeType).Except(ts.SchemeVariables);

        public static ImmutableHashSet<TVar> FTV(TEnv tEnv) =>
            tEnv
            .Select(binding => FTV(binding.Value))
            .Aggregate((acc, ftv) => acc.Union(ftv));

        public static(TypeEquations, Type) Extract(TEnv tEnv, Expr expr) {
            (TypeEquations, Type) ExtractBinOpOnInt(BinOperator binOp, Type returnType) {
                var(equations1, type1) = Extract(tEnv, binOp.Left);
                var(equations2, type2) = Extract(tEnv, binOp.Right);
                return (
                    equations1
                    .AddRange(equations2)
                    .Add(new TypeEquation(type1, Type.TInt))
                    .Add(new TypeEquation(type2, Type.TInt)),
                    returnType
                );
            }

            switch (expr) {
                case CInt ci:
                    return (TypeEquations.Empty, Type.TInt);
                case CBool cb:
                    return (TypeEquations.Empty, Type.TBool);
                case Var v:
                    return (TypeEquations.Empty, tEnv[v.Name]);
                case Add e0:
                    return ExtractBinOpOnInt(e0, Type.TInt);
                case Mul e0:
                    return ExtractBinOpOnInt(e0, Type.TInt);
                case Sub e0:
                    return ExtractBinOpOnInt(e0, Type.TInt);
                case Div e0:
                    return ExtractBinOpOnInt(e0, Type.TInt);
                case Gt e0:
                    return ExtractBinOpOnInt(e0, Type.TBool);
                case Lt e0:
                    return ExtractBinOpOnInt(e0, Type.TBool);
                case Geq e0:
                    return ExtractBinOpOnInt(e0, Type.TBool);
                case Leq e0:
                    return ExtractBinOpOnInt(e0, Type.TBool);
                case Eq e0:
                    return ExtractBinOpOnInt(e0, Type.TBool);
                case Neq e0:
                    return ExtractBinOpOnInt(e0, Type.TBool);

                default:
            }
        }
    }

    public class TypeEquation {
        Type Left;
        Type Right;
        public TypeEquation(Type left, Type right) => (Left, Right) = (left, right);
    }
}

/*
        public static Expr And(Expr l, Expr r) => new And(l, r);
        public static Expr Or(Expr l, Expr r) => new Or(l, r);
        public static Expr Eq(Expr l, Expr r) => new Eq(l, r);
        public static Expr Neq(Expr l, Expr r) => new Neq(l, r);
        public static Expr Not(Expr e) => new Not(e);
        public static Expr Bind(string variable, Expr varBody, Expr exprBody) => new Bind(variable, varBody, exprBody);
        public static Expr LetRec(string function, string argument, Expr varBody, Expr exprBody) => new LetRec(function, argument, varBody, exprBody);
        public static Expr Abs(string variable, Expr body) => new Abs(variable, body);
        public static Expr App(Expr l, Expr r) => new App(l, r);
        public static Expr If(Expr condition, Expr left, Expr right) => new If(condition, left, right);

 */
