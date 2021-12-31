namespace UtilCli.App.Shared
{
    public static class ConsoleUtil
    {
        public static void CreateConsoleLine(int width, string? specificChar = null)
        {
            string appendChar = "=";
            if (specificChar != null) appendChar = specificChar; 

            string line = string.Empty;

            for (int i = 0; i < width - 2; i++)
            {
                line += appendChar;
            }

            Console.WriteLine(line);
        }

        public static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
