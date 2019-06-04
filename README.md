# YuchiKaml

Toy Language Interpreter using Sprache, a C# Parser Combinator language.

## Version

0.1

## What is this

This is an interpreter of YuchiKaml, a toy language designed by me.
YuchiKaml is a dynamic typed language with ML-like surface grammar. For further specification, see the [document](https://github.com/yuchiki/YuchiKaml_Document/blob/master/document.pdf).

This interpreter is written for my personal purpose to get accustomed with Sprache.

## Install(For Windows, Linux and OSX)

1. install dotnet command following an [install instruction of dotnet command](https://dotnet.microsoft.com/learn/dotnet/hello-world-tutorial)
2. clone this repository and do `make` in the directory.

```sh
cd your/working/directory
git clone git://github.com/yuchiki/YuchiKaml.git
cd YuchikiKaml
make
```

3. copy YuchikiML_build to anywhere you want.
4. put the following line on your ~/.bashrc: `export PATH=/path/to/YuchikiML_build:$PATH`
5. `source ~./bashrc`

## YuchiKaml Language

YuchiKaml is a dynamic-typed langauge with ML-like surface grammar.

See [GCD example](https://github.com/yuchiki/YuchiKaml/blob/master/Samples/gcd) and the [document](https://github.com/yuchiki/YuchiKaml_Document/blob/master/document.pdf).

#### Comment

-   // .... end of line
-   (\* ... \*)

## YuchiKaml Interpreter

YuchiKaml interpreter consists of YuchiKaml Preprocessor, Parser and Runner.

### Usage

`YuchikiML *Filename*`

For further information,
`YuchikiML --help`

### Preprocess

-   #include\<name\>
-   #include"name"

### Known Bugs

#### In Parse of Keyword-like Variable

The interpreter cannot parse variables starting with keywords in its definition positions.
For example,

> let ifa = 1 in ()

cannot be parsed. (note that "if" is a keyword.)
