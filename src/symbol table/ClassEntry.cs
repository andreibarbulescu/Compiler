public class ClassEntry : SymbolEntry
{
    public ClassEntry():base()
    {
        
    }

    public ClassEntry(String name, SymbolTable subtable) 
    :base("class",name, subtable)
    {
        
    }

    public override String ToString(){
        return "type : " + _type + " name : " + _name + " subtable : " + _subtable._title;
    }
}