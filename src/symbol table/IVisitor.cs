public interface IVisitor{
    void Visit(StructNode node);
    void Visit(ProgNode progNode);
    void Visit(Node node);
    void Visit(IdNode node);
    void Visit(FuncDefNode node);

    //Function Declaration
    void Visit(FuncDeclNode node);

    void Visit(VarDeclNode node);

    void Visit(ImplNode node);

    void Visit(AddNode node);
    void Visit(MultNode node);

    void Visit(IntlitNode node);

    void Visit(FloatLitNode node);
    
}