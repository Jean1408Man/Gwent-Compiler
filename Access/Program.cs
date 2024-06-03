namespace Compiler
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                string filePath = @"Access/Input.txt";
                string text = File.ReadAllText(filePath);
                text += "\n";
                Console.WriteLine(text);
                Lexer l = new Lexer(text);
                List<Token> tokens = l.Tokenize();
                Parser parser= new(tokens);

                foreach (Token t in tokens)
                {
                    Console.WriteLine(t.Type + ": " + t.Value);
                }
                Console.WriteLine(parser.FindEndOfNumericExpression());
            } catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }
    
    }
}
