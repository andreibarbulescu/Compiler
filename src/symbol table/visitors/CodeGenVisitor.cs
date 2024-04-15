public class CodeGenVisitor : IVisitor{
    public CodeGenVisitor(){
        //creating pool of registers assuming we have register 1 to 12
        for (int i = 12; i >= 1; i--)
        {
            _registerPool.Push("r" + i);
        }        
    }
    Stack<String> _registerPool = new();
    string _moonExecCode = "";
    string _moonDataCode = "";
    string _indent = "  ";
    string _outputFile = "tests/symbolTableTests/mooncode.txt";


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

        try
        {
            using var writer = new StreamWriter(_outputFile);
            //Console.WriteLine(_moonExecCode);
            _moonExecCode += _moonDataCode;
            writer.Write(_moonExecCode);
            //writer.Write(_moonDataCode);   
            writer.Close();

        }
        catch (Exception)
        {
            Console.WriteLine($"An exception occurred while writing to the file: {_outputFile}");
            throw;
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
        foreach (var child in node.getChildren())
        {
           child.Accept(this); 
        }
    }

    public void Visit(FuncDefNode node)
    {
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

        string localRegister = _registerPool.Pop();

        switch (node.getChildren()[1]._value)
        {
            case "INT":
                _moonDataCode += _indent + "% space for variable" + node.getChildren()[0]._value + "\n";
                _moonDataCode += "          " + node.getChildren()[0]._value + " res4\n";
                break;
            case "FLOAT":
                _moonDataCode += _indent + "% space for variable" + node.getChildren()[0]._value + "\n";
                _moonDataCode += "          " + node.getChildren()[0]._value + " res4\n";
                break;
            default:
                _moonDataCode += "%error initializing var decl for " + node.getChildren()[0]._value + "\n";
                break;
        } 

        this._registerPool.Push(localRegister);   

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
        
        //creating local variable and allocate registers
        String localRegister      = this._registerPool.Pop();
		String leftChildRegister  = this._registerPool.Pop();
		String rightChildRegister = this._registerPool.Pop();

        //code gen
        _moonExecCode += _indent + "% processing: " + node._moonVarName + " := " + node.getChildren()[0]._moonVarName + " + " + node.getChildren()[1]._moonVarName + "\n";
		_moonExecCode += _indent + "lw "  + leftChildRegister  + "," + node.getChildren()[0]._moonVarName + "(r0)\n";
		_moonExecCode += _indent + "lw "  + rightChildRegister + "," + node.getChildren()[1]._moonVarName + "(r0)\n";
		_moonExecCode += _indent + "add " + localRegister      + "," + leftChildRegister + "," + rightChildRegister + "\n"; 
		_moonDataCode += _indent + "% space for " + node.getChildren()[0]._moonVarName + " + " + node.getChildren()[1]._moonVarName + "\n";

        //Needs fixing
		//_moonDataCode += String.format("%-10s",node._moonVarName) + " res 4\n";
        _moonDataCode += node._moonVarName + " res4\n";
		_moonExecCode += _indent + "sw " + node._moonVarName + "(r0)," + localRegister + "\n";

		// deallocate the registers for the two children, and the current node
		_registerPool.Push(leftChildRegister);
		_registerPool.Push(rightChildRegister);
		_registerPool.Push(localRegister);
    }

    public void Visit(MultNode node)
    {
        foreach (var child in node.getChildren())
        {
           child.Accept(this); 
        }
        
        //creating local variable and allocate registers
        String localRegister      = this._registerPool.Pop();
		String leftChildRegister  = this._registerPool.Pop();
		String rightChildRegister = this._registerPool.Pop();

        //code gen
        _moonExecCode += _indent + "% processing: " + node._moonVarName + " := " + node.getChildren()[0]._moonVarName + " * " + node.getChildren()[1]._moonVarName + "\n";
		_moonExecCode += _indent + "lw "  + leftChildRegister  + "," + node.getChildren()[0]._moonVarName + "(r0)\n";
		_moonExecCode += _indent + "lw "  + rightChildRegister + "," + node.getChildren()[1]._moonVarName + "(r0)\n";
		_moonExecCode += _indent + "mul " + localRegister      + "," + leftChildRegister + "," + rightChildRegister + "\n"; 
		_moonDataCode += _indent + "% space for " + node.getChildren()[0]._moonVarName + " * " + node.getChildren()[1]._moonVarName + "\n";

        //Needs fixing
		//_moonDataCode += String.format("%-10s",node._moonVarName) + " res 4\n";
        _moonDataCode += node._moonVarName + " res4\n";
		_moonExecCode += _indent + "sw " + node._moonVarName + "(r0)," + localRegister + "\n";

		// deallocate the registers for the two children, and the current node
		_registerPool.Push(leftChildRegister);
		_registerPool.Push(rightChildRegister);
		_registerPool.Push(localRegister);
    }

    public void Visit(IntlitNode node)
    {
        foreach (var child in node.getChildren())
        {
           child.Accept(this); 
        }

        String localRegister = this._registerPool.Pop();

        //generate code
		_moonDataCode += _indent + "% space for constant " + node._value + "\n";
		//_moonDataCode += String.format("%-10s",node._moonVarName) + " res 4\n";

        //exec code
		_moonExecCode += _indent + "% processing: " + node._moonVarName  + " := " + node._value + "\n";
		_moonExecCode += _indent + "addi " + localRegister + ",r0," + node._value + "\n"; 
		_moonExecCode += _indent + "sw " + node._moonVarName + "(r0)," + localRegister + "\n";
		// deallocate the register for the current node
		this._registerPool.Push(localRegister);
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

        string localRegister = this._registerPool.Pop();

        _moonExecCode += _indent + "% processing: "  + node.getChildren()[0]._moonVarName + " := " + node.getChildren()[1]._moonVarName + "\n";
		_moonExecCode += _indent + "lw " + localRegister + "," + node.getChildren()[1]._moonVarName + "(r0)\n";
		_moonExecCode += _indent + "sw " + node.getChildren()[0]._moonVarName + "(r0)," + localRegister + "\n";

        this._registerPool.Push(localRegister);
    }
}