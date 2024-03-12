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
    }

    private void ReptProg0(){
        
        replace("ReptProg0", "StructOrImplOrFunc");
        derivation.Add("ReptProg0");
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
    private Node StructDecl(){
            Node structNode = new();
        match(TokenType.STRUCT);
            Node idNode = new Node("struct id " + lookahead.GetName(),NodeType.ID);
            idNode._value += idNodeCount;
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
        structNode._value = "Struct ";
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
            idNode._value += idNodeCount;
            idNodeCount++;
        match(TokenType.ID);
        // if (lookahead.GetTokenType() == TokenType.COMMA){
        //     ReptOptStructDecl22();   
        // }
        // else{
        //     //do  nothing
        // }
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
            idNode._value += idNodeCount;
            idNodeCount++;
        match(TokenType.ID);
        List<Node> parameters = new() { idNode };
        // if(lookahead.GetTokenType() == TokenType.COMMA){
        //     ReptOptStructDecl22();
        // }
        // else{
        //     //do nothing
        // }
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

        // if(lookahead.GetTokenType() == TokenType.PUBLIC){
        //     match(TokenType.PUBLIC);
        //     MemberDecl();
        //     ReptStructDecl4();
        // }
        // else if(lookahead.GetTokenType() == TokenType.PRIVATE){
        //     match(TokenType.PRIVATE);
        //     MemberDecl();
        //     ReptStructDecl4();
        // }
        // else{
        //     //do nothing
        // }
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
            funcId._value += idNodeCount;
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
            idNode._value += idNodeCount;
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
            item._value = "parameter Tail " + paramTailCount;
            paramTailCount++;
        }

        Node reptFParams = ast.makeFamily(NodeType.FPARAMS,parameters.ToArray());
        if(parameters.Count == 0){
            Node tail = new Node("No param tail " + paramTailCount,NodeType.EMPTY);
            paramTailCount++;
            return tail;
        }
        else{
            reptFParams._value = "FParam Tail " + paramTailCount;
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
                typeNode._value = "Type: " + typeNode._type + returnTypeCount;
                returnTypeCount++;
                return typeNode;
                
            case TokenType.FLOAT:
                match(TokenType.FLOAT);
                
                typeNode._type = NodeType.FLOAT;
                typeNode._value = "Type: " + typeNode._type + returnTypeCount;
                returnTypeCount++;
                return typeNode;

            case TokenType.ID:
                match(TokenType.ID);
                typeNode._type = NodeType.ID;
                typeNode._value = "Type: " + typeNode._type + returnTypeCount;
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
        Console.WriteLine(implDef.ToDotString());    
            return implDef;
        
    }

    /*
        REPTIMPLDEF3 -> FUNCDEF REPTIMPLDEF3.
        REPTIMPLDEF3 ->.
    */
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
        return implListNode;

    }
    /*
        FUNCDEF -> FUNCHEAD FUNCBODY.
    */
    private Node FuncDef(){
        Node funcHead = FuncHead();
        Node funcBody = FuncBody();
        Node FuncDef = new Node();
        FuncDef = ast.makeFamily(NodeType.FUNCDEF, funcHead, funcBody);
        return FuncDef;
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
        
        // if(lookahead.GetTokenType() == TokenType.LET ||
        //     lookahead.GetTokenType() == TokenType.ID ||
        //     lookahead.GetTokenType() == TokenType.IF ||
        //     lookahead.GetTokenType() == TokenType.WHILE||
        //     lookahead.GetTokenType() == TokenType.READ ||
        //     lookahead.GetTokenType() == TokenType.WRITE||
        //     lookahead.GetTokenType() == TokenType.RETURN){

        //     VarDeclOrStatement();
        //     ReptFuncBody1();
        // }else{
        //     //do nothing
        // }
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

    private Node VarDeclOrStatement(){
        if(lookahead.GetTokenType() == TokenType.LET){
            return VarDecl();
        }
        else{
            return Statement();
        }
    }

    private Node Statement(){
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
        return new Node();

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

