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
    public string getName(){
        return _name;
    }

    public int getLine(){
        return _line;
    }

    public TokenType GetTokenType(){
        return _type;
    }

    public void setType(TokenType type){
        _type = type;
    }

    public void setLine(int line){
        _line = line;
    }

    public void setName(string name){
        _name = name;
    } 
    
    public string toString(){
        return "[" + _type.ToString() + ", " + _name + ", " + _line + "]";
    }

 }