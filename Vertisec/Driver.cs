using Vertisec.FileIO;
using Vertisec.Tokens;
using Vertisec;
public class Driver
{
    static void Main(String[] args)
    {
        // TODO: turn this file path into a command line argument
        string filepath = args[0];
        Vertisec.Vertisec vertisec = new Vertisec.Vertisec(filepath);
        vertisec.BuildClauses();
    }
}