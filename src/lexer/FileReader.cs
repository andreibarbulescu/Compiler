public class FileReader{

    private string _path;

    private string _tokenOutput;
    private string _errorOutput;

        List<Token> _tokenList ;
        List<Token> _errorTokenList;

    public FileReader(string path,string tokenOutput,string errorOutpu)
    {
        this._path = path;
        this._tokenOutput = tokenOutput;
        this._errorOutput = errorOutpu;
        this._tokenList = new List<Token>();
        this._errorTokenList = new List<Token>();
    }

    public void readFile(){
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
                            i++;
                            var token = new Token(id, TokenType.ID, lineNumber);
                            _tokenList.Add(token);
                            id = "";
                        }

                        if (i >= line.Length)
                        {
                            break;
                        }

                        if (char.IsDigit(line[i]))
                        {
                            string number = "";
                            number += line[i];
                            TokenType currentType = TokenType.INTNUM; 

                            i++;

                            // Loop until no more numeric, decimal point, 'e', or '-' for scientific notation are found
                            while (i < line.Length && (char.IsDigit(line[i]) || line[i] == '.' || line[i] == 'e' || (line[i] == '-' && line[i - 1] == 'e')))
                            {
                                if (line[i] == '.' || line[i] == 'e' || (line[i] == '-' && line[i - 1] == 'e'))
                                {
                                    // If a '.' or 'e' or '-' following 'e' is found, the number is a float or in scientific notation
                                    currentType = TokenType.FLOATNUM;
                                }

                                number += line[i];
                                i++;
                            }

                            if (number[0] == '0' && number.Length!=1)
                            {
                                var tokenERROR = new Token(number, TokenType.INVALIDINT, lineNumber);
                                _errorTokenList.Add(tokenERROR);
                            }
                            else
                            {
                                var token = new Token(number, currentType, lineNumber);
                                _tokenList.Add(token);
                            }

                            //i--; // Adjust for over-incrementing in the loop
                        }

                        /*
                        //add error checking for integers
                        if (char.IsDigit(line[i]))
                        {
                            Token notsure = new Token(id, TokenType.INTNUM, lineNumber);
                            id += line[i];                           
                            i++;
                            while (i + 1 < line.Length && (char.IsDigit(line[i + 1]) || line[i+1] == '.' || line[i+1] == 'e' || line[i+1] == '-'))
                            {

                                if(line[i] == '.' || line[i+1] == 'e' || line[i+1] == '-'){
                                    notsure.setType(TokenType.FLOATNUM);
                                    id += line[i];
                                }
                                else{
                                    id += line[i];
                                }
                                i++;
                            }
                            i--;
                            
                            notsure.setName(id);
                            _tokenList.Add(notsure);
                            id = "";
                        }
*/
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
                            case '\t':
                                break;

                            case '.':
                                Token dot = new Token(".",TokenType.DOT,lineNumber);
                                _tokenList.Add(dot);
                                break;

                            //idk about this either
                            case '\n':
                                break;
                            case '*':
                                Token mult = new Token("*",TokenType.MULT,lineNumber);
                                _tokenList.Add(mult);
                                break;

                            case '/':
                                Token div = new Token("/",TokenType.DIV,lineNumber);
                                _tokenList.Add(div);
                                break;
                            case ',':
                                Token comma = new Token(",",TokenType.COMMA,lineNumber);
                                _tokenList.Add(comma);
                                break;
                            case '!':
                                Token bang = new Token("!",TokenType.NOT,lineNumber);
                                _tokenList.Add(bang);
                                break;
                            case '+':
                                Token plus = new Token("+", TokenType.PLUS, lineNumber);                               
                                _tokenList.Add(plus);
                                break;
                            case '>':
                                if (line[i+1] == '=')
                                {
                                    Token greateq = new Token(">=",TokenType.GEQ,lineNumber);
                                    _tokenList.Add(greateq);
                                    i++;
                                    break;
                                }
                                else{
                                    Token greater = new Token(">",TokenType.GT,lineNumber);
                                    _tokenList.Add(greater);
                                    break;
                                }
                            case ':':
                                if(line[i+1] == ':'){
                                    Token coloncolon = new Token("::",TokenType.COLONCOLON,lineNumber);
                                    _tokenList.Add(coloncolon);
                                    i++;
                                    break;
                                }
                                else{
                                    Token colon = new Token(":",TokenType.COLON,lineNumber);
                                    _tokenList.Add(colon);
                                    break;
                                }
                            case '-':
                                if(line[i + 1] == '>'){
                                    Token arrow = new Token("->",TokenType.ARROW, lineNumber);
                                    _tokenList.Add(arrow);
                                    i++;
                                    break;
                                }
                                else
                                {
                                    Token minus = new Token("-", TokenType.MINUS, lineNumber);
                                    _tokenList.Add(minus);
                                    break;
                                }
                            case '<':
                                if (line[i+1] == '>')
                                {
                                    Token noteq = new Token("<>",TokenType.NOTEQ,lineNumber);
                                    _tokenList.Add(noteq);
                                    i++;
                                    break;
                                }
                                else if(line[i+1] == '='){
                                    Token leq = new Token("<=", TokenType.LEQ,lineNumber);
                                    _tokenList.Add(leq);
                                    i++;
                                    break;
                                }
                                else{
                                    Token less = new Token("<",TokenType.LT,lineNumber);
                                    _tokenList.Add(less);
                                    break;
                                }
                            case '(':
                                Token openPar = new Token("(",TokenType.OPENPAR, lineNumber);
                                _tokenList.Add(openPar);
                                break;
                            case ')':
                                Token closedPar = new Token(")",TokenType.CLOSEPAR,lineNumber);
                                _tokenList.Add(closedPar);
                                break;
                            case '{':
                                Token openCurl = new Token("{",TokenType.OPENCUBR,lineNumber);
                                _tokenList.Add(openCurl);
                                break;
                            case '}':
                                Token closedCurl = new Token("}",TokenType.CLOSECUBR,lineNumber);
                                _tokenList.Add(closedCurl);
                                break;
                            case '[':
                                Token openSquare = new Token("[",TokenType.OPENSQBR,lineNumber);
                                _tokenList.Add(openSquare);
                                break;
                            case ']':
                                Token closedSquare = new Token("]",TokenType.CLOSESQBR,lineNumber);
                                _tokenList.Add(closedSquare);
                                break;
                            case ';':
                                Token semi = new Token(";",TokenType.SEMI, lineNumber);
                                _tokenList.Add(semi);
                                break;
                            
                            case '|':
                                Token or = new Token("|", TokenType.OR, lineNumber);
                                _tokenList.Add(or);
                                break;

                            case '&':
                                Token and = new Token("&", TokenType.AND, lineNumber);
                                _tokenList.Add(and);
                                break;
                            case '=':
                                if (line[i + 1] == '=')
                                {

                                    Token equal = new Token("==", TokenType.EQ, lineNumber);
                                    _tokenList.Add(equal);
                                    i++;
                                    break;
                                }
                                else
                                {
                                    Token assign = new Token("=", TokenType.ASSIGN, lineNumber);
                                    _tokenList.Add(assign);
                                    break;
                                }
                            default:
                                //not too sure how to do this...
                                Console.WriteLine(line[i]);
                                Token error = new Token(Char.ToString(line[i]),TokenType.ERROR,lineNumber);
                                _errorTokenList.Add(error);
                                break;

                        }
                    }

                }
            }
            
            UpdateTokenTypesForReservedWords();
            WriteTokensToFile(_tokenList,_tokenOutput);
            WriteTokensToFile(_errorTokenList,_errorOutput);

       }

        
        catch (IOException)
        {
            Console.WriteLine("An Exception has occured");

        }

    }

    private void WriteTokensToFile(List<Token> tokens, string filePath)
    {
        try
        {
            using var writer = new StreamWriter(filePath);
            int currentLine = 1;

            foreach (var token in tokens)
            {
                if (token.getLine() != currentLine)
                {
                    writer.WriteLine();
                    currentLine = token.getLine();
                }
                writer.Write(token.toString());
            }
        }
        catch (Exception)
        {
            Console.WriteLine($"An exception occurred while writing to the file: {filePath}");
            throw;
        }
    }

    private void UpdateTokenTypesForReservedWords()
    {
        foreach (var token in _tokenList)
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
    }
}