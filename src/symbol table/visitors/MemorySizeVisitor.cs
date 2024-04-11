public class MemorySizeVisitor : IVisitor
{
    private string outputFile;
    private string _content;
    public MemorySizeVisitor(){
        this.outputFile = "";
        this._content = "";
    }
    public MemorySizeVisitor(string output){
        this.outputFile = output;
        this._content = "";
    }
    public int sizeOfEntry(Node node) {
		int size = 0;
		if(node._symtabentry._type == "INTEGER")
			size = 4;
		else if(node._symtabentry._type == "FLOAT")
			size = 8;

		VarEntry ve = (VarEntry) node._symtabentry; 
		if(ve._dimensions.Count != 0)
			foreach (var dimen in ve._dimensions)
            {
                size *= dimen;
            }	
		return size;
	}
	
	public int sizeOfTypeNode(Node node) {
		int size = 0;
		if(node._type == NodeType.INT || node._type == NodeType.INTLIT)
			size = 4;
		else if(node._type == NodeType.FLOAT || node._type == NodeType.FLOATLIT)
			size = 8;
		return size;
	}

    public void Visit(StructNode node)
    {
        foreach(var child in node.getChildren()){
            child.Accept(this);
        }
        
        foreach (var entry in node._symbolTable._entries)
        {
            entry._offset = node._symbolTable._size - entry._size;
            node._symbolTable._size -= entry._size;
        }

        _content += node._symbolTable;
        
    }

    public void Visit(ProgNode progNode)
    {
        foreach(var child in progNode.getChildren()){
            child.Accept(this);
        }
        _content += progNode._symbolTable;
        Console.WriteLine(_content);
    }

    public void Visit(Node node)
    {
        foreach(var child in node.getChildren()){
            child.Accept(this);
        }
    }

    public void Visit(IdNode node)
    {
        foreach(var child in node.getChildren()){
            child.Accept(this);
        }
    }

    public void Visit(FuncDefNode node)
    {
        foreach(var child in node.getChildren()){
            child.Accept(this);
        }

        foreach (var entry in node._symbolTable._entries)
        {
            entry._offset = node._symbolTable._size - entry._size;
            node._symbolTable._size -= entry._size;
        }
        _content += node._symbolTable;
    }

    public void Visit(FuncDeclNode node)
    {
        foreach(var child in node.getChildren()){
            child.Accept(this);
        }
    }

    public void Visit(VarDeclNode node)
    {
        foreach(var child in node.getChildren()){
            child.Accept(this);
        }
        node._symtabentry._size = sizeOfEntry(node);
        Console.WriteLine(node._symtabentry._size);
    }

    public void Visit(ImplNode node)
    {
        foreach(var child in node.getChildren()){
            child.Accept(this);
        }
    }

    public void Visit(AddNode node)
    {
        foreach(var child in node.getChildren()){
            child.Accept(this);
        }
        node._symtabentry._size = sizeOfEntry(node);
    }

    public void Visit(MultNode node)
    {
        foreach(var child in node.getChildren()){
            child.Accept(this);
        }
        node._symtabentry._size = sizeOfEntry(node);
    }

    public void Visit(IntlitNode node)
    {
        foreach(var child in node.getChildren()){
            child.Accept(this);
        }
        node._symtabentry._size = sizeOfEntry(node);
    }

    public void Visit(FloatLitNode node)
    {
        foreach(var child in node.getChildren()){
            child.Accept(this);
        }
        node._symtabentry._size = sizeOfEntry(node);
    }

    public void Visit(AssignNode node)
    {
        throw new NotImplementedException();
    }
}