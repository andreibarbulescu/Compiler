using System.Linq.Expressions;

public class ProgNode : Node
{
    public ProgNode():base()
    {
        
    }

    public override void Accept(IVisitor visitor){
        visitor.Visit(this);
    }
}