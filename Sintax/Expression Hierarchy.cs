using System.Drawing;
using System.Reflection.Metadata;

namespace Compiler;

 public abstract class Expression
 {
    public object? Value;
    public Scope? Scope;
    public ValueType? Type; 
    public string? printed;
    public virtual void Print(int indentLevel = 0)
    {
        Console.WriteLine(new string(' ', indentLevel * 4) + printed);
    }
    public abstract ValueType? Semantic(Scope scope);
    public abstract object Evaluate();
 }
public class ProgramExpression: Expression
{
    public List<Expression?> EffectsAndCards;
    public ProgramExpression()
    {
        EffectsAndCards= new();
        printed = "Program";
    }
    public override object Evaluate()
    {
        throw new NotImplementedException();
    }
    public override ValueType? Semantic(Scope scope)
    {
        foreach(Expression? exp in  EffectsAndCards)
        {
            if(exp is CardExpression card)
            {
                if(card.Semantic(null)!=ValueType.CardDeclaration)
                {
                    return ValueType.Null;
                }
            }
            else if(exp is EffectDeclarationExpr eff)
            {
                if(eff.Semantic(null)!=ValueType.EffectDeclaration)
                {
                    return ValueType.Null;
                }
            }
            else
                throw new Exception("Unexpected code entrance");
        }
        return ValueType.Program;

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
    public override ValueType? Semantic(Scope scope)
    {
        #region Name
        if(Name== null || Name.Semantic(scope)!= ValueType.String)
        {
            return ValueType.Null;
        }
        #endregion
        #region Action
        if(Action== null || Action.Semantic(scope)!= ValueType.Action)
        {
            return ValueType.Null;
        }
        #endregion
        #region Params
        if(Params== null)
        {
            return ValueType.Null;
        }
        foreach(Expression exp in Params)
        {
            exp.Semantic(scope);
        }
        #endregion
        return ValueType.EffectDeclaration;
    }
}
public class InstructionBlock: Expression
{
    public List<Expression>? Instructions= new();
    public override object Evaluate()
    {
        throw new NotImplementedException();
    }
    public override ValueType? Semantic(Scope scope)
    {
        if(Instructions == null)
        return ValueType.Null;
        foreach(Expression exp in Instructions)
        {
            exp.Semantic(scope);
        }
        return ValueType.InstructionBlock;
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
    public override ValueType? Semantic(Scope scope)
    {
        Scope = new Scope(scope);
        if(Targets != null)
        {
            Targets.Type= ValueType.ListCard;
            Scope.AddVar(Targets,Targets);
        }
        else return ValueType.Null;
        if(Context != null)
        {
            Context.Type= ValueType.Context;
            Scope.AddVar(Context,Context);
        }
        else return ValueType.Null;
        if(Instructions != null)
        {
            if(!(Instructions.Semantic(Scope)== ValueType.InstructionBlock))
                return ValueType.Null;
        }
        return ValueType.Action;
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
    public override ValueType? Semantic(Scope scope)
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
    public override ValueType? Semantic(Scope scope)
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
    public override ValueType? Semantic(Scope scope)
    {
        #region Name
        if(Name== null || Name.Semantic(scope)!= ValueType.String)
        {
            return ValueType.Null;
        }
        #endregion
        
        #region Type
        if(Type== null || Type.Semantic(scope)!= ValueType.String)
        {
            return ValueType.Null;
        }
        #endregion
        
        #region Faction
        if(Faction== null || Faction.Semantic(scope)!= ValueType.String)
        {
            return ValueType.Null;
        }
        #endregion
        
        #region Power
        if(Name== null || Name.Semantic(scope)!= ValueType.Number)
        {
            return ValueType.Null;
        }
        #endregion
        
        #region Range
        if(Range== null)
        {
            return ValueType.Null;
        }
        foreach(Expression exp in Range)
        {
            if(exp.Semantic(scope)!= ValueType.String)
            {
                return ValueType.Null;
            }
        }
        #endregion
        
        #region OnActivation
        if(OnActivation== null || OnActivation.Semantic(scope)!= ValueType.OnActivacion)
        {
            return ValueType.Null;
        }
        #endregion
        return ValueType.Card;
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
    public PredicateExp()
    {
        printed = "Predicate";
    }
    public override object Evaluate()
    {
        throw new NotImplementedException();
    }
    public override ValueType? Semantic(Scope scope)
    {
        Unit.Type = ValueType.Card;
        Scope LocalForPredicate= new(scope);
        LocalForPredicate.AddVar(Unit, Unit);
        if(Condition== null || Condition.Semantic(LocalForPredicate)!= ValueType.Boolean)
            return ValueType.Null;
            return ValueType.Predicate;
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
    public override ValueType? Semantic(Scope scope)
    {
        if(Effects==null)
            return ValueType.Null;
        foreach(EffectAssignment assignment in Effects)
        {
            if(assignment.Semantic(scope)!= ValueType.EffectAssignment)
                return ValueType.Null;
        }
        return ValueType.OnActivacion;
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
    public override ValueType? Semantic(Scope scope)
    {
        throw new NotImplementedException();
        #region Effect
        if(Effect== null)
        {
            return ValueType.Null;
        }
        if(Effect.Count==1)
        {

        }
        #endregion
    }
    public override void Print(int indentLevel = 0)
    {
        printed = "Effect Assignment";
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
    public override ValueType? Semantic(Scope scope)
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
    public bool Fixed= false;

    public BinaryOperator(Expression left, Expression right, TokenType Op)
    {
        Left = left;
        Right = right;
        Operator = Op;
        this.printed = Op.ToString();
    }
    public override bool Equals(object? obj)
    {
        if(obj is BinaryOperator bin && bin.Left.Equals(this.Left) && bin.Right.Equals(this.Right) && bin.Operator== this.Operator&& this.Operator== TokenType.POINT)
        {//Ambos deben ser ids y deben tener el mismo nombre
            return true;
        }
        return false;
    }
    public override ValueType? Semantic(Scope scope)
    {
        switch(Operator)
        {
            // Math
            case TokenType.PLUS:
            
            case TokenType.MINUS:
            case TokenType.MULTIPLY:
            case TokenType.DIVIDE:
            case TokenType.POW:
            case TokenType.LESS_EQ:
            case TokenType.MORE_EQ:
            case TokenType.MORE:
            case TokenType.LESS:
            {
                if(Left.Semantic(scope)== ValueType.Number && Right.Semantic(scope)== ValueType.Number)
                    return ValueType.Number;
                else
                    throw new Exception($"You are trying to operate with a {Operator.GetType} but the operands must be number espressions");
            }
            // Booleans
            case TokenType.EQUAL:
            if(Left.Semantic(scope)== Right.Semantic(scope))
                    return ValueType.Boolean;
                else
                    throw new Exception($"You are trying to operate with a {Operator.GetType} but the operands must be from the same type");
            case TokenType.AND:
            case TokenType.OR:
            {
                if(Left.Semantic(scope)== ValueType.Boolean && Right.Semantic(scope)== ValueType.Boolean)
                    return ValueType.Boolean;
                else
                    throw new Exception($"You are trying to operate with a {Operator.GetType} but the operands must be number espressions");
            }
            // String
            case TokenType.CONCATENATION:
            case TokenType.SPACE_CONCATENATION:
            if(Left.Semantic(scope)== ValueType.String && Right.Semantic(scope)== ValueType.String)
                    return ValueType.Number;
            else
                throw new Exception($"You are trying to operate with a {Operator.GetType} but the operands must be number espressions");
            // Point            
            case TokenType.POINT:
            if(!Fixed) 
            {
                Expression exp = SintaxFacts.TwoPointsFixer(this);
                if(exp is BinaryOperator binary)
                {
                    Left= binary.Left;
                    Right = binary.Right;
                }
                else
                throw new Exception("Unexpected code entrance");
            }
            ValueType? type = Left.Semantic(scope);
            if(type != ValueType.Null && Right is Terminal right && SintaxFacts.PointPosibbles[type].Contains(right.Value.Type))
            {
                return SintaxFacts.TypeOf[right.Value.Type];
            }
            else
                throw new Exception("Semantic from Point");

            //Two Points
            case TokenType.TWOPOINT:
            Right.Semantic(scope);
            if(Scope!=null)
            {
                Left.Semantic(scope);
                Scope.AddVar(Left, Right);
            }
            return Right.Type;
            break;
            default:
            throw new Exception("Invalid Operator");
        }
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
    public override ValueType? Semantic(Scope scope)
    {
        throw new NotImplementedException();
    }
    public override bool Equals(object? obj)
    {
        if(obj is Terminal id && id.Value.Value== this.Value.Value)
        {//Ambos deben ser ids y deben tener el mismo nombre 
            return true;
        }
        return false;
    }
}

public class UnaryOperator : Terminal
{//Functions are included into Unary Operators because at the moment they only have one parameter
    public Expression Operand { get; set; }
    public TokenType Operator { get; set; }

    public UnaryOperator(Expression operand, Token Op):base(Op)
    {
        Operand = operand;
        Operator = Op.Type;
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
    public override bool Equals(object? obj)
    {
        if(obj is UnaryOperator unary && unary.Operator.Equals(this.Operator) && unary.Operand.Equals(this.Operand))
        {
            return true;
        }
        return false;
    }
    public override ValueType? Semantic(Scope scope)
    {
        throw new NotImplementedException();
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
    public override ValueType? Semantic(Scope scope)
    {
        this.Scope = scope;
        Type = ValueType.Number;
        return Type;
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
    public override ValueType? Semantic(Scope scope)
    {
        this.Scope = scope;
        Type = ValueType.Boolean;
        return Type;
    }
}

 

public class IdentifierExpression : Terminal
{
    public IdentifierExpression(Token token):base(token)
    {
        this.printed = "ID";
    }
    public override ValueType? Semantic(Scope scope)
    {
        ValueType? tipo;
        if(scope.Find(this, out tipo))
        {
            Type= tipo;
            return tipo;
        }
        else
        {
            throw new Exception($"Use of an unassigned variable {this.Value.Value}");
        }
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
    public override ValueType? Semantic(Scope scope)
    {
        this.Scope = scope;
        Type = ValueType.String;
        return Type;
    }
}
#endregion


