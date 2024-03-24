
public class StructNode : Node
{
    public StructNode(string value, NodeType type) : base(value,type)
    {
        
    }
    public StructNode():base (){

    }

    public override void Accept(IVisitor visitor){
        visitor.Visit(this);
    }
}