namespace Compiler
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                string filePath = @"Input.txt";
                string text = File.ReadAllText(filePath);
                Console.WriteLine(text);
                Lexer l = new Lexer(text);
                List<Token> tokens = l.Tokenize();
                foreach (Token t in tokens)
                {
                    Console.WriteLine(t.Type + ": " + t.Value);
                }
            } catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }
    
    }
}
