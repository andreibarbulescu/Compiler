public class TypeCheckingVisitor : IVisitor
{
    private string _warningFile = "";
    private string _errorFile = "";

    private string _warningString = "";
    private string _erorrString = "";

    int mainFuncCount = 0;
    List<String> structList = new List<String>();
    List<String> freeFunctionList = new List<String>();
    public TypeCheckingVisitor(string warningFile,string errorFile){
        this._warningFile = warningFile;
        this._errorFile = errorFile;
    }
    public void Visit(StructNode node)
    {   structList.Add(node._symtabentry._name);

        checkForMultiplyDeclaredDataMember(node);    
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
        checkMainMethod();
        checkMultiplyDeclaredStruct();
        checkForMultiplyDeclaredFreeFunc();
        checkForUndeclaredFreeFunc();
        checkForDotOpNotUsedOnClass();
        write(_errorFile,_erorrString);
        write(_warningFile,_warningString);
    
     }

    private void checkForDotOpNotUsedOnClass()
    {
    }

    private void checkForUndeclaredFreeFunc()
    {
    }

    private void checkForUndeclaredClass(Node node)
    {

        if (node._symtabentry._type != "FLOAT" && node._symtabentry._type != "INTEGER"){

            if (!structList.Contains(node._symtabentry._type))
            {
               _erorrString += "Undeclared Class " + node._symtabentry._type + "\n"; 
            }
        }
    }

    private void checkForMultiplyDeclaredIdentifierInFunction(Node node)
    {
        Dictionary<string,int> varDeclCount = new();
        foreach (var variable in node._symbolTable._entries)
        {
            if (varDeclCount.ContainsKey(variable._name))
            {
                varDeclCount[variable._name]++;
            }
            else{
                varDeclCount.Add(variable._name, 1);
            }
        }
        foreach (KeyValuePair<string, int> entry in varDeclCount)
        {
            if (entry.Value > 1)
            {
                _erorrString += $"Variable '{entry.Key}' is declared {entry.Value} times in {node._symtabentry._name} function.\n";
            }
        }
    }

    private void checkForMultiplyDeclaredDataMember(Node node)
    {
        Dictionary<string,int> varDeclCount = new();
        foreach (var variable in node._symbolTable._entries)
        {
            if (varDeclCount.ContainsKey(variable._name))
            {
                varDeclCount[variable._name]++;
            }
            else{
                varDeclCount.Add(variable._name, 1);
            }
        }
        foreach (KeyValuePair<string, int> entry in varDeclCount)
        {
            if (entry.Value > 1)
            {
                _erorrString += $"Variable '{entry.Key}' is declared {entry.Value} times in {node._symtabentry._name}.\n";
            }
        }
    }

    private void checkForMultiplyDeclaredFreeFunc()
    {
        Dictionary<string,int> FreeFuncCount = new();
        foreach (string funcName in freeFunctionList)
        {
            if (FreeFuncCount.ContainsKey(funcName))
            {
                FreeFuncCount[funcName]++;
            }
            else{
                FreeFuncCount.Add(funcName, 1);
            }
        }
        foreach (KeyValuePair<string, int> entry in FreeFuncCount)
        {
            if (entry.Value > 1)
            {
                _erorrString += $"Free Function '{entry.Key}' is declared {entry.Value} times.\n";
            }
        }
    }

    private void checkMultiplyDeclaredStruct()
    {
        Dictionary<string,int> structCount = new();
        foreach (string structName in structList)
        {
            if (structCount.ContainsKey(structName))
            {
                structCount[structName]++;
            }
            else{
                structCount.Add(structName, 1);
            }
        }
        foreach (KeyValuePair<string, int> entry in structCount)
        {
            if (entry.Value > 1)
            {
                _erorrString += $"Struct '{entry.Key}' is declared {entry.Value} times.\n";
            }
        }
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
        
        if (node._rightSibling != null && node._rightSibling._type == NodeType.DOT)
        {
            Console.WriteLine(node._value + "aaah");
        }

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

        if (node._symbolTable._uppertable._title == "Global")
        {
            freeFunctionList.Add(node._symtabentry._name);
        }

        string returnType = node._LeftMostchild.GetRightMostChild()._value;
        string actualReturn = "";

        if(node.GetRightMostChild() != null && node.GetRightMostChild().GetRightMostChild() != null ){

            actualReturn = node.GetRightMostChild().GetRightMostChild()._value;
        }

        checkForMultiplyDeclaredIdentifierInFunction(node);

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
        
        checkForUndeclaredClass(node);
        
        foreach (var child in node.getChildren())
        {
            child.Accept(this);
        }
    }

    public void Visit(ImplNode node)
    {
        string implNodeName = node._LeftMostchild._value;

        if(!structList.Contains(implNodeName)){
            _erorrString += "Class not declared cannot implement " + implNodeName + "\n";
        }
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
        NodeType leftTypet = node._LeftMostchild._type;
        string leftType = node._LeftMostchild._type.ToString();
        string rightType = node._LeftMostchild._rightSibling._type.ToString();

        if (!leftType.Equals(rightType)){
            _erorrString += "error performing addition on " + leftType + " and " + rightType + "\n"; 
        }else{

            node._type = leftTypet;
        }

    }

    public void Visit(MultNode node)
    {
        foreach (var child in node.getChildren())
        {
            child.Accept(this);
        }


        NodeType leftTypet = node._LeftMostchild._type;
        string leftType = node._LeftMostchild._type.ToString();
        string rightType = node._LeftMostchild._rightSibling._type.ToString();

        if (!leftType.Equals(rightType)){
            _erorrString += "error performing multiplication on " + leftType + " and " + rightType + "/n"; 
        }else{

            node._type = leftTypet;
        }
    }

    public void Visit(IntlitNode node)
    {
        node._type = NodeType.INT;
        foreach (var child in node.getChildren())
        {
            child.Accept(this);
        }
    }

    public void Visit(FloatLitNode node)
    {
        node._type = NodeType.FLOAT;
        foreach (var child in node.getChildren())
        {
            child.Accept(this);
        }
    }

    public void Visit(AssignNode node)
    {

        // Console.WriteLine(node._LeftMostchild._symtabentry._type);
        // var piece = node._LeftMostchild._rightSibling._type;
        // Console.WriteLine(piece);
        // Console.WriteLine("-----");


        foreach (var child in node.getChildren())
        {
            child.Accept(this);
        }
        string leftType = node._LeftMostchild._type.ToString();
        NodeType type = node._LeftMostchild._type;
        string righType = node._LeftMostchild._rightSibling._type.ToString();

        if (leftType.Equals(righType))
        {
           node._type = type;  
        }
        else{
            //_erorrString += "Assign Error " + leftType + " and " + righType + "\n";
        }
        // Console.WriteLine(leftType);
        // Console.WriteLine(righType);
        // Console.WriteLine("=====");

        //Console.WriteLine(node.GetRightMostChild()._symtabentry._type);
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

    public void checkMainMethod(){
        if (mainFuncCount == 0){
            _erorrString += "No Main Function Encountered in the file\n";
        }
        else if(mainFuncCount > 1){
            _erorrString += "More than one main function Encountered\n";
        }
    }
}