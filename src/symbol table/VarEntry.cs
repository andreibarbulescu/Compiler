public class VarEntry : SymbolEntry{
    public VarEntry(string kind, string type, string name, List<int> dimension)
    :base(kind,type,name,null){
        _dimensions = dimension;
    }
    public VarEntry() : base(){

    }

    public override string ToString(){
		return "kind : " + _kind + " type: " + _type + " name " 
		+ _name + " size: " + _size + " offset: " + _offset;
	}
}