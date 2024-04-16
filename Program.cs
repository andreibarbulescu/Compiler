using System.Linq.Expressions;
using System.Runtime.InteropServices;

string polynomialExample = "tests/ParserTests/polynomial.src";
string bubblesortExample = "tests/ParserTests/bubblesort.src";
string simpleMain = "";

Console.WriteLine("========================================================================");
Console.WriteLine("Welcome to my compiler, please select a test file from the following: \n");
Console.WriteLine("polynomial.src");
Console.WriteLine("bubblesort.src");
Console.WriteLine("pol.txt");
Console.WriteLine("polynomialSemError.src");
Console.WriteLine("polynomialSemError2.src");
Console.WriteLine("========================================================================");


string inputFile = "";

string userAnswer = Console.ReadLine();


inputFile = "tests/ParserTests/" + userAnswer;

string trimmedInput = userAnswer.Substring(0, userAnswer.Length - 4);

string TokenOutPutFile = "tests/lexerTests/" + trimmedInput  + ".outtokens";
string TokenErrorFile = "tests/lexerTests/" + trimmedInput + ".outerrortokens";


string DerivationFile = "tests/ParserTests/" + trimmedInput + ".outDerivation";
string AstFile = "tests/ParserTests/" + trimmedInput + ".dotoutast";
string SymbolTableFile = "tests/symbolTableTests/" + trimmedInput + ".symboltable";
string CodeGenFile = "tests/symbolTableTests/" + trimmedInput + ".moon";

string warningSemFile = "tests/symbolTableTests/" + trimmedInput + ".outsemanticwarnings";
string errorSemFile = "tests/symbolTableTests/" + trimmedInput + ".outsemanticerrors";


Lexer reader = new(inputFile, TokenOutPutFile, TokenErrorFile);

reader.readFile();

List<Token> list = reader.GetList();

Parser pars = new Parser(DerivationFile,list,AstFile);

ProgNode astTree = pars.Parse();

SymbolTableGen vistor = new(""); 
MemorySizeVisitor memSizeVisitor = new MemorySizeVisitor(SymbolTableFile);
TypeCheckingVisitor typeVisitor = new(warningSemFile,errorSemFile);
CodeGenVisitor codeGenVisitor= new CodeGenVisitor();

astTree.Accept(vistor);
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
Console.WriteLine(errorSemFile);
Console.WriteLine(warningSemFile);