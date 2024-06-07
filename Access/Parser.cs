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
    public bool Test= false|| true;
    public Parser(List<Token> tokens)
    {
        position = 0;
        this.tokens = tokens;
    }
    private bool LookAhead(TokenType expectedTokenType)
    {
        if (position < tokens.Count)
        {
            return tokens[position].Type == expectedTokenType;
        }
        return false;
    }
    public Expression Parse()
    {
        var expression = ParseExpression();
        return expression;
    }
    public Expression ParseExpression(int parentprecedence =0)
    {
        var left = ParsePrimaryExpression();

        while (true)
        {
            var precedence = GetPrecedence(tokens[position].Type);
            if(precedence==0|| precedence<= parentprecedence) 
            break;
            
            var operatortoken = tokens[position++].Type;
            var right = ParseExpression(precedence);
            left = new BinaryOperator(left, right, operatortoken);
        }
        return left;
    }
    public Expression ParsePrimaryExpression()
    {
        if (position >= tokens.Count) throw new Exception("Unexpected end of input");
        if (tokens[position].Type == TokenType.LPAREN)
        {
            position++;
            Expression expr = ParseExpression(); // Asumiendo que ParseNumericalExpression maneja correctamente las expresiones entre paréntesis
            if (tokens[position].Type!= TokenType.RPAREN)
            {
                throw new Exception("Missing closing parenthesis");
            }
            position++;
            return expr;
        }
        // Comprueba si el siguiente token es un identificador
        else if (tokens[position].Type == TokenType.ID)
        {
            position++; // Avanzamos al siguiente token después del identificador
            // Creamos una nueva expresión de identificador usando el token actual
            return new IdentifierExpression(tokens[position - 1]);
        }
        else if (tokens[position].Type == TokenType.INT)
        {
            position++;
            return new Number(tokens[position - 1].Value);
        }
        else if (tokens[position].Type == TokenType.MINUS && (position == 0 || (tokens[position - 1].Type != TokenType.INT&& tokens[position - 1].Type != TokenType.ID)))
        {
            // Lógica existente para manejar el caso de un operador unario 
            position++;
            Expression operand = ParsePrimaryExpression();
            return new BinaryOperator(new Number("0"), operand, tokens[position - 2].Type);
        }
        throw new Exception("Not recognizable primary token");
    }
    
    public int GetPrecedence(TokenType type)
    {
        switch (type)
        {
            case TokenType.PLUS:
            case TokenType.MINUS:
            return 1;
            case TokenType.MULTIPLY:
            case TokenType.DIVIDE:
            return 2;
            case TokenType.POW:
            return 3;            
            default:
            return 0;
        }
    }
    
}







