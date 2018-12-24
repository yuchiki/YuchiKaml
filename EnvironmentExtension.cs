namespace YuchikiML {
    using System;
    using Environment = System.Collections.Immutable.ImmutableDictionary<string, Value>;

    public static class EnvironmentExtension {
        public static Environment Update(this Environment env, string variable, Value value) =>
            variable == "_" ? env
            : env.Remove(variable).Add(variable, value);
    }
}
