public class FloatLitNode : Node
{
    public FloatLitNode() : base(){
        
    }
    public FloatLitNode(string value, NodeType type):base(value,type)
    {
        
    }
    public override void Accept(IVisitor visitor){
        visitor.Visit(this);
    }
    }