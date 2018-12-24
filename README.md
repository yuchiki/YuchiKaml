# YuchiKaml
Toy Language Interpreter using Sprache, a C# Parser Combinator language.

## What is this?

This is an interpreter of YuchiKaml, a toy language designed by me.
YuchiKaml is a dynamic typed language with ML-like surface grammar.

This interpreter is written for my personal purpose to get accustomed with Sprache.

## YuchiKaml Language

I define and explain YuchiKaml Language.

### Syntax

To be Described.

#### BNF

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
  
#### Comment
// .... end of line
(* ... *)

### Semantics

To be Described.

### Note

To be Described.

## YuchiKaml Interpreter

YuchiKaml is an interpreter.

### Usage

To be described

### Preprocess

- #include\<name\>
- #include"name"

### Known Bugs

#### In Parse of Keyword-like Variable

The interpreter cannot parse variables starting with keywords in its definition positions.
For example,

> let ifa = 1 in ()

cannot be parsed. (note that "if" is a keyword.)
