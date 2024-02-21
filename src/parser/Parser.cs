using System.Linq.Expressions;
using System.Text.RegularExpressions;

public class Parser{

    private List<Token> tokenList;
    private string outputFilePath;
    private int counter = 0;
    private Token lookahead = new Token();
    String outputStr = "";
    

    public Parser(string path, List<Token> list)
    {
        this.outputFilePath = path;
        this.tokenList = list;
        this.lookahead = tokenList[counter];
    }

    private void match(TokenType t){
        if(lookahead.GetTokenType() == t){
            outputStr += lookahead.GetName() + " ";
            lookahead = nextToken();
             
        }
        else{
            lookahead = nextToken();
            throw new ParseException("Unexpected token: expected " + t + " but found " + lookahead);
        
        }
    }

    private Token nextToken()
    {
        try
        {
            counter++;
            return tokenList[counter];
        }
        catch (Exception)
        {
            return new Token("end", TokenType.END, 0);
        }
    }

    public void Parse(){
        Start();
        outputStr += "Parsing Complete";

        write();
    }


    private void Start()
    {
        Console.WriteLine("Start -> Prog");
        Prog();
        if(lookahead.GetTokenType() == TokenType.END){
            Console.WriteLine("Parsing ended, reached end of file");
        }else{
            Console.WriteLine("Error Somewhere");
            
        }
    }

    private void Prog()
    {
        outputStr += "Prog\n";
        ReptProg0();
    }

    private void ReptProg0(){

        outputStr += "ReptProg0\n";
        if(lookahead.GetTokenType() == TokenType.STRUCT){
            outputStr += "StructDecl ReptProg0 \n";
            StructDecl();
            ReptProg0();
        }
        else if(lookahead.GetTokenType() == TokenType.IMPL){
            ImplDef();
            ReptProg0();
        }
        else if (lookahead.GetTokenType() == TokenType.FUNC){
            FuncDef();
            ReptProg0();
        }
        
         
    }
    /*
        STRUCTDECL -> struct id OPTSTRUCTDECL2 lcurbr REPTSTRUCTDECL4 rcurbr semi 
    */
    private void StructDecl(){

        match(TokenType.STRUCT);
        match(TokenType.ID);
        if(lookahead.GetTokenType() == TokenType.INHERITS){
            OptStructDecl2();
        }

        match(TokenType.OPENCUBR);

        outputStr += "ReptStructDecl4 ";
        if(lookahead.GetTokenType() == TokenType.PUBLIC ||lookahead.GetTokenType() == TokenType.PRIVATE){
            ReptStructDecl4();
        }
        match(TokenType.CLOSECUBR);
        match(TokenType.SEMI);
        
    }

    /*
        OPTSTRUCTDECL2 -> inherits id REPTOPTSTRUCTDECL22  . 
        OPTSTRUCTDECL2 ->  . 
    */
    private void OptStructDecl2(){
        match(TokenType.INHERITS);
        match(TokenType.ID);
        if (lookahead.GetTokenType() == TokenType.COMMA){
            ReptOptStructDecl22();   
        }
        else{
            //do  nothing
        }

    }

    /*
        REPTOPTSTRUCTDECL22 -> comma id REPTOPTSTRUCTDECL22  . 
        REPTOPTSTRUCTDECL22 ->  .
    */
    private void ReptOptStructDecl22(){
        match(TokenType.COMMA);
        match(TokenType.ID);
        if(lookahead.GetTokenType() == TokenType.COMMA){
            ReptOptStructDecl22();
        }
        else{
            //do nothing
        }
    }

    /*
        REPTSTRUCTDECL4 -> VISIBILITY MEMBERDECL REPTSTRUCTDECL4  . 
        REPTSTRUCTDECL4 ->  . 
    */
    private void ReptStructDecl4(){

        if(lookahead.GetTokenType() == TokenType.PUBLIC){
            match(TokenType.PUBLIC);
            MemberDecl();
            ReptStructDecl4();
        }
        else if(lookahead.GetTokenType() == TokenType.PRIVATE){
            match(TokenType.PRIVATE);
            MemberDecl();
            ReptStructDecl4();
        }
        else{
            //do nothing
        }

    }

    /*
        MEMBERDECL -> FUNCDECL  . 
        MEMBERDECL -> VARDECL  . 
    */
    private void MemberDecl(){
        if(lookahead.GetTokenType() == TokenType.FUNC){
            FuncDecl();
        }
        else if(lookahead.GetTokenType() == TokenType.LET){
            VarDecl();
        }
    }

    /*
        FUNCDECL -> FUNCHEAD semi  .
    */
    private void FuncDecl(){
        FuncHead();
        match(TokenType.SEMI);
    }


    /*
        FUNCHEAD -> func id lpar FPARAMS rpar arrow RETURNTYPE  .
    */
    private void FuncHead(){
        match(TokenType.FUNC);
        match(TokenType.ID);
        match(TokenType.OPENPAR);
        if(lookahead.GetTokenType() == TokenType.ID){
            Fparams();
        }
        match(TokenType.CLOSEPAR);
        match(TokenType.ARROW);
        ReturnType();
    }

    /*
        FPARAMS -> id colon TYPE REPTFPARAMS3 REPTFPARAMS4  . 
        FPARAMS ->  .
    */
    private void Fparams(){
        if(lookahead.GetTokenType() == TokenType.ID){
            match(TokenType.ID);
            match(TokenType.COLON);
            Type();
            ReptFParams3();
            ReptFParams4();
        }
        else{
            //do nothing
        }
    }

    /*
        REPTFPARAMS3 -> ARRAYSIZE REPTFPARAMS3  . 
        REPTFPARAMS3 ->  .
    */
    private void ReptFParams3(){
        if(lookahead.GetTokenType() == TokenType.OPENSQBR){
            ArraySize();
            ReptFParams3();
        }
        else{
            //do nothing
        }

    }

    /*
        ARRAYSIZE -> lsqbr ARRAYSIZE2  .
    */
    public void ArraySize(){
        match(TokenType.OPENSQBR);
        ArraySize2();
    }

    /*
        ARRAYSIZE2 -> intlit rsqbr .
        ARRAYSIZE2 -> rsqbr .
    */
    public void ArraySize2(){
        if(lookahead.GetTokenType() == TokenType.INTNUM){
            match(TokenType.INTNUM);
            match(TokenType.CLOSESQBR);
        }
        else{
            match(TokenType.CLOSESQBR);
        }
    }

    /*
        REPTFPARAMS4 -> FPARAMSTAIL REPTFPARAMS4  . 
        REPTFPARAMS4 ->  . 
    */
    private void ReptFParams4(){
        if(lookahead.GetTokenType() == TokenType.COMMA){
            FParamsTail();
            ReptFParams4();
        }
        else{
            //do nothing
        }           
    }

    /*
        FPARAMSTAIL -> comma id colon TYPE REPTFPARAMSTAIL4  .
    */
    private void FParamsTail(){
        match(TokenType.COMMA);
        match(TokenType.ID);
        match(TokenType.COLON);
        Type();
        ReptFParamsTail4();

    }

    /*
        REPTFPARAMSTAIL4 -> ARRAYSIZE REPTFPARAMSTAIL4  . 
        REPTFPARAMSTAIL4 ->  .
    */
    private void ReptFParamsTail4(){
        if(lookahead.GetTokenType() == TokenType.OPENSQBR){
            ArraySize();
            ReptFParamsTail4();
        }
        else{
            //do nothing
        }
    }

    /*
        RETURNTYPE -> TYPE  . 
        RETURNTYPE -> void  .
    */
    private void ReturnType(){
        if(lookahead.GetTokenType() == TokenType.VOID){
            match(TokenType.VOID);
           }
        else{
            Type();
        }
    }

    /*
        TYPE -> integer  . 
        TYPE -> float  . 
        TYPE -> id  .
    */
    private void Type(){
        switch (lookahead.GetTokenType())
        {
            case TokenType.INTEGER:
                match(TokenType.INTEGER);
                break;
            case TokenType.FLOAT:
                match(TokenType.FLOAT);
                break;
            case TokenType.ID:
                match(TokenType.ID);
                break;
            default:
                Console.WriteLine("Error buddy");
                break;
        }

    }
    /*
        VARDECL -> let id colon TYPE REPTVARDECL4 semi  . 
    */
    private void VarDecl(){
        match(TokenType.LET);
        match(TokenType.ID);
        match(TokenType.COLON);
        Type();
        ReptVarDecl4();
        match(TokenType.SEMI);
        
    }

    /*
        REPTVARDECL4 -> ARRAYSIZE REPTVARDECL4  . 
        REPTVARDECL4 ->  .
    */
    private void ReptVarDecl4(){
        if(lookahead.GetTokenType() == TokenType.OPENSQBR){
            ArraySize();
            ReptVarDecl4();
        }
        else{
            //do nothing
        }
    }
    private void ImplDef(){
        match(TokenType.IMPL);
        match(TokenType.ID);
        match(TokenType.OPENCUBR);
        ReptImplDef3();
        match(TokenType.CLOSECUBR);
    }

    /*
        REPTIMPLDEF3 -> FUNCDEF REPTIMPLDEF3.
        REPTIMPLDEF3 ->.
    */
    private void ReptImplDef3(){
        if(lookahead.GetTokenType() == TokenType.FUNC){
            FuncDef();
            ReptImplDef3();
        }
        else{
            //do nothng
        }

    }
    /*
        FUNCDEF -> FUNCHEAD FUNCBODY.
    */
    private void FuncDef(){
        FuncHead();
        FuncBody();
    }
    
    private void FuncBody(){
        match(TokenType.OPENCUBR);
        ReptFuncBody1();
        match(TokenType.CLOSECUBR);
    }

    //to Fix
    //FIIIIX
    private void ReptFuncBody1(){
        
        if(lookahead.GetTokenType() == TokenType.LET ||
            lookahead.GetTokenType() == TokenType.ID ||
            lookahead.GetTokenType() == TokenType.IF ||
            lookahead.GetTokenType() == TokenType.WHILE||
            lookahead.GetTokenType() == TokenType.READ ||
            lookahead.GetTokenType() == TokenType.WRITE||
            lookahead.GetTokenType() == TokenType.RETURN){

            VarDeclOrStatement();
            ReptFuncBody1();
        }else{
            //do nothing
        }
    }

    private void VarDeclOrStatement(){
        if(lookahead.GetTokenType() == TokenType.LET){
            VarDecl();
        }
        else{
            Statement();
        }
    }

    private void Statement(){
        switch (lookahead.GetTokenType())
        {
            case TokenType.ID:
                match(TokenType.ID);
                AssignStatOrFuncCall();
                match(TokenType.SEMI);
            break;

            case TokenType.IF:
                match(TokenType.IF);
                match(TokenType.OPENPAR);
                RelExpr();
                match(TokenType.CLOSEPAR);
                match(TokenType.THEN);
                StatBlock();
                match(TokenType.ELSE);
                StatBlock();
                match(TokenType.SEMI);
            break;
            case TokenType.WHILE:
                match(TokenType.WHILE);
                match(TokenType.OPENPAR);
                RelExpr();
                match(TokenType.CLOSEPAR);
                StatBlock();
                lookahead.GetTokenType();
                match(TokenType.SEMI);
            break;

            case TokenType.READ:
                match(TokenType.READ);
                match(TokenType.OPENPAR);
                Variable();
                match(TokenType.CLOSEPAR);
                match(TokenType.SEMI);
            break;

            case TokenType.WRITE:
                match(TokenType.WRITE);
                match(TokenType.OPENPAR);
                Expr();
                match(TokenType.CLOSEPAR);
                match(TokenType.SEMI);
            break;

            case TokenType.RETURN:
                match(TokenType.RETURN);
                match(TokenType.OPENPAR);
                Expr();
                match(TokenType.CLOSEPAR);
                match(TokenType.SEMI);
            break;

            default:
                Console.WriteLine("Error in the statement");
            break;
        }

    }

    public void AssignStatOrFuncCall(){
        if(lookahead.GetTokenType() == TokenType.OPENPAR){
            match(TokenType.OPENPAR);
            AParams();
            match(TokenType.CLOSEPAR);
            AssignStatOrFuncCall3();
        }
        else{
            ReptIdNest1();
            AssignStatOrFuncCall2();
        }
    }

    public void AssignStatOrFuncCall2(){
        if(lookahead.GetTokenType() == TokenType.DOT){
            match(TokenType.DOT);
            match(TokenType.ID);
            AssignStatOrFuncCall();
        }
        else {
            match(TokenType.ASSIGN);
            Expr();
        }
    }

    public void AssignStatOrFuncCall3(){
        if(lookahead.GetTokenType() == TokenType.DOT){
            match(TokenType.DOT);
            match(TokenType.ID);
            AssignStatOrFuncCall();
        }
        else{
            //Do nothing
        }
    }

    public void Variable(){
        match(TokenType.ID);
        Variable2();
    }

    public void Variable2(){
        if(lookahead.GetTokenType() == TokenType.OPENPAR){
            match(TokenType.OPENPAR);
            AParams();
            match(TokenType.CLOSEPAR);
            VarIdNest();
        }else{
            ReptIdNest1();
            ReptVariable();
        }
    }

    public void ReptVariable(){
        if(lookahead.GetTokenType() == TokenType.DOT){
            VarIdNest();
            ReptVariable();
        }
        else{
            //do nothing
        }
    }

    private void VarIdNest(){
        
                match(TokenType.DOT);
                match(TokenType.ID);
                VarIdNest2();
    }

    private void VarIdNest2(){
        if(lookahead.GetTokenType() == TokenType.OPENPAR){
            match(TokenType.OPENPAR);
            AParams();
            match(TokenType.CLOSEPAR);
            VarIdNest();
        }else{
            ReptIdNest1();
        }
    }

    public void ReptStatBlock1(){
         switch(lookahead.GetTokenType()) {
            
            case TokenType.ID:
                Statement();
                ReptStatBlock1();
            break;
            case TokenType.IF:
                Statement();
                ReptStatBlock1();
            break;                        
            case TokenType.WHILE:
                Statement();
                ReptStatBlock1();
            break;
            case TokenType.READ:
                Statement();
                ReptStatBlock1();
            break;
            case TokenType.WRITE:
                Statement();
                ReptStatBlock1();
            break;
            case TokenType.RETURN:
                Statement();
                ReptStatBlock1();
            break;
            default:
            break;
       }
    }

    private void StatBlock(){
       switch(lookahead.GetTokenType()) {
            case TokenType.OPENCUBR:
                match(TokenType.OPENCUBR);
                ReptStatBlock1();
                match(TokenType.CLOSECUBR);
            break;

            case TokenType.ID:
                Statement();
            break;
            case TokenType.IF:
                Statement();
            break;                        
            case TokenType.WHILE:
                Statement();
            break;
            case TokenType.READ:
                Statement();
            break;
            case TokenType.WRITE:
                Statement();
            break;
            case TokenType.RETURN:
                Statement();
            break;
            default:
            break;
       }
    }

    private void RelExpr(){
        ArithExpr();
        RelOp();
        ArithExpr();
    }

    private void ArithExpr(){
        Term();
        RightRecArithExpr();
    }

    private void RightRecArithExpr(){
        if(lookahead.GetTokenType() == TokenType.PLUS ||
        lookahead.GetTokenType() == TokenType.MINUS ||
        lookahead.GetTokenType() == TokenType.OR){
            Addop();
            Term();
            RightRecArithExpr();
        }
        else{
            //do nothing
        }
    }

    private void Addop(){
        switch (lookahead.GetTokenType())
        {
            case TokenType.PLUS:
                match(TokenType.PLUS);
                break;
            case TokenType.MINUS:
                match(TokenType.MINUS);
                break;
            case TokenType.OR:
                match(TokenType.OR);
                break;
                        
            default:
                Console.WriteLine("Error wrong addop");
                break;
        }
    }

    private void Term(){
        Factor();
        RightRecTerm();
    }

    private void RightRecTerm(){
        if(lookahead.GetTokenType() == TokenType.MULT || 
            lookahead.GetTokenType() == TokenType.DIV ||
            lookahead.GetTokenType() == TokenType.AND){
                MultOp();
                Factor();
                RightRecTerm();
            }
            else{
                //do nothing
            }
    }
    
    private void MultOp(){
        switch(lookahead.GetTokenType()){
            case TokenType.MULT:
                match(TokenType.MULT);
                break;
            case TokenType.AND:
                match(TokenType.AND);
                break;
            case TokenType.DIV:
                match(TokenType.DIV);
                break;
            default:
                Console.WriteLine("iNVALID mult op");
                break;
        }
    }

    private void Factor(){
        switch(lookahead.GetTokenType()){

            case TokenType.ID:
            match(TokenType.ID);
                Factor2();
                ReptVariableOrFunctionCall();
            break;

            case TokenType.INTNUM:
                match(TokenType.INTNUM);
            break;

            case TokenType.FLOATNUM:
                match(TokenType.FLOATNUM);
            break;

            case TokenType.OPENPAR:
                match(TokenType.OPENPAR);
                ArithExpr();
                match(TokenType.CLOSEPAR);
            break;

            case TokenType.NOT:
                match(TokenType.NOT);
                Factor();
            break;

            case TokenType.PLUS:
                match(TokenType.PLUS);
                Factor();
            break;    

        }
    }

    private void ReptVariableOrFunctionCall(){
        if(lookahead.GetTokenType() == TokenType.DOT){
            IdNest();
            ReptVariableOrFunctionCall();
        }
    }

    private void IdNest(){
        match(TokenType.DOT);
        match(TokenType.ID);
        IdNest2();
    }

    private void IdNest2(){
        if(lookahead.GetTokenType() == TokenType.OPENPAR){
            match(TokenType.OPENPAR);
            AParams();
            match(TokenType.CLOSEPAR);
        }
        else{
            ReptIdNest1();
        }
    }

    private void Factor2(){
        if(lookahead.GetTokenType() == TokenType.OPENPAR){
            match(TokenType.OPENPAR);
            AParams();
            match(TokenType.CLOSEPAR);
        }
        else{
            ReptIdNest1();
        }
    }

    private void ReptIdNest1(){
        if(lookahead.GetTokenType() == TokenType.OPENSQBR){
            Indice();
            ReptIdNest1();
        }
        else{
            //do nothing
        }
    }

    private void Indice(){
        match(TokenType.OPENSQBR);
        ArithExpr();
        match(TokenType.CLOSESQBR);
    }

    private void AParams(){
        if(lookahead.GetTokenType() == TokenType.ID ||
        lookahead.GetTokenType() == TokenType.INTNUM ||
        lookahead.GetTokenType() == TokenType.FLOATNUM ||
        lookahead.GetTokenType() == TokenType.OPENPAR ||
        lookahead.GetTokenType() == TokenType.NOT ||
        lookahead.GetTokenType() == TokenType.PLUS ||
        lookahead.GetTokenType() == TokenType.MINUS){
            Expr();
            ReptAParams1();
        }
        else{
            //do nothing
        }
    }

    private void ReptAParams1(){
        if(lookahead.GetTokenType() == TokenType.COMMA){
            AParamsTail();
            ReptAParams1();
        }
        else{
            //do nothing
        }
    }

    private void AParamsTail(){
        match(TokenType.COMMA);
        Expr();
    }

    private void Expr(){
        ArithExpr();
        Expr2();
    }

    private void Expr2(){
        if(lookahead.GetTokenType() == TokenType.EQ ||
           lookahead.GetTokenType() == TokenType.NOTEQ ||
           lookahead.GetTokenType() == TokenType.LT ||
           lookahead.GetTokenType() == TokenType.GT ||
           lookahead.GetTokenType() == TokenType.LEQ ||
           lookahead.GetTokenType() == TokenType.GEQ){
            RelOp();
            ArithExpr();
           }
        else{
            //do nothing
        }
    }

    private void RelOp(){
        switch (lookahead.GetTokenType())
        {
            case TokenType.EQ:
                match(TokenType.EQ);
                break;
            case TokenType.NOTEQ:
                match(TokenType.NOTEQ);
                break;
            case TokenType.LT:
                match(TokenType.LT);
                break;
            case TokenType.GT:
                match(TokenType.GT);
                break;
            case TokenType.LEQ:
                match(TokenType.LEQ);
                break;
            case TokenType.GEQ:
                match(TokenType.GEQ);
                break;
            default:
                Console.WriteLine("Invalid Relop");
                break;
        }
    }
 
    public void write(){
        try
        {
            using var writer = new StreamWriter(outputFilePath);
            writer.Write(outputStr);
            writer.Close();

        }
        catch (Exception)
        {
            Console.WriteLine($"An exception occurred while writing to the file: {outputFilePath}");
            throw;
        }
    }

}

