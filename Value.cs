namespace expression {
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System;

    using Environment = System.Collections.Immutable.ImmutableDictionary<string, Value>;

    using BuiltInFunction = System.Func<Value, Value>;
    using BuiltInFunctionPair =
        System.Collections.Generic.KeyValuePair<string, System.Func<Value, Value>>;

    public abstract class Value {}

    public class VUnit : Value {
        public override string ToString() => "()";
        public override bool Equals(object obj) {
            if (obj == null || this.GetType() != obj.GetType()) return false;
            return true;
        }
        public override int GetHashCode() => 0;
    }

    public class VInt : Value {
        public int Value { get; }
        public VInt(int value) => Value = value;
        public override string ToString() => $"{Value}";
        public override bool Equals(object obj) {
            if (obj == null || this.GetType() != obj.GetType()) return false;
            return this.Value == ((VInt) obj).Value;
        }
        public override int GetHashCode() => Value;
    }
    public class VBool : Value {
        public bool Value { get; }
        public VBool(bool value) => Value = value;
        public override string ToString() => $"{Value}";
        public override bool Equals(object obj) {
            if (obj == null || this.GetType() != obj.GetType()) return false;
            return this.Value == ((VBool) obj).Value;
        }
        public override int GetHashCode() => Value.GetHashCode();
    }

    public class VString : Value {
        public string Value { get; }
        public VString(string value) => Value = value;
        public override string ToString() => $"{Value}";
        public override bool Equals(object obj) {
            if (obj == null || this.GetType() != obj.GetType()) return false;
            return this.Value == ((VString) obj).Value;
        }
        public override int GetHashCode() => Value.GetHashCode();
    }

    public class Closure : Value {
        public Environment Env { get; set; }
        public string Variable { get; }
        public Expr Body { get; }
        public Closure(Environment env, string variable, Expr body) => (Env, Variable, Body) = (env, variable, body);

        public override string ToString() => $"\\{Variable} -> {Body}";
    }

    public class BuiltInClosure : Value {
        public BuiltInFunction Function;
        public BuiltInClosure(BuiltInFunction function) => Function = function;

        public override string ToString() => $"BUILT-IN";
    }
}
