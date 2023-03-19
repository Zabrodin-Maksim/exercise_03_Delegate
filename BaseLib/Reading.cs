namespace Fei.BaseLib;

public class Reading
{
    /*
     * For implementation use WriteLine and ReadLine instead of direct use of Console.WriteLine/Console.ReadLine
     * WriteLine(prompt);
     * ... = ReadLine();
     */
    public static Action<string?> WriteLine { get; set; } = Console.WriteLine;
    public static Func<string?> ReadLine { get; set; } = Console.ReadLine;

    private static class ReaderHelper<T>
    {
        public delegate bool ConvertCallback(string input, out T output);

        public static T ReadValue(ConvertCallback convertCallback, string prompt)
        {
            while (true)
            {
                WriteLine(prompt);
                string? line = ReadLine();

                if (line == null)
                    throw new EndOfStreamException();

                if (convertCallback(line, out T result))
                    return result;
            }
        }
    }

    public static int ReadInt(string prompt)
    {
        return ReaderHelper<int>.ReadValue(int.TryParse, prompt);
    }

    public static double ReadDouble(string prompt)
    {
        return ReaderHelper<double>.ReadValue(double.TryParse, prompt);
    }

    public static char ReadChar(string prompt)
    {
        return ReaderHelper<char>.ReadValue(char.TryParse, prompt);
    }

    public static string ReadString(string prompt)
    {
        return ReaderHelper<string>.ReadValue((string s, out string r) => { r = s; return true; }, prompt);
    }


}
