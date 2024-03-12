string positiveExample = "tests/ParserTests/bubblesort.src";
string structs = "tests/ParserTests/SimpleStruct.src";

string polynomialExample = "tests/ParserTests/polynomial.src";

Lexer reader = new(polynomialExample, "tests/lexerTests/bubblewsort.outtokens", "tests/lexerTests/positive.error");

reader.readFile();

List<Token> list = reader.GetList();
Parser pars = new Parser("tests/ParserTests/bubblewoutput.outDerivation",list);
pars.Parse();

Node nodeMaker = new Node();

var child1 = Node.MakeNode("1", NodeType.INTLIT);
var child2 = Node.MakeNode("2",NodeType.INTLIT);


Node assign = nodeMaker.makeFamily(NodeType.ASSIGN,child1,child2);
assign._value = "assign";
Node anotherchild = Node.MakeNode("seven",NodeType.EQUALS);
Node mult = nodeMaker.makeFamily(NodeType.MULT,assign,anotherchild);




// Console.WriteLine(child1._parent);
// Console.WriteLine(child2._parent);

//assign.PrintTree(mult);
//Console.WriteLine(mult.ToDotString());


