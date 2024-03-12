    // // Method to make a sibling
    // public Node MakeSiblings(Node y)
    // {

    //     Node xSibs = this.FindRightmostSibling();
    //     xSibs._rightSibling = y;
    //     if (y != null) {
    //         y._leftMostSibling = this._leftMostSibling ?? this; // Ensure correct leftMostSibling linkage
    //         y._parent = this._parent;
    //     }

    //     // Setting siblings for y ensuring no circular references
    //     Node ySibs = y;
    //     while (ySibs != null && ySibs._rightSibling != this && ySibs._rightSibling != y) // Additional checks to avoid loops
    //     {
    //         ySibs._leftMostSibling = this._leftMostSibling ?? this;
    //         ySibs._parent = this._parent;
    //         ySibs = ySibs._rightSibling;
    //     }

    //     return ySibs;
    // }

    // // Method to adopt children
    // public Node AdoptChildren(Node child)
    // {     
    //     if (this._LeftMostchild == null)
    //     {
    //         this._LeftMostchild = child;
    //         // Assuming MakeSiblings has already set up the sibling relationships
    //     }
    //     else
    //     {
    //         // Find the rightmost child and link the new child as a sibling
    //         Node lastChild = this.FindRightMostChild(); // This method should traverse _LeftMostchild and its siblings to find the last one
    //         lastChild._rightSibling = child;
    //     }

    //     // Set the parent reference for child and all its siblings
    //     Node current = child;
    //     while (current != null)
    //     {
    //         current._parent = this;
    //         current = current._rightSibling;
    //     }

    //     return current;

    // }
    // public void PrintTree(Node node, int depth = 0)
    // {
    //     // Print the current node's value with indentation based on its depth
    //     Console.WriteLine(new String(' ', depth * 2) + node._value);

    //     // Recursively print each child, increasing the depth
    //     foreach (var child in node._LeftMostchild)
    //     {
    //         PrintTree(child, depth + 1);
    //     }
    // }
    // private void GenerateDot(Node node, StringBuilder sb, HashSet<Node> visited) {
    //     if (visited.Contains(node)) return;
    //     visited.Add(node);

    //     foreach (var child in node._LeftMostchild) {
    //         sb.AppendLine($"\"{node._value}\" -> \"{child._value}\";");
    //         GenerateDot(child, sb, visited);
    //     }
    // }