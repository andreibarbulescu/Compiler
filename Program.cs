using System.Linq.Expressions;
using System.Runtime.InteropServices;

string polynomialExample = "tests/ParserTests/polynomial.src";
string bubblesortExample = "tests/ParserTests/bubblesort.src";

Lexer reader = new(polynomialExample, "tests/lexerTests/bubblewsort.outtokens", "tests/lexerTests/positive.error");

reader.readFile();

List<Token> list = reader.GetList();

Parser pars = new Parser("tests/ParserTests/bubblewoutput.outDerivation",list);

ProgNode astTree = pars.Parse();

SymbolTableGen vistor = new(""); 
TypeCheckingVisitor visitor2 = new();
MemorySizeVisitor memSizeVisitor = new MemorySizeVisitor();

astTree.Accept(vistor);
astTree.Accept(visitor2);
astTree.Accept(memSizeVisitor); 