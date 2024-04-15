using System.Linq.Expressions;
using System.Runtime.InteropServices;

string polynomialExample = "tests/ParserTests/polynomial.src";
string bubblesortExample = "tests/ParserTests/bubblesort.src";
string simpleMain = "";

Console.WriteLine("========================================================================");
Console.WriteLine("Welcome to my compiler, please select a test file from the following: \n");
Console.WriteLine("1. Polynomial.src");
Console.WriteLine("2. Bubblesort.src");
Console.WriteLine("3. SimpleMain.src");
Console.WriteLine("========================================================================");


string inputFile = "";
int answ = Convert.ToInt32(Console.ReadLine());
switch (answ){
    case 1: inputFile = polynomialExample; break;
    case 2: inputFile = bubblesortExample; break;

}

string TokenOutPutFile = "tests/lexerTests/" + "Final" + ".outtokens";
string TokenErrorFile = "tests/lexerTests/" + "Final" + ".outerrortokens";
string DerivationFile = "tests/ParserTests/" + "Final" + ".outDerivation";
string AstFile = "tests/ParserTests/" + "Final" + ".dot";
string SymbolTableFile = "tests/ParserTests/" + "Final" + ".symboltable";
string CodeGenFile = "tests/ParserTests/" + "codeGen" + ".moon";

string warningSemFile = "tests/symbolTableTests/warningSem.txt";
string errorSemFile = "tests/symbolTableTests/errorSem.txt";


Lexer reader = new(inputFile, TokenOutPutFile, TokenErrorFile);

reader.readFile();

List<Token> list = reader.GetList();

Parser pars = new Parser(DerivationFile,list);

ProgNode astTree = pars.Parse();

SymbolTableGen vistor = new(""); 
//TypeCheckingVisitor visitor2 = new();
MemorySizeVisitor memSizeVisitor = new MemorySizeVisitor();
TypeCheckingVisitor typeVisitor = new(warningSemFile,errorSemFile);
CodeGenVisitor codeGenVisitor= new CodeGenVisitor();

astTree.Accept(vistor);
//astTree.Accept(visitor2);
astTree.Accept(memSizeVisitor);
astTree.Accept(typeVisitor);
astTree.Accept(codeGenVisitor);

Console.WriteLine("Compilation Done");
Console.WriteLine("The output files are : ");
Console.WriteLine(TokenOutPutFile);
Console.WriteLine(TokenErrorFile);
Console.WriteLine(DerivationFile);
Console.WriteLine(CodeGenFile);
Console.WriteLine(AstFile);
Console.WriteLine(SymbolTableFile);