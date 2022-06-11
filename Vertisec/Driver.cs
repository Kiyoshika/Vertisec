using Vertisec.FileIO;
using Vertisec.Tokens;
using Vertisec;
public class Driver
{
    static void Main()
    {
        // TODO: turn this file path into a command line argument
        string filepath = @"C:\Users\zach_\Desktop\sampleSQL.sql";
        Vertisec.Vertisec vertisec = new Vertisec.Vertisec(filepath);
        vertisec.BuildClauses();

        Console.WriteLine("SELECT CLAUSE TOKENS:");
        foreach (Token token in vertisec.clauses[0].GetTokens())
            Console.WriteLine(token.GetText() + " : " + token.GetLineNumber());

        Console.WriteLine("\n\n");

        Console.WriteLine("REMAINING TOKENS:");
        foreach (Token token in vertisec.GetTokens())
            Console.WriteLine(token.GetText() + " : " + token.GetLineNumber());
    }
}