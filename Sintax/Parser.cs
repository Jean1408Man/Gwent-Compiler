using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
    private Expression ParsePrimaryExpression()
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
        {
            Token token= tokens[position];
            if(tokens[++position].Type== TokenType.LPAREN && tokens[++position].Type== TokenType.RPAREN)
            {
                return new UnaryOperator(null, token.Type);
            }
        }
        else if (tokens[position].Type == TokenType.HAND||tokens[position].Type == TokenType.PUSH ||
                tokens[position].Type == TokenType.SENDBOTTOM||tokens[position].Type == TokenType.REMOVE
                ||tokens[position].Type == TokenType.HANDOFPLAYER||tokens[position].Type == TokenType.DECKOFPLAYER
                ||tokens[position].Type == TokenType.FIELDOFPLAYER||tokens[position].Type == TokenType.GRAVEYARDOFPLAYER)
                {
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
        throw new Exception("Not recognizable primary token");
    }
    #endregion
    private Expression ParseAssignment(bool expectedAssign)
    {//true means it expects value, false means it expects ValueType
        Expression left= new IdentifierExpression(tokens[position++]);
        Token token= tokens[position++];
        if(expectedAssign)
        {
            if (token.Type == TokenType.ASSIGN|| token.Type == TokenType.TWOPOINT)//Agregar formas como incremento etc...
            {
                Expression right = ParseExpression();
                if(tokens[position].Type==TokenType.COMA || tokens[position].Type==TokenType.POINTCOMA)
                {
                    position++;
                    return new BinaryOperator(left, right,token.Type);
                }
                throw new Exception($"Unexpected assign token at {token.lugar.fila} file and {token.lugar.colmna} column({token.Type}), expected Comma or Semicolon");
            }//NADA DE ESTO ESTÁ HECHO
            else if(token.Type == TokenType.INCREEMENT|| token.Type == TokenType.DECREMENT)
            {
                Expression right = new UnaryOperator(ParseExpression(), token.Type);
                if(tokens[position].Type==TokenType.COMA || tokens[position].Type==TokenType.POINTCOMA)
                {
                    position++;
                    return new BinaryOperator(left, right,token.Type);
                }
                throw new Exception($"Unexpected assign token at {token.lugar.fila} file and {token.lugar.colmna} column({token.Type}), expected Comma or Semicolon");
            }
            else if(token.Type == TokenType.PLUSACCUM|| token.Type == TokenType.MINUSACCUM)
            {
                Expression right = new UnaryOperator(ParseExpression(), token.Type);
                if(tokens[position].Type==TokenType.COMA || tokens[position].Type==TokenType.POINTCOMA)
                {
                    position++;
                    return new BinaryOperator(left, right,token.Type);
                }
                throw new Exception($"Unexpected assign token at {token.lugar.fila} file and {token.lugar.colmna} column({token.Type}), expected Comma or Semicolon");
            }
        }
        else 
        {
            if (token.Type == TokenType.ASSIGN|| token.Type == TokenType.TWOPOINT)//Agregar formas como incremento etc...
            {
                token = tokens[position];
                if(tokens[position].Type==TokenType.NUMBERTYPE || tokens[position].Type==TokenType.STRINGTYPE)
                    {
                        
                        Expression right = new IdentifierExpression(tokens[position]);
                        position++;
                        return new BinaryOperator(left, right,token.Type);
                    }
            }
        }
        throw new Exception($"Unexpected assign token at {token.lugar.fila} file and {token.lugar.colmna} column({token.Type})");
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
                        program.Effects.Add(ParseEffect() as EffectDeclarationExpr);
                    }
                    else if(token.Type== TokenType.CARD)
                    {
                        program.Cards.Add(ParseCard() as CardExpression);
                    }
                }
                else
                throw new Exception($"Invalid token {tokens[position-1]}, expected Left Curly");
            }
            else
            throw new Exception($"Invalid Token at {token.lugar.fila} row and {token.lugar.colmna} column expected Effect or Card ");
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
                        throw new Exception($"Name has been declared already");
                    card.Name = ParseAssignment(true);
                    token= tokens[position];
                    break;
                case TokenType.TYPE:
                    if(card.Type!=null)
                        throw new Exception($"Type has been declared already");
                    card.Type = ParseAssignment(true);
                    token= tokens[position];
                    break;
                case TokenType.RANGE:
                    if(card.Range!=null)
                        throw new Exception($"Range has been declared already");
                    card.Range = ParseRanges();// Manejo para TokenType.RANGE
                    if(tokens[position].Type== TokenType.COMA)
                    position++;
                    else
                    throw new Exception($"Invalid Token at {tokens[position].lugar.fila} row and {tokens[position].lugar.colmna} column expected Comma in range end definition");
                    token= tokens[position];
                    break;
                case TokenType.POWER:
                    if(card.Power!=null)
                        throw new Exception($"Power has been declared already");
                    card.Power = ParseAssignment(true);// Manejo para TokenType.POWER
                    token= tokens[position];
                    break;
                case TokenType.FACTION:
                    if(card.Faction!=null)
                        throw new Exception($"Faction has been declared already");
                    card.Faction = ParseAssignment(true);
                    token= tokens[position];
                    break;
                case TokenType.RCURLY:
                        position++;
                        return card;
                default:
                    throw new Exception($"Invalid Token at {token.lugar.fila} row and {token.lugar.colmna} column expected card item");
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
                        throw new Exception($"Invalid Token at {tokens[position].lugar.fila} row and {tokens[position].lugar.colmna} column expected Comma");
                }
                else
                    throw new Exception($"Invalid Token at {tokens[position].lugar.fila} row and {tokens[position].lugar.colmna} column expected in ");
            }
            return ranges;
        }
        else
            throw new Exception($"Invalid Token at {tokens[position].lugar.fila} row and {tokens[position].lugar.colmna} column ");
    }
 #endregion

    #region Parsing Effects and associated 
    private EffectDeclarationExpr ParseEffect()
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
                    effect.Params = ParseParams();
                    token= tokens[position];
                    break;
                case TokenType.ACTION:
                effect.Action = ParseAction();// Manejo para TokenType.ACTION
                token= tokens[position];
                break;
                case TokenType.RCURLY:
                break;
                default:
                    throw new Exception($"Invalid Token at {token.lugar.fila} row and {token.lugar.colmna} column expected card item");
            }
            if(tokens[position].Type== TokenType.RCURLY)
            {
                position++;
                break;
            }
        }
        return effect;
    }
    private List<Expression> ParseParams()
    {
        List<Expression> parameters = new();
        Token token = tokens[++position];
        if(tokens[position++].Type== TokenType.TWOPOINT&& tokens[position++].Type== TokenType.LCURLY)
        while(true)
        {
            token= tokens[position];
            if(token.Type==TokenType.ID)
            {
                parameters.Add(ParseAssignment(false));
                token = tokens[position];
                if(token.Type== TokenType.COMA)
                    position++;
                else
                throw new Exception($"Invalid Token at {token.lugar.fila} row and {token.lugar.colmna} column expected Comma in Params body end of assignment");
            }
            else if(tokens[position++].Type== TokenType.RCURLY)
            {
                if(tokens[position++].Type== TokenType.POINTCOMA)
                break;
            }
            else
            throw new Exception($"Invalid Token at {token.lugar.fila} row and {token.lugar.colmna} column expected in Params definition");
        }
        else
        throw new Exception($"Invalid Token at {token.lugar.fila} row and {token.lugar.colmna} column expected in ");
        return parameters;
    } 
    private ActionExpression ParseAction()
    {
        ActionExpression Action = new();
        position++;
        if( tokens[position++].Type== TokenType.TWOPOINT )
        {//Action initial sintaxis
            if(tokens[position++].Type== TokenType.LPAREN && tokens[position++].Type == TokenType.TARGETS 
            && tokens[position++].Type == TokenType.COMA && tokens[position++].Type == TokenType.CONTEXT 
            && tokens[position++].Type== TokenType.RPAREN && tokens[position++].Type== TokenType.ARROW
            && tokens[position++].Type== TokenType.LCURLY)
            {
                Action.Instructions= ParseInstructionBlock();
            }
        }
        else
        throw new Exception($"Invalid Token at {tokens[position].lugar.fila} row and {tokens[position].lugar.colmna} on an Action declaration statement");
        return Action;
    }
    private InstructionBlock ParseInstructionBlock()
    {//No debuggeado problemas a la hora de parsear Id, diferenciacion entre parseo de asignacion y uso de id para llamar un metodo o usar una propiedad
        InstructionBlock block = new();
        while(true)
        {
            if(tokens[position].Type==TokenType.ID)
            {
                block.Instruccions.Add(ParseExpression());
                if(tokens[position].Type== TokenType.COMA)
                    position++;
                else
                throw new Exception($"Invalid Token at {tokens[position].lugar.fila} row and {tokens[position].lugar.colmna} column expected Comma in Params body end of assignment");
            }
            else if(tokens[position].Type==TokenType.FOR)
            {
                block.Instruccions.Add(ParseFor());
            }
            else if(tokens[position].Type==TokenType.WHILE)
            {
                block.Instruccions.Add(ParseWhile());
            }
            else if(tokens[position++].Type== TokenType.RCURLY)
            {
                if(tokens[position++].Type== TokenType.POINTCOMA)
                break;
            }
            else
            throw new Exception($"Invalid Token at {tokens[position].lugar.fila} row and {tokens[position].lugar.colmna} column expected in Params definition");
        }
        return block;
    }

    private ForExpression ParseFor()
    {//No debbugeado
        ForExpression ForExp = new();
        position++;
        if( tokens[position++].Type== TokenType.ID )
        {//ForExp initial sintaxis
            ForExp.Variable = new IdentifierExpression(tokens[position-1]);
            if(tokens[position++].Type == TokenType.TARGETS|| tokens[position-1].Type== TokenType.ID)
            {
                ForExp.Collection= new IdentifierExpression(tokens[position-1]);
                if(tokens[position++].Type== TokenType.LCURLY)
                {
                    ForExp.Instructions=ParseInstructionBlock();
                    if(tokens[position].Type== TokenType.POINTCOMA)
                        position++;
                    else
                        throw new Exception($"Invalid Token at {tokens[position].lugar.fila} row and {tokens[position].lugar.colmna} column expected Semicolon in For end sintax");
                }
            }
        }
        else
        throw new Exception($"Invalid Token at {tokens[position].lugar.fila} row and {tokens[position].lugar.colmna} on an Action declaration statement");
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
                    if(tokens[position].Type== TokenType.POINTCOMA)
                        position++;
                    else
                        throw new Exception($"Invalid Token at {tokens[position].lugar.fila} row and {tokens[position].lugar.colmna} column expected Semicolon in For end sintax");
                }
            }
        }
        else
        throw new Exception($"Invalid Token at {tokens[position].lugar.fila} row and {tokens[position].lugar.colmna} on an Action declaration statement");
        return WhileExp;
    }
    #endregion
}







