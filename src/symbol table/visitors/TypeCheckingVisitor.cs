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
        throw new NotImplementedException();
    }

    public void Visit(Node node)
    {
        throw new NotImplementedException();
    }

    public void Visit(IdNode node)
    {
        throw new NotImplementedException();
    }

    public void Visit(FuncDefNode node)
    {
        throw new NotImplementedException();
    }

    public void Visit(FuncDeclNode node)
    {
        throw new NotImplementedException();
    }

    public void Visit(VarDeclNode node)
    {
        throw new NotImplementedException();
    }

    public void Visit(ImplNode node)
    {
        throw new NotImplementedException();
    }
}