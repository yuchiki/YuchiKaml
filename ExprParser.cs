namespace expression {
    using System.Collections.Generic;
    using System.Linq;
    using System;
    using Sprache;

    using OperatorCreator = System.Func<Expr, Expr, Expr>;

    static class ExprParser {
        public static readonly string[] KeyWords = new [] { "true", "false", "let", "rec", "in", "if", "then", "else" };

        public static Parser<Expr> UnitParser =
            from _ in Parse.String("()")
        select new Unit();

        public static Parser<Expr> StringParser =
            from _ in Parse.Char('"')
        from s in Parse.CharExcept(new char[] { '"', }).Many().Text()
        from __ in Parse.Char('"')
        select new CString(s);

        public static Parser<Expr> IntParser =
            from digits in Parse.Digit.AtLeastOnce().Text().Token()
        select new CInt(int.TryParse(digits, out var n) ? n : -1);

        public static Parser<Expr> BoolParser =
            from str in OrParser("true".ToToken(), "false".ToToken())
        select new CBool(str == "true");

        public static readonly Parser<string> IDParser =
            Parse.Letter.AtLeastOnce().Text().ExceptTokens(KeyWords).Token();

        public static readonly Parser<Expr> VarParser =
            from id in IDParser select new Var(id);

        public static readonly Parser<Expr> ParenParser =
            from _ in Parse.Char('(')
        from e in MainParser
        from __ in Parse.Char(')')
        select e;

        public static readonly Parser<Expr> PrimaryParser =
            OrParser(StringParser, IntParser, UnitParser, BoolParser, VarParser, ParenParser).Token();

        public static readonly Parser<Expr> AppParser =
            PrimaryParser.AtLeastOnce().Select(primaries => primaries.Aggregate(Expr.App));

        public static readonly Parser<Expr> UnaryParser =
            from negSequence in Parse.Char('!').Token().Many()
        from app in AppParser
        select Times(negSequence.Count(), Expr.Not, app);

        public static readonly Parser<Expr> LogicalOrParser =
            BinOpParser(UnaryParser, new(string, OperatorCreator) [][] {
                new(string, OperatorCreator) [] {
                    ("*", Expr.Mul), ("/", Expr.Div)
                },
                new(string, OperatorCreator) [] {
                    ("+", Expr.Add), ("-", Expr.Sub)
                },
                new(string, OperatorCreator) [] {
                    ("<=", Expr.Leq), ("<", Expr.Lt), (">=", Expr.Geq), (">", Expr.Gt)
                },
                new(string, OperatorCreator) [] {
                    ("==", Expr.Eq), ("!=", Expr.Neq)
                },
                new(string, OperatorCreator) [] {
                    ("&&", Expr.And)
                },
                new(string, OperatorCreator) [] {
                    ("||", Expr.Or)
                },
            });

        public static readonly Parser<Expr> IfParser =
            from _ in "if".ToToken()
        from e1 in MainParser
        from __ in "then".ToToken()
        from e2 in MainParser
        from ___ in "else".ToToken()
        from e3 in MainParser
        select new If(e1, e2, e3);

        public static readonly Parser<Expr> LetParser =
            from _ in "let".ToToken()
        from x in IDParser
        from variables in IDParser.Many()
        from ___ in "=".ToToken()
        from e1 in MainParser
        from ____ in "in".ToToken()
        from e2 in MainParser
        select new Bind(x, variables.Reverse().Aggregate(e1, Flip<string, Expr, Expr>(Expr.Abs)), e2);

        public static readonly Parser<Expr> LetRecParser =
            from _ in "let".ToToken()
        from __ in "rec".ToToken()
        from f in IDParser
        from x in IDParser
        from variables in IDParser.Many()
        from ___ in "=".ToToken()
        from e1 in MainParser
        from ____ in "in".ToToken()
        from e2 in MainParser
        select new LetRec(f, x, variables.Reverse().Aggregate(e1, Flip<string, Expr, Expr>(Expr.Abs)), e2);

        public static readonly Parser<Expr> AbsParser =
            from _ in "\\".ToToken()
        from x in IDParser
        from __ in "->".ToToken()
        from e in MainParser
        select new Abs(x, e);

        public static readonly Parser<Expr> TopExprParser =
            OrParser(IfParser, LetRecParser, LetParser, AbsParser, LogicalOrParser);

        public static readonly Parser<Expr> MainParser = TopExprParser;

        /* Followings are helper functions */

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

        public static Parser<T> OrParser<T>(IEnumerable<Parser<T>> parsers) => parsers.Aggregate((a, b) => a.Or(b));
        public static Parser<T> OrParser<T>(params Parser<T>[] parsers) => OrParser((IEnumerable<Parser<T>>) parsers);

        public static Parser<T> ExceptTokens<T>(this Parser<T> parser, IEnumerable<string> tokens) => tokens.Aggregate(parser, (acc, token) => acc.Except(Parse.String(token).Token()));
        static Parser<String> ToToken(this string tokenExpression) => Parse.String(tokenExpression).Token().Text();

        static Func<T2, T1, T3> Flip<T1, T2, T3>(this Func<T1, T2, T3> f) => (t2, t1) => f(t1, t2);
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
