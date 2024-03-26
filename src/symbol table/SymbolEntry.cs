using System.ComponentModel;

public class SymbolEntry
{
    public String          _kind       = null;
	public String          _type       = null;
	public String          _name       = null;
	public int             _size       = 0;
	public int             _offset     = 0;
	public SymbolTable     _subtable   = null;
	public List<int>       _dimensions = new();
	
	public SymbolEntry() {}

	public SymbolEntry(string type, string name, SymbolTable table){
		_type = type;
		_name = name;
		_subtable = table;
		_dimensions = new List<int>();
	}
	
	public SymbolEntry(String kind, String type, String name, SymbolTable subtable){
		_kind = kind;
		_type = type;
		_name = name;
		_subtable = subtable;
		_dimensions = new List<int>();
	}

	public override string ToString(){
		return "kind : " + _kind + " type: " + _type + " name " 
		+ _name + " subtable : " + _subtable._title;
	}


}