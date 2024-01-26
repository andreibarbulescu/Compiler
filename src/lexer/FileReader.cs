public class FileReader{

    private string _path;

    public FileReader(string path)
    {
        this._path = path;
    }

    public void readFile(){

        List<Token> tokenList = new List<Token>();
        List<Token> errorTokenList = new List<Token>();


        try
        {
            using (StreamReader reader = new(_path))
            {
                int lineNumber = 0;
                string line;
                // Read and display lines from the file until the end of the file is reached
                while ((line = reader.ReadLine()) != null)
                {
                    string id = "";
                    
                    lineNumber++;
                    for (int i = 0; i < line.Length; i++)
                    {
                        if (char.IsLetter(line[i]))
                        {
                            id += line[i];
                            while (i + 1 < line.Length && (char.IsLetter(line[i + 1]) || char.IsDigit(line[i+1]) || line[i+1] == '_'))
                            {
                                i++;
                                id += line[i];
                            }
                            var token = new Token(id, TokenType.ID, lineNumber);
                            tokenList.Add(token);
                            id = "";
                        }

                        if (i>= line.Length)
                        {
                            break;
                        }

                        if(char.IsDigit(line[i])){
                            id += line[i];

                            while(i + 1 < line.Length && char.IsDigit(line[i+1])){
                                i++;
                                id+=line[i];
                            }



                            var token = new Token(id,TokenType.INTNUM,lineNumber);
                            tokenList.Add(token);
                            id = "";
                        }

                        if (i>= line.Length)
                        {
                            break;
                        }
                        switch (line[i])
                        {
                            case ' ':
                                //maybe remove this?
                                //i++;
                                break;
                            //idk about this either
                            case '\n':
                                break;
                            case '*':
                                Token mult = new Token("*",TokenType.MULT,lineNumber);
                                tokenList.Add(mult);
                                break;

                            case '/':
                                Token div = new Token("/",TokenType.DIV,lineNumber);
                                tokenList.Add(div);
                                break;
                            case ',':
                                Token comma = new Token(",",TokenType.COMMA,lineNumber);
                                tokenList.Add(comma);
                                break;
                            case '!':
                                Token bang = new Token("!",TokenType.NOT,lineNumber);
                                tokenList.Add(bang);
                                break;
                            case '+':
                                Token plus = new Token("+", TokenType.PLUS, lineNumber);                               
                                tokenList.Add(plus);
                                break;
                            case '>':
                                if (line[i+1] == '=')
                                {
                                    Token greateq = new Token(">=",TokenType.GEQ,lineNumber);
                                    tokenList.Add(greateq);
                                    i++;
                                    break;
                                }
                                else{
                                    Token greater = new Token(">",TokenType.GT,lineNumber);
                                    tokenList.Add(greater);
                                    break;
                                }
                            case ':':
                                if(line[i+1] == ':'){
                                    Token coloncolon = new Token("::",TokenType.COLONCOLON,lineNumber);
                                    tokenList.Add(coloncolon);
                                    i++;
                                    break;
                                }
                                else{
                                    Token colon = new Token(":",TokenType.COLON,lineNumber);
                                    tokenList.Add(colon);
                                    break;
                                }
                            case '-':
                                if(line[i + 1] == '>'){
                                    Token arrow = new Token("->",TokenType.ARROW, lineNumber);
                                    tokenList.Add(arrow);
                                    i++;
                                    break;
                                }
                                else
                                {
                                    Token minus = new Token("-", TokenType.MINUS, lineNumber);
                                    tokenList.Add(minus);
                                    break;
                                }
                            case '<':
                                if (line[i+1] == '>')
                                {
                                    Token noteq = new Token("<>",TokenType.NOTEQ,lineNumber);
                                    tokenList.Add(noteq);
                                    i++;
                                    break;
                                }
                                else if(line[i+1] == '='){
                                    Token leq = new Token("<=", TokenType.LEQ,lineNumber);
                                    tokenList.Add(leq);
                                    i++;
                                    break;
                                }
                                else{
                                    Token less = new Token("<",TokenType.LT,lineNumber);
                                    tokenList.Add(less);
                                    break;
                                }
                            case '(':
                                Token openPar = new Token("(",TokenType.OPENPAR, lineNumber);
                                tokenList.Add(openPar);
                                break;
                            case ')':
                                Token closedPar = new Token(")",TokenType.CLOSEPAR,lineNumber);
                                tokenList.Add(closedPar);
                                break;
                            case '{':
                                Token openCurl = new Token("{",TokenType.OPENCUBR,lineNumber);
                                tokenList.Add(openCurl);
                                break;
                            case '}':
                                Token closedCurl = new Token("}",TokenType.CLOSECUBR,lineNumber);
                                tokenList.Add(closedCurl);
                                break;
                            case '[':
                                Token openSquare = new Token("[",TokenType.OPENSQBR,lineNumber);
                                tokenList.Add(openSquare);
                                break;
                            case ']':
                                Token closedSquare = new Token("]",TokenType.CLOSESQBR,lineNumber);
                                tokenList.Add(closedSquare);
                                break;
                            case ';':
                                Token semi = new Token(";",TokenType.SEMI, lineNumber);
                                tokenList.Add(semi);
                                break;
                            
                            case '|':
                                Token or = new Token("|", TokenType.OR, lineNumber);
                                tokenList.Add(or);
                                break;

                            case '&':
                                Token and = new Token("&", TokenType.AND, lineNumber);
                                tokenList.Add(and);
                                break;
                            case '=':
                                if (line[i + 1] == '=')
                                {

                                    Token equal = new Token("==", TokenType.EQ, lineNumber);
                                    tokenList.Add(equal);
                                    i++;
                                    break;
                                }
                                else
                                {
                                    Token assign = new Token("=", TokenType.ASSIGN, lineNumber);
                                    tokenList.Add(assign);
                                    break;
                                }
                            default:
                                //not too sure how to do this...
                                Token error = new Token(Char.ToString(line[i]),TokenType.ERROR,lineNumber);
                                errorTokenList.Add(error);
                                break;

                        }
                    }

                }
            }
            foreach (var token in tokenList)
            {
                switch (token.getName())
                {
                    case "if":
                        token.setType(TokenType.IF);
                        break;
                    case "public":
                        token.setType(TokenType.PUBLIC);
                        break;
                    case "read":
                        token.setType(TokenType.READ);
                        break;
                    case "then":
                        token.setType(TokenType.THEN);
                        break;
                    case "else":
                        token.setType(TokenType.ELSE);
                        break;
                    case "void":
                        token.setType(TokenType.VOID);
                        break;
                    case "private":
                        token.setType(TokenType.PRIVATE);
                        break;
                    case "func":
                        token.setType(TokenType.FUNC);
                        break;
                    case "var":
                        token.setType(TokenType.VAR);
                        break;
                    case "struct":
                        token.setType(TokenType.STRUCT);
                        break;
                    case "while":
                        token.setType(TokenType.WHILE);
                        break;
                    case "write":
                        token.setType(TokenType.WRITE);
                        break;
                    case "return":
                        token.setType(TokenType.RETURN);
                        break;
                    case "self":
                        token.setType(TokenType.SELF);
                        break;
                    case "inherits":
                        token.setType(TokenType.INHERITS);
                        break;
                    case "let":
                        token.setType(TokenType.LET);
                        break;
                    case "impl":
                        token.setType(TokenType.IMPL);
                        break;
                    case "integer":
                        token.setType(TokenType.INTEGER);
                        break;
                    case "float":
                        token.setType(TokenType.FLOAT);
                        break;
                    default:
                        break;
                }
            }


            try
            {
                StreamWriter write = new("lexPositiveGrading.outlextoken");
                int currentLine = 1;
                foreach (var tokenItem in tokenList)
                {

                    if (tokenItem.getLine() != currentLine)
                    {
                        currentLine++;
                        write.Write("\n" + tokenItem.toString());
                    }
                    else
                    {

                        write.Write(tokenItem.toString());
                    }


                }
                write.Close();

            }
            catch (System.Exception)
            {
                
                throw;
            }

            try
            {
                StreamWriter errorWriter = new StreamWriter("lexPositiveGrading.outlexerrors");
                int currentLine = 1;
                foreach (var tokenItem in errorTokenList)
                {

                    if (tokenItem.getLine() != currentLine)
                    {
                        currentLine++;
                        errorWriter.Write("\n" + tokenItem.toString());
                    }
                    else
                    {

                        errorWriter.Write(tokenItem.toString());
                    }


                }
                errorWriter.Close();

            }
            catch (System.Exception)
            {
                
                throw;
            }

       }

        
        catch (IOException)
        {
            Console.WriteLine("An Exception has occured");

        }

    }
}