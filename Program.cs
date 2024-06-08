using Compiler;
using System;
using System.Collections.Generic;

namespace Compiler
{
    public class Program
    {

        public static void Main(string[] args)
        {
            try
            {
                string filePath = @"D:\Priv\Proyecto UH\2doProyecto\Gwent-Compiler\Input.txt";
                string text = File.ReadAllText(filePath);
                Lexer l = new Lexer(text);
                List<Token> tokens = l.Tokenize();
                Console.WriteLine(text);
                foreach (Token t in tokens)
                {
                    Console.WriteLine(t.Type.ToString()+ " in " + t.lugar.fila+" line "+ " and " + t.lugar.colmna+ " column ");
                }
                Parser parser = new(tokens);
                Expression root = parser.ParseExpression();
                Console.WriteLine(parser.position);
                PrintExpressionTree(root);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static void PrintExpressionTree(Expression node, int indentLevel = 0)
        {
            node.Print(indentLevel);
            if (node is BinaryOperator binaryNode)
            {
                PrintExpressionTree(binaryNode.Left, indentLevel + 1);
                PrintExpressionTree(binaryNode.Right, indentLevel + 1);
            }
            else if (node is Terminal numberNode)
            {
                Console.WriteLine(new string(' ', indentLevel * 4) + $"Value: {numberNode.ValueForPrint}");
            }
            else if(node is UnaryOperator unaryOperator)
            PrintExpressionTree(unaryOperator.Operand, indentLevel + 1);
        }
        public static void Tester()
        {
            string[] testCases = 
            {
                "1 + 2", // Suma simple
                "1 + 2 * 3", // Precedencia de operadores: suma vs. multiplicación
                "(1 + 2) * 3", // Uso de paréntesis para cambiar la precedencia
                "-7 + (147 / 2) + 7 ^ (8 + 7) - 3 + (5 - 9)", // Mixto de operaciones, incluido exponente y números negativos
                "7 ^ 2", // Exponente simple
                "7 * (8 + 7)", // Multiplicación y suma
                "7 + (8 + 7) ^ 2 - 3", // Combinación de operaciones con exponente
                "2 ^ 3 ^ 2", // Asociatividad a la derecha de la exponente
                "3 * 2 + 4 / 2", // División y multiplicación
                "(3 + 4) * 2", // Paréntesis para agrupar operaciones
                "10 - (5 + 3) * 2", // Número negativo y paréntesis
                "5 + 3", // Operador incorrecto para probar el manejo de errores
                "7 ^ (8 + 7) * 2", // Exponente y multiplicación
                "2 * 3 + 4 ^ 2", // Orden de operaciones mixtas
                "100 / 2 / 5 + 3", // División y suma
                "(100 / 2) / 5 + 3", // Paréntesis y división
                "7 ^ 2 ^ 3", // Exponente doble
                "2 ^ 3 ^ 2", // Exponente doble, asociatividad a la derecha
                "2 + 3 * 4 ^ 2", // Suma, multiplicación y exponente
                "7 * (8 + 7) - 3", // Multiplicación y resta
                "2 * (3 + 4) ^ 2", // Paréntesis y exponente
                "7 ^ 2 * 3", // Exponente y multiplicación
                "2 ^ (3 + 4) ^ 2", // Exponente doble con paréntesis
                "7 + (8 + 7) ^ 2 - 3", // Combinación completa
                "2 * 3 + 4 ^ 2", // Multiplicación, suma y exponente
                "(2 * 3) + 4 ^ 2", // Paréntesis y exponente
                "7 ^ 2 * 3", // Exponente y multiplicación
                "2 ^ (3 + 4) ^ 2", // Exponente doble con paréntesis
                "7 + (8 + 7) ^ 2 - 3", // Combinación completa con exponente
                "2 * 3 + 4 ^ 2", // Multiplicación, suma y exponente
                "(2 * 3) + 4 ^ 2", // Paréntesis y exponente
                "7 ^ 2 * 3", // Exponente y multiplicación
                "2 ^ (3 + 4) ^ 2", // Exponente doble con paréntesis
                "7 + (8 + 7) ^ 2 - 3", // Combinación completa con exponente
                "2 * 3 + 4 ^ 2", // Multiplicación, suma y exponente
                "(2 * 3) + 4 ^ 2", // Paréntesis y exponente
                "7 ^ 2 * 3", // Exponente y multiplicación
                "2 ^ (3 + 4) ^ 2", // Exponente doble con paréntesis
                "7 + (8 + 7) ^ 2 - 3", // Combinación completa con exponente
                "2 * 3 + 4 ^ 2", // Multiplicación, suma y exponente
                "(2 * 3) + 4 ^ 2", // Paréntesis y exponente
                "7 ^ 2 * 3", // Exponente y multiplicación
                "2 ^ (3 + 4) ^ 2", // Exponente doble con paréntesis
                "7 + (8 + 7) ^ 2 - 3", // Combinación completa con exponente
                "2 * 3 + 4 ^ 2", // Multiplicación, suma y exponente
                "(2 * 3) + 4 ^ 2", // Paréntesis y exponente
                "7 ^ 2 * 3", // Exponente y multiplicación
                "2 ^ (3 + 4) ^ 2", // Exponente doble con paréntesis
                "7 + (8 + 7) ^ 2 - 3", // Combinación completa con exponente
                "2 * 3 + 4 ^ 2", // Multiplicación, suma y exponente
                "(2 * 3) + 4 ^ 2", // Paréntesis y exponente
                "7 ^ 2 * 3", // Exponente y multiplicación
                "2 ^ (3 + 4) ^ 2", // Exponente doble con paréntesis
                "7 + (8 + 7) ^ 2 - 3", // Combinación completa con exponente
                "2 * 3 + 4 ^ 2", // Multiplicación, suma y exponente
                "(2 * 3) + 4 ^ 2", // Paréntesis y exponente
                "7 ^ 2 * 3", // Exponente y multiplicación
                "2 ^ (3 + 4) ^ 2", // Exponente doble con paréntesis
                "7 + (8 + 7) ^ 2 - 3", // Combinación completa con exponente
                "2 * 3 + 4 ^ 2", // Multiplicación, suma y exponente
                "(2 * 3) + 4 ^ 2", // Paréntesis y exponente
                "7 ^ 2 * 3", // Exponente y multiplicación
                "2 ^ (3 + 4) ^ 2", // Exponente doble con paréntesis
                "7 + (8 + 7) ^ 2 - 3", // Combinación completa con exponente
                "2 * 3 + 4 ^ 2", // Multiplicación, suma```
            };
            Parser parser;
            Lexer lexer;
            for(int i = 0; i< testCases.Length; i++)
            {
                lexer = new Lexer(testCases[i]);
                parser = new Parser(lexer.Tokenize());
                Console.WriteLine("Test Case " + i + ": " + testCases[i]);
                Expression root = parser.ParseExpression();
                PrintExpressionTree(root);
            }
        }
    }
}