using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
namespace LogicalSide;

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
        { TokenType.Name, @"\bName\b" },
        { TokenType.Params, @"\bParams\b" },
        { TokenType.ACTION, @"\bAction\b" },
        { TokenType.Type, @"\bType\b" },
        { TokenType.Faction, @"\bFaction\b" },
        { TokenType.Power, @"\bPower\b" },
        { TokenType.Range, @"\bRange\b" },
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
        { TokenType.Hand, @"\bHand\b" },
        { TokenType.Board, @"\bBoard\b" },
        { TokenType.TRUE, @"\btrue\b" },
        { TokenType.FALSE, @"\bfalse\b" },
        { TokenType.TriggerPlayer, @"\bTriggerPlayer\b" },
        { TokenType.DeckOfPlayer, @"\bDeckOfPlayer\b" },
        { TokenType.Deck, @"\bDeck\b" },
        { TokenType.HandOfPlayer, @"\bHandOfPlayer\b" },
        { TokenType.Add, @"\bAdd\b" },
        { TokenType.GraveYardOfPlayer, @"\bGraveYardOfPlayer\b"},
        { TokenType.GraveYard, @"\bGraveYard\b"},
        { TokenType.FieldOfPlayer, @"\bFieldOfPlayer\b" },
        { TokenType.Field, @"\bField\b" },
        { TokenType.Find, @"\bFind\b" },
        { TokenType.Push, @"\bPush\b" },
        { TokenType.SendBottom, @"\bSendBottom\b" },
        { TokenType.Pop, @"\bPop\b" },
        { TokenType.Remove, @"\bRemove\b" },
        { TokenType.Shuffle, @"\bShuffle\b" },
        { TokenType.Owner, @"\bOwner\b" },
        
        // Data Types
        { TokenType.NUMBERTYPE, @"\bNumber\b" },
        { TokenType.STRINGTYPE, @"\bString\b" },
        { TokenType.BOOLEAN, @"\bBool\b" },

        // Symbols
        { TokenType.LPAREN, @"\(" },
        { TokenType.RPAREN, @"\)" },
        { TokenType.LCURLY, @"\{" },
        { TokenType.RCURLY, @"\}" },
        { TokenType.LBRACKET, @"\[" },
        { TokenType.INDEXER, @"\[" },
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
        { TokenType.INCREMENT, @"\+\+" },
        { TokenType.RINCREMENT, @"\+\+" },
        { TokenType.LINCREMENT, @"\+\+" },
        { TokenType.DECREMENT, @"\-\-" },
        { TokenType.RDECREMENT, @"\-\-" },
        { TokenType.LDECREMENT, @"\-\-" },
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
        { TokenType.END_OF_FILE, "\\\"" },
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
        { TokenType.LINECHANGE, @"\r" },
        { TokenType.WHITESPACE, "[ \t\r\n]" },
        

        // Comments
        { TokenType.SINGLECOMMENT, @"//[^\n]*" },
        { TokenType.MULTICOMMENT, @"/\*.*?\*/" }
    };

    public List<Token> Tokenize() 
    {
        SintaxFacts.CompilerPhase= "Lexer";
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
    LINECHANGE,
    WHITESPACE,
    SINGLECOMMENT,
    MULTICOMMENT,
    INCREMENT,
    RINCREMENT,
    LINCREMENT,
    DECREMENT,
    RDECREMENT,
    LDECREMENT,
    PLUSACCUM,
    MINUSACCUM,
    EFFECTDECLARATION,
    SPACE_CONCATENATION,
    CONCATENATION,
    CARD,
    Add,
    Name,
    Params,
    ACTION,
    Type,
    Faction,
    Power,
    Range,
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
    HandOfPlayer,
    Hand,
    Owner,
    DeckOfPlayer,
    Deck,
    FieldOfPlayer,
    Field,
    GraveYardOfPlayer,
    GraveYard,
    Board,
    TRUE,
    FALSE,
    TriggerPlayer,
    Find,
    Push,
    SendBottom,
    Pop,
    Remove,
    Shuffle,
    NUMBERTYPE,
    BOOLEAN,
    STRINGTYPE,
    LPAREN,
    RPAREN,
    LCURLY,
    RCURLY,
    LBRACKET,
    INDEXER,
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
    END_OF_FILE,
    ID,
}