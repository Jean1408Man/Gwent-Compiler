namespace Compiler
{
    public class Token 
    {
        public TokenType Type { get; set;}
        public string Value { get; }
        public (int fila, int colmna) lugar;
        public Token(TokenType type, string value, (int fila, int columna)pos) {
            Type = type;
            Value = value;
            lugar = pos;
        }
        public override string ToString()
        {
            return "Type: \"" + Type.ToString() + "\" Value: \"" + Value+ "\" Position: \"" + lugar.fila +"\" row \""+ lugar.colmna +"\" column";
        }
    }
}