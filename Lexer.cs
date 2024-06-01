using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
namespace Compiler
{
    

public class Lexer {
    private string input;
    private List<Token> tokens;

    public Lexer(string input) {
        this.input = input;
        this.tokens = new List<Token>();
    }

    public List<Token> Tokenize() 
    {
        while (input.Length!=0) 
        {
                bool isfound = false;
                foreach (TokenType type in Enum.GetValues(typeof(TokenType))) {
                    string pattern = type.GetPattern();
                    Match match = Regex.Match(input,"^"+ pattern); // Cambio aquí
                    if (match.Success) 
                    {
                        if(type!= TokenType.WHITESPACE){
                        Token token = new Token(type, match.Value);
                        tokens.Add(token);
                        }
                        input= input.Substring(match.Value.Length); // Actualizo la posición
                        isfound = true;
                        break;
                    }
                }   
                if (!isfound){
                    break;
                }
        }
        return tokens;
    }

}

public enum TokenType {
    WHITESPACE,
    SINGLECOMMENT,
    MULTICOMMENT,
    POINT,
    TWOPOINT,
    COMA,
    POINTCOMA,
    ARROW,
    NAME,
    PARAMS,
    NUMBER,
    ACTION,
    CONTEXT,
    POWER,
    DECK,
    HAND,
    POP,
    SHUFFLE,
    
    FOR,
    WHILE,
    EFFECTDECLARATION,
    EFFECTASSIGNMENT,
    CARD,
    IF,
    ELIF,
    ELSE,
    POW,
    INCREMENT,
    DECREMENT,
    PLUS,
    MINUS,
    MULTIPLY,
    DIVIDE,
    AND,
    OR,
    LESS,
    MORE,
    EQUAL,
    LESS_EQ,
    MORE_EQ,
    SPACE_CONCATENATION,
    CONCATENATION,
    ASSIGN,
    LPAREN,
    RPAREN,
    LBRACKET,
    RBRACKET,
    LCURLY,
    RCURLY,
    BOOLEAN,
    INT,
    STRING,
    ID,
    NOT, 
    NOTEQUAL, 
}

public static class TokenTypeExtensions {
    public static string GetPattern(this TokenType type) {
        switch (type) {
            case TokenType.WHITESPACE:
                return @"\s+";
            case TokenType.SINGLECOMMENT:
                return @"\/\/.*";
            case TokenType.MULTICOMMENT:
                return @"(?s)/\*.*?\*/";
            case TokenType.POINT:
                return @"\.";
            case TokenType.POINTCOMA:
                return @"\;";
            case TokenType.COMA:
                return @"\,";
            case TokenType.ARROW:
                return @"=>";
            case TokenType.NAME:
                return @"\b[a-zA-Z_][a-zA-Z_0-9]*\b";
            case TokenType.PARAMS:
                return @"\(([^)]*)\)";
            case TokenType.NUMBER:
                return @"\b\d+(\.\d+)?\b";
            case TokenType.ACTION:
                return @"\b[action|draw|discard|play]\b";
            case TokenType.CONTEXT:
                return @"\bcontext\b";
            case TokenType.POWER:
                return @"\bPower\b";
            case TokenType.DECK:
                return @"\bDeck\b";
            case TokenType.HAND:
                return @"\bHand\b";
            case TokenType.POP:
                return @"\bPop\b";
            case TokenType.SHUFFLE:
                return @"\bShuffle\b";
            
            case TokenType.FOR:
                return @"\bfor\b";
            case TokenType.WHILE:
                return @"\bwhile\b";
            case TokenType.EFFECTDECLARATION:
                return @"\beffect\b";
                case TokenType.EFFECTASSIGNMENT:
                return @"\bEffect\b";
            case TokenType.CARD:
                return @"\bcard\b";
            case TokenType.IF:
                return @"\bif\b";
            case TokenType.ELIF:
                return @"\belif\b";
            case TokenType.ELSE:
                return @"\belse\b";
            case TokenType.POW:
                return @"\^";
            case TokenType.INCREMENT:
                return @"\+\+";
            case TokenType.DECREMENT:
                return @"\-\-";
            case TokenType.MULTIPLY:
                return @"\*";
            case TokenType.DIVIDE:
                return @"\/";
            case TokenType.PLUS:
                return @"\+";
            case TokenType.MINUS:
                return @"\-";
            case TokenType.AND:
                return @"\&\&";
            case TokenType.OR:
                return @"\|\|";
            
            case TokenType.LESS:
                return "<";
            case TokenType.MORE:
                return ">";
            case TokenType.EQUAL:
                return "==";
            case TokenType.LESS_EQ:
                return "<=";
            case TokenType.MORE_EQ:
                return ">=";
            case TokenType.SPACE_CONCATENATION:
                return "@@";
            case TokenType.CONCATENATION:
                return "@";
            case TokenType.ASSIGN:
                return "=";
            case TokenType.LPAREN:
                return @"\(";
            case TokenType.RPAREN:
                return @"\)";
            case TokenType.LBRACKET:
                return @"\[";
            case TokenType.RBRACKET:
                return @"\]";
            case TokenType.LCURLY:
                return @"\{";
            case TokenType.RCURLY:
                return @"\}";
            case TokenType.BOOLEAN:
                return @"\b(true|false)\b";
            case TokenType.INT:
                return @"\b\d+\b";
            case TokenType.STRING:
                return "\".*?\"";
            case TokenType.ID:
                return @"\b[A-Za-z_][A-Za-z_0-9]*\b";
            case TokenType.TWOPOINT:
                return @"\:";
            case TokenType.NOT:
                return @"\!";
            case TokenType.NOTEQUAL:
                return @"!=";
            default:
                throw new ArgumentException("Invalid token type");
        }
    }
}
}