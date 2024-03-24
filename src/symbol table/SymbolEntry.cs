using System.ComponentModel;

public class SymbolEntry
{
    public String          _kind       = null;
	public String          _type       = null;
	public String          _name       = null;
	public int             _size       = 0;
	public int             _offset     = 0;
	public SymbolTable     _subtable   = null;
	public List<int>       _dimensions ;
	
	public SymbolEntry() {}
	
	public SymbolEntry(String p_kind, String p_type, String p_name, SymbolTable p_subtable){
		_kind = p_kind;
		_type = p_type;
		_name = p_name;
		_subtable = p_subtable;
	}
}