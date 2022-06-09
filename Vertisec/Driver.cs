using Vertisec;

public class Driver
{
    static void Main()
    {
        ReadFile file = new ReadFile(@"C:\Users\zach_\Desktop\sampleSQL.sql");
        string[] sqlLines = file.read();

        foreach (string sqlLine in sqlLines)
            Console.WriteLine(sqlLine);
    }
}