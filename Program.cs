string positiveExample = "src/lexer/examples/bubblesort.src";
Lexer reader = new(positiveExample, "tests/lexerTests/PositiveTokensPostrefactor.outtokens", "tests/lexerTests/positive.error");
reader.readFile();
