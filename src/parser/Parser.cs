using System.Formats.Asn1;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

public class Parser{

    private List<Token> tokenList;
    private string outputFilePath;
    private int counter = 0;
    private Token lookahead = new Token();
    String outputStr = "";
    private List<String> derivation;

    private Node ast;
    

    public Parser(string path, List<Token> list)
    {
        derivation = new List<string>();
        this.outputFilePath = path;
        this.tokenList = list;
        this.lookahead = tokenList[counter];
        ast = new Node();
    }

    private int idNodeCount;

    private void match(TokenType t){
        if(lookahead.GetTokenType() == t){
            outputStr += lookahead.GetName() + " ";
            derivation.Add(lookahead.GetName() + " ");
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
        derivation.Add("Start");
        printDerivation();

        Prog();
        if(lookahead.GetTokenType() == TokenType.END){
            Console.WriteLine("Parsing ended, reached end of file");
        }else{
            Console.WriteLine("Error Somewhere");
            
        }
    }

    private void printDerivation(){
        foreach (var item in derivation)
                {
                    
                    Console.Write(item + " ");
                }
                Console.WriteLine();
    }

    private void replace(string itemToReplace, string newItem){
        int index = derivation.IndexOf(itemToReplace);
        if (index != -1) // Check if the item was found
        {
            derivation[index] = newItem; // Replace the item
        }
        else
        {
            Console.WriteLine("Item not found.");
        }
        printDerivation();
        
    }


    private void Prog()
    {

        outputStr += "Prog\n";
        replace("Start", "Prog");
        replace("Prog","ReptProg0");
        ReptProg0();
        foreach(var item in members){
            Console.WriteLine(item.ToDotString());
        }
    }
    private List<Node> members = new();

    private void ReptProg0(){
        
        replace("ReptProg0", "StructOrImplOrFunc");
        derivation.Add("ReptProg0");
        outputStr += "ReptProg0\n";
        if(lookahead.GetTokenType() == TokenType.STRUCT){
            outputStr += "StructDecl ReptProg0 \n";
            members.Add(StructDecl());
            ReptProg0();
        }
        else if(lookahead.GetTokenType() == TokenType.IMPL){
            members.Add(ImplDef());
            ReptProg0();
        }
        else if (lookahead.GetTokenType() == TokenType.FUNC){
            members.Add(FuncDef());
            //Console.WriteLine(func.ToDotString());
            ReptProg0();
        }
        
         
    }
    /*
        STRUCTDECL -> struct id OPTSTRUCTDECL2 lcurbr REPTSTRUCTDECL4 rcurbr semi 
    */
    int structCount = 0;
    private Node StructDecl(){
            Node structNode = new();
        match(TokenType.STRUCT);
            Node idNode = new Node("struct id " + lookahead.GetName(),NodeType.ID);
            idNode._value = "struct id" + String.Concat(Enumerable.Repeat(" ", idNodeCount));
            idNodeCount++;
        match(TokenType.ID);
            Node inheritanceNode = new();
        if(lookahead.GetTokenType() == TokenType.INHERITS){
             inheritanceNode = OptStructDecl2();
        }
        else{
            inheritanceNode = ast.MakeEmptyNode();
            inheritanceNode._value = "Empty inheritance list";
        }

        match(TokenType.OPENCUBR);
        Node contentNode = new();
        if(lookahead.GetTokenType() == TokenType.PUBLIC ||lookahead.GetTokenType() == TokenType.PRIVATE){
            contentNode = ReptStructDecl4();
            contentNode._value = "Content Node";
        }
        match(TokenType.CLOSECUBR);
        match(TokenType.SEMI);
        structNode = ast.makeFamily(NodeType.STRUCT,idNode,inheritanceNode,contentNode);
        structNode._value = "Struct" + String.Concat(Enumerable.Repeat(" ", structCount));
        structCount++;
        Console.WriteLine(structNode.ToDotString());
        
        return structNode;
    }

    /*
        OPTSTRUCTDECL2 -> inherits id REPTOPTSTRUCTDECL22  . 
        OPTSTRUCTDECL2 ->  . 
    */
    private Node OptStructDecl2(){
        int count = 0;
        match(TokenType.INHERITS);
            Node idNode = new Node(lookahead.GetName(), NodeType.ID);
            idNode._value = lookahead.GetName() + String.Concat(Enumerable.Repeat(" ", idNodeCount));
            idNodeCount++;
        match(TokenType.ID);

        List<Node> parameters = new()
        {
            idNode
        };
        while(lookahead.GetTokenType() == TokenType.COMMA){
            parameters.Add(ReptOptStructDecl22());
        }
        Node inheritsNode = new Node();
        inheritsNode = ast.makeFamily(NodeType.INHERITSLIST,parameters.ToArray());
        inheritsNode._value = "inherits list " + count;
        count++;
        return inheritsNode;
    }

    /*
        REPTOPTSTRUCTDECL22 -> comma id REPTOPTSTRUCTDECL22  . 
        REPTOPTSTRUCTDECL22 ->  .
    */
    private Node ReptOptStructDecl22(){
        
        match(TokenType.COMMA);
            Node idNode = new Node(lookahead.GetName(),NodeType.ID);
            idNode._value +=idNode._value = lookahead.GetName() + String.Concat(Enumerable.Repeat(" ", idNodeCount)); 
            idNodeCount++;
        match(TokenType.ID);
        List<Node> parameters = new() { idNode };

        while(lookahead.GetTokenType() == TokenType.COMMA){
            parameters.Add(ReptOptStructDecl22());
        }
        return ast.makeFamily(NodeType.INHERITSLIST,parameters.ToArray());

    }

    /*
        REPTSTRUCTDECL4 -> VISIBILITY MEMBERDECL REPTSTRUCTDECL4  . 
        REPTSTRUCTDECL4 ->  . 
    */
    private Node ReptStructDecl4(){

        List<Node> declarationList = new();
        int count = 0;
        while(lookahead.GetTokenType() == TokenType.PUBLIC || lookahead.GetTokenType() == TokenType.PRIVATE){
            if(lookahead.GetTokenType() == TokenType.PUBLIC){
                match(TokenType.PUBLIC);
                Node publicNode = new Node(NodeType.PUBLIC);
                Node decl = MemberDecl();
                decl._value += count;
                declarationList.Add(decl);
                count ++;
                
            }
            else if(lookahead.GetTokenType() == TokenType.PRIVATE){
                Node privateNode = new Node(NodeType.PRIVATE);
                match(TokenType.PRIVATE);
                Node decl = MemberDecl();
                decl._value += count;
                declarationList.Add(decl);
                count ++;
            }
            else{
                //do nothing
            }
        }
        Node memberDeclNode = ast.makeFamily(NodeType.MEMBERDECL,declarationList.ToArray());
        return memberDeclNode;

    }

    /*
        MEMBERDECL -> FUNCDECL  . 
        MEMBERDECL -> VARDECL  . 
    */
    private Node MemberDecl(){
        if(lookahead.GetTokenType() == TokenType.FUNC){
            return FuncDecl();
        }
        else if(lookahead.GetTokenType() == TokenType.LET){
            return VarDecl();
        }
        else{
            return new Node("No Member or Var Decl",NodeType.EMPTY);
        }
    }

    /*
        FUNCDECL -> FUNCHEAD semi  .
    */
    private Node FuncDecl(){
        Node FuncDecl = FuncHead();
        
        match(TokenType.SEMI);
        return FuncDecl;
    }


    /*
        FUNCHEAD -> func id lpar FPARAMS rpar arrow RETURNTYPE  .
    */
    private int fParamsCount = 1;
    private int returnTypeCount = 1;
    private Node FuncHead(){
            Node funcHead = new Node();
        match(TokenType.FUNC);
            Node funcId = new Node(lookahead.GetName(),NodeType.ID);
            funcId._value = lookahead.GetName() + String.Concat(Enumerable.Repeat(" ", idNodeCount));
            idNodeCount++;
        match(TokenType.ID);
        match(TokenType.OPENPAR);
            Node parameters = new Node();
        if(lookahead.GetTokenType() == TokenType.ID){
            parameters = Fparams();
            parameters._value += fParamsCount;
            fParamsCount ++;
        }
        match(TokenType.CLOSEPAR);
        match(TokenType.ARROW);
            Node returnType = new Node();
            returnType = ReturnType();
            returnType._value = "Return " + returnType._value ;
            
        //might be wrong
        if(parameters._value == ""){
            funcHead = ast.makeFamily(NodeType.FUNCHEAD,funcId,returnType);
        }else{
            funcHead = ast.makeFamily(NodeType.FUNCHEAD, funcId, parameters, returnType);
        }
        funcHead._value = "FuncHead";
        return funcHead;
    }

    /*
        FPARAMS -> id colon TYPE REPTFPARAMS3 REPTFPARAMS4  . 
        FPARAMS ->  .
    */
    private int fParamType;
    private int noArrayElement;
    private Node Fparams(){
        Node fParams = new Node();
        if(lookahead.GetTokenType() == TokenType.ID){
            Node idNode = new Node(lookahead.GetName(),NodeType.ID);
            idNode._value = lookahead.GetName() + String.Concat(Enumerable.Repeat(" ", idNodeCount));
            idNodeCount++;
            match(TokenType.ID);
            match(TokenType.COLON);
                var typeNode = Type();
                typeNode._value = "Param Type: " + typeNode._type + fParamType;
                fParamType ++;
                //potential arrayParameter
                Node arrayParams = ReptFParams3();


            Node restOfParams = ReptFParams4();
            if(arrayParams._value == ""){
                arrayParams._value = "No Array Element" + noArrayElement;
                noArrayElement ++;
            }           
            fParams = ast.makeFamily(NodeType.FPARAMS, typeNode, idNode, arrayParams,restOfParams);    
            
            // else{
            //     fParams = ast.makeFamily(NodeType.FPARAMS,typeNode,idNode,restOfParams);
            // }
                fParams._value = "fParams";
            return fParams;
        }
        else{
            //do nothing
            return null;
        }

    }

    /*
        REPTFPARAMS3 -> ARRAYSIZE REPTFPARAMS3  . 
        REPTFPARAMS3 ->  .
    */
    private Node ReptFParams3(){
        List<Node> parameters = new();
        // if(lookahead.GetTokenType() == TokenType.OPENSQBR){
        //     ArraySize();
        //     ReptFParams3();
        // }
        // else{
        //     //do nothing
        // }
        while(lookahead.GetTokenType() == TokenType.OPENSQBR){
            parameters.Add(ArraySize());
        }
        // if(parameters.Count > 0 && parameters[parameters.Count-1] == null){
        //     parameters.RemoveAt(parameters.Count-1);
        // }
        if(parameters.Count == 0){
            return ast.MakeEmptyNode();
        }
        else{
            for (int i = 0; i < parameters.Count; i++)
            {
                if (parameters[i] == null)
                {
                    var newNode = new Node();
                    newNode._type = NodeType.ARR;
                    newNode._value = "[]" +i ;
                    parameters[i] = newNode;
                }               
                
            }

            //Console.WriteLine(parameters);
            Node reptFParams3 = ast.makeFamily(NodeType.ARRSIZE,parameters.ToArray());
            reptFParams3._value = "Array Size" + String.Concat(Enumerable.Repeat(" ", ArraySizeCount));
            ArraySizeCount++;
            return reptFParams3;        
        }


    }

    /*
        ARRAYSIZE -> lsqbr ARRAYSIZE2  .
    */
    public Node ArraySize(){
        match(TokenType.OPENSQBR);
        return ArraySize2();
    }

    /*
        ARRAYSIZE2 -> intlit rsqbr .
        ARRAYSIZE2 -> rsqbr .
    */
    public Node ArraySize2(){
        if(lookahead.GetTokenType() == TokenType.INTNUM){
            string num = lookahead.GetName();
            match(TokenType.INTNUM);
            
            match(TokenType.CLOSESQBR);
            return new Node(num,NodeType.INTLIT);
        }
        else{
            match(TokenType.CLOSESQBR);
            return null;
        }
    }

    /*
        REPTFPARAMS4 -> FPARAMSTAIL REPTFPARAMS4  . 
        REPTFPARAMS4 ->  . 
    */
    private int paramTailCount = 0;
    private Node ReptFParams4(){
        // if(lookahead.GetTokenType() == TokenType.COMMA){
        //     FParamsTail();
        //     ReptFParams4();
        // }
        // else{
        //     //do nothing
        // }
        List<Node> parameters = new List<Node>();
        while(lookahead.GetTokenType() == TokenType.COMMA){
            parameters.Add(FParamsTail());
        }

        if(parameters.Count > 0 && parameters[parameters.Count-1] == null){
            parameters.RemoveAt(parameters.Count-1);
        }
        

        foreach (var item in parameters)
        {
            item._value = "parameter Tail " + String.Concat(Enumerable.Repeat(" ", paramTailCount));
            paramTailCount++;
        }

        Node reptFParams = ast.makeFamily(NodeType.FPARAMS,parameters.ToArray());
        if(parameters.Count == 0){
            Node tail = new Node("No param tail " + String.Concat(Enumerable.Repeat(" ", paramTailCount)),NodeType.EMPTY);
            paramTailCount++;
            return tail;
        }
        else{
            reptFParams._value = "FParam Tail " + String.Concat(Enumerable.Repeat(" ", paramTailCount));
            paramTailCount++;
            return reptFParams;        
        }

    }

    /*
        FPARAMSTAIL -> comma id colon TYPE REPTFPARAMSTAIL4  .
    */
    private Node FParamsTail(){
        
        match(TokenType.COMMA);
        Node idNode = new Node(lookahead.GetName(), NodeType.ID);
        idNode._value += idNodeCount;
        idNodeCount++;
        match(TokenType.ID);
        match(TokenType.COLON);
        Node type = Type();
        
        Node potentialArrSize = ReptFParamsTail4();
        

        Node FParamsTail = new Node();
        FParamsTail = ast.makeFamily(NodeType.FPARAMSTAIL,type,idNode,potentialArrSize);
        
        return FParamsTail;
    }

    /*
        REPTFPARAMSTAIL4 -> ARRAYSIZE REPTFPARAMSTAIL4  . 
        REPTFPARAMSTAIL4 ->  .
    */
    private int emptyNodeCount = 0;
    private Node ReptFParamsTail4(){
        // if(lookahead.GetTokenType() == TokenType.OPENSQBR){
        //     ArraySize();
        //     ReptFParamsTail4();
        // }
        // else{
        //     //do nothing
        // }
        List<Node> parameters = new();
        while(lookahead.GetTokenType() == TokenType.OPENSQBR){
            parameters.Add(ArraySize());
        }

        if(parameters.Count == 0){
            Node empty = new("empty",NodeType.EMPTY);
            empty._value = "empty" + String.Concat(Enumerable.Repeat(" ", emptyNodeCount));
            emptyNodeCount++;
            return empty;
        }
        else{
            for (int i = 0; i < parameters.Count; i++)
            {
                if (parameters[i] == null)
                {
                    var newNode = new Node();
                    newNode._type = NodeType.ARR;
                    newNode._value = "[]" +i ;
                    parameters[i] = newNode;
                }               
                
            }

            //Console.WriteLine(parameters);
            Node reptFParams = ast.makeFamily(NodeType.ARRSIZE,parameters.ToArray());
            reptFParams._value = "Array Size" + String.Concat(Enumerable.Repeat(" ", ArraySizeCount));
            ArraySizeCount++;
            return reptFParams;        
        }
                

    }

    /*
        RETURNTYPE -> TYPE  . 
        RETURNTYPE -> void  .
    */
    private Node ReturnType(){
        if(lookahead.GetTokenType() == TokenType.VOID){
            match(TokenType.VOID);
            return new Node("void",NodeType.VOID);
           }
        else{
            return Type();
        }
    }

    /*
        TYPE -> integer  . 
        TYPE -> float  . 
        TYPE -> id  .
    */
    private Node Type(){
        string tok = lookahead.GetName();
        Node typeNode = new Node();
        switch (lookahead.GetTokenType())
        {
            case TokenType.INTEGER:
                
                match(TokenType.INTEGER);
                typeNode._type = NodeType.INT;
                typeNode._value = "Type: " + typeNode._type + String.Concat(Enumerable.Repeat(" ", returnTypeCount));
                returnTypeCount++;
                return typeNode;
                
            case TokenType.FLOAT:
                match(TokenType.FLOAT);
                
                typeNode._type = NodeType.FLOAT;
                typeNode._value = "Type: " + typeNode._type + String.Concat(Enumerable.Repeat(" ", returnTypeCount));
                returnTypeCount++;
                return typeNode;

            case TokenType.ID:
                match(TokenType.ID);
                typeNode._type = NodeType.ID;
                typeNode._value = "Type: " + typeNode._type + String.Concat(Enumerable.Repeat(" ", returnTypeCount));
                returnTypeCount++;
                return typeNode;
           
            default:
                Console.WriteLine("Error buddy");
                return null;
        }

    }
    /*
        VARDECL -> let id colon TYPE REPTVARDECL4 semi  . 
    */
    private Node VarDecl(){
            Node VarDecl = new();
        match(TokenType.LET);

            var idNode = new Node(lookahead.GetName(),NodeType.ID);
            idNode._value += idNodeCount;
            idNodeCount++;
        match(TokenType.ID);
        match(TokenType.COLON);

            Node type = Type();
            Node array = ReptVarDecl4();

        match(TokenType.SEMI);

            if (array._value == "")
            {
                VarDecl = ast.makeFamily(NodeType.VARDECL,type,idNode);
                VarDecl._value = "Variable declaration";
            }
            else{
                VarDecl = ast.makeFamily(NodeType.VARDECL,type,idNode,array);
                VarDecl._value = "variable declaration";
            }
        return VarDecl;
    }

    /*
        REPTVARDECL4 -> ARRAYSIZE REPTVARDECL4  . 
        REPTVARDECL4 ->  .
    */
    private int ArraySizeCount = 0;
    private Node ReptVarDecl4(){
        List<Node> parameters = new();

        while(lookahead.GetTokenType() == TokenType.OPENSQBR){
            parameters.Add(ArraySize());            
        }
        
        // if(lookahead.GetTokenType() == TokenType.OPENSQBR){
        //     parameters.Add(ArraySize());
        //     ReptVarDecl4();
        // }
        // else{
        //     //do nothing
        //     return null;
        // }
        if(parameters.Count > 0 && parameters[parameters.Count-1] == null){
            parameters.RemoveAt(parameters.Count-1);
        }
        Node reptVarDecl4 = ast.makeFamily(NodeType.ARRSIZE,parameters.ToArray());
        if(parameters.Count == 0){
            return ast.MakeEmptyNode();
        }
        else{
            reptVarDecl4._value = "Array Size" + String.Concat(Enumerable.Repeat(" ", ArraySizeCount));
            ArraySizeCount++;
            return reptVarDecl4;        
        }

    }
    private Node ImplDef(){
        match(TokenType.IMPL);
            Node idNode = new Node(lookahead.GetName(),NodeType.ID);
        match(TokenType.ID);
        match(TokenType.OPENCUBR);
        Node implContent = ReptImplDef3();
        match(TokenType.CLOSECUBR);
            Node implDef = new();
            implDef = ast.makeFamily(NodeType.IMPLDEF,idNode,implContent);
            implDef._value = "Implementation Definition";           
            return implDef;
        
    }

    /*
        REPTIMPLDEF3 -> FUNCDEF REPTIMPLDEF3.
        REPTIMPLDEF3 ->.
    */
    int funcImplListCount = 0;
    private Node ReptImplDef3(){
        // if(lookahead.GetTokenType() == TokenType.FUNC){
        //     FuncDef();
        //     ReptImplDef3();
        // }
        // else{
        //     //do nothng
        // }
        List<Node> implList = new();
        while(lookahead.GetTokenType() == TokenType.FUNC){
            implList.Add(FuncDef());
        }

        Node implListNode = new();
        implListNode = ast.makeFamily(NodeType.FUNCLIST,implList.ToArray());
            implListNode._value = "Func Impl List" + String.Concat(Enumerable.Repeat(" ", funcImplListCount));
            funcImplListCount++;
        return implListNode;

    }
    /*
        FUNCDEF -> FUNCHEAD FUNCBODY.
    */
    int funcDefCount = 0;
    int FuncHeadspace = 0;
    int funcBodyCount = 0;
    private Node FuncDef(){
        Node funcHead = FuncHead();
            funcHead._value = "Func Head" + String.Concat(Enumerable.Repeat(" ", FuncHeadspace));
            FuncHeadspace++;
        Node funcBody = FuncBody();
            funcBody._value = "Func Body" + String.Concat(Enumerable.Repeat(" ", funcBodyCount));
            funcBodyCount++;
        Node funcDef = new Node();
        funcDef = ast.makeFamily(NodeType.FUNCDEF, funcHead, funcBody);
        funcDef._value = "Func Def" + String.Concat(Enumerable.Repeat(" ", funcDefCount));
        funcDefCount++;
        return funcDef;
    }
    
    private Node FuncBody(){
        match(TokenType.OPENCUBR);
        Node FuncBody = ReptFuncBody1();
        match(TokenType.CLOSECUBR);
        return FuncBody;
    }

    //to Fix
    //FIIIIX
    private Node ReptFuncBody1(){
        List<Node> varOrStatementList = new();
        while(lookahead.GetTokenType() == TokenType.LET ||
            lookahead.GetTokenType() == TokenType.ID ||
            lookahead.GetTokenType() == TokenType.IF ||
            lookahead.GetTokenType() == TokenType.WHILE||
            lookahead.GetTokenType() == TokenType.READ ||
            lookahead.GetTokenType() == TokenType.WRITE||
            lookahead.GetTokenType() == TokenType.RETURN){
                varOrStatementList.Add(VarDeclOrStatement());
            }
        Node reptFuncBody1Node = new Node();
        reptFuncBody1Node = ast.makeFamily(NodeType.VARORSTATLIST,varOrStatementList.ToArray());
        return reptFuncBody1Node;    


    }
    int statementCount = 0;
    int varDeclCount = 0;
    private Node VarDeclOrStatement(){
        if(lookahead.GetTokenType() == TokenType.LET){
            Node varDecl = VarDecl();
            varDecl._value = "Var Declaration" + String.Concat(Enumerable.Repeat(" ", varDeclCount));
            varDeclCount++;
            return varDecl;
        }
        else{
            Node stateNode = new();
            stateNode = Statement();
            statementCount++;
            return stateNode;
        }
    }
    private int returnNodeCount;
    private int ifNodeCount;
    private int whileNodeCount;
    private int readNodeCount;
    private int writeNodeCount;

    private int statAssOrFunc = 0;
    public Node Statement(){
        Node statNode = new();
        Node returnNode = new();
        switch (lookahead.GetTokenType())
        {
            case TokenType.ID:
                Node idNode = new(lookahead.GetName() + String.Concat(Enumerable.Repeat(" ", idNodeCount)),NodeType.ID);
                idNodeCount++;
                match(TokenType.ID);
                Node statParams = AssignStatOrFuncCall();
                match(TokenType.SEMI);
                statNode = ast.makeFamily(NodeType.STATEMENT,idNode,statParams);
                statNode._value = "Assign or Func Stat"+ String.Concat(Enumerable.Repeat(" ", statAssOrFunc));
                statAssOrFunc++;
                return statNode;
            

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
                 
                returnNode = Expr();
                returnNode._value = "Function Returns" + String.Concat(Enumerable.Repeat(" ", returnNodeCount));
                returnNodeCount++;
                match(TokenType.CLOSEPAR);
                match(TokenType.SEMI);
                return returnNode;
            break;

            default:
                Console.WriteLine("Error in the statement");
            break;
        }
        return new Node();

    }
private int assorFuncCount = 0;
    public Node AssignStatOrFuncCall(){
        Node assOrFun = new();
        if(lookahead.GetTokenType() == TokenType.OPENPAR){
            match(TokenType.OPENPAR);
            Node aparams = AParams();
            match(TokenType.CLOSEPAR);
            Node assOrFun3 = AssignStatOrFuncCall3();
            assOrFun = ast.makeFamily(NodeType.ASSORFUNC,aparams,assOrFun3);
        }
        else{
            Node reptIDNest = ReptIdNest1();
            Node assOrFunc2 = AssignStatOrFuncCall2();
            assOrFun = ast.makeFamily(NodeType.ASSORFUNC,reptIDNest,assOrFunc2);
        }
        assOrFun._value = "Assign or Func"+ String.Concat(Enumerable.Repeat(" ", assorFuncCount));
            assorFuncCount++;
        return assOrFun;
    }
    private int assignCount = 0;
    private int dotCount = 0;
    public Node AssignStatOrFuncCall2(){
        Node assOrFun2 = new();
        if(lookahead.GetTokenType() == TokenType.DOT){
            match(TokenType.DOT);
                Node idNode = new(lookahead.GetName(),NodeType.ID);
            match(TokenType.ID);

            Node assOrFun = AssignStatOrFuncCall();
            assOrFun2 = ast.makeFamily(NodeType.FUNCCALL,idNode,assOrFun);
            assOrFun2._value = "Func Call" + String.Concat(Enumerable.Repeat(" ", dotCount));
            dotCount++;
        }
        else {
            match(TokenType.ASSIGN);
            Node espr = Expr();
            assOrFun2 = ast.makeFamily(NodeType.ASSIGNEXPR,espr);
            assOrFun2._value = "=" + String.Concat(Enumerable.Repeat(" ", assignCount));
            assignCount++;
        }
        return assOrFun2;
    }

    public Node AssignStatOrFuncCall3(){
        Node A3 = new();
        if(lookahead.GetTokenType() == TokenType.DOT){
            match(TokenType.DOT);
                Node idNode = new(lookahead.GetName() + String.Concat(Enumerable.Repeat(" ", idNodeCount)),NodeType.ID);
                idNodeCount++;
            match(TokenType.ID);
            Node assignStatOrFuncCall = AssignStatOrFuncCall();
            A3 = ast.makeFamily(NodeType.DOT, idNode,assignStatOrFuncCall);
        }
        else{
            //Do nothing
        }
        return A3;
    }

    public Node Variable(){
        Node idNode = new(lookahead.GetName() + String.Concat(Enumerable.Repeat(" ", idNodeCount)),NodeType.ID);
        idNodeCount++;
        match(TokenType.ID);
        Node var2 = Variable2();
        Node variable = ast.makeFamily(NodeType.VARIABLE,idNode,var2);
        return variable;
    }

    public Node Variable2(){
        Node variable2 = new();
        if(lookahead.GetTokenType() == TokenType.OPENPAR){
            match(TokenType.OPENPAR);
                var aparams = AParams();
            match(TokenType.CLOSEPAR);
                var varidNest = VarIdNest();
            return ast.makeFamily(NodeType.VARIABLE2,aparams,varidNest);
        }else{
            var reptidnest = ReptIdNest1();
            Node reptVariable = ReptVariable();
            return ast.makeFamily(NodeType.VARIABLE2,reptidnest,reptVariable);
        }
    }

    public Node ReptVariable(){
        // if(lookahead.GetTokenType() == TokenType.DOT){
        //     VarIdNest();
        //     ReptVariable();
        // }
        // else{
        //     //do nothing
        // }
    Node reptVariable = new();
    List<Node> varIDNestList = new();    
    while(lookahead.GetTokenType() == TokenType.DOT){
            varIDNestList.Add(VarIdNest());
            
        }
        reptVariable.makeFamily(NodeType.DOT,varIDNestList.ToArray());
    return reptVariable;


    }

    private Node VarIdNest(){
        
        match(TokenType.DOT);
            Node idNode = new(lookahead.GetName() + String.Concat(Enumerable.Repeat(" ", idNodeCount)),NodeType.ID);
                idNodeCount++;
        match(TokenType.ID);
            Node varidnest2 = VarIdNest2();
            return ast.makeFamily(NodeType.VARIDNEST,idNode,varidnest2);
    }

    private Node VarIdNest2(){
        if(lookahead.GetTokenType() == TokenType.OPENPAR){
            match(TokenType.OPENPAR);
            Node aparams = AParams();
            match(TokenType.CLOSEPAR);
            Node varIDNest = VarIdNest();
            return ast.makeFamily(NodeType.VARIDNEST2,aparams,varIDNest);
        }else{
            return ReptIdNest1();
        }
    }

    public Node ReptStatBlock1(){
        Node stat = new();
        Node statBlock = new();
         switch(lookahead.GetTokenType()) {
            
            case TokenType.ID:
                 stat = Statement();
                 statBlock = ReptStatBlock1();
                return ast.makeFamily(NodeType.REPTSTATBLOCK,stat,statBlock);
            break;
            case TokenType.IF:
                 stat = Statement();
                 statBlock = ReptStatBlock1();
                return ast.makeFamily(NodeType.REPTSTATBLOCK,stat,statBlock);
            break;                        
            case TokenType.WHILE:
                 stat = Statement();
                 statBlock = ReptStatBlock1();
                return ast.makeFamily(NodeType.REPTSTATBLOCK,stat,statBlock);
            break;
            case TokenType.READ:
                 stat = Statement();
                statBlock = ReptStatBlock1();
                return ast.makeFamily(NodeType.REPTSTATBLOCK,stat,statBlock);
            break;
            case TokenType.WRITE:
                 stat = Statement();
                 statBlock = ReptStatBlock1();
                return ast.makeFamily(NodeType.REPTSTATBLOCK,stat,statBlock);
            break;
            case TokenType.RETURN:
                 stat = Statement();
                 statBlock = ReptStatBlock1();
                return ast.makeFamily(NodeType.REPTSTATBLOCK,stat,statBlock);
            break;
            default:
                return ast.MakeEmptyNode();
            break;
       }
    }

    private Node StatBlock(){
       switch(lookahead.GetTokenType()) {
            case TokenType.OPENCUBR:
                match(TokenType.OPENCUBR);
                Node block = ReptStatBlock1();
                match(TokenType.CLOSECUBR);
                return block;
            case TokenType.ID:
                return Statement();
            case TokenType.IF:
                return Statement();
            case TokenType.WHILE:
                return Statement();
            case TokenType.READ:
                return Statement();
            case TokenType.WRITE:
                return Statement();
            case TokenType.RETURN:
                return Statement();
            default:
                return ast.MakeEmptyNode();
       }
    }

    private Node RelExpr(){
        Node arithexpr = ArithExpr();
        Node relOp = RelOp();
        Node arithexpr2 = ArithExpr();
        Node relExpr = new();
        relExpr = ast.makeFamily(NodeType.RELEXPR,arithexpr,relOp,arithexpr2);
        return relExpr;
    }
    private int arithExprCount = 0;
    private Node ArithExpr(){
        Node term = Term();
        Node rightRecArtithExpr = RightRecArithExpr();
        Node ArithExpr = new();
        ArithExpr = ast.makeFamily(NodeType.ARITHEXPR, term,rightRecArtithExpr);
        ArithExpr._value = "Arith Expr"+ String.Concat(Enumerable.Repeat(" ", arithExprCount));
        arithExprCount++;
        return ArithExpr;
    }

    private int rightRecArithEXPRCount = 0;
    private int operationCount = 0;
    private Node RightRecArithExpr(){

        Node rightRecArithExpr = new();
        List<Node> rightRecArithExprList = new();
        while(lookahead.GetTokenType()== TokenType.PLUS ||
         lookahead.GetTokenType() == TokenType.MINUS ||
         lookahead.GetTokenType() == TokenType.OR){
            Node addop = Addop();
            Node term = Term();
            Node operation = ast.makeFamily(NodeType.ADDOPANDTERM,addop,term);
            operation._value = "operation"+ String.Concat(Enumerable.Repeat(" ", operationCount));
            operationCount++;
            rightRecArithExprList.Add(operation); 

         }
        rightRecArithExpr = ast.makeFamily(NodeType.RIGHTRECARITHEXPR,rightRecArithExprList.ToArray());
        rightRecArithExpr._value = "Right RecArith EXpr"+ String.Concat(Enumerable.Repeat(" ", rightRecArithEXPRCount));
        rightRecArithEXPRCount++;
         return rightRecArithExpr;
    }

    private int addopCount = 0;
    private Node Addop(){
        Node addop = new();
        switch (lookahead.GetTokenType())
        {
            case TokenType.PLUS:
                match(TokenType.PLUS);
                addop = new Node("+", NodeType.ADD)
                {
                    _value = "+" + String.Concat(Enumerable.Repeat(" ", addopCount))
                };
                addopCount++;
                return addop;
            case TokenType.MINUS:
                match(TokenType.MINUS);
                addop = new Node("-", NodeType.ADD)
                {
                    _value = "-" + String.Concat(Enumerable.Repeat(" ", addopCount))
                };
                addopCount++;
                return addop;
            case TokenType.OR:
                match(TokenType.OR);
                addop = new Node("||", NodeType.ADD)
                {
                    _value = "||" + String.Concat(Enumerable.Repeat(" ", addopCount))
                };
                addopCount++;
                return addop;
                        
            default:
                Console.WriteLine("Error wrong addop");
                return new Node("error Adop",NodeType.EMPTY);
        }
    }

    private int termCount = 0;
    private Node Term(){
        Node term = new();
        Node factor  = Factor();
        Node rightRecTerm = RightRecTerm();
        term = ast.makeFamily(NodeType.TERM,factor,rightRecTerm);
        term._value = "Term"+ String.Concat(Enumerable.Repeat(" ", termCount));
        termCount ++;
        return term;
    }

    private int rightRecTermCount = 0;
    private int multOPCount = 0;
    private Node RightRecTerm(){
        List<Node> rightRecTermList = new();
        Node rightRecTerm = new();
        while(lookahead.GetTokenType() == TokenType.MULT || 
            lookahead.GetTokenType() == TokenType.DIV ||
            lookahead.GetTokenType() == TokenType.AND){
                Node multOp = MultOp();
                Node factor = Factor();
            Node operation = ast.makeFamily(NodeType.ADDOPANDTERM,multOp,factor);
            operation._value = "Mult Op"+ String.Concat(Enumerable.Repeat(" ", multOPCount));
            multOPCount++;
            rightRecTermList.Add(operation);
            }
        rightRecTerm = ast.makeFamily(NodeType.RIGHTRECTERM,rightRecTermList.ToArray());
        rightRecTerm._value = "Right Rec Term"+ String.Concat(Enumerable.Repeat(" ", rightRecTermCount));
        rightRecTermCount++;
        return rightRecTerm;    

    }


    private int multOPeratorCount = 0;
    private Node MultOp(){
        Node multop = new();
        switch(lookahead.GetTokenType()){
            case TokenType.MULT:
                match(TokenType.MULT);
                multop = new("*",NodeType.MULT);
                multop._value = "*" + String.Concat(Enumerable.Repeat(" ", multOPeratorCount));
                multOPeratorCount++;
                return multop;
            case TokenType.AND:
                match(TokenType.AND);
                multop = new("*",NodeType.MULT);
                multop._value = "*" + String.Concat(Enumerable.Repeat(" ", multOPeratorCount));
                multOPeratorCount++;
                return multop;
            case TokenType.DIV:
                match(TokenType.DIV);
                multop = new("*",NodeType.MULT);
                multop._value = "*" + String.Concat(Enumerable.Repeat(" ", multOPeratorCount));
                multOPeratorCount++;
                return multop;
            default:
                Console.WriteLine("iNVALID mult op");
                return new Node("Empty MultOP",NodeType.EMPTY);
        }
    }

    int factorCount = 0;
    private Node Factor(){
        Node factor = new();
        switch(lookahead.GetTokenType()){

            case TokenType.ID:
                Node idNode = new(lookahead.GetName() + String.Concat(Enumerable.Repeat(" ", idNodeCount)),NodeType.ID);
                idNodeCount++;

            match(TokenType.ID);
                Node factor2 = Factor2();
                Node reptVarOrDFFuncCall = ReptVariableOrFunctionCall();
                factor = ast.makeFamily(NodeType.FACTOR, idNode,factor2,reptVarOrDFFuncCall);
                factor._value = "Factor"+ String.Concat(Enumerable.Repeat(" ", factorCount));
                factorCount++;
                return factor;

            case TokenType.INTNUM:
                Node intNode = new(lookahead.GetName(),NodeType.INTLIT);
                match(TokenType.INTNUM);
                return intNode;

            case TokenType.FLOATNUM:
                Node floatNode = new(lookahead.GetName(),NodeType.FLOATLIT);
                match(TokenType.FLOATNUM);
                return floatNode;

            case TokenType.OPENPAR:
                match(TokenType.OPENPAR);
                Node arithExpr = ArithExpr();
                arithExpr._value = "Arith Expr" + String.Concat(Enumerable.Repeat(" ", arithExprCount));
                arithExprCount++;
                match(TokenType.CLOSEPAR);
                return arithExpr;


            case TokenType.NOT:
                match(TokenType.NOT);
                return Factor();
            break;

            case TokenType.PLUS:
                match(TokenType.PLUS);
                return Factor();
            break;    

        }
        return new Node("Empty Factor",NodeType.EMPTY);
    }

    private int reptVarOrFuncCallCount = 0;
    private Node ReptVariableOrFunctionCall(){
        // if(lookahead.GetTokenType() == TokenType.DOT){
        //     IdNest();
        //     ReptVariableOrFunctionCall();
        // }
        List<Node> idnestList = new();
        
        while(lookahead.GetTokenType() == TokenType.DOT){
            idnestList.Add(IdNest());
        }
        Node idnestListNode = new();
        idnestListNode = ast.makeFamily(NodeType.IDNESTLIST,idnestList.ToArray());
        idnestListNode._value = "ReptVarORFunc"+ String.Concat(Enumerable.Repeat(" ", reptVarOrFuncCallCount));
        reptVarOrFuncCallCount++;
        return idnestListNode;
    }

    private int idNestCount = 0;
    private Node IdNest(){
            Node idNestNode = new();
        match(TokenType.DOT);
            Node idNode = new(lookahead.GetName() + String.Concat(Enumerable.Repeat(" ", idNodeCount)),NodeType.ID);
            idNodeCount++;
        match(TokenType.ID);
            Node idNest2 = IdNest2();
            idNestNode = ast.makeFamily(NodeType.IDNEST,idNode,idNest2);
            idNestNode._value = "idNest"+ String.Concat(Enumerable.Repeat(" ", idNestCount));
            idNestCount ++;
        return idNestNode;
    }

    private int aparamsCount = 0;
    private Node IdNest2(){
        if(lookahead.GetTokenType() == TokenType.OPENPAR){
            match(TokenType.OPENPAR);
            Node aparams = AParams();
            match(TokenType.CLOSEPAR);
            aparams._value = "aparams"+ String.Concat(Enumerable.Repeat(" ", aparamsCount));
            aparamsCount ++;
            return aparams;
        }
        else{
            return ReptIdNest1();
        }
    }

    private Node Factor2(){
        if(lookahead.GetTokenType() == TokenType.OPENPAR){
            match(TokenType.OPENPAR);
            Node aparams = AParams();
            match(TokenType.CLOSEPAR);
            aparams._value = "aparams";
            return aparams;
        }
        else{
            Node reptIdNest= ReptIdNest1();
            //reptIdNest._value = "reptIdNest1";
            return reptIdNest;
        }
    }
    private int indiceNodeCount = 0;
    private Node ReptIdNest1(){

        List<Node> indiceList = new();
        while(lookahead.GetTokenType() == TokenType.OPENSQBR){
            indiceList.Add(Indice());
        }
        Node indiceNode = new();
        indiceNode = ast.makeFamily(NodeType.INDICELIST,indiceList.ToArray());
        indiceNode._value = "indice value" + String.Concat(Enumerable.Repeat(" ", indiceNodeCount));
        indiceNodeCount++;
        return indiceNode;
    }

    private int indiceCount = 0;
    private Node Indice(){
        Node indice = new();
        match(TokenType.OPENSQBR);
        indice = ArithExpr();
        indice._value = "indice value" + String.Concat(Enumerable.Repeat(" ", indiceCount));
        indiceCount ++;
        match(TokenType.CLOSESQBR);
        return indice;
    }

    
    private Node AParams(){

        Node aparams = new();
        if(lookahead.GetTokenType() == TokenType.ID ||
        lookahead.GetTokenType() == TokenType.INTNUM ||
        lookahead.GetTokenType() == TokenType.FLOATNUM ||
        lookahead.GetTokenType() == TokenType.OPENPAR ||
        lookahead.GetTokenType() == TokenType.NOT ||
        lookahead.GetTokenType() == TokenType.PLUS ||
        lookahead.GetTokenType() == TokenType.MINUS){
            Node expr = Expr();
            Node reptAParams = ReptAParams1();
            aparams = ast.makeFamily(NodeType.APARAMS,expr,reptAParams);
            
            return aparams;
        }
        else{
            return new Node("Empty PArams",NodeType.EMPTY);
        }
    }

    private Node ReptAParams1(){
        List<Node> list = new();
        Node reptaparams1 = new();
        while(lookahead.GetTokenType() == TokenType.COMMA){
            list.Add(AParamsTail());           
        }
        reptaparams1 = ast.makeFamily(NodeType.REPTAPARAMS,list.ToArray());
        return reptaparams1;
        
    }

    private Node AParamsTail(){
        match(TokenType.COMMA);
        return Expr();
    }

    private int exprCount = 0;

    private Node Expr(){
        Node arithExpr = ArithExpr();

        Node expr2 = Expr2();
        Node expr = new();

        expr = ast.makeFamily(NodeType.EXPR,arithExpr,expr2);
        expr._value = "Expr" + String.Concat(Enumerable.Repeat(" ", exprCount));
        exprCount++;
        return expr;
    }

    private int expr2Count = 0;
    private Node Expr2(){
        Node expr2 = new();
        if(lookahead.GetTokenType() == TokenType.EQ ||
           lookahead.GetTokenType() == TokenType.NOTEQ ||
           lookahead.GetTokenType() == TokenType.LT ||
           lookahead.GetTokenType() == TokenType.GT ||
           lookahead.GetTokenType() == TokenType.LEQ ||
           lookahead.GetTokenType() == TokenType.GEQ){
            Node relop = RelOp();
            Node arithExpr = ArithExpr();
            expr2 = ast.makeFamily(NodeType.EXPR2,relop,arithExpr);
            expr2._value = "Expr2" + String.Concat(Enumerable.Repeat(" ", expr2Count));
            expr2Count++;
            return expr2;
            
           }
        else{
            expr2._value = "Expr2" + String.Concat(Enumerable.Repeat(" ", expr2Count));
            expr2Count++;
            return expr2;
        }
    }

    private Node RelOp(){
        switch (lookahead.GetTokenType())
        {
            case TokenType.EQ:
                match(TokenType.EQ);
                return new("==",NodeType.EQUALS);
                break;
            case TokenType.NOTEQ:
                match(TokenType.NOTEQ);
                return new("!=",NodeType.NOTEQ);
                break;
            case TokenType.LT:
                match(TokenType.LT);
                return new("<",NodeType.LT);
                break;
            case TokenType.GT:
                match(TokenType.GT);
                return new(">",NodeType.GT);
                break;
            case TokenType.LEQ:
                match(TokenType.LEQ);
                return new("<=",NodeType.LEQ);
                break;
            case TokenType.GEQ:
                match(TokenType.GEQ);
                return new(">=",NodeType.GEQ);
                break;
            default:
                Console.WriteLine("Invalid Relop");
                return new Node("invalid rel op",NodeType.EMPTY);
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

