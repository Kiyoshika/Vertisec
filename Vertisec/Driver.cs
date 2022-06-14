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
    }
}