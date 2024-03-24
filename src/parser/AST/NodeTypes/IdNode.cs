public class IdNode : Node{
    public IdNode(string value, NodeType type):base(value,type){

    }

    public override void Accept(IVisitor p_visitor)
    {
        p_visitor.Visit(this);
    }
}