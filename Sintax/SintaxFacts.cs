namespace Compiler;

public static class SintaxFacts
{
    public static Token Numerical(int x){ 
        return new Token(TokenType.INT,x.ToString(),(-1,-1));
    }
    public static bool EqualTerm(object left, object right)
    {
        if(left is int _left && right is int _right)
        {
            return _left== _right;
        }
        else if(left is bool _leftb && right is bool _rightb)
        {
            return _leftb== _rightb;
        }
        else if(left is string _lefts && right is string _rights)
        {
            return _lefts== _rights;
        }
        return false;
    }
    public static int GetPrecedence(TokenType type)
    {
        switch (type)
        {
            //Booleans
            case TokenType.AND:
            case TokenType.OR:
            return 2;
            //Comparison
            case TokenType.EQUAL:
            case TokenType.LESS_EQ:
            case TokenType.MORE_EQ:
            case TokenType.MORE:
            case TokenType.LESS:
            return 3;
            //1st Operators
            case TokenType.PLUS:
            case TokenType.MINUS:
            case TokenType.CONCATENATION:
            case TokenType.SPACE_CONCATENATION:
            return 4;
            //2nd Operators 
            case TokenType.MULTIPLY:
            case TokenType.DIVIDE:
            case TokenType.NOT:
            return 5;
            //3rd Operators
            case TokenType.POW:
            return 6;
            case TokenType.POINT:
            return 7;            
            default:
            return 0;
        }
    }
}
