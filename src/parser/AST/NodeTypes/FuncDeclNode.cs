public class FuncDeclNode : Node
{
        public FuncDeclNode(string value, NodeType type) : base(value,type)
    {
        
    }
    public FuncDeclNode():base (){

    }

    public override void Accept(IVisitor visitor){
        visitor.Visit(this);
    }
}