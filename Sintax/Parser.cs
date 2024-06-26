using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Compiler;

public class Parser
{
    public int position;
    List<Token> tokens;
    public Parser(List<Token> tokens)
    {
        position = 0;
        this.tokens = tokens;
    }
    public Expression Parse()
    {
        Expression expression;
        expression = ParseProgram();
        return expression;
    }
    #region Parsing Terminals
    private Expression ParseExpression(int parentprecedence =0)
    {
        var left = ParsePrimaryExpression();

        while (position < tokens.Count)
        {
            var precedence = SintaxFacts.GetPrecedence(tokens[position].Type);
            if(precedence==0|| precedence<= parentprecedence) 
            break;
            
            var operatortoken = tokens[position++].Type;
            var right = ParseExpression(precedence);
            left = new BinaryOperator(left, right, operatortoken);
        }
        return left;
    }
    private Expression ParsePrimaryExpression(bool left= false)
    {//Aqui agregue funciones al parser
        #region Expresiones Literales
        if (position >= tokens.Count) throw new Exception("Unexpected end of input");
        if (tokens[position].Type == TokenType.LPAREN)
        {
            position++;
            Expression expr = ParseExpression(); 
            if (tokens[position].Type!= TokenType.RPAREN)
            {
                throw new Exception("Missing closing parenthesis");
            }
            position++;
            return expr;
        }
        else if (tokens[position].Type == TokenType.FALSE|| tokens[position].Type == TokenType.TRUE)
        {
            position++; 
            return new BooleanLiteral(tokens[position - 1]);
        }
        else if (tokens[position].Type == TokenType.ID)
        {
            position++; 
            return new IdentifierExpression(tokens[position - 1]);
        }
        else if (tokens[position].Type == TokenType.NAME || tokens[position].Type == TokenType.TYPE 
                ||tokens[position].Type == TokenType.FACTION ||tokens[position].Type == TokenType.POWER 
                ||tokens[position].Type == TokenType.EFFECTASSIGNMENT || tokens[position].Type == TokenType.SOURCE 
                ||tokens[position].Type == TokenType.SINGLE||tokens[position].Type == TokenType.OWNER
                ||tokens[position].Type == TokenType.DECK||tokens[position].Type == TokenType.GRAVEYARD
                ||tokens[position].Type == TokenType.FIELD||tokens[position].Type == TokenType.BOARD
                ||tokens[position].Type == TokenType.HAND)
        {
            position++; 
            return new IdentifierExpression(tokens[position - 1]);
        }
        else if (tokens[position].Type == TokenType.STRING)
        {
            position++;
            return new StringExpression(tokens[position - 1]);
        }
        else if (tokens[position].Type == TokenType.INT)
        {
            position++;
            return new Number(tokens[position - 1]);
        }
        else if ((tokens[position].Type == TokenType.NOT)||(tokens[position].Type == TokenType.PLUS)||(tokens[position].Type == TokenType.MINUS) && (position == 0 || (tokens[position - 1].Type != TokenType.INT&& tokens[position - 1].Type != TokenType.ID)))
        {
            TokenType unary = tokens[position].Type;
            position++;
            Expression operand = ParsePrimaryExpression();
            return new BinaryOperator(new Number(SintaxFacts.Numerical(0)), operand, unary);
        }
        #endregion
        #region Funciones
        else if (tokens[position].Type == TokenType.SHUFFLE||tokens[position].Type == TokenType.POP)
        {//Functions without parameters
            Token token= tokens[position];
            if(tokens[++position].Type== TokenType.LPAREN && tokens[++position].Type== TokenType.RPAREN)
            {
                position++;
                return new UnaryOperator(null, token.Type);
            }
        }
        else if (tokens[position].Type == TokenType.PUSH ||
                tokens[position].Type == TokenType.SENDBOTTOM||tokens[position].Type == TokenType.REMOVE
                ||tokens[position].Type == TokenType.HANDOFPLAYER||tokens[position].Type == TokenType.DECKOFPLAYER
                ||tokens[position].Type == TokenType.FIELDOFPLAYER||tokens[position].Type == TokenType.GRAVEYARDOFPLAYER
                ||tokens[position].Type == TokenType.ADD)
                {//Functions with parameters
                    Token token= tokens[position];
                    if(tokens[++position].Type== TokenType.LPAREN)
                    {
                        position++;
                        Expression argument = ParseExpression();
                        if(tokens[position++].Type== TokenType.RPAREN)
                        {
                            return new UnaryOperator(argument, token.Type);
                        }
                    }
                }
        #endregion
        #region Propiedades
        #endregion
        throw new Exception($"{position} Not recognizable primary token {tokens[position].Type}");
    }
    #endregion
    private Expression ParseAssignment(bool expectedAssign, bool IsProperty= true)
    {//true means it expects value, false means it expects ValueType
        Expression left;
        if(!IsProperty)
            left = ParseExpression();
        else
            left= ParsePrimaryExpression();
        Token token= tokens[position];
        Expression right=null;
        Expression Binary= null;
        if(expectedAssign)
        {
            if (token.Type == TokenType.ASSIGN|| token.Type == TokenType.TWOPOINT)//Agregar formas como incremento etc...
            {
                position++;
                right = ParseExpression();
                Binary= new BinaryOperator(left, right,token.Type);
            }//NADA DE ESTO ESTÁ HECHO
            else if(token.Type == TokenType.INCREEMENT|| token.Type == TokenType.DECREMENT)
            {
                position++;
                right = new UnaryOperator(ParseExpression(), token.Type);
                Binary= new BinaryOperator(left, right,token.Type);
            }
            else if(token.Type == TokenType.PLUSACCUM|| token.Type == TokenType.MINUSACCUM)
            {
                position++;
                right = new UnaryOperator(ParseExpression(), token.Type);
                Binary= new BinaryOperator(left, right,token.Type);
            }
        }
        else 
        {
            if (token.Type == TokenType.ASSIGN|| token.Type == TokenType.TWOPOINT)//Agregar formas como incremento etc...
            {
                position++;
                if(tokens[position].Type==TokenType.NUMBERTYPE || tokens[position].Type==TokenType.STRINGTYPE)
                {
                    right = new IdentifierExpression(tokens[position]);
                    position++;
                    Binary= new BinaryOperator(left, right,token.Type);
                }
            }
        }
        if(tokens[position].Type==TokenType.COMA || tokens[position].Type==TokenType.POINTCOMA||tokens[position].Type==TokenType.RCURLY)
        {
            if(tokens[position].Type!=TokenType.RCURLY)
                position++;
            if(Binary!= null)
                return Binary;
            else
                return left;
        }
        else
        throw new Exception($"{position} Unexpected assign token at {token.lugar.fila} file and {token.lugar.colmna} column({token.Type}), expected Comma or Semicolon");
    }
    
    private Expression ParseProgram()
    {
        ProgramExpression program= new();
        Token token;
        while(position< tokens.Count)
        {
            token = tokens[position++];
            if(token.Type== TokenType.EFFECTDECLARATION || token.Type == TokenType.CARD)
            {
                if(tokens[position++].Type== TokenType.LCURLY)
                {
                    if(token.Type== TokenType.EFFECTDECLARATION)
                    {
                        program.Effects.Add(ParseEffectDeclaration());
                    }
                    else if(token.Type== TokenType.CARD)
                    {
                        program.Cards.Add(ParseCard());
                    }
                }
                else
                throw new Exception($"{position} Invalid token {tokens[position-1]}, expected Left Curly");
            }
            else
            throw new Exception($"{position} Invalid Token at {token.lugar.fila} row and {token.lugar.colmna} column expected Effect or Card ");
        }
        return program;
    }
    #region Parsing Cards and associated
    private CardExpression ParseCard()
    {
        CardExpression card= new();
        Token token = tokens[position];
        while(position< tokens.Count)
        {
            switch (token.Type)
            {
                case TokenType.NAME:
                    if(card.Name!=null)
                        throw new Exception($"{position} Name has been declared already");
                    card.Name = ParseAssignment(true);
                    token= tokens[position];
                    break;
                case TokenType.TYPE:
                    if(card.Type!=null)
                        throw new Exception($"{position} Type has been declared already");
                    card.Type = ParseAssignment(true);
                    token= tokens[position];
                    break;
                case TokenType.RANGE:
                    if(card.Range!=null)
                        throw new Exception($"{position} Range has been declared already");
                    card.Range = ParseRanges();// Manejo para TokenType.RANGE
                    if(tokens[position].Type== TokenType.COMA)
                    position++;
                    else
                    throw new Exception($"{position} Invalid Token at {tokens[position].lugar.fila} row and {tokens[position].lugar.colmna} column expected Comma in range end definition");
                    token= tokens[position];
                    break;
                case TokenType.POWER:
                    if(card.Power!=null)
                        throw new Exception($"{position} Power has been declared already");
                    card.Power = ParseAssignment(true);// Manejo para TokenType.POWER
                    token= tokens[position];
                    break;
                case TokenType.FACTION:
                    if(card.Faction!=null)
                        throw new Exception($"{position} Faction has been declared already");
                    card.Faction = ParseAssignment(true);
                    token= tokens[position];
                    break;
                    case TokenType.ONACTIVATION:
                    if(card.OnActivation!=null)
                        throw new Exception($"{position} Faction has been declared already");
                    card.OnActivation = ParseOnActivation();
                    token= tokens[position];
                    break;
                case TokenType.RCURLY:
                        position++;
                        return card;
                default:
                    throw new Exception($"{position} Invalid Token at {token.lugar.fila} row and {token.lugar.colmna} column expected card item");
            }
        }
        return card;
    }
    public List<Expression> ParseRanges()
    {
        if(tokens[++position].Type== TokenType.TWOPOINT && tokens[++position].Type== TokenType.LBRACKET)
        {
            List<Expression> ranges = new();
            position++;
            while(position< tokens.Count)
            {
                if(tokens[position].Type== TokenType.STRING)
                {
                    ranges.Add(ParseExpression());
                    if(tokens[position].Type== TokenType.COMA|| tokens[position].Type== TokenType.RBRACKET)
                    {
                        position++;
                        if(tokens[position-1].Type== TokenType.RBRACKET)
                        break;
                    }
                    else
                        throw new Exception($"{position} Invalid Token at {tokens[position].lugar.fila} row and {tokens[position].lugar.colmna} column expected Comma");
                }
                else
                    throw new Exception($"{position} Invalid Token at {tokens[position].lugar.fila} row and {tokens[position].lugar.colmna} column expected in ");
            }
            return ranges;
        }
        else
            throw new Exception($"{position} Invalid Token at {tokens[position].lugar.fila} row and {tokens[position].lugar.colmna} column ");
    }
    public PredicateExp ParsePredicate()
    {
        if(tokens[++position].Type== TokenType.TWOPOINT)
        {
            PredicateExp predicate= new();
            if(tokens[++position].Type== TokenType.LPAREN && tokens[++position].Type== TokenType.ID)
                predicate.Unit= new IdentifierExpression(tokens[position]);
                if(tokens[++position].Type== TokenType.RPAREN && tokens[++position].Type== TokenType.ARROW)
                {
                    position++;
                    predicate.Condition= ParseExpression();
                    if(tokens[position].Type== TokenType.COMA|| tokens[position].Type== TokenType.RCURLY)
                    {
                        if(tokens[position].Type== TokenType.COMA)
                        position++;
                        return predicate;
                    }
                    else
                        throw new Exception($"{position} Invalid Token at {tokens[position].lugar.fila} row and {tokens[position].lugar.colmna} column expected Comma");
                }
                else
                    throw new Exception($"{position} Invalid Token at {tokens[position].lugar.fila} row and {tokens[position].lugar.colmna} column expected in ");
        
        }
        else
            throw new Exception($"{position} Invalid Token at {tokens[position].lugar.fila} row and {tokens[position].lugar.colmna} column ");
    }
    private OnActivationExpression ParseOnActivation()
    {
        OnActivationExpression activation = new();
        position++;
        if(tokens[position++].Type== TokenType.TWOPOINT && tokens[position++].Type== TokenType.LBRACKET)
        while(position< tokens.Count)
        {
            if(tokens[position].Type== TokenType.LCURLY)
            {
                activation.Effects.Add(ParseEffectAssignment());
            }
            else if(tokens[position].Type== TokenType.RBRACKET)
            {
                position++;
                break;
            }
            else
            throw new Exception($"{position} Invalid Token at {tokens[position].lugar.fila} row and {tokens[position].lugar.colmna} column expected ????? in OnActivation");
        }
        return activation;
    }
    private EffectAssignment ParseEffectAssignment()
    {
        EffectAssignment efecto= new();
        if(tokens[position++].Type== TokenType.LCURLY)
        while(position< tokens.Count)
        {
            switch (tokens[position].Type)
            {
                case TokenType.EFFECTASSIGNMENT:
                    if(tokens[position+2].Type== TokenType.LCURLY)
                        efecto.Effect = ParseParams();
                    else
                        efecto.Effect.Add(ParseAssignment(true));
                    break;
                case TokenType.SELECTOR:
                    efecto.Selector= ParseSelector();
                    break;
                case TokenType.POSTACTION:
                    position++;
                    if(tokens[position++].Type== TokenType.TWOPOINT)
                    efecto.PostAction = ParseEffectAssignment();// Manejo para TokenType.RANGE
                    else
                    throw new Exception($"Invalid Token at {tokens[position-1].lugar.fila} row and {tokens[position-1].lugar.colmna} column expected Colon in PostAction statement");
                    break;
                case TokenType.RCURLY:
                    if(tokens[++position].Type==TokenType.COMA|| tokens[position].Type==TokenType.POINTCOMA|| tokens[position].Type==TokenType.RCURLY)
                    {
                        if(tokens[position].Type!=TokenType.RCURLY)
                        position++;
                    }
                    return efecto;
                    break;
                default:
                    throw new Exception($"{position} Invalid Token at {tokens[position].lugar.fila} row and {tokens[position].lugar.colmna} column expected card item");
            }
        }
        return efecto;
    }
    private SelectorExpression ParseSelector()
    {
        SelectorExpression selector= new();
        position++;
        if(tokens[position++].Type== TokenType.TWOPOINT&& tokens[position++].Type== TokenType.LCURLY)
        while(position< tokens.Count)
        {
            switch (tokens[position].Type)
            {
                case TokenType.SOURCE:
                    selector.Source = ParseAssignment(true);//No Implementado
                    break;
                case TokenType.SINGLE:
                    selector.Single= ParseAssignment(true);
                    break;
                case TokenType.PREDICATE:
                    selector.Predicate = ParsePredicate();
                    break;
                case TokenType.RCURLY:
                    if(tokens[++position].Type==TokenType.COMA|| tokens[position].Type==TokenType.POINTCOMA)
                    {
                        position++;
                        return selector;
                    }
                    else
                    throw new Exception($"{position} Invalid Token at {tokens[position].lugar.fila} row and {tokens[position].lugar.colmna} column expected in ");
                default:
                    throw new Exception($"{position} Invalid Token at {tokens[position].lugar.fila} row and {tokens[position].lugar.colmna} column expected selector item");
            }
        }
        return selector;
    }

    #endregion

    #region Parsing Effects and associated 
    private EffectDeclarationExpr ParseEffectDeclaration()
    {
        EffectDeclarationExpr effect= new();
        Token token = tokens[position];
        while(position< tokens.Count)
        {
            switch (token.Type)
            {
                case TokenType.NAME:
                    effect.Name = ParseAssignment(true);
                    token= tokens[position];
                    break;
                case TokenType.PARAMS:
                    effect.Params = ParseParams(false);
                    token= tokens[position];
                    break;
                case TokenType.ACTION:
                effect.Action = ParseAction();
                token= tokens[position];
                break;
                case TokenType.RCURLY:
                break;
                default:
                    throw new Exception($"{position} Invalid Token at {token.lugar.fila} row and {token.lugar.colmna} column expected card item");
            }
            if(token.Type== TokenType.RCURLY)
            {
                position++;
                break;
            }
        }
        return effect;
    }
    private List<Expression> ParseParams(bool efectito=true)
    {//false means a real parameter statement, true is used also on Selector and other similar statements
        List<Expression> parameters = new();
        Token token = tokens[++position];
        if(tokens[position++].Type== TokenType.TWOPOINT&& tokens[position++].Type== TokenType.LCURLY)
        while(true)
        {
            token= tokens[position];
            if(token.Type==TokenType.ID)
            {
                parameters.Add(ParseAssignment(efectito));
                token = tokens[position];
            }
            else if(token.Type==TokenType.NAME&& efectito)
            {
                parameters.Add(ParseAssignment(efectito));
            }
            else if(tokens[position++].Type== TokenType.RCURLY)
            {
                if(tokens[position++].Type== TokenType.POINTCOMA|| tokens[position-1].Type== TokenType.COMA)
                break;
            }
            else
            throw new Exception($"{position} Invalid Token at {token.lugar.fila} row and {token.lugar.colmna} column expected in Params definition");
        }
        else
        throw new Exception($"{position} Invalid Token at {token.lugar.fila} row and {token.lugar.colmna} column expected in ");
        return parameters;
    } 
    private ActionExpression ParseAction()
    {
        ActionExpression Action = new();
        position++;
        if( tokens[position++].Type== TokenType.TWOPOINT )
        {//Action initial sintaxis
            if(tokens[position++].Type== TokenType.LPAREN && tokens[position++].Type == TokenType.ID)
                Action.Targets= new IdentifierExpression(tokens[position-1]);
                if(tokens[position++].Type == TokenType.COMA && tokens[position++].Type == TokenType.ID)
                Action.Context = new IdentifierExpression(tokens[position-1]);
                if(tokens[position++].Type== TokenType.RPAREN && tokens[position++].Type== TokenType.ARROW
                && tokens[position++].Type== TokenType.LCURLY)
                {
                    Action.Instructions= ParseInstructionBlock();
                }
        }
        else
        throw new Exception($"{position} Invalid Token at {tokens[position].lugar.fila} row and {tokens[position].lugar.colmna} on an Action declaration statement");
        return Action;
    }
    private InstructionBlock ParseInstructionBlock(bool single= false)
    {//No debuggeado problemas a la hora de parsear Id, diferenciacion entre parseo de asignacion y uso de id para llamar un metodo o usar una propiedad
        InstructionBlock block = new();
        do
        {
            if(tokens[position].Type==TokenType.ID)
            {
                block.Instructions.Add(ParseAssignment(true, false));
            }
            else if(tokens[position].Type==TokenType.FOR)
            {
                block.Instructions.Add(ParseFor());
            }
            else if(tokens[position].Type==TokenType.WHILE)
            {
                block.Instructions.Add(ParseWhile());
            }
            else if(tokens[position++].Type== TokenType.RCURLY)
            {
                break;
            }
            else
            throw new Exception($"{position} Invalid Token at {tokens[position].lugar.fila} row and {tokens[position].lugar.colmna} column expected in Instruction Block definition");
        }while(true && !single);
        return block;
    }

    private ForExpression ParseFor()
    {//No debbugeado
        ForExpression ForExp = new();
        position++;
        if( tokens[position++].Type== TokenType.ID )
        {//ForExp initial sintaxis
            ForExp.Variable = new IdentifierExpression(tokens[position-1]);
            if(tokens[position++].Type == TokenType.IN && tokens[position++].Type== TokenType.ID)
            {
                ForExp.Collection= new IdentifierExpression(tokens[position-1]);
                if(tokens[position++].Type== TokenType.LCURLY)
                {
                    ForExp.Instructions=ParseInstructionBlock();
                    if(tokens[position].Type== TokenType.COMA||tokens[position].Type== TokenType.POINTCOMA)
                    {
                        position++;
                    }
                }
                else
                {
                    position--;
                    ForExp.Instructions= ParseInstructionBlock(true);
                }
            }
            else
            {
                throw new Exception($"{position} Invalid Token at {tokens[position-1].lugar.fila} row and {tokens[position-1].lugar.colmna} column expected in ");
            }
        }
        else
        throw new Exception($"{position} Invalid Token at {tokens[position].lugar.fila} row and {tokens[position].lugar.colmna} on a For declaration statement");
        return ForExp;
    }
    private WhileExpression ParseWhile()
    {//No debbugeado
        WhileExpression WhileExp = new();
        position++;
        if( tokens[position++].Type== TokenType.LPAREN)
        {//WhileExp initial sintaxis
            WhileExp.Condition = ParseExpression();
            if(tokens[position++].Type == TokenType.RPAREN)
            {
                if(tokens[position++].Type== TokenType.LCURLY)
                {
                    WhileExp.Instructions=ParseInstructionBlock();
                }
                else{
                    position--;
                    WhileExp.Instructions= ParseInstructionBlock(true);
                }
                    
            }
        }
        else
        throw new Exception($"{position} Invalid Token at {tokens[position].lugar.fila} row and {tokens[position].lugar.colmna} on an Action declaration statement");
        return WhileExp;
    }
    #endregion
}







