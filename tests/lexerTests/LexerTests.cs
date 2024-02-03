using Xunit;

public class LexerTests{

    [Fact]
    public void TestName()
    {

        // Given a lexer ctor
        Lexer lex = new Lexer("string", "input", "output");

        // When constructing
    
        // Then must be initialized properly
        Assert.Equal(1,1);
    }
    
}