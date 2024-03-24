public interface IVisitor{
    void Visit(StructNode node);
    void Visit(ProgNode progNode);
    void Visit(Node node);
    void Visit(IdNode node);
    
}