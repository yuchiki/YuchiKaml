namespace expression {
    using System.Collections.Generic;
    using System.Linq;
    using System;
    using Sprache;

    using OperatorCreator = System.Func<Expr, Expr, Expr>;

    static class ExprParser {
        private static readonly string[] KeyWords = new [] { "true", "false", "let", "rec", "in", "if", "then", "else" };

        private static readonly Parser<Expr> UnitParser =
            from _ in Parse.String("()").Named("unit")
        select new Unit();

        private static Parser<Expr> stringParserInner =
            from _ in Parse.Char('"')
        from s in Parse.CharExcept(new char[] { '"', }).Many().Text()
        from __ in Parse.Char('"')
        select new CString(s.Replace("\\n", "\n"));
        private static readonly Parser<Expr> StringParser = stringParserInner.Named("string");

        private static Parser<Expr> intParserInner =
            from digits in Parse.Digit.AtLeastOnce().Text().Token()
        select new CInt(int.TryParse(digits, out var n) ? n : -1);
        private static readonly Parser<Expr> IntParser = intParserInner.Named("int");

        private static Parser<Expr> boolParserInner =
            from str in OrParser("true".ToToken(), "false".ToToken())
        select new CBool(str == "true");
        private static readonly Parser<Expr> BoolParser = boolParserInner.Named("bool");

        // FIXME: It can parse "spin", but cannot parse "init",
        private static readonly Parser<string> IDParser =
            Parse.Letter.Or(Parse.Char('_')).AtLeastOnce().Text().Token().ExceptTokens(KeyWords).Named("id");

        private static readonly Parser<Expr> VarParser = IDParser.Named("var").Select(id => new Var(id));

        private static readonly Parser<Expr> parenParserInner =
            from _ in Parse.Char('(')
        from e in MainParser
        from __ in Parse.Char(')')
        select e;
        private static readonly Parser<Expr> ParenParser = parenParserInner.Named("paren expr");

        private static readonly Parser<Expr> PrimaryParser =
            OrParser(StringParser, IntParser, UnitParser, BoolParser, VarParser, ParenParser).Token().Named("primary");

        private static readonly Parser<Expr> AppParser =
            PrimaryParser.AtLeastOnce().Select(primaries => primaries.Aggregate(Expr.App)).Named("app");

        private static readonly Parser<Expr> unaryParserInner =
            from negSequence in Parse.Char('!').Token().Many()
        from app in AppParser
        select Times(negSequence.Count(), Expr.Not, app);
        private static readonly Parser<Expr> UnaryParser = unaryParserInner.Named("unary");

        private static readonly Parser<Expr> LogicalOrParser =
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
            }).Named("Logical Or");

        private static readonly Parser<Expr> ifParserInner =
            from _ in "if".ToToken()
        from e1 in MainParser
        from __ in "then".ToToken()
        from e2 in MainParser
        from ___ in "else".ToToken()
        from e3 in MainParser
        select new If(e1, e2, e3);
        private static readonly Parser<Expr> IfParser = ifParserInner.Named("if");

        private static readonly Parser<Expr> letParserInner =
            from _ in "let".ToToken()
        from x in IDParser
        from variables in IDParser.Many()
        from ___ in "=".ToToken()
        from e1 in MainParser.Named("let e1")
        from ____ in "in".ToToken()
        from e2 in MainParser.Named("let e2")
        select new Bind(x, variables.Reverse().Aggregate(e1, Flip<string, Expr, Expr>(Expr.Abs)), e2);
        private static readonly Parser<Expr> LetParser = letParserInner.Named("let");

        private static readonly Parser<Expr> letRecParserInner =
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
        private static readonly Parser<Expr> LetRecParser = letRecParserInner.Named("let");

        private static readonly Parser<Expr> absParserInner =
            from _ in "\\".ToToken()
        from x in IDParser
        from __ in "->".ToToken()
        from e in MainParser
        select new Abs(x, e);
        private static readonly Parser<Expr> AbsParser = absParserInner.Named("let");

        private static readonly Parser<Expr> TopExprParser =
            OrParser(IfParser, LetRecParser, LetParser, AbsParser, LogicalOrParser).Named("top");

        public static readonly Parser<Expr> MainParser = TopExprParser;

        /* Followings are helper functions */

        private static T Times<T>(int n, Func<T, T> f, T value) => n == 0 ? value : (Times(n - 1, f, f(value)));
        private static Parser<Expr> BinOpParser(Parser<Expr> elemParser, IEnumerable < (string, OperatorCreator) > operators) {
            Parser<Func<Expr, Expr>> restParser =
                operators
                .Select(x => from _ in Parse.String(x.Item1).Token() from elem in elemParser select new Func<Expr, Expr>(l => x.Item2(l, elem)))
                .Aggregate((x, y) => x.Or(y));
            return
            from elem in elemParser
            from rest in restParser.Many()
            select rest.Aggregate(elem, (acc, f) => f(acc));
        }

        private static Parser<Expr> BinOpParser(Parser<Expr> elemParser, IEnumerable < IEnumerable < (string, OperatorCreator) >> operators) => operators.Aggregate(elemParser, (acc, definitions) =>
            BinOpParser(acc, definitions));

        private static Parser<T> OrParser<T>(IEnumerable<Parser<T>> parsers) => parsers.Aggregate((a, b) => a.Or(b));
        private static Parser<T> OrParser<T>(params Parser<T>[] parsers) => OrParser((IEnumerable<Parser<T>>) parsers);

        private static Parser<T> ExceptTokens<T>(this Parser<T> parser, IEnumerable<string> tokens) => tokens.Aggregate(parser, (acc, token) => acc.Except(token.ToToken())).Token();
        private static Parser<String> ToToken(this string tokenExpression) =>
            Parse.String(tokenExpression).Token().Text();

        private static Func<T2, T1, T3> Flip<T1, T2, T3>(this Func<T1, T2, T3> f) => (t2, t1) => f(t1, t2);
    }
}

/*

        primary 0 ::=  <unit> | <int> | <bool> | <ident> | (<expr>)
        app 1::= <primary> | <app> <primary>
        unary 2 ::= <app> | !<app> |
        multiplicative 3::= <unary> | <multiplicative> * <unary> | <multiplicative> / <unary>
        additive 4::= <unary> | <additive> + <unary> | <additive> - <unary>
        relational 5::=  <additive> | <relational> <= <additive> | <relational> < <additive> | <relational> >= <additive> | <relational> > <additive> |
        equality 6::= <relational> | <equality> == <relational> | <equality> != <relational>
        logical_and 7::= <equality> | <logical_and> && <equality>
        logical_or 8::= <equality> | <logical_or> && <equality>
        topexpr 9::= <logical_or> | if <expr> then <expr> else <expr> | let rec <ident> <ident> = <expr> in <expr> | let <ident> = <expr> in <expr> | \ <ident> -> <expr>
 */
