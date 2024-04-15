class TypeNode : Node
{
    private string _objName = "";
    public TypeNode(string value,NodeType type)
    :base(value,type){

    }

    public TypeNode(string value, NodeType type, string objName):base(value,type,objName)
        {
    
        }

}