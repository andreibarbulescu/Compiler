public class SymbolTable{
    string _title = "";
    List<SymbolEntry> _entries;
    int _offset = 0;
	public int _size = 0;
	public int _tablelevel = 0;
	public SymbolTable? _uppertable = null;

    public SymbolTable()
    {
        
    }
    public SymbolTable(int level, SymbolTable uppertable){
		_tablelevel = level;
		_title = "";
		_entries = new List<SymbolEntry>();
		_uppertable = uppertable;
	}
	
	public SymbolTable(int p_level, String p_name, SymbolTable p_uppertable){
		_tablelevel = p_level;
		_title = p_name;
		_entries = new List<SymbolEntry>();
		_uppertable = p_uppertable;
	}
	
	public void addEntry(SymbolEntry p_entry){
		_entries.Add(p_entry);	
	}
}