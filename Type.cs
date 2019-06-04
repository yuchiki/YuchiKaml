namespace YuchikiML {
    using System.Collections.Generic;
    using TEnv = System.Collections.Generic.Dictionary<string, TypeScheme>;

    public abstract class Type {
        public int Priority { get; }
        public Type(int priority) => Priority = priority;
        public static TBool TBool => TBool.Instance;
        public static TInt TInt => TInt.Instance;
        public static TString TString => TString.Instance;

    }

    public abstract class TVarLiteral : Type {
        public TVarLiteral() : base(0) {}
    }

    public sealed class TVar : TVarLiteral {
        public int Id { get; }
        public TVar(int id) => Id = id;

        public override string ToString() => $"t{Id}";
    }
    public sealed class TBool : TVarLiteral {
        private static TBool instance = new TBool();
        public static TBool Instance => instance;
        private TBool() {}

        public override string ToString() => "bool";
    }
    public sealed class TInt : TVarLiteral {
        private static TInt instance = new TInt();
        public static TInt Instance => instance;
        private TInt() {}

        public override string ToString() => "int";
    }
    public sealed class TString : TVarLiteral {
        private static TString instance = new TString();
        public static TString Instance => instance;
        private TString() {}

        public override string ToString() => "string";
    }
    public sealed class TFun : Type {
        public Type Left { get; }
        public Type Right { get; }

        public TFun(Type left, Type right) : base(1) => (Left, Right) = (left, right);

        private string showLeft(Type t) => t.Priority < Priority ? $"{t}" : $"({t})";

        public override string ToString() => $"{showLeft(Left)} -> {Right}";
    }

    public class TypeScheme {
        public List<TVar> SchemeVariables { get; }
        public Type SchemeType { get; }

        public TypeScheme(List<TVar> schemeVariables, Type schemeType) =>
            (SchemeVariables, SchemeType) = (schemeVariables, schemeType);
    }

}
