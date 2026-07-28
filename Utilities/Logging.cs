namespace OfficeManagementSystem.Utilities
{
    public class Logging
    {
        private static readonly string LogDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
        private static readonly string FileName = Path.Combine(LogDirectory, "errors.txt");

        public static void SaveError(string error)
        {
            try
            {
                if (!Directory.Exists(LogDirectory))
                    Directory.CreateDirectory(LogDirectory);

                string entry = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " : " + error;
                File.AppendAllText(FileName, entry + Environment.NewLine);
            }
            catch
            {
                // swallow logging errors to avoid cascading failures
            }
        }
    }
}
