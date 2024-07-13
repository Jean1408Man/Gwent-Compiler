using System.Security.Cryptography.X509Certificates;

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
    public static Dictionary<ValueType?, HashSet<TokenType>> PointPosibbles= new Dictionary<ValueType?, HashSet<TokenType>>
    {
        {ValueType.Card, new HashSet<TokenType>(){TokenType.NAME, TokenType.OWNER, TokenType.POWER, TokenType.FACTION,TokenType.RANGE, TokenType.TYPE}},


        {ValueType.Context, new HashSet<TokenType>(){TokenType.DECK, TokenType.DECKOFPLAYER, TokenType.GRAVEYARD, TokenType.GRAVEYARDOFPLAYER
        , TokenType.FIELD,TokenType.FIELDOFPLAYER, TokenType.HAND, TokenType.HANDOFPLAYER,TokenType.BOARD, TokenType.TRIGGERPLAYER}},

        
        {ValueType.ListCard, new HashSet<TokenType>(){TokenType.FIND, TokenType.PUSH, TokenType.SENDBOTTOM, TokenType.POP
        }},

    };
    public static Dictionary<TokenType, ValueType> TypeOf = new Dictionary<TokenType, ValueType>
    {
        //  Strings
        {TokenType.NAME, ValueType.String},
        {TokenType.FACTION, ValueType.String},
        {TokenType.TYPE, ValueType.String},

        //Players
        {TokenType.OWNER, ValueType.Player},
        {TokenType.TRIGGERPLAYER, ValueType.ListCard},
        
        //Numbers
        {TokenType.POWER, ValueType.Number},
        
        // List Cards
        {TokenType.DECK, ValueType.ListCard},
        {TokenType.DECKOFPLAYER, ValueType.ListCard},
        {TokenType.GRAVEYARD, ValueType.ListCard},
        {TokenType.GRAVEYARDOFPLAYER, ValueType.ListCard},
        {TokenType.FIELD, ValueType.ListCard},
        {TokenType.FIELDOFPLAYER, ValueType.ListCard},
        {TokenType.HAND, ValueType.ListCard},
        {TokenType.HANDOFPLAYER, ValueType.ListCard},
        {TokenType.BOARD, ValueType.ListCard},
        {TokenType.FIND, ValueType.ListCard},
        
        //Cards
        {TokenType.POP, ValueType.Card},
        
        //Voids
        {TokenType.SENDBOTTOM, ValueType.Void},
        {TokenType.PUSH, ValueType.Void},

    };

    #region Two Points Fixer
    public static Expression TwoPointsFixer(BinaryOperator TwoPointer)
    {
        List<Expression?> expressions = ListPoint(TwoPointer);
        return BuildFixedBin(expressions,0, expressions.Count-1);
    }
    private static List<Expression?> ListPoint(BinaryOperator binary)
    {
        List<Expression?> list = new List<Expression>();
        list.Add(binary.Left);
        if(binary.Right is BinaryOperator bin)
        list.Concat(ListPoint(bin));
        else
        list.Add(binary.Right);
        return list;
    }
    private static Expression BuildFixedBin(List<Expression?> list, int ini, int fin)
    {
        if(ini==fin)
        {
            return list[ini];        
        }
        BinaryOperator binary = new BinaryOperator(BuildFixedBin(list, ini,fin-1), list[fin], TokenType.POINT);
        binary.Fixed= true;
        return binary;
    } 
    #endregion

    

}
public enum ValueType
{
    Null,


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
    EffectDeclaration,
    CardDeclaration,
    Action,
    InstructionBlock,
    Program
    #endregion


}
public class Scope
{//No Debuggeado
    public Scope parentScope;
    public Scope(Scope Parent= null)
    {
        parentScope= Parent;
    }
    public Dictionary< Expression , Expression > Variables;
    private void InternalFind(Expression tofind, out Expression Finded, out Scope Where)
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
        Scope Where;
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
        Scope Where;
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


