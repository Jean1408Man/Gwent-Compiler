namespace Compiler;

public static class SintaxFacts
{
    public static Token Numerical(int x)
    { 
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
public enum ValueType
{
    Number,
    String,
    Boolean,
    Card,
    ListCard,
    Context,
}
public class Scope
{
    public Scope parentScope;
    public Dictionary< string , Expression > Variables;

    public bool Find(IdentifierExpression exp, out ValueType? type)
    {
        bool finder= false;
        if(Variables.ContainsKey(exp.Value.Value))
        {
            if(Variables[exp.Value.Value]!= null)
                type = exp.Type;
            else
                type=null;
            finder=true;
        }
        else
            type=null;
        if(!finder && parentScope!=null)
        {
            return parentScope.Find(exp, out type);
        }
        else
        return finder;
    }
    public void AddVar(string name, Expression Value, Scope initial=null)
    {
        if(initial==null)
        {
            initial=this;
        }
        if(Variables.ContainsKey(name))
        {
            Variables[name]= Value;
            Value.Semantic(Value.Scope);
        }
        else
        {
            if(parentScope==null)
            {
                initial.Variables[name]= Value;
                Value.Semantic(Value.Scope);
            }
            else
            {
                parentScope.AddVar(name, Value, initial);
            }
        }
    }
}
