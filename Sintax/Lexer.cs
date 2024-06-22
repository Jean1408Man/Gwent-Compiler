using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
namespace Compiler;

public class Lexer {
    private string input;
    private List<Token> tokens;

    public Lexer(string input) {
        this.input = input;
        this.tokens = new List<Token>();
    }
    private Dictionary<TokenType, string> TokenPatterns = new Dictionary<TokenType, string>
    {
        // Keywords
        { TokenType.EFFECTDECLARATION, @"\beffect\b" },
        { TokenType.CARD, @"\bcard\b" },
        { TokenType.NAME, @"\bName\b" },
        { TokenType.PARAMS, @"\bParams\b" },
        { TokenType.ACTION, @"\bAction\b" },
        { TokenType.TYPE, @"\bType\b" },
        { TokenType.FACTION, @"\bFaction\b" },
        { TokenType.POWER, @"\bPower\b" },
        { TokenType.RANGE, @"\bRange\b" },
        { TokenType.ONACTIVATION, @"\bOnActivation\b" },
        { TokenType.EFFECTASSIGNMENT, @"\bEffect\b" },
        { TokenType.SELECTOR, @"\bSelector\b" },
        { TokenType.POSTACTION, @"\bPostAction\b" },
        { TokenType.SOURCE, @"\bSource\b" },
        { TokenType.SINGLE, @"\bSingle\b" },
        { TokenType.PREDICATE, @"\bPredicate\b" },
        { TokenType.FOR, @"\bfor\b" },
        { TokenType.IN, @"\bin\b" },
        { TokenType.WHILE, @"\bwhile\b" },
        { TokenType.HAND, @"\bhand\b" },
        { TokenType.DECK, @"\bdeck\b" },
        { TokenType.BOARD, @"\bboard\b" },
        { TokenType.TARGETS, @"\btargets\b" },
        { TokenType.CONTEXT, @"\bcontext\b" },
        { TokenType.TRUE, @"\btrue\b" },
        { TokenType.FALSE, @"\bfalse\b" },
        { TokenType.TRIGGERPLAYER, @"\bTriggerPlayer\b" },
        { TokenType.DECKOFPLAYER, @"\bDeckOfPlayer\b" },
        { TokenType.HANDOFPLAYER, @"\bHandOfPlayer\b" },
        { TokenType.GRAVEYARDOFPLAYER, @"\bBoardOfPlayer\b" },
        { TokenType.FIELDOFPLAYER, @"\bFieldOfPlayer\b" },
        { TokenType.FIND, @"\bFind\b" },
        { TokenType.PUSH, @"\bPush\b" },
        { TokenType.SENDBOTTOM, @"\bSendBottom\b" },
        { TokenType.POP, @"\bPop\b" },
        { TokenType.REMOVE, @"\bRemove\b" },
        { TokenType.SHUFFLE, @"\bShuffle\b" },
        { TokenType.OWNER, @"\bOwner\b" },
        
        // Data Types
        { TokenType.NUMBERTYPE, @"\bNumber\b" },
        { TokenType.STRINGTYPE, @"\bString\b" },

        // Symbols
        { TokenType.LPAREN, @"\(" },
        { TokenType.RPAREN, @"\)" },
        { TokenType.LCURLY, @"\{" },
        { TokenType.RCURLY, @"\}" },
        { TokenType.LBRACKET, @"\[" },
        { TokenType.RBRACKET, @"\]" },
        { TokenType.TWOPOINT, @"\:" },
        { TokenType.COMA, @"\," },
        { TokenType.POINTCOMA, @"\;" },
        { TokenType.POINT, @"\." },
        { TokenType.ARROW, @"\=\>" },
        { TokenType.ASSIGN, @"\=" },
        
        //MathOperator
        { TokenType.PLUSACCUM, @"\+\=" },
        { TokenType.MINUSACCUM, @"\-\=" },
        { TokenType.INCREEMENT, @"\+\+" },
        { TokenType.DECREMENT, @"\-\-" },
        { TokenType.PLUS, @"\+" },
        { TokenType.MINUS, @"\-" },
        { TokenType.MULTIPLY, @"\*" },
        { TokenType.DIVIDE, @"\/" },
        { TokenType.POW, @"\^" },
        

        // Identifiers
        { TokenType.ID, @"[a-zA-Z_][\w]*" },

        // Numbers
        { TokenType.INT, @"\b\d+\b" },

        // Strings (double-quoted)
        { TokenType.STRING, @"""[^""]*""" },
        { TokenType.SPACE_CONCATENATION, @"@@" },
        { TokenType.CONCATENATION, @"@" },

        // Booleans
        { TokenType.NOT, @"!" },
        { TokenType.AND, @"&&" },
        { TokenType.OR, @"\|\|"},
        { TokenType.EQUAL, @"==" },
        { TokenType.NOTEQUAL, @"!=" },
        { TokenType.LESS, @"<" }, 
        { TokenType.MORE, @">" },
        { TokenType.LESS_EQ, @"<=" },
        { TokenType.MORE_EQ, @">=" },

        // Whitespace
        { TokenType.WHITESPACE, "[ \t\r\n]" },
        { TokenType.LINECHANGE, @"\r" },

        // Comments
        { TokenType.SINGLECOMMENT, @"//[^\n]*" },
        { TokenType.MULTICOMMENT, @"/\*.*?\*/" }
    };

    public List<Token> Tokenize() 
    {
        int fila=0;
        int columna =0;
        while (input.Length!=0) 
        {
                bool isfound = false;
                foreach (TokenType type in Enum.GetValues(typeof(TokenType))){
                    
                    string pattern = TokenPatterns[type];
                    Match match = Regex.Match(input,"^"+ pattern); // Cambio aquí
                    if(match==null)
                    {

                    }
                    if (match!.Success)
                    {
                        if(type!= TokenType.WHITESPACE && type!= TokenType.LINECHANGE){
                        Token token = new Token(type, match.Value, (fila,columna));
                        tokens.Add(token);
                        }
                        if(type== TokenType.LINECHANGE)
                        {
                            fila++;
                            columna=0;
                        }
                        input= input.Substring(match.Value.Length); // Actualizo la posición
                        columna+= match.Value.Length;
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
public enum TokenType
{
    WHITESPACE,
    SINGLECOMMENT,
    MULTICOMMENT,
    INCREEMENT,
    DECREMENT,
    PLUSACCUM,
    MINUSACCUM,
    LINECHANGE,
    EFFECTDECLARATION,
    SPACE_CONCATENATION,
    CONCATENATION,
    CARD,
    NAME,
    PARAMS,
    ACTION,
    TYPE,
    FACTION,
    POWER,
    RANGE,
    ONACTIVATION,
    EFFECTASSIGNMENT,
    SELECTOR,
    POSTACTION,
    SOURCE,
    SINGLE,
    PREDICATE,
    FOR,
    IN,
    WHILE,
    HAND,
    OWNER,
    DECK,
    HANDOFPLAYER,
    DECKOFPLAYER,
    FIELDOFPLAYER,
    GRAVEYARDOFPLAYER,
    BOARD,
    CONTEXT,
    TARGETS,
    TRUE,
    FALSE,
    TRIGGERPLAYER,
    FIND,
    PUSH,
    SENDBOTTOM,
    POP,
    REMOVE,
    SHUFFLE,
    NUMBERTYPE,
    STRINGTYPE,
    LPAREN,
    RPAREN,
    LCURLY,
    RCURLY,
    LBRACKET,
    RBRACKET,
    POINT,
    TWOPOINT,
    COMA,
    POINTCOMA,
    ARROW,
    
    PLUS,
    POW,
    MINUS,
    MULTIPLY,
    DIVIDE,
    
    INT,
    STRING,
    AND,
    OR,
    LESS_EQ,
    MORE_EQ,
    EQUAL,
    ASSIGN,
    NOT,
    NOTEQUAL,
    LESS,
    MORE,
    ID,
}