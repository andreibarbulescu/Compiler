public class TypeCheckingVisitor : IVisitor
{
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
}