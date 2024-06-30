 namespace Compiler;

 public abstract class Expression
 {
    public string? printed;
    public virtual void Print(int indentLevel = 0)
    {
        Console.WriteLine(new string(' ', indentLevel * 4) + printed);
    }
    public abstract object Evaluate();
 }
public class ProgramExpression: Expression
{
    public List<EffectDeclarationExpr> Effects;
    public List<CardExpression> Cards;
    public ProgramExpression()
    {
        Effects= new();
        Cards= new();
        printed = "Program";
    }
    public override object Evaluate()
    {
        throw new NotImplementedException();
    }
}
#region Effect Expressions and associated
public class EffectDeclarationExpr: Expression
{
    public Expression? Name;
    public List<Expression>? Params;
    public Expression? Action;
    public override object Evaluate()
    {
        throw new NotImplementedException();
    }
    public override void Print(int indentLevel = 0)
    {
        printed = "Effect";
        Console.WriteLine(new string(' ', indentLevel * 4) + printed);
    }
}
public class InstructionBlock: Expression
{
    public List<Expression>? Instructions= new();
    public override object Evaluate()
    {
        throw new NotImplementedException();
    }
    public override void Print(int indentLevel = 0)
    {
        printed = "Instruction Block";
        Console.WriteLine(new string(' ', indentLevel * 4) + printed);
    }
}
public class ActionExpression: Expression
{
    public IdentifierExpression? Targets;
    public IdentifierExpression? Context;
    public InstructionBlock? Instructions;
    public override object Evaluate()
    {
        throw new NotImplementedException();
    }
    public override void Print(int indentLevel = 0)
    {
        printed = "Action";
        Console.WriteLine(new string(' ', indentLevel * 4) + printed);
    }
}
public class ForExpression: Expression
{
    public InstructionBlock? Instructions= new();
    public IdentifierExpression? Variable;
    public IdentifierExpression? Collection;

    public override object Evaluate()
    {
        throw new NotImplementedException();
    }
    public override void Print(int indentLevel = 0)
    {
        printed = "For";
        Console.WriteLine(new string(' ', indentLevel * 4) + printed);
    }
}
public class WhileExpression: Expression
{
    public InstructionBlock? Instructions= new();
    public Expression? Condition;

    public override object Evaluate()
    {
        throw new NotImplementedException();
    }
    public override void Print(int indentLevel = 0)
    {
        printed = "While";
        Console.WriteLine(new string(' ', indentLevel * 4) + printed);
    }
}

#endregion











#region Cards Expressions and associated
public class CardExpression: Expression
{
    public Expression? Name;
    public Expression? Type;
    public Expression? Effect;
    public Expression? Faction;
    public Expression? Power;
    public List<Expression>? Range;
    public OnActivationExpression? OnActivation;

    public override object Evaluate()
    {
        throw new NotImplementedException();
    }
    public override void Print(int indentLevel = 0)
    {
        printed = "Card";
        Console.WriteLine(new string(' ', indentLevel * 4) + printed);
    }
}
public class PredicateExp: Expression
{
    public IdentifierExpression? Unit;
    public Expression? Condition;
    public override object Evaluate()
    {
        throw new NotImplementedException();
    }
}
public class OnActivationExpression: Expression
{
    public List<EffectAssignment>? Effects= new();
    public override object Evaluate()
    {
        throw new NotImplementedException();
    }
    public override void Print(int indentLevel = 0)
    {
        printed = "OnActivacion";
        Console.WriteLine(new string(' ', indentLevel * 4) + printed);
    }
}
public class EffectAssignment: Expression
{
    public List<Expression>? Effect = new();
    public SelectorExpression? Selector;
    public EffectAssignment? PostAction;
    public override object Evaluate()
    {
        throw new NotImplementedException();
    }
    public override void Print(int indentLevel = 0)
    {
        printed = "OnActivacion";
        Console.WriteLine(new string(' ', indentLevel * 4) + printed);
    }
}
public class SelectorExpression: Expression
{
    public Expression? Source;
    public Expression? Single;
    public Expression? Predicate;
    public override object Evaluate()
    {
        throw new NotImplementedException();
    }
    public override void Print(int indentLevel = 0)
    {
        printed = "Selector";
        Console.WriteLine(new string(' ', indentLevel * 4) + printed);
    }
}


#endregion











#region FirstExpressions
public class BinaryOperator : Expression
{
    public Expression Left { get; set; }
    public Expression Right { get; set; }
    public TokenType Operator { get; set; }

    public BinaryOperator(Expression left, Expression right, TokenType Op)
    {
        Left = left;
        Right = right;
        Operator = Op;
        this.printed = Op.ToString();
    }
    public override object Evaluate()
    {
        switch(Operator)
        {
            // Math
            case TokenType.PLUS:
            return (double)Left.Evaluate() + (double)Right.Evaluate();
            case TokenType.MINUS:
            return (double)Left.Evaluate() - (double)Right.Evaluate();
            case TokenType.MULTIPLY:
            return (double)Left.Evaluate() * (double)Right.Evaluate();
            case TokenType.DIVIDE:
            return (double)Left.Evaluate() / (double)Right.Evaluate();
            case TokenType.POW:
            return Math.Pow((double)Left.Evaluate(), (double)Right.Evaluate());
            // Booleans
            case TokenType.EQUAL:
            return SintaxFacts.EqualTerm(Left.Evaluate(), Right.Evaluate());
            case TokenType.LESS_EQ:
            return (double)Left.Evaluate() <= (double)Right.Evaluate();
            case TokenType.MORE_EQ:
            return (double)Left.Evaluate() >= (double)Right.Evaluate();
            case TokenType.MORE:
            return (double)Left.Evaluate() > (double)Right.Evaluate();
            case TokenType.LESS:
            return (double)Left.Evaluate() < (double)Right.Evaluate();
            case TokenType.AND:
            return (bool)Left.Evaluate() && (bool)Right.Evaluate();
            case TokenType.OR:
            return (bool)Left.Evaluate() || (bool)Right.Evaluate();
            // String
            case TokenType.CONCATENATION:
            return (string)Left.Evaluate() + (string)Right.Evaluate();
            case TokenType.SPACE_CONCATENATION:
            return (string)Left.Evaluate() +" "+ (string)Right.Evaluate();
            //...
            default:
            throw new Exception("Invalid Operator");
        }
    }
}
public class Terminal: Expression
{
    public string? ValueForPrint;
    public Token Value { get; }
    public Terminal(Token token)
    {
        this.ValueForPrint = token.Value;
        Value= token;
    }
    public override object Evaluate()
    {
        throw new NotImplementedException();
    }
}

public class UnaryOperator : Expression
{//Functions are included into Unary Operators because at the moment they only have one parameter
    public Expression Operand { get; set; }
    public TokenType Operator { get; set; }

    public UnaryOperator(Expression operand, TokenType Op)
    {
        Operand = operand;
        Operator = Op;
    }
    public override object Evaluate()
    {
        switch(Operator)
        {
            case TokenType.NOT:
                return !(bool)Operand.Evaluate();
            default:
            throw new Exception("Unknown unary operator");
        }
    }
}
public class Number: Terminal
{
    public Number(Token token): base(token)
    {
        this.printed= "Number";
    }
    public override object Evaluate()
    {
        return Convert.ToDouble(Value.Value);
    }
}
public class BooleanLiteral : Terminal
{
    public BooleanLiteral(Token token): base(token)
    {
        this.printed = "Boolean";
    }
    public override object Evaluate()
    {
        return Convert.ToBoolean(Value.Value);
    }
}

 

public class IdentifierExpression : Terminal
{
    public IdentifierExpression(Token token):base(token)
    {
        this.printed = "ID"; // O alguna otra forma de representar el identificador visualmente
    }
}
public class StringExpression : Terminal
{
    public StringExpression(Token token):base(token)
    {
        this.printed = "STRING"; // O alguna otra forma de representar el identificador visualmente
    }
    public override object Evaluate()
    {
        return Value.Value.Substring(1,Value.Value.Length-2);
    }
}
#endregion


