using System.Globalization;


namespace SCC_Gasso.Core.Application.Helpers
{
    public static class GetDateTime
    {
        public static string GetDateTimeInString()
        {
            DateTime time = DateTime.Now;

            return time.ToString("dd/MM/yyyy hh:mm:ss tt", new CultureInfo("es-ES"));
        }

        public static string ConvertTimeToString(DateTime time)
        {
            return time.ToString("dd/MM/yyyy hh:mm:ss tt", new CultureInfo("es-ES"));
        }
    }
}
