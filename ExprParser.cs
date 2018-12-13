namespace expression {
    using System.Collections.Generic;
    using System.Linq;
    using System;
    using Sprache;

    static class ExprParser {

        public static Parser<Expr> ParseInt =
            from digits in Parse.Digit.Many().Text().Token()
        select new CInt(int.TryParse(digits, out var n) ? n : -1);

        public static Parser<Expr> ParseBool =
            from str in Parse.String("true").Text().Token()
            .Or(Parse.String("false").Text().Token())
        select new CBool(str == "true" ? true : false);

        public static readonly Parser<string> ParseID =
            Parse.Letter.AtLeastOnce().Text().Token();

        public static readonly Parser<Expr> ParseVar =
            from id in ParseID select new Var(id);

        public static readonly Parser<Expr> ParseParen =
            from _ in Parse.Char('(')
        from e in MainParser
        from __ in Parse.Char(')')
        select e;

        public static readonly Parser<Expr> PrimaryParser =
            ParseInt.Or(ParseBool).Or(ParseVar).Or(ParseParen).Token();

        public static readonly Parser<Expr> AppParser = PrimaryParser.Many().Select(primaries => primaries.Aggregate((e1, e2) => new App(e1, e2)));

        public static readonly Parser<Expr> UnaryParser =
            from negSequence in Parse.Char('!').Token().Many()
        from app in AppParser
        select Times(negSequence.Count(), x => new Not(x), app);

        public static readonly Parser<Func<Expr, Expr>> MultiplicativeRestParser =
            (
                from _ in Parse.Char('*').Token() from u in UnaryParser select new Func<Expr, Expr>(e => new Mul(e, u))).Or(
                from __ in Parse.Char('/').Token() from u in UnaryParser select new Func<Expr, Expr>(e => new Div(e, u)));

        public static readonly Parser<Expr> MultiplicativeParser =
            from u in UnaryParser
        from rest in MultiplicativeRestParser.Many()
        select rest.Aggregate(u, (acc, f) => f(acc));

        public static readonly Parser<Expr> MainParser = MultiplicativeParser;

        public static T Times<T>(int n, Func<T, T> f, T value) => n == 0 ? value : (Times(n - 1, f, f(value)));
    }
}

/*

        primary ::=    <int> | <bool> | <ident> | (<expr>)
        app ::= <primary> | <app> <primary>
        unary ::= <app> | !<app> |
        multiplicative ::= <unary> | <multiplicative> * <unary> | <multiplicative> / <unary>
        additive ::= <unary> | <additive> + <unary> | <additive> / <unary>
        relational ::=  <additive> | <relational> <= <additive> | <relational> < <additive> | <relational> >= <additive> | <relational> > <additive> |
        equality ::= <relational> | <equality> == <relational> | <equality> != <relational>
        logical_and ::= <equality> | <logical_and> && <equality>
        logical_or ::= <equality> | <logical_or> && <equality>
        expr ::= <logical_or> | if <expr> then <expr> else <expr> | let rec <ident> <ident> = <expr> in <expr> | let <ident> = <expr> in <expr> | \ <ident> -> <expr>
 */
