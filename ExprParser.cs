namespace expression {
    using System.Collections.Generic;
    using System.Linq;
    using System;
    using Sprache;

    static class ExprParser {

        public static Parser<CInt> ParseInt =
            from digits in Parse.Digit.Many().Text()
        select new CInt(int.Parse(digits));

        public static Parser<CBool> ParseBool =
            from str in Parse.String("true").Text()
            .Or(Parse.String("false").Text())
        select new CBool(str == "true" ? true : false);

    }
}

/*
        expression e ::= n | x | e + e | e * e | e - e | e / e | let x = e in e | \x -> e | e e
            | b | e && e | e || e | !e
            | e == e | e != e | e <= e | e < e | e >= e | e > e
            | if e then e else e
            | let rec f x = e in e

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
