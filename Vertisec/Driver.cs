using Vertisec.FileIO;
using Vertisec.Tokens;

public class Driver
{
    static void Main()
    {
        ReadFile file = new ReadFile(@"C:\Users\zach_\Desktop\sampleSQL.sql");
        string[] sqlLines = file.read();
        List<Token> tokens = Tokenizer.Tokenize(sqlLines);

        foreach (Token token in tokens)
            Console.WriteLine(token.GetText() + " : " + token.GetLineNumber());
    }
}