namespace expression {
    /*
        expression e ::= n | x | e + e | e * e | e - e | e / e | let x1 ... xn = e in e | \x -> e | e e
            | b | e && e | e || e | !e
            | e == e | e != e | e <= e | e < e | e >= e | e > e
            | if e then e else e
            | let rec f x = e in e

        // I restrict mu in the function form , that is, of in the form let rec f x = e in e.
        // Because Only in closure are unevaluated values allowed to be.
     */

    public abstract class Expr {
        public T Cast<T>() where T : Expr => (T) this;
    }

    abstract class BinOperator : Expr {
        string Symbol { get; }
        public Expr Left { get; }
        public Expr Right { get; }
        public BinOperator(string symbol, Expr left, Expr right) => (Symbol, Left, Right) = (symbol, left, right);
        public override string ToString() => $"({Left}) {Symbol} ({Right})";
    }

    class Unit : Expr {
        public override string ToString() => "()";

        public override bool Equals(object obj) {
            if (obj == null || this.GetType() != obj.GetType()) return false;
            return true;
        }
        public override int GetHashCode() => 0;
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
}
