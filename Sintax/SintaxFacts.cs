using System.Security.Cryptography.X509Certificates;

namespace LogicalSide;

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
    
    public static string CompilerPhase;
    
    
    public static Dictionary<ValueType?, HashSet<TokenType>> PointPosibbles= new Dictionary<ValueType?, HashSet<TokenType>>
    {
        {ValueType.Card, new HashSet<TokenType>(){TokenType.Name, TokenType.Owner, TokenType.Power, TokenType.Faction,TokenType.Range, TokenType.Type}},


        {ValueType.Context, new HashSet<TokenType>(){TokenType.Find,TokenType.Deck, TokenType.DeckOfPlayer, TokenType.GraveYard, TokenType.GraveYardOfPlayer
        , TokenType.Field,TokenType.FieldOfPlayer, TokenType.Hand, TokenType.HandOfPlayer,TokenType.Board, TokenType.TriggerPlayer}},

        
        {ValueType.ListCard, new HashSet<TokenType>(){ TokenType.Push, TokenType.SendBottom, TokenType.Pop, TokenType.Shuffle
        }},

    };
    public static Dictionary<TokenType, ValueType> TypeOf = new Dictionary<TokenType, ValueType>
    {
        //  Strings
        {TokenType.Name, ValueType.String},
        {TokenType.Faction, ValueType.String},
        {TokenType.Type, ValueType.String},
        {TokenType.STRINGTYPE, ValueType.String},
        {TokenType.SOURCE, ValueType.String},
        {TokenType.EFFECTASSIGNMENT, ValueType.String},

        //Players
        {TokenType.Owner, ValueType.Player},
        {TokenType.TriggerPlayer, ValueType.ListCard},
        
        //Numbers
        {TokenType.Power, ValueType.Number},
        {TokenType.PLUS, ValueType.Number},
        {TokenType.MINUS, ValueType.Number},
        {TokenType.NUMBERTYPE, ValueType.Number},

        //Predicates
        {TokenType.PREDICATE, ValueType.Predicate},

        //Booleans
        {TokenType.NOT, ValueType.Boolean},
        {TokenType.BOOLEAN, ValueType.Boolean},
        {TokenType.SINGLE, ValueType.Boolean},
        
        // List Cards
        {TokenType.Deck, ValueType.ListCard},
        {TokenType.DeckOfPlayer, ValueType.ListCard},
        {TokenType.GraveYard, ValueType.ListCard},
        {TokenType.GraveYardOfPlayer, ValueType.ListCard},
        {TokenType.Field, ValueType.ListCard},
        {TokenType.FieldOfPlayer, ValueType.ListCard},
        {TokenType.Hand, ValueType.ListCard},
        {TokenType.HandOfPlayer, ValueType.ListCard},
        {TokenType.Board, ValueType.ListCard},
        {TokenType.Find, ValueType.ListCard},
        
        //Cards
        {TokenType.Pop, ValueType.Card},
        
        //Voids
        {TokenType.SendBottom, ValueType.Void},
        {TokenType.Push, ValueType.Void},
        {TokenType.Shuffle, ValueType.Void},
        {TokenType.Add, ValueType.Void},

    };

    public static readonly List<ValueType?> AssignableTypes= new List<ValueType?>
    {
        ValueType.Number,
        ValueType.Boolean,
        ValueType.UnassignedVar,
        ValueType.String
    };
}

public enum ValueType
{
    Null,
    UnassignedVar,

    Number,
    String,
    Boolean,
    Void,
    
    
    Card,
    ListCard,
    
    
    Context,
    Player,


    #region Tipos Orientados solo a chequeo
    OnActivacion,
    Predicate,
    EffectAssignment,
    For,
    EffectDeclaration,
    CardDeclaration,
    Action,
    InstructionBlock,
    Selector,
    Program,
    #endregion
}








public class Scope
{

}
public class SemanticalScope: Scope
{//No Debuggeado
    public SemanticalScope parentScope;
    public SemanticalScope(SemanticalScope Parent= null)
    {
        parentScope= Parent;
    }
    public bool WithoutReps=false;
    public Dictionary< Expression , Expression > Variables= new();
    private void InternalFind(Expression tofind, out Expression Finded, out SemanticalScope Where)
    {
        bool b= false;
        Finded = null;
        Where = null;
        foreach(Expression indic in Variables.Keys)
        {
            if(tofind.Equals(indic)) 
            {
                Where = this;
                Finded = indic;
                b=true;
            }
        }
        if(!b)
        {
            if(parentScope!=null)
            {
                parentScope.InternalFind(tofind, out Finded, out Where);
            }
            else{
                Where = null;
                Finded = null;
            }
        }
    }
    public bool Find(Expression exp, out ValueType? type)
    {
        Expression Finded;
        SemanticalScope Where;
        InternalFind(exp,out Finded, out Where);
        if(Where!= null)
        {
            type= Finded.Type;
            return true;
        }
        else
        {
            type = null;
            return false;
        }
    }
    public void AddVar(Expression exp, Expression Value= null)
    {
        Expression Finded;
        SemanticalScope Where;
        InternalFind(exp,out Finded, out Where);
        if(Where!= null)
        {
            if(!WithoutReps)
                Where.Variables[Finded]= Value;
            else
                throw new Exception("A no Reps statement was violated");
        }
        else
        {
            Variables.Add(exp,Value);
        }
    }
}

public class EvaluateScope: Scope
{//No Debuggeado
    public EvaluateScope parentScope;
    public EvaluateScope(EvaluateScope Parent= null)
    {
        parentScope= Parent;
    }
    public Dictionary< Expression , object > Variables= new();
    private void InternalFind(Expression tofind, out Expression Finded, out EvaluateScope Where)
    {
        bool b= false;
        Finded = null;
        Where = null;
        foreach(Expression indic in Variables.Keys)
        {
            if(tofind.Equals(indic)) 
            {
                Where = this;
                Finded = indic;
                b=true;
            }
        }
        if(!b)
        {
            if(parentScope!=null)
            {
                parentScope.InternalFind(tofind, out Finded, out Where);
            }
            else{
                Where = null;
                Finded = null;
            }
        }
    }
    public bool Find(Expression exp, out object type)
    {
        Expression Finded;
        EvaluateScope Where;
        InternalFind(exp,out Finded, out Where);
        if(Where!= null)
        {
            type= Finded.Value;
            return true;
        }
        else
        {
            type = null;
            return false;
        }
    }
    public void AddVar(Expression exp, object Value= null)
    {
        Expression Finded;
        EvaluateScope Where;
        InternalFind(exp,out Finded, out Where);
        if(Where!= null)
        {
            Where.Variables[Finded]= Value;
        }
        else
        {
            Variables.Add(exp,Value);
        }
    }
    
}



