using Microsoft.VisualBasic;

class Token{

    private string _name;
    private int _line;

    private TokenType _type;

    public Token(string name, TokenType type, int line)
    {
        this._line = line;
        this._type = type;
        this._name = name;
        
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

 }