public class Lexer
{

    private readonly string _path;
    private string _tokenOutput;
    private string _errorOutput;
    private List<Token> _tokenList;
    private List<Token> _errorTokenList;
    private string _lineString;

    //the lines start at 1
    private int _lineNumber = 1;

    public Lexer(string path, string tokenOutput, string errorOutpu)
    {
        this._path = path;
        this._tokenOutput = tokenOutput;
        this._errorOutput = errorOutpu;
        this._tokenList = new List<Token>();
        this._errorTokenList = new List<Token>();
        this._lineString = "";

    }

    public List<Token> GetList(){
        return _tokenList;
    }

    public void returnNextToken(int position){
        try
        {
            
        Console.WriteLine(_tokenList[position]);
        }
        catch (System.Exception)
        {
            throw; 
            Console.WriteLine("End of file");
        }
    }
    public void readFile()
    {
        try
        {
            using (StreamReader reader = new(_path))
            {
                // Read and display lines from the file until the end of the file is reached
                while ((_lineString = reader.ReadLine()) != null)
                {

                    //Acts as a pointer going from left to right till the end of the line
                    for (int counter = 0; counter < _lineString.Length; counter++)
                    {
                        //Checks for a valid number 
                        if (char.IsDigit(_lineString[counter]))
                        {
                            string number = "";
                            number += _lineString[counter];
                            TokenType currentType = TokenType.INTNUM;

                            Boolean addedtolist = false;
                            counter++;

                            // Loop until no more numeric, decimal point, 'e', or '-' for scientific notation are found
                            while (counter < _lineString.Length && (char.IsDigit(_lineString[counter]) || _lineString[counter] == '.' || _lineString[counter] == 'e' || (_lineString[counter] == '-' && _lineString[counter - 1] == 'e')))
                            {
                                if (_lineString[counter] == '.' || _lineString[counter] == 'e' || (_lineString[counter] == '-' && _lineString[counter - 1] == 'e'))
                                {
                                    // If a '.' or 'e' or '-' following 'e' is found, the number is a float or in scientific notation
                                    currentType = TokenType.FLOATNUM;
                                }


                                number += _lineString[counter];
                                counter++;
                            }


                            if (number[0] == '0' && number.Length > 1)
                            {
                                var tokenERROR = new Token(number, currentType, _lineNumber);
                                if (tokenERROR.GetTokenType() == TokenType.INTNUM)
                                {
                                    tokenERROR.SetType(TokenType.INVALIDINT);
                                }
                                else
                                {
                                    tokenERROR.SetType(TokenType.INVALIDFLOAT);
                                }
                                _errorTokenList.Add(tokenERROR);
                                addedtolist = true;
                            }


                            if (number.Contains('e'))
                            {
                                for (int j = 0; j < number.Length; j++)
                                {
                                    if (number[j] == 'e' && number[j + 1] == '0')
                                    {
                                        var tokenERROR = new Token(number, TokenType.INVALIDFLOAT, _lineNumber);
                                        addedtolist = true;
                                        _errorTokenList.Add(tokenERROR);
                                    }
                                }
                            }
                            if (!number.Contains('e') && number.Contains('.') && number[number.Length - 1] == '0')
                            {

                                var tokenERROR = new Token(number, TokenType.INVALIDFLOAT, _lineNumber);
                                addedtolist = true;
                                _errorTokenList.Add(tokenERROR);
                            }

                            if (!addedtolist)
                            {
                                var token = new Token(number, currentType, _lineNumber);
                                _tokenList.Add(token);
                            }
                        }

                        if (counter >= _lineString.Length)
                        {
                            break;
                        }
                        //Id starts with a letter necessarily
                        if (char.IsLetter(_lineString[counter]) || _lineString[counter] == '_')
                        {
                            var (identifier, newPosition) = IdentifierProcessing.ProcessIdentifier(_lineString, counter);
                            counter = newPosition; // Update the index to the new position after processing the identifier
                            if (identifier[0] == '_')
                            {
                                var errorToken = new Token(identifier, TokenType.INVALIDID, _lineNumber);
                                _errorTokenList.Add(errorToken);
                            }
                            else
                            {
                                var token = new Token(identifier, TokenType.ID, _lineNumber);
                                _tokenList.Add(token);
                            }
                            counter++;
                        }

                        if (counter >= _lineString.Length)
                        {
                            break;
                        }

                        SymbolsProcessing.processSymbol(_lineString,counter);

                        switch (_lineString[counter])
                        {
                            case ' ':
                                break;
                            case '\t':
                                break;
                            case '.':
                                Token dot = new Token(".", TokenType.DOT, _lineNumber);
                                _tokenList.Add(dot);
                                break;
                            case '\n':
                                break;
                            case '*':
                                Token mult = new Token("*", TokenType.MULT, _lineNumber);
                                _tokenList.Add(mult);
                                break;
                            case '/':
                                Token div = new Token("/", TokenType.DIV, _lineNumber);
                                _tokenList.Add(div);
                                break;
                            case ',':
                                Token comma = new Token(",", TokenType.COMMA, _lineNumber);
                                _tokenList.Add(comma);
                                break;
                            case '!':
                                Token bang = new Token("!", TokenType.NOT, _lineNumber);
                                _tokenList.Add(bang);
                                break;
                            case '+':
                                Token plus = new Token("+", TokenType.PLUS, _lineNumber);
                                _tokenList.Add(plus);
                                break;
                            case '>':
                                if (_lineString[counter + 1] == '=')
                                {
                                    Token greateq = new Token(">=", TokenType.GEQ, _lineNumber);
                                    _tokenList.Add(greateq);
                                    counter++;
                                    break;
                                }
                                else
                                {
                                    Token greater = new Token(">", TokenType.GT, _lineNumber);
                                    _tokenList.Add(greater);
                                    break;
                                }
                            case ':':
                                if (_lineString[counter + 1] == ':')
                                {
                                    Token coloncolon = new Token("::", TokenType.COLONCOLON, _lineNumber);
                                    _tokenList.Add(coloncolon);
                                    counter++;
                                    break;
                                }
                                else
                                {
                                    Token colon = new Token(":", TokenType.COLON, _lineNumber);
                                    _tokenList.Add(colon);
                                    break;
                                }
                            case '-':
                                if (_lineString[counter + 1] == '>')
                                {
                                    Token arrow = new Token("->", TokenType.ARROW, _lineNumber);
                                    _tokenList.Add(arrow);
                                    counter++;
                                    break;
                                }
                                else
                                {
                                    Token minus = new Token("-", TokenType.MINUS, _lineNumber);
                                    _tokenList.Add(minus);
                                    break;
                                }
                            case '<':
                                if (_lineString[counter + 1] == '>')
                                {
                                    Token noteq = new Token("<>", TokenType.NOTEQ, _lineNumber);
                                    _tokenList.Add(noteq);
                                    counter++;
                                    break;
                                }
                                else if (_lineString[counter + 1] == '=')
                                {
                                    Token leq = new Token("<=", TokenType.LEQ, _lineNumber);
                                    _tokenList.Add(leq);
                                    counter++;
                                    break;
                                }
                                else
                                {
                                    Token less = new Token("<", TokenType.LT, _lineNumber);
                                    _tokenList.Add(less);
                                    break;
                                }
                            case '(':
                                Token openPar = new Token("(", TokenType.OPENPAR, _lineNumber);
                                _tokenList.Add(openPar);
                                break;
                            case ')':
                                Token closedPar = new Token(")", TokenType.CLOSEPAR, _lineNumber);
                                _tokenList.Add(closedPar);
                                break;
                            case '{':
                                Token openCurl = new Token("{", TokenType.OPENCUBR, _lineNumber);
                                _tokenList.Add(openCurl);
                                break;
                            case '}':
                                Token closedCurl = new Token("}", TokenType.CLOSECUBR, _lineNumber);
                                _tokenList.Add(closedCurl);
                                break;
                            case '[':
                                Token openSquare = new Token("[", TokenType.OPENSQBR, _lineNumber);
                                _tokenList.Add(openSquare);
                                break;
                            case ']':
                                Token closedSquare = new Token("]", TokenType.CLOSESQBR, _lineNumber);
                                _tokenList.Add(closedSquare);
                                break;
                            case ';':
                                Token semi = new Token(";", TokenType.SEMI, _lineNumber);
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
                                if (_lineString[counter + 1] == '=')
                                {
                                    Token equal = new Token("==", TokenType.EQ, _lineNumber);
                                    _tokenList.Add(equal);
                                    counter++;
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
                                Token error = new Token(Char.ToString(_lineString[counter]), TokenType.INVALIDSYMBOL, _lineNumber);
                                _errorTokenList.Add(error);
                                break;
                        }
                    }
                    _lineNumber++;
                }
            }

            Token.UpdateTokenTypesForReservedWords(_tokenList);
            Token.WriteTokensToFile(_tokenList, _tokenOutput);
            Token.WriteTokensToFile(_errorTokenList, _errorOutput);

        }

        catch (IOException)
        {
            Console.WriteLine("An Exception has occured");

        }

    }

}