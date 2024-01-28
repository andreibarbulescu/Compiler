string positiveExample = "src/lexer/examples/lexpositivegrading.src";
string negativeExample = "src/lexer/examples/lexnegativegrading.src";
FileReader reader = new(positiveExample, "positive.token", "positive.error");
FileReader reader2 = new(negativeExample, "negative.token", "negative.errors");
reader.readFile();
reader2.readFile();
