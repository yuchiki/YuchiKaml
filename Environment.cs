namespace expression {
    using System.Collections.Generic;
    using System.Linq;
    using System;

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

    public static class EnvironmentExtension {
        // NOTE: it may be better to use some persistent data structures instead of Dictionary
        public static Environment AddAndCopy(this Environment env, string variable, Value value) {
            var newEnv = new Environment(env);
            newEnv[variable] = value;
            return newEnv;
        }
    }
}
