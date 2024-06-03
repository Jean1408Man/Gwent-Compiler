using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Compiler
{
    public class Parser
    {
        int position;
        List<Token> tokens;
        
        // public Dictionary<TokenType, Expression> Gramatic= new Dictionary<TokenType, Expression>
        // {
        //     //Parseo de numero
        //     {TokenType.NUMBER, NUMBER()},
        //     //Luego de un id debe estar:

        // };
        
        public Parser(List<Token> tokens)
        {
            position = 0;
            this.tokens = tokens;
        }
        
        #region Numerical Parser
        // private Expression NUMBER()
        // {
        //      int limit= FindEndOfNumericExpression();
        // }
        
        public int FindEndOfNumericExpression()
        {
            int pos = position;
            int level = 0;

            while (pos < tokens.Count)
            {
                Token token = tokens[pos];

                if (token.Type == TokenType.NUMBER|| token.Type == TokenType.ID && tokens[pos-1].Type!=  TokenType.NUMBER && tokens[pos-1].Type!=  TokenType.ID)
                {
                    pos++;
                    continue;
                }

                if (token.Type == TokenType.LPAREN)
                {
                    level++;
                    pos++;
                    continue;
                }

                if (token.Type == TokenType.RPAREN)
                {
                    if (level == 0)
                        break;

                    level--;
                    pos++;
                    continue;
                    
                }

                if (level > 0)
                {
                    pos++;
                    continue;
                }
                    if ((token.Type == TokenType.PLUS ||
                        token.Type == TokenType.MINUS ||
                        token.Type == TokenType.MULTIPLY ||
                        token.Type == TokenType.DIVIDE ||
                        token.Type == TokenType.POW)
                        &&
                        (tokens[pos-1].Type != TokenType.PLUS ||
                        tokens[pos-1].Type != TokenType.MINUS||
                        tokens[pos-1].Type != TokenType.MULTIPLY||
                        tokens[pos-1].Type != TokenType.DIVIDE||
                        tokens[pos-1].Type != TokenType.POW)
                        )
                    {
                        pos++;
                        continue;
                    }
                    break;
                }

                return pos - position;
            }

            
        }
        #endregion




    }
