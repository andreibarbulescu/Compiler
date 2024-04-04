using System.Formats.Asn1;
using System.Globalization;
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

    public ProgNode Parse(){
        ProgNode ast = Start();
        outputStr += "Parsing Complete";

        write();
        return ast;
    }


    private ProgNode Start()
    {
        derivation.Add("Start");
        printDerivation();

        return Prog();
        if(lookahead.GetTokenType() == TokenType.END){
            Console.WriteLine("Parsing ended, reached end of file");
        }else{
            Console.WriteLine("Error Somewhere");
            
        }
    }

    private void printDerivation(){
        // foreach (var item in derivation)
        //         {
                    
        //             Console.Write(item + " ");
        //         }
        //         Console.WriteLine();
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


    private ProgNode Prog()
    {
        outputStr += "Prog\n";
        replace("Start", "Prog");
        replace("Prog","ReptProg0");
        ReptProg0();
  
        ProgNode big = new();
        
        foreach (var item in members)
        {
            big.newAdoptChildren(item);
        }
        big._value = "Prog";
        //Console.WriteLine(big.ToDotString());
        this.write(big.ToDotString(),"ast2.txt");
        Console.WriteLine("Type of " + big._LeftMostchild.GetType().Name);
        return big;
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
            ReptProg0();
        }
        
         
    }
    /*
        STRUCTDECL -> struct id OPTSTRUCTDECL2 lcurbr REPTSTRUCTDECL4 rcurbr semi 
    */
    private StructNode StructDecl(){
            StructNode structNode = new("struct",NodeType.STRUCT);
        match(TokenType.STRUCT);
            IdNode idNode = new IdNode(lookahead.GetName(),NodeType.ID);
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
        List<Node> contentList = new();
        if(lookahead.GetTokenType() == TokenType.PUBLIC ||lookahead.GetTokenType() == TokenType.PRIVATE){
            contentList = ReptStructDecl4();
            
        }
        match(TokenType.CLOSECUBR);
        match(TokenType.SEMI);
            structNode.newAdoptChildren(idNode);
            structNode.newAdoptChildren(inheritanceNode);
            foreach (var item in contentList)
            {
                structNode.newAdoptChildren(item);
            }
                
        return structNode;
    }

    /*
        OPTSTRUCTDECL2 -> inherits id REPTOPTSTRUCTDECL22  . 
        OPTSTRUCTDECL2 ->  . 
    */
    private Node OptStructDecl2(){
        int count = 0;
        match(TokenType.INHERITS);
            IdNode idNode = new IdNode(lookahead.GetName(),NodeType.ID);
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
            IdNode idNode = new IdNode(lookahead.GetName(),NodeType.ID);
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
    private List<Node> ReptStructDecl4(){

        List<Node> declarationList = new();
        while(lookahead.GetTokenType() == TokenType.PUBLIC || lookahead.GetTokenType() == TokenType.PRIVATE){
            if(lookahead.GetTokenType() == TokenType.PUBLIC){
                match(TokenType.PUBLIC);
                Node publicNode = new Node(NodeType.PUBLIC);
                Node decl = MemberDecl();
                declarationList.Add(decl);
                
            }
            else if(lookahead.GetTokenType() == TokenType.PRIVATE){
                Node privateNode = new Node(NodeType.PRIVATE);
                match(TokenType.PRIVATE);
                Node decl = MemberDecl();
                declarationList.Add(decl);
            }
            else{
                //do nothing
            }
        }
        
        return declarationList;

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
    private FuncDeclNode FuncDecl(){
        FuncDeclNode FuncDeclNode = new();

        FuncDeclNode = FuncHead2();
        
        match(TokenType.SEMI);
        return FuncDeclNode;
    }

        private FuncDeclNode FuncHead2(){
            FuncDeclNode funcHead = new ();
        match(TokenType.FUNC);
            IdNode funcId = new IdNode(lookahead.GetName(),NodeType.ID);
        match(TokenType.ID);
        match(TokenType.OPENPAR);
            Node parameters = new Node();
        if(lookahead.GetTokenType() == TokenType.ID){
            parameters = Fparams();

        }
        match(TokenType.CLOSEPAR);
        match(TokenType.ARROW);
            Node returnType = new Node();
            returnType = ReturnType();
            
        //might be wrong
        if(parameters._value == ""){
            funcHead.newAdoptChildren(funcId);
            funcId.newMakeSiblings(returnType);
        }else{
            funcHead.newAdoptChildren(funcId);
            funcId.newMakeSiblings(parameters);
            parameters.newMakeSiblings(returnType);
        }
        funcHead._value = "Func Declaration";
        funcHead._type = NodeType.FUNCDECL;
        return funcHead;
    }


    /*
        FUNCHEAD -> func id lpar FPARAMS rpar arrow RETURNTYPE  .
    */
    private int returnTypeCount = 1;
    private FuncHeadNode FuncHead(){
            FuncHeadNode funcHead = new ();
        match(TokenType.FUNC);
            IdNode funcId = new IdNode(lookahead.GetName(),NodeType.ID);
        match(TokenType.ID);
        match(TokenType.OPENPAR);
            Node parameters = new Node();
        if(lookahead.GetTokenType() == TokenType.ID){
            parameters = Fparams();

        }
        match(TokenType.CLOSEPAR);
        match(TokenType.ARROW);
            Node returnType = new Node();
            returnType = ReturnType();
            
            
        //might be wrong
        if(parameters._value == ""){
            funcHead.newAdoptChildren(funcId);
            funcId.newMakeSiblings(returnType);
        }else{
            funcHead.newAdoptChildren(funcId);
            funcId.newMakeSiblings(parameters);
            parameters.newMakeSiblings(returnType);
        }
        funcHead._value = "FuncHead";
        return funcHead;
    }

    /*
        FPARAMS -> id colon TYPE REPTFPARAMS3 REPTFPARAMS4  . 
        FPARAMS ->  .
    */
    private FuncParamsNode Fparams(){
        FuncParamsNode fParams = new("Func Params", NodeType.FPARAMS);
        if(lookahead.GetTokenType() == TokenType.ID){
            IdNode idNode = new IdNode(lookahead.GetName(),NodeType.ID);
            match(TokenType.ID);
            match(TokenType.COLON);
                var typeNode = Type();
                //potential arrayParameter
                Node arrayParams = ReptFParams3();
            
            List<Node> restOfParams = ReptFParams4();
            fParams.newAdoptChildren(idNode);
            fParams.newAdoptChildren(typeNode);
            if(arrayParams._value != ""){
                fParams.newAdoptChildren(arrayParams);
            }
            if(restOfParams != null){
                foreach (var item in restOfParams)
                {
                    fParams.newAdoptChildren(item);
                }
            }           
                    
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
            Node reptFParams3 = ast.makeFamily(NodeType.ARRSIZE,parameters.ToArray());
            reptFParams3._value = "Array Size";
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
    int array2Count = 0;
    public Node ArraySize2(){
        if(lookahead.GetTokenType() == TokenType.INTNUM){
            string num = lookahead.GetName();
            match(TokenType.INTNUM);
            
            match(TokenType.CLOSESQBR);
            Node arr = new Node(num +  String.Concat(Enumerable.Repeat(" ", array2Count))
            ,NodeType.INTLIT);
            array2Count++;
            return arr;
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
    private List<Node> ReptFParams4(){

        List<Node> parameters = new List<Node>();
        while(lookahead.GetTokenType() == TokenType.COMMA){
            var paramList = FParamsTail();
            foreach (var item in paramList)
            {
                parameters.Add(item);
            }
        }

        if(parameters.Count > 0 && parameters[parameters.Count-1] == null){
            parameters.RemoveAt(parameters.Count-1);
        }

        return parameters;

    }

    /*
        FPARAMSTAIL -> comma id colon TYPE REPTFPARAMSTAIL4  .
    */
    private List<Node> FParamsTail(){
        
        match(TokenType.COMMA);
            IdNode idNode = new IdNode(lookahead.GetName(),NodeType.ID);
        match(TokenType.ID);
        match(TokenType.COLON);
            Node type = Type();
        
            Node potentialArrSize = ReptFParamsTail4();
        

        List<Node> list = new(){
            idNode,type
        };

        if(potentialArrSize._value != "" && potentialArrSize._type != NodeType.EMPTY){
            list.Add(potentialArrSize);
        }
        
        return list;
    }

    /*
        REPTFPARAMSTAIL4 -> ARRAYSIZE REPTFPARAMSTAIL4  . 
        REPTFPARAMSTAIL4 ->  .
    */
    
    private Node ReptFParamsTail4(){

        List<Node> parameters = new();
        while(lookahead.GetTokenType() == TokenType.OPENSQBR){
            parameters.Add(ArraySize());
        }

        if(parameters.Count == 0){
            Node empty = new("empty",NodeType.EMPTY);
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
            reptFParams._value = "Array Size";
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
            Node voidN = new("void",NodeType.VOID);
            return voidN;
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
    private TypeNode Type(){
        string tok = lookahead.GetName();
        Node typeNode = new Node();
        switch (lookahead.GetTokenType())
        {
            case TokenType.INTEGER:  
                match(TokenType.INTEGER);
                 
                return new("INTEGER",NodeType.TYPE);;
                
            case TokenType.FLOAT:
                match(TokenType.FLOAT);
                
                return new TypeNode("FLOAT",NodeType.TYPE);

            case TokenType.ID:
                match(TokenType.ID);
                typeNode._type = NodeType.ID;

                return new TypeNode("ID",NodeType.TYPE);
           
            default:
                Console.WriteLine("Error buddy");
                return null;
        }

    }
    /*
        VARDECL -> let id colon TYPE REPTVARDECL4 semi  . 
    */
    private VarDeclNode VarDecl(){
        match(TokenType.LET);
            IdNode idNode = new IdNode(lookahead.GetName(),NodeType.ID);
        match(TokenType.ID);
        match(TokenType.COLON);
            Node type = Type();
            Node array = ReptVarDecl4();
        match(TokenType.SEMI);
            VarDeclNode VarDecl= new("variable declaration",NodeType.VARDECL);
            if (array._value == "")
            {
                VarDecl.newAdoptChildren(idNode);
                VarDecl.newAdoptChildren(type);
                
            }
            else{
                VarDecl.newAdoptChildren(idNode);
                VarDecl.newAdoptChildren(type);
                VarDecl.newAdoptChildren(array);
                
            }
        return VarDecl;
    }

    /*
        REPTVARDECL4 -> ARRAYSIZE REPTVARDECL4  . 
        REPTVARDECL4 ->  .
    */
    
    private Node ReptVarDecl4(){
        List<Node> parameters = new();

        while(lookahead.GetTokenType() == TokenType.OPENSQBR){
            parameters.Add(ArraySize());            
        }
        
        if(parameters.Count > 0 && parameters[parameters.Count-1] == null){
            parameters.RemoveAt(parameters.Count-1);
        }
        Node reptVarDecl4 = ast.makeFamily(NodeType.ARRSIZE,parameters.ToArray());
        if(parameters.Count == 0){
            return ast.MakeEmptyNode();
        }
        else{
            reptVarDecl4._value = "Array Size";
            return reptVarDecl4;        
        }

    }

    private ImplNode ImplDef(){
        var implDefNode = new ImplNode(); 
        match(TokenType.IMPL);
            Node idNode = new Node(lookahead.GetName(),NodeType.ID);
            implDefNode.newAdoptChildren(idNode);
        match(TokenType.ID);
        match(TokenType.OPENCUBR);
            Node implContent = ReptImplDef3();
            implDefNode.newAdoptChildren(implContent);
        match(TokenType.CLOSECUBR);
            implDefNode._value = "Implementation Definition";
           implDefNode._type = NodeType.IMPLDEF;
        return implDefNode;
        
    }

    /*
        REPTIMPLDEF3 -> FUNCDEF REPTIMPLDEF3.
        REPTIMPLDEF3 ->.
    */

    private Node ReptImplDef3(){

        List<Node> implList = new();
        while(lookahead.GetTokenType() == TokenType.FUNC){
            implList.Add(FuncDef());
        }

        Node implListNode = new();
        implListNode = ast.makeFamily(NodeType.FUNCLIST,implList.ToArray());
            implListNode._value = "Func Impl List";
        return implListNode;

    }
    /*
        FUNCDEF -> FUNCHEAD FUNCBODY.
    */

    private FuncDefNode FuncDef(){
        FuncHeadNode funcHead = FuncHead();

        Node funcBody = FuncBody();
        funcBody._value = "Func Body";
        FuncDefNode funcDef = new ("Function Definition", NodeType.FUNCDEF);
        funcDef.newAdoptChildren(funcHead);
        funcDef.newAdoptChildren(funcBody);

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
                Node varOrStat = VarDeclOrStatement();
                varOrStatementList.Add(varOrStat);
            }
        Node reptFuncBody1Node = new Node();
        reptFuncBody1Node = ast.makeFamily(NodeType.VARORSTATLIST,varOrStatementList.ToArray());
        return reptFuncBody1Node;    

    }

    private Node VarDeclOrStatement(){
        if(lookahead.GetTokenType() == TokenType.LET){
            VarDeclNode varDecl = VarDecl();
            
            return varDecl;
        }
        else{
            Node stateNode = new();
            stateNode = Statement();
            return stateNode;
        }
    }

    /*
        STATEMENT -> id ASSIGNSTATORFUNCCALL semi . 
        STATEMENT -> if lpar RELEXPR rpar then STATBLOCK else STATBLOCK semi.
        STATEMENT -> while lpar RELEXPR rpar STATBLOCK semi.
        STATEMENT -> read lpar VARIABLE rpar semi.
        STATEMENT -> write lpar EXPR rpar semi.
        STATEMENT -> return lpar EXPR rpar semi.
    */
    public Node Statement(){
        Node statNode = new();
        Node returnNode = new();
        switch (lookahead.GetTokenType())
        {
            case TokenType.ID:

                
                Node statOrFuncCall = AssignStatOrFuncCall();
                statOrFuncCall._type = NodeType.STATEMENT;
                //statParams.Insert(0,idNode);
                match(TokenType.SEMI);
                //statNode = ast.makeFamily(NodeType.STATEMENT,statParams.ToArray());
                //statOrFuncCall._value = "Assign or Func Stat";
                return statOrFuncCall;
            

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
                return new Node("IF",NodeType.IF);
            break;
            case TokenType.WHILE:
                match(TokenType.WHILE);
                match(TokenType.OPENPAR);
                RelExpr();
                match(TokenType.CLOSEPAR);
                StatBlock();
                match(TokenType.SEMI);
                return new Node("while",NodeType.WHILE);
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
                 
                returnNode = ast.makeFamily(NodeType.RETURN,Expr());
                returnNode._value = "Function Returns";
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
/*
ASSIGNSTATORFUNCCALL -> REPTIDNEST1 ASSIGNSTATORFUNCCALL2.
ASSIGNSTATORFUNCCALL -> lpar APARAMS rpar ASSIGNSTATORFUNCCALL3.
ASSIGNSTATORFUNCCALL -> dot id ASSIGNSTATORFUNCCALL.
ASSIGNSTATORFUNCCALL -> ASSIGNOP EXPR

*/

    public Node AssignStatOrFuncCall(){
        List<Node> dotList = new();
        
        Node innerDot = new(NodeType.EMPTY);
        IdNode idPrevNode = new("em",NodeType.ID);
        if(lookahead.GetTokenType() == TokenType.ID){
            idPrevNode = new IdNode(lookahead.GetName(),NodeType.ID);
            match(TokenType.ID);
            //list.Add(idPrevNode);
        }

        if(lookahead.GetTokenType() == TokenType.DOT){
            match(TokenType.DOT);
            Node dot = new Node(".",NodeType.DOT);
            IdNode node = new IdNode(lookahead.GetName(),NodeType.ID);
            match(TokenType.ID);
           
            

            if(idPrevNode._value != "em"){
                dot.newAdoptChildren(idPrevNode);
            }
            dot.newAdoptChildren(node);
            dotList.Add(dot);
            innerDot = dot;
            var prevDot = dot;
            //var prevDot = dot;
            while(lookahead.GetTokenType() == TokenType.DOT){
                match(TokenType.DOT);
                innerDot = new(".",NodeType.DOT);
                Node idNode = new Node(lookahead.GetName(),NodeType.ID);
                innerDot.newAdoptChildren(prevDot);
                innerDot.newAdoptChildren(idNode);
                //dotList.Add(dotNode);
                match(TokenType.ID);
                prevDot = innerDot;
            }

        }

        
        
        if(lookahead.GetTokenType() == TokenType.OPENPAR){
            Node Func = new();
            Func.newAdoptChildren(idPrevNode);
            
            
            match(TokenType.OPENPAR);
            Node aparams = AParams();
           
            match(TokenType.CLOSEPAR);
            Node assOrFun3 = AssignStatOrFuncCall3();
          
            Func.newAdoptChildren(aparams);
            
            if (assOrFun3._type == NodeType.DOT)
            {
                Func.newAdoptChildren(assOrFun3);
            }
            Func._value = "function call";
            return Func;
        }

        if(lookahead.GetTokenType() == TokenType.ASSIGN){
            match(TokenType.ASSIGN);
            Node AssignNode = new Node("=",NodeType.ASSIGN);
            Node expr = Expr();
            // foreach (var item in dotList)
            // {
            //     AssignNode.newAdoptChildren(item);
            // }
            if (innerDot._type == NodeType.EMPTY)
            {
                AssignNode.newAdoptChildren(idPrevNode);
            }
            else {AssignNode.newAdoptChildren(innerDot);}
            
            AssignNode.newAdoptChildren(expr);

            //AssignNode.newAdoptChildren(expr);

            return AssignNode;
            //list.Add(expr);
        }
        else{
            Node reptIDNest = ReptIdNest1();
            Node assOrFunc2 = AssignStatOrFuncCall2();
            Node idk = new();
            idk.newAdoptChildren(reptIDNest);
            idk.newAdoptChildren(assOrFunc2);

            return idk;
        }
        
        //return list;
    }

    public Node AssignStatOrFuncCall2(){
        Node assOrFun2 = new();
        if(lookahead.GetTokenType() == TokenType.DOT){
            match(TokenType.DOT);
            IdNode idNode = new IdNode(lookahead.GetName(),NodeType.ID);
            match(TokenType.ID);

            Node assOrFun = AssignStatOrFuncCall();
            
            
            assOrFun2 = ast.makeFamily(NodeType.FUNCCALL,assOrFun,idNode);
            assOrFun2._value = "Dot";
        }
        else {
            match(TokenType.ASSIGN);
            Node espr = Expr();
            //assOrFun2 = ast.makeFamily(NodeType.ASSIGNEXPR,espr.ToArray());
            assOrFun2.newAdoptChildren(espr);
            assOrFun2._value = "=" ;
        }
        return assOrFun2;
    }

    public Node AssignStatOrFuncCall3(){
        Node A3 = new();
        if(lookahead.GetTokenType() == TokenType.DOT){
            match(TokenType.DOT);
            IdNode idNode = new IdNode(lookahead.GetName(),NodeType.ID);

            match(TokenType.ID);
            Node assignStatOrFuncCall = AssignStatOrFuncCall();
            
            A3 = ast.makeFamily(NodeType.DOT,idNode, assignStatOrFuncCall);
        }
        else{
            //Do nothing
        }
        return A3;
    }

    public Node Variable(){
            IdNode idNode = new IdNode(lookahead.GetName(),NodeType.ID);

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
            IdNode idNode = new IdNode(lookahead.GetName(),NodeType.ID);

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

    //RELEXPR -> ARITHEXPR RELOP ARITHEXPR.
    private Node RelExpr(){
        Node arithexpr = ArithExpr();
        Node relOp = RelOp();
        Node arithexpr2 = ArithExpr();
        relOp.newAdoptChildren(arithexpr);
        relOp.newAdoptChildren(arithexpr2);
        
        return relOp;
    }
    
    //ARITHEXPR -> TERM RIGHTRECARITHEXPR.
    private Node ArithExpr(){
        var ArithExpr = new Node();
           Node term = Term();
        Node rightRecArtithExpr = RightRecArithExpr();
        if (rightRecArtithExpr._type == NodeType.EMPTY)
        {
            return term;
        }
        ArithExpr = new("+",NodeType.ADD);
        ArithExpr.newAdoptChildren(term);
        ArithExpr.newAdoptChildren(rightRecArtithExpr);
        
        return ArithExpr;

    }

//RIGHTRECARITHEXPR ->.
//RIGHTRECARITHEXPR -> ADDOP TERM RIGHTRECARITHEXPR.
    private Node RightRecArithExpr(){

        Node rightRecArithExpr = new();
        List<Node> rightRecArithExprList = new();
        if(lookahead.GetTokenType()!= TokenType.PLUS &&
         lookahead.GetTokenType() != TokenType.MINUS &&
         lookahead.GetTokenType() != TokenType.OR){
            rightRecArithExpr._type = NodeType.EMPTY;
            return rightRecArithExpr;
        }

        Node addParent = Addop();
        Node potentialParent = addParent;
        Node firstTerm = Term();
        if (lookahead.GetTokenType() != TokenType.PLUS)
        {
            return firstTerm;
        }
        Console.WriteLine(firstTerm._value);
        addParent.newAdoptChildren(firstTerm);

        while(lookahead.GetTokenType()== TokenType.PLUS ||
         lookahead.GetTokenType() == TokenType.MINUS ||
         lookahead.GetTokenType() == TokenType.OR){
            Node addop = Addop();
            Node term = Term();
            if (lookahead.GetTokenType() != TokenType.PLUS)
            {
                potentialParent.newAdoptChildren(term);
            }
            else
            {

                potentialParent.newAdoptChildren(addop);
            }
            addop.newAdoptChildren(term);
            potentialParent = addop;

        }

         return addParent;
    }


    private AddNode Addop(){
        AddNode addop = new();
        switch (lookahead.GetTokenType())
        {
            case TokenType.PLUS:
                match(TokenType.PLUS);
                addop = new AddNode("+", NodeType.ADD)
                {
                    _value = "+"
                };
                
                return addop;
            case TokenType.MINUS:
                match(TokenType.MINUS);
                addop = new AddNode("-", NodeType.ADD)
                {
                    _value = "-"
                };

                return addop;
            case TokenType.OR:
                match(TokenType.OR);
                addop = new AddNode("||", NodeType.ADD)
                {
                    _value = "||"
                };

                return addop;
                        
            default:
                Console.WriteLine("Error wrong addop");
                return new AddNode("error Adop",NodeType.EMPTY);
        }
    }

    //TERM -> FACTOR RIGHTRECTERM.
    private Node Term(){
        Node Term = new();
        Node rightRecTerm = new();
        Node factor  = Factor();
        rightRecTerm = RightRecTerm();
        if (rightRecTerm._type == NodeType.EMPTY)
        {
            return factor;
        }
        
        Term = new("*",NodeType.MULT);
        Term.newAdoptChildren(factor);
        Term.newAdoptChildren(rightRecTerm);
        //rightRecTerm.newAdoptChildren(rightRecTerm);
        
        return Term;
    }

    // RIGHTRECTERM ->.
    // RIGHTRECTERM -> MULTOP FACTOR RIGHTRECTERM.
    private Node RightRecTerm(){
        Node rightRecTerm = new(NodeType.EMPTY);
        if (lookahead.GetTokenType() != TokenType.MULT && 
            lookahead.GetTokenType() != TokenType.DIV &&
            lookahead.GetTokenType() != TokenType.AND)
        {
            return rightRecTerm;
        }
        Node multParent = MultOp();
        Node potentialParent = multParent;
        Node firstFactor = Factor();
        if (lookahead.GetTokenType() != TokenType.MULT
            &&lookahead.GetTokenType() != TokenType.DIV &&
                lookahead.GetTokenType() != TokenType.AND)
        {
            return firstFactor;
        }
        Console.WriteLine(firstFactor._value);
        multParent.newAdoptChildren(firstFactor);
        while(lookahead.GetTokenType() == TokenType.MULT || 
            lookahead.GetTokenType() == TokenType.DIV ||
            lookahead.GetTokenType() == TokenType.AND){
                Node multOp = MultOp();
                Node factor = Factor();
            if (lookahead.GetTokenType() != TokenType.MULT)
            {
                potentialParent.newAdoptChildren(factor);
            }else{

                potentialParent.newAdoptChildren(multOp);
            }
            multOp.newAdoptChildren(factor);
            potentialParent = multOp;
            
            }

        
        return multParent;    

    }



    private MultNode MultOp(){
        MultNode multop;
        switch(lookahead.GetTokenType()){
            case TokenType.MULT:
                match(TokenType.MULT);
                multop = new("*",NodeType.MULT);
                return multop;
            case TokenType.AND:
                match(TokenType.AND);
                multop = new("AND",NodeType.AND);
                return multop;
            case TokenType.DIV:
                match(TokenType.DIV);
                multop = new("/",NodeType.DIV);
                return multop;
            default:
                Console.WriteLine("iNVALID mult op");
                return new MultNode("Empty MultOP",NodeType.EMPTY);
        }
    }


    /*
    
    FACTOR -> id FACTOR2 REPTVARIABLEORFUNCTIONCALL.
    FACTOR -> intLit.
    FACTOR -> floatLit.
    FACTOR -> lpar ARITHEXPR rpar.
    FACTOR -> not FACTOR.
    FACTOR -> SIGN FACTOR.
*/
    private Node Factor(){
        Node factor = new();
        switch(lookahead.GetTokenType()){

            case TokenType.ID:
            IdNode idNode = new IdNode(lookahead.GetName(),NodeType.ID);
            factor.newAdoptChildren(idNode);

            match(TokenType.ID);
                Node factor2 = Factor2();
                
                if(factor2._type != NodeType.EMPTY){
                    factor.newAdoptChildren(factor2);
                }
                Node reptVarOrDFFuncCall = ReptVariableOrFunctionCall();
                if (reptVarOrDFFuncCall._type != NodeType.EMPTY)
                {
                    factor.newAdoptChildren(reptVarOrDFFuncCall);
                }
                if(factor2._type == NodeType.EMPTY && reptVarOrDFFuncCall._type == NodeType.EMPTY){
                    return idNode;
                }
                //factor = ast.makeFamily(NodeType.FACTOR, idNode,factor2,reptVarOrDFFuncCall);
                factor._value = "Factor";
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
                Node arithExprList = ArithExpr();
                match(TokenType.CLOSEPAR);
                return arithExprList;


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
        if(idnestList.Count == 0){
            return new Node("",NodeType.EMPTY);
        }
        idnestListNode = ast.makeFamily(NodeType.IDNESTLIST,idnestList.ToArray());
        idnestListNode._value = "ReptVarORFunc";
        return idnestListNode;
    }


    private Node IdNest(){
            Node idNestNode = new();
        match(TokenType.DOT);
            IdNode idNode = new IdNode(lookahead.GetName(),NodeType.ID);

        match(TokenType.ID);
            Node idNest2 = IdNest2();
            idNestNode = ast.makeFamily(NodeType.IDNEST,idNode,idNest2);
            idNestNode._value = "idNest";
        return idNestNode;
    }


    private Node IdNest2(){
        if(lookahead.GetTokenType() == TokenType.OPENPAR){
            match(TokenType.OPENPAR);
            Node aparams = AParams();
            match(TokenType.CLOSEPAR);
            aparams._value = "aparams";
            return aparams;
        }
        else{
            return ReptIdNest1();
        }
    }

/*  FACTOR2 -> lpar APARAMS rpar.
    FACTOR2 -> REPTIDNEST1.
*/
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
    /*
        REPTIDNEST1 -> INDICE REPTIDNEST1.
        REPTIDNEST1 ->.
    */
    private Node ReptIdNest1(){

        Node indiceList = new();
        while(lookahead.GetTokenType() == TokenType.OPENSQBR){
            var tempList = Indice();
            indiceList.newAdoptChildren(tempList);
        }
        indiceList._value = "indice value";
        return indiceList;
    }

    //INDICE -> lsqbr ARITHEXPR rsqbr.
    private Node Indice(){
        Node indice = new();
        match(TokenType.OPENSQBR);
        indice = ArithExpr();
        match(TokenType.CLOSESQBR);
        return indice;
    }

    /*
        APARAMS -> EXPR REPTAPARAMS1.
        APARAMS ->.
    */
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
            if (reptAParams._type == NodeType.EMPTY)
            {
               return expr; 
            }
            else{
                expr.newAdoptChildren(reptAParams);
                return expr;
 
            }
       }
        else{
            return new Node("No parameters",NodeType.EMPTY);
        }
    }

// REPTAPARAMS1 -> APARAMSTAIL REPTAPARAMS1.
// REPTAPARAMS1 ->.
    private Node ReptAParams1(){
    
        Node reptaparams1 = new();
        while(lookahead.GetTokenType() == TokenType.COMMA){
            var innerList = AParamsTail();
            reptaparams1.newAdoptChildren(innerList);          
        }
        
        return reptaparams1;
        
    }

    //APARAMSTAIL -> comma EXPR.
    private Node AParamsTail(){
        match(TokenType.COMMA);
        return Expr();
    }

    /*
        EXPR -> ARITHEXPR EXPR2.
    */
    private Node Expr(){
        
        Node arithExpr = ArithExpr();
        Node expr2 = Expr2();
        

        if (expr2._type == NodeType.EMPTY)
        {
            return arithExpr;
        }
        else{
            arithExpr.newAdoptChildren(expr2);
        }
        
        return arithExpr;
    }
    /*
        EXPR2 -> RELOP ARITHEXPR.
        EXPR2 ->.
    */
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
            if (arithExpr._type == NodeType.EMPTY)
            {
               return relop; 
            }
            else{
                relop.newAdoptChildren(arithExpr);
                return relop;
            }
            
           }
        else{
            expr2._type =NodeType.EMPTY ;
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

        public void write(string output, string endFile){
        try
        {
            using var writer = new StreamWriter(endFile);
            writer.Write(output);
            writer.Close();

        }
        catch (Exception)
        {
            Console.WriteLine($"An exception occurred while writing to the file: {outputFilePath}");
            throw;
        }
    }

}

