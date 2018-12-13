namespace expression {
    using System.Collections.Generic;
    using System.Linq;
    using System;
    using Sprache;

    using OperatorCreator = System.Func<Expr, Expr, Expr>;

    static class ExprParser {

        public static Parser<Expr> IntParser =
            from digits in Parse.Digit.Many().Text().Token()
        select new CInt(int.TryParse(digits, out var n) ? n : -1);

        public static Parser<Expr> BoolParser =
            from str in Parse.String("true").Text().Token()
            .Or(Parse.String("false").Text().Token())
        select new CBool(str == "true" ? true : false);

        public static readonly Parser<string> IDParser =
            Parse.Letter.AtLeastOnce().Text().Token();

        public static readonly Parser<Expr> VarParser =
            from id in IDParser select new Var(id);

        public static readonly Parser<Expr> ParenParser =
            from _ in Parse.Char('(')
        from e in MainParser
        from __ in Parse.Char(')')
        select e;

        public static readonly Parser<Expr> PrimaryParser =
            IntParser.Or(BoolParser).Or(VarParser).Or(ParenParser).Token();

        public static readonly Parser<Expr> AppParser = PrimaryParser.Many().Select(primaries => primaries.Aggregate((e1, e2) => new App(e1, e2)));

        public static readonly Parser<Expr> UnaryParser =
            from negSequence in Parse.Char('!').Token().Many()
        from app in AppParser
        select Times(negSequence.Count(), x => new Not(x), app);

        public static readonly Parser<Expr> MultiplicativeParser =
            BinOpParser(UnaryParser, new(string, OperatorCreator) [][] {
                new(string, OperatorCreator) [] {
                    ("*", (l, r) => new Mul(l, r)),
                    ("/", (l, r) => new Div(l, r))
                },
                new(string, OperatorCreator) [] {
                    ("+", (l, r) => new Add(l, r)),
                    ("-", (l, r) => new Sub(l, r))
                },
                new(string, OperatorCreator) [] {
                    ("<=", (l, r) => new Leq(l, r)),
                    ("<", (l, r) => new Lt(l, r)),
                    (">=", (l, r) => new Geq(l, r)),
                    (">", (l, r) => new Gt(l, r))
                },
                new(string, OperatorCreator) [] {
                    ("==", (l, r) => new Eq(l, r)),
                    ("!=", (l, r) => new Neq(l, r))
                },
                new(string, OperatorCreator) [] {
                    ("&&", (l, r) => new And(l, r))
                },
                new(string, OperatorCreator) [] {
                    ("||", (l, r) => new Or(l, r))
                },
            });

        public static readonly Parser<Expr> MainParser = MultiplicativeParser;

        public static T Times<T>(int n, Func<T, T> f, T value) => n == 0 ? value : (Times(n - 1, f, f(value)));
        public static Parser<Expr> BinOpParser(Parser<Expr> elemParser, IEnumerable < (string, OperatorCreator) > operators) {
            Parser<Func<Expr, Expr>> restParser =
                operators
                .Select(x => from _ in Parse.String(x.Item1).Token() from elem in elemParser select new Func<Expr, Expr>(l => x.Item2(l, elem)))
                .Aggregate((x, y) => x.Or(y));
            return
            from elem in elemParser
            from rest in restParser.Many()
            select rest.Aggregate(elem, (acc, f) => f(acc));
        }

        public static Parser<Expr> BinOpParser(Parser<Expr> elemParser, IEnumerable < IEnumerable < (string, OperatorCreator) >> operators) => operators.Aggregate(elemParser, (acc, definitions) =>
            BinOpParser(acc, definitions));

    }
}

/*

        primary ::=    <int> | <bool> | <ident> | (<expr>)
        app ::= <primary> | <app> <primary>
        unary ::= <app> | !<app> |
        multiplicative ::= <unary> | <multiplicative> * <unary> | <multiplicative> / <unary>
        additive ::= <unary> | <additive> + <unary> | <additive> - <unary>
        relational ::=  <additive> | <relational> <= <additive> | <relational> < <additive> | <relational> >= <additive> | <relational> > <additive> |
        equality ::= <relational> | <equality> == <relational> | <equality> != <relational>
        logical_and ::= <equality> | <logical_and> && <equality>
        logical_or ::= <equality> | <logical_or> && <equality>
        expr ::= <logical_or> | if <expr> then <expr> else <expr> | let rec <ident> <ident> = <expr> in <expr> | let <ident> = <expr> in <expr> | \ <ident> -> <expr>
 */
