

public class AddNode : Node
{
    public AddNode(string value, NodeType type):base(value,type)
    {
        
    }

    public AddNode():base(){

    }
    public override void Accept(IVisitor visitor){
        visitor.Visit(this);
    }
    
}