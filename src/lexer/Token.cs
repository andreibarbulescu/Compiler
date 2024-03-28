using Microsoft.VisualBasic;

public class Token{

    private string _name;
    private int _line;

    private TokenType _type;

    public Token(string name, TokenType type, int line)
    {
        this._line = line;
        this._type = type;
        this._name = name;
        
    }

    public Token(){
        
    }
    public string GetName(){
        return _name;
    }

    public int GetLine(){
        return _line;
    }

    public TokenType GetTokenType(){
        return _type;
    }

    public void SetType(TokenType type){
        _type = type;
    }

    public void SetLine(int line){
        _line = line;
    }

    public void SetName(string name){
        _name = name;
    } 

    override
    public string ToString(){
        return "[" + _type.ToString() + ", " + _name + ", " + _line + "]";
    }

    public static void WriteTokensToFile(List<Token> tokens, string filePath)
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

        public static void UpdateTokenTypesForReservedWords(List<Token> tokenList)
    {
        foreach (var token in tokenList)
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