public class SymbolTableGen : IVisitor
{
    private string _outputFile;
    private string _content; 
    public SymbolTableGen(string output){
        _outputFile = output;
        _content = "";
    }
    public void Visit(StructNode node)
    {
        Console.WriteLine("Struct Node has been visited!");
        SymbolTable structTable = new SymbolTable(1, node._LeftMostchild._value,null);
        node._symbolTable = structTable;
        foreach (var item in node.getChildren())
        {
            item.Accept(this);
        }
        
    }

    public void Visit(ProgNode progNode)
    {
        Console.WriteLine("Prog Node has been visited");
        Console.WriteLine("Global Table Created");
        SymbolTable globalTable = new(0,"Global",null);
        progNode._symbolTable = globalTable;
        foreach(var child in progNode.getChildren()){

            child._symbolTable = progNode._symbolTable;
			child.Accept(this);
            Console.WriteLine(child._value + " type is of " + child.GetType().Name);
        }
    }

    public void Visit(Node node)
    {
        Console.WriteLine("Simple Node was visited");
        foreach (var child in node.getChildren()) {
			child._symbolTable = node._symbolTable;
			child.Accept(this);
		}
    }

    public void Visit(IdNode node)
    {
        Console.WriteLine(" id node has been visited " + node._value);
    }

    public void write(){
        try
        {
            using var writer = new StreamWriter(_outputFile);
            writer.Write(_content);
            writer.Close();

        }
        catch (Exception)
        {
            Console.WriteLine($"An exception occurred while writing to the file: {_outputFile}");
            throw;
        }
    }
}