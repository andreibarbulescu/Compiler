public class IntlitNode : Node
{
    
    public IntlitNode(string value, NodeType type):base(value,type)
    {
        
    }
    public override void Accept(IVisitor visitor){
        visitor.Visit(this);
    }
    }