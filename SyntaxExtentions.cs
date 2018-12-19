namespace expression {
    using System;
    public static class SyntaxExtentions {
        public static int Size(this Expr e) {
            switch (e) {
                case Literal lit:
                    return 1;
                case Var variable:
                    return 1;

                case BinOperator binOp:
                    return binOp.Left.Size() + binOp.Right.Size() + 1;
                case Not n:
                    return n.Body.Size() + 1;
                case If ifExpr:
                    return ifExpr.Condition.Size() + ifExpr.Left.Size() + ifExpr.Right.Size() + 1;
                case Bind bind:
                    return bind.VarBody.Size() + bind.ExprBody.Size() + 1;
                case LetRec letRec:
                    return letRec.VarBody.Size() + letRec.ExprBody.Size() + 1;
                case Abs abs:
                    return abs.Body.Size() + 1;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
