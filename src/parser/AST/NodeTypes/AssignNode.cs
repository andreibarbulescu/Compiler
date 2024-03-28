
public class AssignNode : Node {
    public AssignNode()
    {
        
    }

    public AssignNode(string value, NodeType type):base(value,type)
    {
        
    }
    public override void Accept(IVisitor visitor){
        visitor.Visit(this);
    }
}