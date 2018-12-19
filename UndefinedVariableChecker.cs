namespace expression {
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System;

    class VariableUndefinedException : Exception {
        public readonly String Variable;
        public readonly ImmutableList<Expr> PartialExpressions;

        public VariableUndefinedException() {}
        public VariableUndefinedException(string message) : base(message) {}
        public VariableUndefinedException(string message, Exception inner) : base(message, inner) {}

        public VariableUndefinedException(string variable, ImmutableList<Expr> partialExpressions) =>
            (Variable, PartialExpressions) = (variable, partialExpressions);
        public VariableUndefinedException(Var variable) : this(variable.Name, ImmutableList<Expr>.Empty) {}
        public VariableUndefinedException(Expr currentExpression, VariableUndefinedException ex) : this(ex.Variable, currentExpression.Size() > 20 ? ex.PartialExpressions : ex.PartialExpressions.Add(currentExpression)) {}
    }

    public static class UndefinedVariableChecker {
        public static void Check(Expr e) => Check(e, ImmutableHashSet<string>.Empty);
        public static void Check(Expr e, ImmutableHashSet<string> occurrence) {
            try {
                switch (e) {
                    case Literal lit:
                        return;
                    case Var variable:
                        if (occurrence.Contains(variable.Name)) return;
                        throw new VariableUndefinedException(variable.Name, ImmutableList<Expr>.Empty);
                    case BinOperator binOp:
                        Check(binOp.Left, occurrence);
                        Check(binOp.Right, occurrence);
                        return;
                    case Not n:
                        Check(n.Body, occurrence);
                        return;
                    case If ifExpr:
                        Check(ifExpr.Condition, occurrence);
                        Check(ifExpr.Left, occurrence);
                        Check(ifExpr.Right, occurrence);
                        return;
                    case Bind bind:
                        {
                            Check(bind.VarBody, occurrence);
                            var newOccurrence = occurrence.Add(bind.Variable);
                            Check(bind.ExprBody, newOccurrence);
                            return;
                        }
                    case LetRec letRec:
                        {
                            var newOccurrence = occurrence.Add(letRec.Argument);
                            Check(letRec.VarBody, newOccurrence);
                            Check(letRec.ExprBody, newOccurrence);
                            return;
                        }
                    case Abs abs:
                        {
                            var newOccurrence = occurrence.Add(abs.Variable);
                            Check(abs.Body, newOccurrence);
                        }
                        return;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            } catch (VariableUndefinedException ex) {
                throw new VariableUndefinedException(e, ex);
            }
        }
    }
}
