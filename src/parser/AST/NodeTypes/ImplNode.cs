public class ImplNode : Node
{
    public ImplNode():base()
    {
    }

    public ImplNode(string value, NodeType type):base(value,type)
    {
        
    }
    public override void Accept(IVisitor visitor){
        visitor.Visit(this);
    }
}