namespace Compiler;

public static class SintaxFacts
{
    public static Token NullNumberToken= new Token(TokenType.INT,"0",(-1,-1));
    public static int GetPrecedence(TokenType type)
    {
        switch (type)
        {
            case TokenType.PLUS:
            case TokenType.MINUS:
            return 1;
            case TokenType.MULTIPLY:
            case TokenType.DIVIDE:
            return 2;
            case TokenType.POW:
            return 3;            
            default:
            return 0;
        }
    }
}
