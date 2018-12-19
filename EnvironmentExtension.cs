namespace expression {
    using System;
    using Environment = System.Collections.Immutable.ImmutableDictionary<string, Value>;

    public static class EnvironmentExtension {
        public static Environment Update(this Environment env, string variable, Value value) =>
            env.Remove(variable).Add(variable, value);
    }
}
