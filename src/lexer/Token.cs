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

    public string toString(){
        return "[" + _type + ", " + _name + ", " + _line + "]";
    }

 }