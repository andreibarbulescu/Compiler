string positiveExample = "src/lexer/examples/bubblesort.src";
string structs = "tests/ParserTests/SimpleStruct.src";
Lexer reader = new(structs, "tests/lexerTests/struct.outtokens", "tests/lexerTests/positive.error");
reader.readFile();
List<Token> list = reader.GetList();
Parser pars = new Parser("tests/ParserTests/output.outDerivation",list);
pars.Parse();



/*
int number = 0;
do{
    try{

    reader.returnNextToken(number);
    number++;
    }
    catch(Exception e){
        break;
    }

} while(true);*/