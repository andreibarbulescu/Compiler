using System.Reflection.Metadata.Ecma335;
using System.Runtime.Serialization;

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

    }

    private void FuncDef(){

    }
    /*


    private Boolean OpStructDecl2(){

    }

    private Boolean ReptStructDecl4(){

    }

    private Boolean ReptOpStructDecl2(){

    }


*/
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

[Serializable]
internal class ParseException : Exception
{
    private object value;

    public ParseException()
    {
    }

    public ParseException(object value)
    {
        this.value = value;
    }

    public ParseException(string? message) : base(message)
    {
    }

    public ParseException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected ParseException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}