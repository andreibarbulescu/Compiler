using System.Text;

public class Node{
    
    public Node? _LeftMostchild;
    public Node? _parent;
    public Node? _rightSibling;
    public Node _leftMostSibling;
    public string _value;
    public int _id;
    public static int _currentId;
    public NodeType _type;
    public Node(string value, NodeType type){
        _LeftMostchild = null; 
        _parent = null;
        _type = type;
        _value = value;
        _leftMostSibling = this;
        _id = _currentId;
        _currentId++;
        
    }

    public Node(){
        _LeftMostchild = null;
        _type = NodeType.EMPTY;
        _value = "";
        _parent = null;
        _leftMostSibling = this;
        _id = _currentId;
        _currentId++;
    }

    public Node(NodeType type){
        _LeftMostchild = null;
        _parent = null;
        _type = type;
        _value = "";
        _leftMostSibling = this;
        _id = _currentId;
        _currentId++;

    }

    public static Node MakeNode(string content, NodeType type){
        return new Node(content, type);
    }

    public Node MakeNode(NodeType type){
        return new Node(type);
    }

    public Node MakeEmptyNode(){
        return new Node();
    }

    //TEACHER'S NOTES
    public Node newMakeSiblings(Node y){
        var xSibs = this;
        while(xSibs._rightSibling != null){
            xSibs  = xSibs._rightSibling;
        }
        var ySibs = y._leftMostSibling;
        xSibs._rightSibling = ySibs;
        ySibs._leftMostSibling = xSibs._leftMostSibling;
     
        ySibs._parent = xSibs._parent;
            
        while(ySibs._rightSibling != null){
            ySibs = ySibs._rightSibling;
            ySibs._leftMostSibling = xSibs._leftMostSibling;
            ySibs._parent = xSibs._parent;
        }
        return ySibs;
    }


    public void newAdoptChildren(Node y){
        if(this._LeftMostchild != null){
            this._LeftMostchild.newMakeSiblings(y);
        }
        else{
            var ySibs = y._leftMostSibling;
            this._LeftMostchild = ySibs;
            while(ySibs != null){
                ySibs._parent = this;
                ySibs = ySibs._rightSibling;
            }
        }
    }
 

public Node makeFamily(NodeType type, params Node[] children)
    {
    var parent = MakeNode(type);
    if (children.Length > 0)
    {
        parent.newAdoptChildren(children[0]);
        for (int i = 1; i < children.Length; i++)
        {
            children[0].newMakeSiblings(children[i]);
        }
    }
    return parent;
}



    public string ToDotString() {
        var visited = new HashSet<Node>();
        var sb = new StringBuilder();
        sb.AppendLine("digraph G {");
        GenerateDot(this, sb, visited);
        sb.AppendLine("}");
        return sb.ToString();
    }


    // public void GenerateDot(Node node, StringBuilder sb, HashSet<Node> visited) {
    //     if (node == null || visited.Contains(node)) return;
    //     visited.Add(node);

    //     Node child = node._LeftMostchild;
    //     while (child != null)
    //     {
    //         sb.AppendLine($"{node._id}  [label=\"{node._value}\"];");
    //         sb.AppendLine($"\"{node._id}\" -> \"{child._id}\";");
    //         GenerateDot(child, sb, visited);
    //         child = child._rightSibling;
    //     }
    // }
    public void GenerateDot(Node node, StringBuilder sb, HashSet<Node> visited) {
    if (node == null || visited.Contains(node)) return;
    visited.Add(node);

    // Define the label for the node here, ensuring it's only done once per node
    sb.AppendLine($"\"{node._id}\" [label=\"{node._value}\"];");

    Node child = node._LeftMostchild;
    while (child != null)
    {
        // Create the edge without redefining the label for the parent node
        sb.AppendLine($"\"{node._id}\" -> \"{child._id}\";");
        GenerateDot(child, sb, visited);
        child = child._rightSibling;
    }
}
    
}