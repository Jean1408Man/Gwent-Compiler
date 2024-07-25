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
        if(Type!= null)
        Console.WriteLine(new string(' ', indentLevel * 4) +"Token: "+ printed+ "---" + "Type: "+ Type);
        else
        Console.WriteLine(new string(' ', indentLevel * 4) +"Token: "+ printed);
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
                    throw new Exception("Semantic Error, Expected Card Declaration Type");
                }
            }
            else if(exp is EffectDeclarationExpr eff)
            {
                if(eff.Semantic(null)!=ValueType.EffectDeclaration)
                {
                    throw new Exception("Semantic Error, Expected Effect Declaration Type");
                }
            }
            else
                throw new Exception("Semantic Error, Unexpected code entrance, Program Statement contains an expression but its not a card or effect");
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
    {//Dependiendo de si queremos que Name sea accesible dentro del Action se pasarará Scope o scope, asumo por ahora que no
        Scope = new Scope(scope);
        
        #region Name
        if(Name== null || Name.Semantic(scope)!= ValueType.String)
        {
            throw new Exception("Semantic Error, Expected String Type");
        }
        #endregion
        
        #region Params
        if(Params!= null)
        {
            Scope.WithoutReps=true;
            foreach(Expression exp in Params)
            {
                exp.Semantic(Scope);
            }
            Scope.WithoutReps=false;
        }
        #endregion
        
        #region Action
        if(Action== null || Action.Semantic(Scope)!= ValueType.Action)
        {
            throw new Exception("Semantic Error, Expected Action Type");
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
        throw new Exception("Semantic Error, Empty Instruction Block");
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
        else throw new Exception("Semantic Error, Targets is Empty");
        if(Context != null)
        {
            Context.Type= ValueType.Context;
            Scope.AddVar(Context,Context);
        }
        else throw new Exception("Semantic Error, Context is Empty");
        if(!(Instructions.Semantic(Scope)== ValueType.InstructionBlock))
            throw new Exception("Semantic Error, Expected Instruction Block Type");
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
    public Expression? Collection;

    public override object Evaluate()
    {
        throw new NotImplementedException();
    }
    public override ValueType? Semantic(Scope scope)
    {
        Scope = new Scope(scope);
        if(Variable != null)
        {
            Variable.Type= ValueType.Card;
            Scope.AddVar(Variable,Variable);
        }
        else throw new Exception("Semantic Error, For Variable is Empty");
        if(Collection != null)
        {
            Collection.Type= ValueType.ListCard;
            Scope.AddVar(Collection,Collection);
        }
        else throw new Exception("Semantic Error, For Collection is Empty");
        if(Instructions != null)
        {
            if(!(Instructions.Semantic(Scope)== ValueType.InstructionBlock))
                throw new Exception("Semantic Error, Expected Instruction Block Type, at a for Expression");
        }
        return ValueType.For;
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
        Scope = new Scope(scope);
        if(Condition != null)
        {
            Condition.Type= Condition.Semantic(scope);
            if(Condition.Type!= ValueType.Boolean)
                throw new Exception("Semantic Error, Expected Boolean Type in While Condition");
        }
        else throw new Exception("Semantic Error, Condition is Empty");
        if(!(Instructions.Semantic(Scope)== ValueType.InstructionBlock))
            throw new Exception("Semantic Error, Expected Instruction Block Type");
        return ValueType.Action;
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
            throw new Exception("Semantic Error, Expected String Type");
        }
        #endregion
        
        #region Type
        if(Type== null || Type.Semantic(scope)!= ValueType.String)
        {
            throw new Exception("Semantic Error, Expected String Type");
        }
        #endregion
        
        #region Faction
        if(Faction== null || Faction.Semantic(scope)!= ValueType.String)
        {
            throw new Exception("Semantic Error, Expected String Type");
        }
        #endregion
        
        #region Power
        if(Name== null || Power.Semantic(scope)!= ValueType.Number)
        {
            throw new Exception("Semantic Error, Expected Number Type");
        }
        #endregion
        
        #region Range
        foreach(Expression exp in Range)
        {
            ValueType? check= exp.Semantic(scope);
            if(check != ValueType.String)
            {
                throw new Exception("Semantic Error, Expected String Type in Ranges");
            }
        }
        #endregion
        
        #region OnActivation
        if(OnActivation== null || OnActivation.Semantic(scope)!= ValueType.OnActivacion)
        {
            throw new Exception("Semantic Error, Expected OnActivation Type");
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
            throw new Exception("Semantic Error, Expected Boolean Type in Predicate Condition");
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
            throw new Exception("Semantic Error, There are not Effects in OnActivation");
        foreach(EffectAssignment assignment in Effects)
        {
            if(assignment.Semantic(scope)!= ValueType.EffectAssignment)
                throw new Exception("Semantic Error, Expected Effect Assignment Type");
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
        Scope = new Scope();
        #region Effect
        if(Effect== null)
        {
            throw new Exception("Semantic Error, Effect is Empty, must contain at least a name");
        }
        Scope.WithoutReps=true;
        foreach(Expression? statements in Effect)
        {
            ValueType? tipo= statements.Semantic(Scope);
        }
        Scope.WithoutReps=false;
        #endregion
        
        #region Selector
        if(Selector== null || Selector.Semantic(scope)!= ValueType.Selector)
        {
            throw new Exception("Semantic Error, Expected Seletor Type");
        }
        #endregion
        #region Post Action
        if(PostAction== null || PostAction.Semantic(scope)!= ValueType.OnActivacion)
        {
            throw new Exception("Semantic Error, Expected OnActivation Type");
        }
        #endregion
        return ValueType.EffectAssignment;
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
        #region Source
        if(Source== null || Source.Semantic(scope)!= ValueType.String)
        {
            throw new Exception("Semantic Error, Expected String Type");
        }
        #endregion
        #region Single
        if(Single== null || Single.Semantic(scope)!= ValueType.Boolean)
        {
            throw new Exception("Semantic Error, Expected Boolean Type");
        }
        #endregion
        #region Predicate
        if(Predicate== null || Predicate.Semantic(scope)!= ValueType.Predicate)
        {
            throw new Exception("Semantic Error, Expected Predicate Type");
        }
        #endregion
        return ValueType.Selector;        
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
            //Acums
            case TokenType.PLUSACCUM:
            case TokenType.MINUSACCUM:
            
            // Math
            case TokenType.PLUS:
            case TokenType.MINUS:
            case TokenType.MULTIPLY:
            case TokenType.DIVIDE:
            case TokenType.POW:
            {
                if(Left.Semantic(scope)== ValueType.Number && Right.Semantic(scope)== ValueType.Number)
                    {
                        Left.Type= ValueType.Number;
                        Right.Type= ValueType.Number;
                        Type=   ValueType.Number;
                        return ValueType.Number;
                    }
                else
                    throw new Exception($"You are trying to operate with a {Operator.GetType} but the operands must be number espressions");
            }
            //Math that return boolean
            case TokenType.LESS_EQ:
            case TokenType.MORE_EQ:
            case TokenType.MORE:
            case TokenType.LESS:
            {
                if(Left.Semantic(scope)== ValueType.Number && Right.Semantic(scope)== ValueType.Number)
                    {
                        Left.Type= ValueType.Number;
                        Right.Type= ValueType.Number;
                        Type=   ValueType.Boolean;
                        return ValueType.Boolean;
                    }
                else
                    throw new Exception($"You are trying to operate with a {Operator.GetType} but the operands must be number espressions");
            }
            // Booleans
            case TokenType.EQUAL:
            Left.Type = Left.Semantic(scope);
            Right.Type = Right.Semantic(scope);
            if(Left.Type== Right.Type)
                    return ValueType.Boolean;
                else
                    throw new Exception($"You are trying to operate with a {Operator.GetType} but the operands must be from the same type");
            case TokenType.AND:
            case TokenType.OR:
            {
                if(Left.Semantic(scope)== ValueType.Boolean && Right.Semantic(scope)== ValueType.Boolean)
                    {
                        Left.Type= ValueType.Boolean;
                        Right.Type= ValueType.Boolean;
                        Type= ValueType.Boolean;
                        return ValueType.Boolean;
                    }
                else
                    throw new Exception($"You are trying to operate with a {Operator.GetType} but the operands must be number espressions");
            }
            
            // String
            case TokenType.CONCATENATION:
            case TokenType.SPACE_CONCATENATION:
            if(Left.Semantic(scope)== ValueType.String && Right.Semantic(scope)== ValueType.String)
            {
                Left.Type= ValueType.String;
                Right.Type= ValueType.String;
                Type= ValueType.String;
                return ValueType.String;
            }
            else
                throw new Exception($"You are trying to operate with a {Operator.GetType} but the operands must be number espressions");
            
            // Point            
            case TokenType.POINT:
            ValueType? type = Left.Semantic(scope);
            Left.Type= type;
            if(type != ValueType.Null && Right is Terminal right && SintaxFacts.PointPosibbles[type].Contains(right.Value.Type))
            {
                type= right.Semantic(scope);
                Right.Type= type;
                return SintaxFacts.TypeOf[right.Value.Type];
            }
            else if(type != ValueType.Null && Right is BinaryOperator binary && binary.Operator== TokenType.INDEXER )
            {
                if(binary.Left is Terminal T && SintaxFacts.PointPosibbles[type].Contains(T.Value.Type))
                {//Chequeo que en el indexado la parte izquierda sea únicamente un terminal, de lo contrario se usaron paréntesis, lo cual no es permitido
                    binary.Left.Type= SintaxFacts.TypeOf[T.Value.Type];
                    type = binary.Semantic(scope);
                    return type; 
                }
                throw new Exception("Semantic, tried to associate from Point");
            }
            else
                throw new Exception("Semantic from Point");
            
            case TokenType.INDEXER:
            if(Left.Type!= ValueType.ListCard)
            {
                if(Left.Semantic(scope)== ValueType.ListCard)
                {
                    Left.Type= ValueType.ListCard;
                }
                else
                    throw new Exception("Semantic, tried to index a non ListCard item");
            }
            if(Right.Semantic(scope)== ValueType.Number)
            {
                Right.Type= ValueType.Number;
                return ValueType.Card;
            }
            else
            {
                throw new Exception("Semantic, tried to index by a non numerical expression");
            }

            //Two Points
            case TokenType.ASSIGN:
            case TokenType.TWOPOINT:
            ValueType? tipo = Right.Semantic(scope);
            Right.Type= tipo;
            ValueType? tempforOut;
            if(scope == null||!scope.Find(Left, out tempforOut)|| !scope.WithoutReps)
            {
            Left.Type= Left.Semantic(scope);
            if(SintaxFacts.AssignableTypes.Contains(Left.Type)){
            if(Left.Type== Right.Type|| Left.Type== ValueType.UnassignedVar)
            {
                Left.Type= Right.Type;
                if(scope!=null)
                    scope.AddVar(Left, Right);
                
            }
            else throw new Exception($"Semantic Error at assignment, between {Left.Type} && {Right.Type}");
            }
            else throw new Exception($"Semantic Error at assignment, because {Left.Type} is readonly");
            }
            else throw new Exception($"At least two declaration statements");
            Type= Right.Type;
            return Right.Type;
            
            
            default:
            throw new Exception("Invalid Operator"+ Operator);
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
        printed= "Unary Operator---"+ Operator;
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
        if(Operand!= null)
        switch(Operator)
        {
            //Card Argument
            case TokenType.SENDBOTTOM:
            case TokenType.REMOVE:
            case TokenType.PUSH:
            case TokenType.ADD:
            if(Operand.Semantic(scope)==ValueType.Card)
                return ValueType.Card;
            else
                throw new Exception("Semantic Error, Expected Card Type");
            //Player Argument
            case TokenType.HANDOFPLAYER:
            case TokenType.DECKOFPLAYER:
            case TokenType.GRAVEYARDOFPLAYER:
            case TokenType.FIELDOFPLAYER:
            if(Operand.Semantic(scope)==ValueType.Player)
                return ValueType.Player;
            else
                throw new Exception("Semantic Error, Expected Player Type");
            //Numbers
            case TokenType.RDECREMENT:
            case TokenType.LDECREMENT:
            case TokenType.RINCREMENT:
            case TokenType.LINCREMENT:
            {
                if(Operand.Semantic(scope)==ValueType.Number)
                    return ValueType.Number;
                else
                    throw new Exception("Semantic Error, Expected Number Type");
            }

            //Boolean
            case TokenType.NOT:
            {
                if(Operand.Semantic(scope)==ValueType.Boolean)
                    return ValueType.Boolean;
                else
                    throw new Exception("Semantic Error, Expected Boolean Type");
            }
            case TokenType.FIND:
            if(Operand.Semantic(scope)!= ValueType.Predicate)
            {
                throw new Exception("Semantic Error, Expected Predicate Type");
            }
            Operand.Type= ValueType.Predicate;
            break;
        }
        if(SintaxFacts.TypeOf.ContainsKey(Operator))
        {
            Type = SintaxFacts.TypeOf[Operator];
            return Type;
        }
        throw new Exception("Invalid Unary Operator");
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
        if(SintaxFacts.TypeOf.ContainsKey(Value.Type))
        {
            Type= SintaxFacts.TypeOf[Value.Type];
            return Type;
        }
        else
        {
            ValueType? tipo;
            if(scope!= null && scope.Find(this, out tipo))
            {
                Type= tipo;
                return tipo;
            }
            else
            {
                Type= ValueType.UnassignedVar;
                return ValueType.UnassignedVar;
            }
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


