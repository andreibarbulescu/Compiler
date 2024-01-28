string positiveExample = "src/lexer/examples/lexpositivegrading.src";
string negativeExample = "src/lexer/examples/lexnegativegrading.src";
Lexer reader = new(positiveExample, "tests/lexerTests/PositiveTokens.outtokens", "tests/lexerTests/positive.error");
Lexer reader2 = new(negativeExample, "tests/lexerTests/negative.token", "tests/lexerTests/negative.errors");
reader.readFile();
reader2.readFile();
