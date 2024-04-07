public class FunctionEntry : SymbolEntry{
	public List<VarEntry> _params   = new List<VarEntry>();
    
    private Boolean isImplemented = false;
    public FunctionEntry():base()
    {
        
    }

	
	public FunctionEntry(String type, String name, List<VarEntry> parameters, SymbolTable table)
    :base("Function",type,name,table){
		
		_params = parameters;
	}

    public override string ToString(){


    string para = "";
        if (_params.Count != 0)
        {
            
            foreach (var item in _params)
            {
                para += item._type + " ";
            }
            return "kind: " + _kind + ", return type : " + _type + ", name : " + _name + 
             ", parameters : " + para + ", subtable : " + _subtable._title;
        }
        

        return "kind: " + _kind + ", return type : " + _type + ", name : " + _name +", subtable: " + _subtable._title ;
        
    }


    
}