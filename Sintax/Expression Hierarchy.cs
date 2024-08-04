using System.Drawing;
using System.Reflection.Metadata;
using System.Reflection;
using System.Linq.Expressions;

namespace LogicalSide;

 public abstract class Expression
 {
    #region EvaluateSection
    public object _value;
    public object? Value
    {
        get
        {
            if(_value==null)
            {
                _value = Evaluate(Evaluator, null);
                return _value;
            }
            else
            return _value;
        } 
        set
        {
            _value= value;
        }
    }
    public object? GetValue(EvaluateScope ev)
    {
        Evaluator= ev;
        return Value;
    }
    #endregion
    public SemanticalScope? SemanticScope;
    public EvaluateScope? Evaluator;
    public ValueType? Type; 
    public string? printed;
    public virtual void Print(int indentLevel = 0)
    {
        if(Type!= null)
        Console.WriteLine(new string(' ', indentLevel * 4) +"Token: "+ printed+ "---" + "Type: "+ Type);
        else
        Console.WriteLine(new string(' ', indentLevel * 4) +"Token: "+ printed);
    }
    public abstract ValueType? Semantic(SemanticalScope scope);
    //This is the method who will build the structure of the card
    public abstract object Evaluate(EvaluateScope scope,object Set, object Before= null);
    //This is the method, that uses the structure given by the Evaluate Method, and will execute the functionality of the card in the game
    //public abstract void Execute();
 }
public class ProgramExpression: Expression
{
    public List<Expression?> EffectsAndCards;
    public ProgramExpression()
    {
        EffectsAndCards= new();
        printed = "Program";
    }
    public override object Evaluate(EvaluateScope scope,object Set, object Before= null)
    {
        List<ICard> cards= new();
        object values=null;
        foreach(Expression exp in EffectsAndCards)
        {
            values= exp.Evaluate(scope, null, Before);
            if(exp is CardExpression card)
            {
                cards.Add((ICard)values);
            }
        }
        return cards;
    }
    public override ValueType? Semantic(SemanticalScope scope)
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
    public ActionExpression? Action;

    public void Execute(IContext context, List<ICard> targets, List<IdentifierExpression> Param)
    {
        Evaluator = new EvaluateScope();
        Action.Context.Value= context;
        Action.Targets.Value= targets;
        EvaluateUtils.SetUpParams(Param, Params);
        IdentifierExpression id;
        foreach(Expression exp in Params)
        {
            if(exp is BinaryOperator bin)
            {
                id= (IdentifierExpression)bin.Left;
                Evaluator.AddVar(id, id.Value);
            }
        }
        Action.Evaluate(Evaluator,null); 
    }

    //TODO: Effect Declarations need a method named Execute, wich will take the variables initialized correctly and act
    public override object Evaluate(EvaluateScope scope,object Set, object Before= null)
    {
        string name= (string)Name.Evaluate(scope, Set);
        if(EvaluateUtils.ParamsRequiered.ContainsKey(name))
            throw new Exception("Evaluate Error, you declared at least two effects with the same name");
        EvaluateUtils.ParamsRequiered.Add(name, new List<IdentifierExpression>());
        EvaluateUtils.Effects.Add(name, this);
        if(Params!= null && Params.Count>0)
        foreach(Expression exp in Params)
        {
            if(exp is BinaryOperator bin)
            {
                if(bin.Left is IdentifierExpression id)
                {
                    EvaluateUtils.ParamsRequiered[name].Add(id);
                }
                else
                    throw new Exception("Unexpected code entrance");
            }
            else
            throw new Exception("Unexpected code entrance");
        }
        return true;
    }
    public override void Print(int indentLevel = 0)
    {
        printed = "Effect";
        Console.WriteLine(new string(' ', indentLevel * 4) + printed);
    }
    public override ValueType? Semantic(SemanticalScope scope)
    {//Dependiendo de si queremos que Name sea accesible dentro del Action se pasarará Scope o scope, asumo por ahora que no
        SemanticScope = new SemanticalScope(scope);
        
        #region Name
        if(Name== null || Name.Semantic(scope)!= ValueType.String)
        {
            throw new Exception("Semantic Error, Name is null or not String Type");
        }
        #endregion
        
        #region Params
        if(Params!= null)
        {
            SemanticScope.WithoutReps=true;
            foreach(Expression exp in Params)
            {
                exp.Semantic(SemanticScope);
            }
            SemanticScope.WithoutReps=false;
        }
        #endregion
        
        #region Action
        if(Action== null || Action.Semantic(SemanticScope)!= ValueType.Action)
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
    public override object Evaluate(EvaluateScope scope,object Set, object Before= null)
    {
        Evaluator = new EvaluateScope(scope);
        foreach(Expression exp in Instructions)
        {
            exp.Evaluate(Evaluator,null);
        }
        return true;
    }
    public override ValueType? Semantic(SemanticalScope scope)
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
    public override object Evaluate(EvaluateScope scope,object Set, object Before= null)
    {
        Evaluator = new EvaluateScope(scope);
        if(Targets.Value != null)
        {
            Evaluator.AddVar(Targets,Targets.Value);
        }
        else throw new Exception("Evaluate Error, Targets is not set correctly");
        if(Context.Value != null)
        {
            Evaluator.AddVar(Context,Context);
        }
        else throw new Exception("Evaluate Error, Targets is not set correctly");
        Instructions.Evaluate(Evaluator,null);
        return true;
    }
    public override ValueType? Semantic(SemanticalScope scope)
    {
        SemanticScope = new SemanticalScope(scope);
        if(Targets != null)
        {
            Targets.Type= ValueType.ListCard;
            SemanticScope.AddVar(Targets,Targets);
        }
        else throw new Exception("Semantic Error, Targets is Empty");
        if(Context != null)
        {
            Context.Type= ValueType.Context;
            SemanticScope.AddVar(Context,Context);
        }
        else throw new Exception("Semantic Error, Context is Empty");
        if(!(Instructions.Semantic(SemanticScope)== ValueType.InstructionBlock))
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

    public override object Evaluate(EvaluateScope scope,object Set, object Before= null)
    {
        Evaluator= new EvaluateScope(scope);

        Collection.Value = Collection.Evaluate(scope, null);
        Evaluator.AddVar(Collection, Collection.Value);
        Evaluator.AddVar(Variable, null);

        if(Collection.Value is List<ICard> list)
        {
            foreach (ICard item in list)
            {
                Variable.Value= item;
                Evaluator.AddVar(Variable, Variable.Value);

                Instructions.Evaluate(Evaluator, null);
            }
        }
        return null;
    }
    public override ValueType? Semantic(SemanticalScope scope)
    {
        SemanticScope = new SemanticalScope(scope);
        if(Variable != null)
        {
            Variable.Type= ValueType.Card;
            SemanticScope.AddVar(Variable,Variable);
        }
        else throw new Exception("Semantic Error, For Variable is Empty");
        if(Collection != null)
        {
            Collection.Type= ValueType.ListCard;
        }
        else throw new Exception("Semantic Error, For Collection is Empty");
        if(Instructions != null)
        {
            if(!(Instructions.Semantic(SemanticScope)== ValueType.InstructionBlock))
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

    public override object Evaluate(EvaluateScope scope,object Set, object Before= null)
    {
        while((bool)Condition.Evaluate(scope, null))
        {
            Instructions.Evaluate(scope, null);
        }
        return null;
    }
    public override ValueType? Semantic(SemanticalScope scope)
    {
        SemanticScope = new SemanticalScope(scope);
        if(Condition != null)
        {
            Condition.Type= Condition.Semantic(scope);
            if(Condition.Type!= ValueType.Boolean)
                throw new Exception("Semantic Error, Expected Boolean Type in While Condition");
        }
        else throw new Exception("Semantic Error, Condition is Empty");
        if(!(Instructions.Semantic(SemanticScope)== ValueType.InstructionBlock))
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
    public Expression? Faction;
    public Expression? Power;
    public List<Expression>? Range;
    public OnActivationExpression? OnActivation;
    private string RangeFormat(List<Expression> ranges)
    {

        List<string> strings= new List<string>{"Range", "Melee", "Siege"};
        string s= "";
        string ev;
        foreach(Expression range in ranges)
        {
            ev= (string)range.Evaluate(null, null);
            if(strings.Contains(ev))
            {
                s+= ev.Substring(0,1);
            }
        }
        return s;
    }
    public override object Evaluate(EvaluateScope scope,object Set, object Before= null)
    {
        MyCard card= new();
        card.Name= (string)Name!.Evaluate(scope, Set);
        card.Type= (string)Type!.Evaluate(scope, Set);
        card.Faction= (string)Faction!.Evaluate(scope, Set);
        card.Power= (int)Power!.Evaluate(scope, Set);
        card.Effects= (List<IEffect>)OnActivation.Evaluate(scope,null);
        
        card.Range= RangeFormat(Range);
        Value = card;
        return card;
    }
    public override ValueType? Semantic(SemanticalScope scope)
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
        return ValueType.CardDeclaration;
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
    public override object Evaluate(EvaluateScope scope,object Set, object Before= null)
    {//At this method Set will be the Card is being analized for this predicate
        Evaluator = new EvaluateScope(scope);
        Evaluator.AddVar(Unit, Unit.Value);
        Unit.Value= Set;
        return Condition.Evaluate(Evaluator, null);//this is supossed to be a boolean that verifies the predicate
    }
    public override ValueType? Semantic(SemanticalScope scope)
    {
        Unit.Type = ValueType.Card;
        SemanticalScope LocalForPredicate= new(scope);
        LocalForPredicate.AddVar(Unit, Unit);
        if(Condition== null || Condition.Semantic(LocalForPredicate)!= ValueType.Boolean)
            throw new Exception("Semantic Error, Expected Boolean Type in Predicate Condition");
        return ValueType.Predicate;
    }
}
public class OnActivationExpression: Expression
{
    public List<EffectAssignment>? Effects= new();
    public override object Evaluate(EvaluateScope scope,object Set, object Before= null)
    {
        List<IEffect> effects= new();
        foreach(EffectAssignment assignment in Effects)
        {
            assignment.Evaluate(scope, Set, effects);
        }
        Value=effects;
        return Value;
    }
    public override void Print(int indentLevel = 0)
    {
        printed = "OnActivacion";
        Console.WriteLine(new string(' ', indentLevel * 4) + printed);
    }
    public override ValueType? Semantic(SemanticalScope scope)
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
    public override object Evaluate(EvaluateScope scope,object Set, object Before= null)
    {
        Value= EvaluateUtils.Finder(Effect);
        Value= new MyEffect((EffectDeclarationExpr)Value, Selector);
        if(Before is List<IEffect> list)
        {
            list.Add((IEffect)Value);
        }
        if(Selector!= null)
        Selector.Evaluate(scope, Set, Before);
        if(PostAction!= null)
        {
            if(Selector!= null)
                PostAction.Evaluate(scope, Selector.Source.Value, Before);
            else
                PostAction.Evaluate(scope, null, Before);
        }
        return null;
    }
    public override ValueType? Semantic(SemanticalScope scope)
    {
        SemanticScope = new SemanticalScope();
        #region Effect
        if(Effect== null)
        {
            throw new Exception("Semantic Error, Effect is Empty, must contain at least a name");
        }
        SemanticScope.WithoutReps=true;
        foreach(Expression? statements in Effect)
        {
            ValueType? tipo= statements.Semantic(SemanticScope);
        }
        SemanticScope.WithoutReps=false;
        #endregion
        
        #region Selector
        if(Selector!= null && Selector.Semantic(scope)!= ValueType.Selector)
        {
            throw new Exception("Semantic Error, Expected Seletor Type");
        }
        #endregion
        #region Post Action
        if(PostAction!= null && PostAction.Semantic(scope)!= ValueType.EffectAssignment)
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

    readonly List<string> SourcesAvailable= new List<string>{"hand", "otherhand", "deck", "otherdeck", "field", "otherfield", "parent"};
    private string FormatSources(string source)
    {
        string format= "";
        switch(source)
        {
            case "hand":
            format= "Hand";
            break;

            case "otherhand":
            format= "OtherHand";
            break;

            case "deck":
            format= "Deck";
            break;

            case "otherdeck":
            format= "OtherDeck";
            break;

            case "field":
            format= "Field";
            break;

            case "otherfield":
            format= "OtherField";
            break;

            case "board":
            format= "Board";
            break;

            case "parent":
            format= "parent";
            break;
            default:
            throw new Exception("Problems formatting the source");
        }
        return format;
    }
    public List<ICard> Execute(IContext context)
    {
        #region Finding Out the Source
        Source.Value= FormatSources((string)Source.Value);
        List<ICard> SourceCards= (List<ICard>)Api.InvokeMethodWithParameters(context, (string)Source.Value, null);
        
    }
    public override object Evaluate(EvaluateScope scope,object Set, object Before= null)
    {
        string s= (string)Source.Evaluate(scope, Set,Before);
        if(!SourcesAvailable.Contains(s))
        {
            throw new Exception($"You are giving an invalid source: {s}, check the available sources and try again");
        }
        if(s== "parent")
        {//Is trying to use the source of its parent
            Source.Value= Set;
        }
        Single.Evaluate(scope, Set, Before);
        return null;
    }
    public override ValueType? Semantic(SemanticalScope scope)
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
    {// Si estamos en fase evaluate, entonces debemos comparar en cuanto a valor de la expression
        if(obj is Expression objexp)
        if(SintaxFacts.CompilerPhase== "Evaluate")
            return Value.Equals(objexp.Value);
        else
        if(obj is BinaryOperator bin && bin.Left.Equals(this.Left) && bin.Right.Equals(this.Right) && bin.Operator== this.Operator&& this.Operator== TokenType.POINT)
        {//Ambos deben ser ids y deben tener el mismo nombre
            return true;
        }
        return false;
    }
    public override ValueType? Semantic(SemanticalScope scope)
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
                    throw new Exception($"You are trying to operate with a {Operator} but the operands must be number espressions");
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
                    throw new Exception($"You are trying to operate with a {Operator} but the operands must be number espressions");
            }
            // Booleans
            case TokenType.EQUAL:
            Left.Type = Left.Semantic(scope);
            Right.Type = Right.Semantic(scope);
            if(Left.Type== Right.Type)
                    return ValueType.Boolean;
                else
                    throw new Exception($"You are trying to operate with a {Operator} but the operands must be from the same type");
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
                    throw new Exception($"You are trying to operate with a {Operator} but the operands must be number espressions");
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
                throw new Exception($"You are trying to operate with a {Operator} but the operands must be number espressions");
            
            // Point            
            case TokenType.POINT:
            ValueType? type = Left.Semantic(scope);
            Left.Type= type;
            if(type != ValueType.Null && Right is Terminal right && SintaxFacts.PointPosibbles[type].Contains(right.ValueAsToken.Type))
            {
                type= right.Semantic(scope);
                Right.Type= type;
                return SintaxFacts.TypeOf[right.ValueAsToken.Type];
            }
            else if(type != ValueType.Null && Right is BinaryOperator binary && binary.Operator== TokenType.INDEXER )
            {
                if(binary.Left is Terminal T && SintaxFacts.PointPosibbles[type].Contains(T.ValueAsToken.Type))
                {//Chequeo que en el indexado la parte izquierda sea únicamente un terminal, de lo contrario se usaron paréntesis, lo cual no es permitido
                    binary.Left.Type= SintaxFacts.TypeOf[T.ValueAsToken.Type];
                    type = binary.Semantic(scope);
                    return type; 
                }
                throw new Exception("Semantic, tried to associate from Point");
            }
            else
                throw new Exception("Semantic from Point");
            //Indexer
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
    public override object Evaluate(EvaluateScope scope,object Set, object Before= null)
    {
        switch(Operator)
        {
             //Acums
            case TokenType.PLUSACCUM:
            Right.Value= (int)Right.Evaluate(scope,Set,Before);
            Left.Value= (int)Left.Evaluate(scope, Set,Before);
            Left.Value= (int)Left.Value! + (int)Right.Value;
            this.Value= Left.Value;
            return Left.Value;

            case TokenType.MINUSACCUM:
            Right.Value= (int)Right.Evaluate(scope,Set,Before);
            Left.Value= (int)Left.Evaluate(scope, Set,Before);
            Left.Value= (int)Left.Value!- (int)Right.Value;
            this.Value= Left.Value;
            return Left.Value;




            // Math
            case TokenType.PLUS:
            Right.Value= (int)Right.Evaluate(scope,Set,Before);
            Left.Value= (int)Left.Evaluate(scope,Set,Before);
            this.Value= (int)Left.Evaluate(scope, Set, Before) + (int)Right.Evaluate(scope,Set,Before);
            return this.Value;

            case TokenType.MINUS:
            Right.Value= (int)Right.Evaluate(scope,Set,Before);
            Left.Value= (int)Left.Evaluate(scope,Set,Before);
            this.Value= (int)Left.Evaluate(scope, Set, Before) - (int)Right.Evaluate(scope,Set,Before);
            return this.Value;

            case TokenType.MULTIPLY:
            Right.Value= (int)Right.Evaluate(scope,Set,Before);
            Left.Value= (int)Left.Evaluate(scope,Set,Before);
            this.Value= (int)Left.Evaluate(scope, Set, Before) * (int)Right.Evaluate(scope,Set,Before);
            return this.Value;

            case TokenType.DIVIDE:
            Right.Value= (int)Right.Evaluate(scope,Set,Before);
            if((int)Right.Value!=0)
            {
            Left.Value= (int)Left.Evaluate(scope,Set,Before);
            this.Value= (int)Left.Evaluate(scope, Set, Before) / (int)Right.Evaluate(scope,Set,Before);
            return this.Value;
            }
            else
            throw new Exception($"Division by cero {Operator}");
            
            case TokenType.POW:
            Right.Value= (int)Right.Evaluate(scope,Set,Before);
            Left.Value= (int)Left.Evaluate(scope,Set,Before);
            this.Value= Math.Pow((int)Left.Evaluate(scope, Set, Before) , (int)Right.Evaluate(scope,Set,Before));
            return this.Value;

            // Booleans
            case TokenType.EQUAL:
            Right.Value= (int)Right.Evaluate(scope,Set,Before);
            Left.Value= (int)Left.Evaluate(scope,Set,Before);
            this.Value= Left.Evaluate(scope, Set, Before).Equals(Right.Evaluate(scope,Set,Before));
            return this.Value;

            case TokenType.LESS_EQ:
            Right.Value= (int)Right.Evaluate(scope,Set,Before);
            Left.Value= (int)Left.Evaluate(scope,Set,Before);
            this.Value= (int)Left.Evaluate(scope, Set, Before) <= (int)Right.Evaluate(scope,Set,Before);
            return this.Value;

            case TokenType.MORE_EQ:
            Right.Value= (int)Right.Evaluate(scope,Set,Before);
            Left.Value= (int)Left.Evaluate(scope,Set,Before);
            this.Value= (int)Left.Evaluate(scope, Set, Before) >= (int)Right.Evaluate(scope,Set,Before);
            return this.Value;

            case TokenType.MORE:
            Right.Value= (int)Right.Evaluate(scope,Set,Before);
            Left.Value= (int)Left.Evaluate(scope,Set,Before);
            this.Value= (int)Left.Evaluate(scope, Set, Before) > (int)Right.Evaluate(scope,Set,Before);
            return this.Value;

            case TokenType.LESS:
            Right.Value= (int)Right.Evaluate(scope,Set,Before);
            Left.Value= (int)Left.Evaluate(scope,Set,Before);
            this.Value= (int)Left.Evaluate(scope, Set, Before) < (int)Right.Evaluate(scope,Set,Before);
            return this.Value;

            case TokenType.AND:
            Right.Value= (int)Right.Evaluate(scope,Set,Before);
            Left.Value= (int)Left.Evaluate(scope,Set,Before);
            this.Value= (bool)Left.Evaluate(scope, Set, Before) && (bool)Right.Evaluate(scope,Set,Before);
            return this.Value;

            case TokenType.OR:
            Right.Value= (int)Right.Evaluate(scope,Set,Before);
            Left.Value= (int)Left.Evaluate(scope,Set,Before);
            this.Value= (bool)Left.Evaluate(scope, Set, Before) || (bool)Right.Evaluate(scope,Set,Before);
            return this.Value;

            // String   
            case TokenType.CONCATENATION:
            Right.Value= (int)Right.Evaluate(scope,Set,Before);
            Left.Value= (int)Left.Evaluate(scope,Set,Before);
            this.Value= (string)Left.Evaluate(scope, Set, Before) + (string)Right.Evaluate(scope,Set,Before);
            return this.Value;

            case TokenType.SPACE_CONCATENATION:
            Right.Value= (int)Right.Evaluate(scope,Set,Before);
            Left.Value= (int)Left.Evaluate(scope,Set,Before);
            this.Value= (string)Left.Evaluate(scope, Set, Before) + " "+ (string)Right.Evaluate(scope,Set,Before);
            return this.Value;
            // Point            
            case TokenType.POINT:
            Left.Value= Left.Evaluate(scope, Set, Before);
            Right.Value= Left.Evaluate(scope, Set, Left.Value);
            
            this.Value= Right.Value;
            return Right.Value;
            //Indexer
            case TokenType.INDEXER:
            if(Left.Value is List<ICard> list)
            {
                Right.Value= (int)Right.Evaluate(scope,Set,Before);
                if((int)Right.Value< list.Count)
                {
                    Left.Value=list[(int)Right.Value];
                    return Left.Value;
                }
                else 
                    throw new Exception($"Evaluate Error at Indexer, because index out of range of List");
            }
            else throw new Exception($"Evaluate Error at Indexer, {Left.Type} must be List Type");
            //Two Points
            case TokenType.ASSIGN:
            case TokenType.TWOPOINT:
            
            Right.Value= Right.Evaluate(scope, null);
            Left.Value= Left.Evaluate(scope, Right.Value);
            Value= Left.Value;
            return Left.Value;
            
            default:
            throw new Exception("Invalid Operator"+ Operator);
        }
    }

}
public class Terminal: Expression
{
    public string? ValueForPrint;
    public Token ValueAsToken { get; }
    public Terminal(Token token)
    {
        this.ValueForPrint = token.Value;
        ValueAsToken= token;
    }
    public override object Evaluate(EvaluateScope scope,object Set, object Before= null)
    {
        throw new NotImplementedException();
    }
    public override ValueType? Semantic(SemanticalScope scope)
    {
        throw new NotImplementedException();
    }
    public override bool Equals(object? obj)
    {
        if(obj is Expression exp && SintaxFacts.CompilerPhase== "Evaluate"&& ValueAsToken== exp.Value)
        {
            return true;
        }
        if(SintaxFacts.CompilerPhase== "Semantic" && obj is Terminal id && id.ValueAsToken.Value== this.ValueAsToken.Value)
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
    public override object Evaluate(EvaluateScope scope,object Set, object Before= null)
    {
        if(Operand!= null)
        switch(Operator)
        {
            //Card Argument
            case TokenType.SendBottom:
            case TokenType.Remove:
            case TokenType.Push:
            case TokenType.Add:
            Api.InvokeMethodWithParameters(Before, Operator.ToString(), Operand.Evaluate(scope, Set));
            return null;
            //Player Argument
            case TokenType.HandOfPlayer:
            case TokenType.DeckOfPlayer:
            case TokenType.GraveYardOfPlayer:
            case TokenType.FieldOfPlayer:
            case TokenType.Find:
            Value= Api.InvokeMethodWithParameters(Before, Operator.ToString(), Operand.Evaluate(scope, Set));
            return Value;
            //Numbers
            case TokenType.RDECREMENT:
            Value= (int)Operand.Evaluate(scope, Set)-1;
            return (int)Value+1;
            case TokenType.LDECREMENT:
            Value= (int)Operand.Evaluate(scope, Set)-1;
            return (int)Value+1;
            case TokenType.RINCREMENT:
            Value= (int)Operand.Evaluate(scope, Set)+1;
            return (int)Value-1;
            case TokenType.LINCREMENT:
            Value= (int)Operand.Evaluate(scope, Set)+1;
            return (int)Value;
            case TokenType.MINUS:
            Value= (int)Operand.Evaluate(scope, Set)*-1;
            return (int)Value-1;
            case TokenType.PLUS:
            Value= Operand.Evaluate(scope, Set);
            return Value;
            //Boolean
            case TokenType.NOT:
            {
                Value= Operand.Evaluate(scope, Set);
                return !(bool)Value;
            }
            
            
        }
        if(SintaxFacts.TypeOf.ContainsKey(Operator))
        {
            Type = SintaxFacts.TypeOf[Operator];
            return Type;
        }
        throw new Exception("Invalid Unary Operator");
    }
    public override bool Equals(object? obj)
    {
        if(obj is UnaryOperator unary && unary.Operator.Equals(this.Operator) )
        {
            if(SintaxFacts.CompilerPhase== "Evaluate"&& Operand.Value== unary.Operand.Value)
            {
                return true;
            }
            else if(SintaxFacts.CompilerPhase== "Semantic" && unary.Operand.Equals(this.Operand))
            {
                return true;
            }
        }
        return false;
    }
    public override ValueType? Semantic(SemanticalScope scope)
    {
        if(Operand!= null)
        switch(Operator)
        {
            //Card Argument
            case TokenType.SendBottom:
            case TokenType.Remove:
            case TokenType.Push:
            case TokenType.Add:
            if(Operand.Semantic(scope)==ValueType.Card)
                return ValueType.Card;
            else
                throw new Exception("Semantic Error, Expected Card Type");
            //Player Argument
            case TokenType.HandOfPlayer:
            case TokenType.DeckOfPlayer:
            case TokenType.GraveYardOfPlayer:
            case TokenType.FieldOfPlayer:
            if(Operand.Semantic(scope)==ValueType.Player)
                return ValueType.Player;
            else
                throw new Exception("Semantic Error, Expected Player Type");
            //Numbers
            case TokenType.RDECREMENT:
            case TokenType.LDECREMENT:
            case TokenType.RINCREMENT:
            case TokenType.LINCREMENT:
            case TokenType.MINUS:
            case TokenType.PLUS:
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
            case TokenType.Find:
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
    public override object Evaluate(EvaluateScope scope,object Set, object Before= null)
    {
        return Convert.ToInt32(ValueAsToken.Value);
    }
    public override ValueType? Semantic(SemanticalScope scope)
    {
        this.SemanticScope = scope;
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
    public override object Evaluate(EvaluateScope scope,object Set, object Before= null)
    {
        return Convert.ToBoolean(ValueAsToken.Value);
    }
    public override ValueType? Semantic(SemanticalScope scope)
    {
        this.SemanticScope = scope;
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
    public override ValueType? Semantic(SemanticalScope scope)
    {
        if(SintaxFacts.TypeOf.ContainsKey(ValueAsToken.Type))
        {
            Type= SintaxFacts.TypeOf[ValueAsToken.Type];
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
    public override object Evaluate(EvaluateScope scope,object Set, object Before= null)
    {
        if(SintaxFacts.PointPosibbles[ValueType.Context].Contains(ValueAsToken.Type))
        {
            if(Before is IContext context)
            {
                return Api.GetProperty(context, ValueAsToken.Value);
            }
            else
            throw new Exception("Troubles with IContext Interface");
        }
        else if(SintaxFacts.PointPosibbles[ValueType.Card].Contains(ValueAsToken.Type)&& Before is ICard card)
        {
                if(Set!= null)//Este id se encuentra a la izquierda de una operacion de igualdad
                switch(ValueAsToken.Type)
                {
                    case TokenType.Name:
                    card.Name= (string)Set;
                    break;
                    case TokenType.Owner:
                    card.Owner= (IPlayer)Set;
                    break;
                    case TokenType.Power:
                    card.Power= (int)Set;
                    break;
                    case TokenType.Faction:
                    card.Faction= (string)Set;
                    break;
                    case TokenType.Range:
                    card.Range= (string)Set;
                    break;
                    case TokenType.Type:
                    card.Type= (string)Set;
                    break;
                }
                else//se encuentra a la derecha de una igualdad, solo se solicita su valor, no se pretende setear
                switch(ValueAsToken.Type)
                {
                    case TokenType.Name:
                    return card.Name;

                    case TokenType.Owner:
                    return card.Owner;
                    
                    case TokenType.Power:
                    return card.Power;

                    case TokenType.Faction:
                    return card.Faction;
                    
                    case TokenType.Range:
                    return card.Range;

                    case TokenType.Type:
                    return card.Type;
                }

        }
            if(Set!= null)
            {
                Value= Set;
                if(scope!=null)
                scope.AddVar(this, Value);
                return Value;
            }
            else
            {
                object value= null;
                if(ValueAsToken!= null)
                {
                    scope.Find(this, out value);
                    return value;
                }
                else return value;
            }
            throw new Exception($"Unexpected problems at the identifier: {ValueAsToken}");
    }
}
public class StringExpression : Terminal
{
    public StringExpression(Token token):base(token)
    {
        this.printed = "STRING"; // O alguna otra forma de representar el identificador visualmente
    }
    public override object Evaluate(EvaluateScope scope,object Set, object Before= null)
    {
        return ValueAsToken.Value.Substring(1,ValueAsToken.Value.Length-2);
    }
    public override ValueType? Semantic(SemanticalScope scope)
    {
        this.SemanticScope = scope;
        Type = ValueType.String;
        return Type;
    }
}
#endregion


