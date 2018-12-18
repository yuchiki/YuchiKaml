namespace expression {
    using System.Collections.Generic;
    using System.Linq;
    using System;
    using Sprache;

    using OperatorCreator = System.Func<Expr, Expr, Expr>;

    static class ExprParser {
        public static Parser<Expr> UnitParser =
            from _ in Parse.String("()")
        select new Unit();

        public static Parser<Expr> IntParser =
            from digits in Parse.Digit.AtLeastOnce().Text().Token()
        select new CInt(int.TryParse(digits, out var n) ? n : -1);

        public static Parser<Expr> BoolParser =
            from str in "true".ToToken()
            .Or("false".ToToken())
        select new CBool(str == "true" ? true : false);

        //FIXME: this way to rule out tokens doesn't appear work well.
        // This send empty token into AppParser, and AppParser throws exception.
        public static readonly Parser<string> IDParser =
            Parse.Letter.AtLeastOnce().Text().ExceptTokens(new [] { "true", "false", "let", "rec", "in", "if", "then", "else" }).Token();

        public static readonly Parser<Expr> VarParser =
            from id in IDParser select new Var(id);

        public static readonly Parser<Expr> ParenParser =
            from _ in Parse.Char('(')
        from e in MainParser
        from __ in Parse.Char(')')
        select e;

        public static readonly Parser<Expr> PrimaryParser =
            IntParser.Or(UnitParser).Or(BoolParser).Or(VarParser).Or(ParenParser).Token();

        public static readonly Parser<Expr> AppParser =
            PrimaryParser.AtLeastOnce().Select(primaries => primaries.Aggregate((e1, e2) => new App(e1, e2)));

        public static readonly Parser<Expr> UnaryParser =
            from negSequence in Parse.Char('!').Token().Many()
        from app in AppParser
        select Times(negSequence.Count(), x => new Not(x), app);

        public static readonly Parser<Expr> LogicalOrParser =
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

        public static readonly Parser<Expr> LetParser =
            from _ in "let".ToToken()
        from x in IDParser
        from ___ in "=".ToToken()
        from e1 in MainParser
        from ____ in "in".ToToken()
        from e2 in MainParser
        select new Bind(x, e1, e2);

        public static readonly Parser<Expr> LetRecParser =
            from _ in "let".ToToken()
        from __ in "rec".ToToken()
        from f in IDParser
        from x in IDParser
        from ___ in "=".ToToken()
        from e1 in MainParser
        from ____ in "in".ToToken()
        from e2 in MainParser
        select new LetRec(f, x, e1, e2);

        public static readonly Parser<Expr> TopExprParser =
            OrParser(new Parser<Expr>[] {
                from _ in "if".ToToken()
                from e1 in MainParser
                from __ in "then".ToToken()
                from e2 in MainParser
                from ___ in "else".ToToken()
                from e3 in MainParser
                select new If(e1, e2, e3),
                    LetRecParser,
                    LetParser,
                    from _ in "\\".ToToken()
                from x in IDParser
                from __ in "->".ToToken()
                from e in MainParser
                select new Abs(x, e),
                    LogicalOrParser
            });

        public static readonly Parser<Expr> MainParser = TopExprParser;

        public static readonly string[] Keywords = { "true", "false", "let", "rec", "in", "if", "then", "else" };

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

        public static Parser<Expr> OrParser(IEnumerable<Parser<Expr>> parsers) => parsers.Aggregate((a, b) => a.Or(b));

        public static Parser<T> ExceptTokens<T>(this Parser<T> parser, IEnumerable<string> tokens) => tokens.Aggregate(parser, (acc, token) => acc.Except(Parse.String(token).Token()));
        static Parser<String> ToToken(this string tokenExpression) => Parse.String(tokenExpression).Token().Text();

    }
}

/*

        primary ::=  <unit> | <int> | <bool> | <ident> | (<expr>)
        app ::= <primary> | <app> <primary>
        unary ::= <app> | !<app> |
        multiplicative ::= <unary> | <multiplicative> * <unary> | <multiplicative> / <unary>
        additive ::= <unary> | <additive> + <unary> | <additive> - <unary>
        relational ::=  <additive> | <relational> <= <additive> | <relational> < <additive> | <relational> >= <additive> | <relational> > <additive> |
        equality ::= <relational> | <equality> == <relational> | <equality> != <relational>
        logical_and ::= <equality> | <logical_and> && <equality>
        logical_or ::= <equality> | <logical_or> && <equality>
        topexpr ::= <logical_or> | if <expr> then <expr> else <expr> | let rec <ident> <ident> = <expr> in <expr> | let <ident> = <expr> in <expr> | \ <ident> -> <expr>
 */
