namespace expression {
    using System.Collections.Generic;
    using System.Linq;
    using System;

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
}
