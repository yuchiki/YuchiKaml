namespace YuchikiML {
    using System.Collections.Generic;
    using System.Linq;

    /*
        // I restrict mu in the function form , that is, of in the form let rec f x = e in e.
        // Because only in closure are unevaluated values allowed to be.
     */

    public abstract class Expr {
        public int Priority { get; }
        public Expr(int priority) => Priority = priority;

        public T Cast<T>() where T : Expr => (T) this;

        public static readonly Expr Unit = new Unit();
        public static Expr CInt(int i) => new CInt(i);
        public static Expr CBool(bool b) => new CBool(b);
        public static Expr Var(string x) => new Var(x);
        public static Expr Add(Expr l, Expr r) => new Add(l, r);
        public static Expr Mul(Expr l, Expr r) => new Mul(l, r);
        public static Expr Sub(Expr l, Expr r) => new Sub(l, r);
        public static Expr Div(Expr l, Expr r) => new Div(l, r);
        public static Expr And(Expr l, Expr r) => new And(l, r);
        public static Expr Or(Expr l, Expr r) => new Or(l, r);
        public static Expr Eq(Expr l, Expr r) => new Eq(l, r);
        public static Expr Neq(Expr l, Expr r) => new Neq(l, r);
        public static Expr Gt(Expr l, Expr r) => new Gt(l, r);
        public static Expr Lt(Expr l, Expr r) => new Lt(l, r);
        public static Expr Geq(Expr l, Expr r) => new Geq(l, r);
        public static Expr Leq(Expr l, Expr r) => new Leq(l, r);
        public static Expr Not(Expr e) => new Not(e);
        public static Expr Bind(string variable, Expr varBody, Expr exprBody) => new Bind(variable, varBody, exprBody);
        public static Expr LetRec(string function, string argument, Expr varBody, Expr exprBody) => new LetRec(function, argument, varBody, exprBody);
        public static Expr Abs(string variable, Expr body) => new Abs(variable, body);
        public static Expr App(Expr l, Expr r) => new App(l, r);
        public static Expr If(Expr condition, Expr left, Expr right) => new If(condition, left, right);

        public string ShowLeft(Expr e) => e.Priority <= Priority ? $"{e}" : $"({e})";
        public string ShowRight(Expr e) => e.Priority < Priority ? $"{e}" : $"({e})";
        public string ShowMono(Expr e) => e.Priority < Priority ? $"{e}" : $"({e})";

    }

    abstract class Literal : Expr {
        public Literal() : base(0) {}
    }

    abstract class BinOperator : Expr {
        string Symbol { get; }
        public Expr Left { get; }
        public Expr Right { get; }
        public BinOperator(string symbol, int priority, Expr left, Expr right) : base(priority) => (Symbol, Left, Right) = (symbol, left, right);
        public override string ToString() => $"{ShowLeft(Left)} {Symbol} {ShowRight(Right)}";
    }

    class Unit : Literal {
        public override string ToString() => "()";

        public override bool Equals(object obj) {
            if (obj == null || this.GetType() != obj.GetType()) return false;
            return true;
        }
        public override int GetHashCode() => 0;
    }

    class CString : Literal {
        public string Value { get; }
        public CString(string value) => Value = value;
        public override string ToString() => $"\"{Value}\"";
        public override bool Equals(object obj) {
            if (obj == null || this.GetType() != obj.GetType()) return false;
            return this.Value == ((CString) obj).Value;
        }
        public override int GetHashCode() => Value.GetHashCode();
    }

    class CInt : Literal {
        public int Value { get; }
        public CInt(int value) => Value = value;
        public override string ToString() => $"{Value}";

        public override bool Equals(object obj) {
            if (obj == null || this.GetType() != obj.GetType()) return false;
            return this.Value == ((CInt) obj).Value;
        }
        public override int GetHashCode() => Value.GetHashCode();
    }

    class CBool : Literal {
        public bool Value { get; }
        public CBool(bool value) => Value = value;
        public override string ToString() => $"{Value}";
    }

    class Var : Expr {
        public string Name { get; }
        public Var(string name) : base(0) => Name = name;
        public override string ToString() => Name;

        public override bool Equals(object obj) {
            if (obj == null || this.GetType() != obj.GetType()) return false;
            return this.Name == ((Var) obj).Name;
        }
        public override int GetHashCode() => Name.GetHashCode();
    }

    class App : BinOperator {
        public App(Expr left, Expr right) : base("", 1, left, right) {}
        public override string ToString() => $"{ShowLeft(Left)} {ShowRight(Right)}";

    }

    class Not : Expr {
        public Expr Body { get; }
        public Not(Expr body) : base(2) => Body = body;

        public override string ToString() => $"!{Body}";
    }
    class Mul : BinOperator { public Mul(Expr left, Expr right) : base("*", 3, left, right) {} }
    class Div : BinOperator { public Div(Expr left, Expr right) : base("/", 3, left, right) {} }

    class Add : BinOperator { public Add(Expr left, Expr right) : base("+", 4, left, right) {} }
    class Sub : BinOperator { public Sub(Expr left, Expr right) : base("-", 4, left, right) {} }

    class Gt : BinOperator { public Gt(Expr left, Expr right) : base(">", 5, left, right) {} }
    class Lt : BinOperator { public Lt(Expr left, Expr right) : base("<", 5, left, right) {} }
    class Geq : BinOperator { public Geq(Expr left, Expr right) : base(">=", 5, left, right) {} }
    class Leq : BinOperator { public Leq(Expr left, Expr right) : base("<=", 5, left, right) {} }

    class Eq : BinOperator { public Eq(Expr left, Expr right) : base("==", 6, left, right) {} }
    class Neq : BinOperator { public Neq(Expr left, Expr right) : base("!=", 6, left, right) {} }

    class And : BinOperator { public And(Expr left, Expr right) : base("&", 7, left, right) {} }
    class Or : BinOperator { public Or(Expr left, Expr right) : base("|", 8, left, right) {} }

    class Bind : Expr {
        public string Variable { get; }
        public Expr VarBody { get; }
        public Expr ExprBody { get; }
        public Bind(string variable, Expr varBody, Expr exprBody) : base(9) => (Variable, VarBody, ExprBody) = (variable, varBody, exprBody);

        public override string ToString() => $"let {Variable} =\n{PrintHelper.Indent($"{VarBody}")} in\n{ExprBody}";
    }

    class LetRec : Expr {
        public string Function { get; }
        public string Argument { get; }
        public Expr VarBody { get; }
        public Expr ExprBody { get; }
        public LetRec(string function, string argument, Expr varBody, Expr exprBody) : base(9) => (Function, Argument, VarBody, ExprBody) = (function, argument, varBody, exprBody);

        public override string ToString() => $"let rec {Function} {Argument} =\n{PrintHelper.Indent($"{VarBody}")} in\n{ExprBody}";
    }

    class Abs : Expr {
        public string Variable { get; }
        public Expr Body { get; }
        public Abs(string variable, Expr body) : base(9) => (Variable, Body) = (variable, body);

        public override string ToString() => $"\\{Variable} ->\n{PrintHelper.Indent($"{Body}")}";
    }

    class If : Expr {
        public Expr Condition { get; }
        public Expr Left { get; }
        public Expr Right { get; }
        public If(Expr condition, Expr left, Expr right) : base(9) => (Condition, Left, Right) = (condition, left, right);
        public override string ToString() => $"if {Condition} then\n{PrintHelper.Indent($"{Left}")}\nelse\n{PrintHelper.Indent($"{Right}")}";
    }

    static class PrintHelper {
        public static int IndentSize => 4;
        public static string Spaces => new string(Enumerable.Repeat(' ', IndentSize).ToArray());
        public static string Indent(string source) => string.Join("\n", source.Split("\n").Select(x => Spaces + x));
    }
}
