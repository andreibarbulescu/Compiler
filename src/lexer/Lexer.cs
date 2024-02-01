public class Lexer{

    private readonly string _path;

    private string _tokenOutput;
    private string _errorOutput;

    private List<Token> _tokenList ;
    private List<Token> _errorTokenList;
    private string _lineString;

    //the lines start at 1
    private int _lineNumber = 1;
    
    public Lexer(string path,string tokenOutput,string errorOutpu)
    {
        this._path = path;
        this._tokenOutput = tokenOutput;
        this._errorOutput = errorOutpu;
        this._tokenList = new List<Token>();
        this._errorTokenList = new List<Token>();
        this._lineString = "";

    }

    // Method to Process Identifier
    private (string identifier, int newPosition) ProcessIdentifier(string line, int startIndex)
    {
        int i = startIndex;
        string id = "" + line[i]; 

        // Loop to capture the rest of the identifier
        while (i + 1 < line.Length && (char.IsLetterOrDigit(line[i + 1]) || line[i + 1] == '_'))
        {
            i++;
            id += line[i];
        }

        return (id, i); 
    }

    private (string identifier, int newPosition) ProcessIntegerAndFloat(string line, int startIndex){

        int i = startIndex;
        string number = "" + line[i];
        TokenType initial = TokenType.INTNUM;
        Boolean inList = false;

        //while(i+1 < line.Length && )


        return (number,i);
    }

    public void readFile(){
        try
        {
            using (StreamReader reader = new(_path))
            {
                // Read and display lines from the file until the end of the file is reached
                while ((_lineString = reader.ReadLine()) != null)
                {

                    //Acts as a pointer going from left to right till the end of the line
                    for (int i = 0; i < _lineString.Length; i++)
                    {
                        //Checks for a valid number 
                        if (char.IsDigit(_lineString[i]))
                        {
                            string number = "";
                            number += _lineString[i];
                            TokenType currentType = TokenType.INTNUM; 

                            Boolean addedtolist = false;
                            i++;

                            // Loop until no more numeric, decimal point, 'e', or '-' for scientific notation are found
                            while (i < _lineString.Length && (char.IsDigit(_lineString[i]) || _lineString[i] == '.' || _lineString[i] == 'e' || (_lineString[i] == '-' && _lineString[i - 1] == 'e')))
                            {
                                if (_lineString[i] == '.' || _lineString[i] == 'e' || (_lineString[i] == '-' && _lineString[i - 1] == 'e'))
                                {
                                    // If a '.' or 'e' or '-' following 'e' is found, the number is a float or in scientific notation
                                    currentType = TokenType.FLOATNUM;
                                }


                                number += _lineString[i];
                                i++;
                            }


                            if (number[0] == '0' && number.Length > 1)
                            {
                                var tokenERROR = new Token(number,currentType, _lineNumber);
                                if(tokenERROR.GetTokenType() == TokenType.INTNUM){
                                    tokenERROR.SetType(TokenType.INVALIDINT);
                                }
                                else{
                                    tokenERROR.SetType(TokenType.INVALIDFLOAT);
                                }
                                _errorTokenList.Add(tokenERROR);
                                addedtolist = true;
                            }


                            if(number.Contains('e')){
                                for (int j = 0; j < number.Length; j++)
                                {
                                    if (number[j] == 'e' && number[j+1] == '0')
                                    {
                                            var tokenERROR = new Token(number,TokenType.INVALIDFLOAT,_lineNumber);
                                            addedtolist = true;
                                            _errorTokenList.Add(tokenERROR);                                       
                                    }
                                }
                            }
                            if(!number.Contains('e') && number.Contains('.') && number[number.Length-1] == '0'){

                                            var tokenERROR = new Token(number,TokenType.INVALIDFLOAT,_lineNumber);
                                            addedtolist = true;
                                            _errorTokenList.Add(tokenERROR);                                       
                            }
                            
                            
                            if(!addedtolist)
                            {
                                var token = new Token(number, currentType, _lineNumber);
                                _tokenList.Add(token);
                            }

                           // i--; // Adjust for over-incrementing in the loop
                        }

                        if (i >= _lineString.Length)
                        {
                            break;
                        }
                        //Id starts with a letter necessarily
                        if (char.IsLetter(_lineString[i]) || _lineString[i] == '_')
                        {
                            var (identifier, newPosition) = ProcessIdentifier(_lineString, i);
                            i = newPosition; // Update the index to the new position after processing the identifier
                            
                            if(identifier[0] == '_'){
                                var errorToken = new Token(identifier,TokenType.INVALIDID,_lineNumber);
                                _errorTokenList.Add(errorToken);
                            }
                            else{

                            var token = new Token(identifier, TokenType.ID, _lineNumber);
                            _tokenList.Add(token);
                            }
                            i++;

                        }

  
                       if (i>= _lineString.Length)
                        {
                            break;
                        }

                        switch (_lineString[i])
                        {
                            
                            case ' ':
                                //maybe remove this?
                                //i++;
                                break;
                            case '\t':
                                break;

                            case '.':
                                Token dot = new Token(".",TokenType.DOT,_lineNumber);
                                _tokenList.Add(dot);
                                break;

                            //idk about this either
                            case '\n':
                                break;
                            case '*':
                                Token mult = new Token("*",TokenType.MULT,_lineNumber);
                                _tokenList.Add(mult);
                                break;

                            case '/':
                                Token div = new Token("/",TokenType.DIV,_lineNumber);
                                _tokenList.Add(div);
                                break;
                            case ',':
                                Token comma = new Token(",",TokenType.COMMA,_lineNumber);
                                _tokenList.Add(comma);
                                break;
                            case '!':
                                Token bang = new Token("!",TokenType.NOT,_lineNumber);
                                _tokenList.Add(bang);
                                break;
                            case '+':
                                Token plus = new Token("+", TokenType.PLUS, _lineNumber);                               
                                _tokenList.Add(plus);
                                break;
                            case '>':
                                if (_lineString[i+1] == '=')
                                {
                                    Token greateq = new Token(">=",TokenType.GEQ,_lineNumber);
                                    _tokenList.Add(greateq);
                                    i++;
                                    break;
                                }
                                else{
                                    Token greater = new Token(">",TokenType.GT,_lineNumber);
                                    _tokenList.Add(greater);
                                    break;
                                }
                            case ':':
                                if(_lineString[i+1] == ':'){
                                    Token coloncolon = new Token("::",TokenType.COLONCOLON,_lineNumber);
                                    _tokenList.Add(coloncolon);
                                    i++;
                                    break;
                                }
                                else{
                                    Token colon = new Token(":",TokenType.COLON,_lineNumber);
                                    _tokenList.Add(colon);
                                    break;
                                }
                            case '-':
                                if(_lineString[i + 1] == '>'){
                                    Token arrow = new Token("->",TokenType.ARROW, _lineNumber);
                                    _tokenList.Add(arrow);
                                    i++;
                                    break;
                                }
                                else
                                {
                                    Token minus = new Token("-", TokenType.MINUS, _lineNumber);
                                    _tokenList.Add(minus);
                                    break;
                                }
                            case '<':
                                if (_lineString[i+1] == '>')
                                {
                                    Token noteq = new Token("<>",TokenType.NOTEQ,_lineNumber);
                                    _tokenList.Add(noteq);
                                    i++;
                                    break;
                                }
                                else if(_lineString[i+1] == '='){
                                    Token leq = new Token("<=", TokenType.LEQ,_lineNumber);
                                    _tokenList.Add(leq);
                                    i++;
                                    break;
                                }
                                else{
                                    Token less = new Token("<",TokenType.LT,_lineNumber);
                                    _tokenList.Add(less);
                                    break;
                                }
                            case '(':
                                Token openPar = new Token("(",TokenType.OPENPAR, _lineNumber);
                                _tokenList.Add(openPar);
                                break;
                            case ')':
                                Token closedPar = new Token(")",TokenType.CLOSEPAR,_lineNumber);
                                _tokenList.Add(closedPar);
                                break;
                            case '{':
                                Token openCurl = new Token("{",TokenType.OPENCUBR,_lineNumber);
                                _tokenList.Add(openCurl);
                                break;
                            case '}':
                                Token closedCurl = new Token("}",TokenType.CLOSECUBR,_lineNumber);
                                _tokenList.Add(closedCurl);
                                break;
                            case '[':
                                Token openSquare = new Token("[",TokenType.OPENSQBR,_lineNumber);
                                _tokenList.Add(openSquare);
                                break;
                            case ']':
                                Token closedSquare = new Token("]",TokenType.CLOSESQBR,_lineNumber);
                                _tokenList.Add(closedSquare);
                                break;
                            case ';':
                                Token semi = new Token(";",TokenType.SEMI, _lineNumber);
                                _tokenList.Add(semi);
                                break;
                            
                            case '|':
                                Token or = new Token("|", TokenType.OR, _lineNumber);
                                _tokenList.Add(or);
                                break;

                            case '&':
                                Token and = new Token("&", TokenType.AND, _lineNumber);
                                _tokenList.Add(and);
                                break;
                            case '=':
                                if (_lineString[i + 1] == '=')
                                {

                                    Token equal = new Token("==", TokenType.EQ, _lineNumber);
                                    _tokenList.Add(equal);
                                    i++;
                                    break;
                                }
                                else
                                {
                                    Token assign = new Token("=", TokenType.ASSIGN, _lineNumber);
                                    _tokenList.Add(assign);
                                    break;
                                }
                            default:
                                //not too sure how to do this...
                                Token error = new Token(Char.ToString(_lineString[i]),TokenType.INVALIDSYMBOL,_lineNumber);
                                _errorTokenList.Add(error);
                                break;

                        }
                    }
                _lineNumber++;
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
                if (token.GetLine() != currentLine)
                {
                    writer.WriteLine();
                    currentLine = token.GetLine();
                }
                writer.Write(token.ToString());
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
                switch (token.GetName())
                {
                    case "if":
                        token.SetType(TokenType.IF);
                        break;
                    case "public":
                        token.SetType(TokenType.PUBLIC);
                        break;
                    case "read":
                        token.SetType(TokenType.READ);
                        break;
                    case "then":
                        token.SetType(TokenType.THEN);
                        break;
                    case "else":
                        token.SetType(TokenType.ELSE);
                        break;
                    case "void":
                        token.SetType(TokenType.VOID);
                        break;
                    case "private":
                        token.SetType(TokenType.PRIVATE);
                        break;
                    case "func":
                        token.SetType(TokenType.FUNC);
                        break;
                    case "var":
                        token.SetType(TokenType.VAR);
                        break;
                    case "struct":
                        token.SetType(TokenType.STRUCT);
                        break;
                    case "while":
                        token.SetType(TokenType.WHILE);
                        break;
                    case "write":
                        token.SetType(TokenType.WRITE);
                        break;
                    case "return":
                        token.SetType(TokenType.RETURN);
                        break;
                    case "self":
                        token.SetType(TokenType.SELF);
                        break;
                    case "inherits":
                        token.SetType(TokenType.INHERITS);
                        break;
                    case "let":
                        token.SetType(TokenType.LET);
                        break;
                    case "impl":
                        token.SetType(TokenType.IMPL);
                        break;
                    case "integer":
                        token.SetType(TokenType.INTEGER);
                        break;
                    case "float":
                        token.SetType(TokenType.FLOAT);
                        break;
                    default:
                        break;
                }
            }
    }
}