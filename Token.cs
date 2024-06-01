namespace Compiler
{
    public class Token 
    {
        public TokenType Type { get; }
        public string Value { get; }
        public (int fila, int colmna) lugar;
        public Token(TokenType type, string value) {
            Type = type;
            Value = value;
        }
    }
}