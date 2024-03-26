public class FuncDefNode : Node{
    public FuncDefNode(string name, NodeType type) : base(name,type)
    {
        
    }

    public override void Accept(IVisitor visitor){
        visitor.Visit(this);
    }
}