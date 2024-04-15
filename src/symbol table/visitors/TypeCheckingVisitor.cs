public class TypeCheckingVisitor : IVisitor
{
    private string _warningFile = "";
    private string _errorFile = "";

    private string _warningString = "";
    private string _erorrString = "";

    int mainFuncCount = 0;
    List<String> structList = new List<String>();
    public TypeCheckingVisitor(string warningFile,string errorFile){
        this._warningFile = warningFile;
        this._errorFile = errorFile;
    }
    public void Visit(StructNode node)
    {
        foreach (var child in node.getChildren())
        {
           child.Accept(this); 
        }
    }

    public void Visit(ProgNode progNode)
    {
        foreach (var child in progNode.getChildren())
        {
            child.Accept(this);
        }
        if (mainFuncCount == 0){
            _erorrString += "No Main Function Encountered in the file";
        }
        else if(mainFuncCount > 1){
            _erorrString += "More than one main function Encountered";
        }
        
        write(_errorFile,_erorrString);
        write(_warningFile,_warningString);
    
     }

    public void Visit(Node node)
    {
        foreach (var child in node.getChildren())
        {
            child.Accept(this);
        }
        
    }

    public void Visit(IdNode node)
    {
        foreach (var child in node.getChildren())
        {
            child.Accept(this);
        }
    }

    public void Visit(FuncDefNode node)
    {
        //Console.WriteLine(node._symtabentry._name);
        if (node._symtabentry._name == "main")
        {
            mainFuncCount++;
        }
        foreach (var child in node.getChildren())
        {
            child.Accept(this);
        }
    }

    public void Visit(FuncDeclNode node)
    {   
        foreach (var child in node.getChildren())
        {
            child.Accept(this);
        }
    }

    public void Visit(VarDeclNode node)
    {
        foreach (var child in node.getChildren())
        {
            child.Accept(this);
        }
    }

    public void Visit(ImplNode node)
    {
        foreach (var child in node.getChildren())
        {
            child.Accept(this);
        }
    }

    public void Visit(AddNode node)
    {
        foreach (var child in node.getChildren())
        {
            child.Accept(this);
        }
    }

    public void Visit(MultNode node)
    {
        foreach (var child in node.getChildren())
        {
            child.Accept(this);
        }
    }

    public void Visit(IntlitNode node)
    {
        foreach (var child in node.getChildren())
        {
            child.Accept(this);
        }
    }

    public void Visit(FloatLitNode node)
    {
        foreach (var child in node.getChildren())
        {
            child.Accept(this);
        }
    }

    public void Visit(AssignNode node)
    {
        
        foreach (var child in node.getChildren())
        {
            child.Accept(this);
        }
    }

    public void write(string errorFile, string error){
        try
        {
            using var writer = new StreamWriter(errorFile);
            writer.Write(error);
            writer.Close();

        }
        catch (Exception)
        {
            Console.WriteLine($"An exception occurred while writing to the file: {errorFile}");
            throw;
        }
    }
}