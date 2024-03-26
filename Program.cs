string positiveExample = "tests/ParserTests/bubblesort.src";
string structs = "tests/ParserTests/SimpleStruct.src";

string polynomialExample = "tests/ParserTests/polynomial.src";

Lexer reader = new(polynomialExample, "tests/lexerTests/bubblewsort.outtokens", "tests/lexerTests/positive.error");

reader.readFile();

List<Token> list = reader.GetList();
Parser pars = new Parser("tests/ParserTests/bubblewoutput.outDerivation",list);

ProgNode astTree = pars.Parse();

SymbolTableGen vistor = new("");
astTree.Accept(vistor);



