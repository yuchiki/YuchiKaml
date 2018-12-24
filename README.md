# YuchiKaml

Toy Language Interpreter using Sprache, a C# Parser Combinator language.

## What is this?

This is an interpreter of YuchiKaml, a toy language designed by me.
YuchiKaml is a dynamic typed language with ML-like surface grammar.

This interpreter is written for my personal purpose to get accustomed with Sprache.

## Install(For Windows, Linux and OSX)

1. install dotnet command following an [install instruction of dotnet command](https://dotnet.microsoft.com/learn/dotnet/hello-world-tutorial)

2. clone this repositotry and do `make` in the directory.

```sh
cd your/working/directory
git clone git://github.com/yuchiki/YuchiKaml.git
cd YuchikiKaml
make
```

3. copy YuchikiML_build to anywhere you want, and put the following line on your ~/.bashrc.
`export PATH=/path/to/YuchikiML_build:$PATH`

4. `source ~./bashrc`


## YuchiKaml Language

I define and explain YuchiKaml Language.

### Syntax

To be Described.

#### BNF

-   \<expr\> 10 **:=** \<control_expr> **|** \<exprs\> **;** \<control_expr>
-   \<control_expr\> 9 **::=** \<logical_or\> **|** **if** \<expr\> **then** \<expr\> **else** \<expr\> **|** **let** **rec** \<ident\> **\*\* <ident\> **=** \<expr\> **in** \<expr\> **|\*\* **let** \<ident\> **=** \<expr\> **in** \<expr\> **|** **\\** \<ident\> **->** \<expr\>
-   \<logical_or\> 8 **::=** \<equality\> **|** \<logical_or\> **&&** \<equality\>
-   \<logical_and\> 7 **::=** \<equality\> **|** \<logical_and\> **&&** \<equality\>
-   \<equality\> 6 **::=** \<relational\> **|** \<equality\> **==** \<relational\> **|** \<equality\> **!=** \<relational\>
-   \<relational\> 5 **::=** \<additive\> **|** \<relational\> **<=** \<additive\> **|** \<relational\> **<** \<additive\> **|** <relational\> **>=** \<additive\> **|** \<relational\> **>** \<additive\>
-   \<additive\> 4 **::=** \<unary\> **|** \<additive\> **+** \<unary\> **|** \<additive\> **-** \<unary\>
-   \<multiplicative\> 3 **::=** \<unary\> **|** \<multiplicative\> **\*** \<unary\> **|** \<multiplicative\> **/** \<unary\>
-   \<unary\> 2 **::=** \<app\> **|** **!**\<unary\>
-   \<app\> 1 **::=** \<primary\> **|** \<app\> \*<primary\>
-   \<primary\> 0 **::=** \<unit\> **|** \<int\> **|** \<bool\> **|** \<ident\> **|** **(**\<expr\>**)**

#### Comment

-   // .... end of line
-   (* ... *)

### Semantics

To be Described.

### Note

To be Described.

## YuchiKaml Interpreter

YuchiKaml interpreter consists of YuchiKaml Preprocessor, Parser and Runner.

### Usage

To be described

### Preprocess

-   #include\<name\>
-   #include"name"

### Known Bugs

#### In Parse of Keyword-like Variable

The interpreter cannot parse variables starting with keywords in its definition positions.
For example,

> let ifa = 1 in ()

cannot be parsed. (note that "if" is a keyword.)
