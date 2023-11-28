using System.Text.RegularExpressions;

namespace OtusHomework13
{
    public static class StringExtension
    {
        /// <summary>
        /// Разделяет строку на подстроки, используя заданный разделитель. 
        /// </summary>
        public static IEnumerable<string> SplitCsv(this string value, char delimiter)
        {
            Regex regCsv = new Regex(string.Format("(\"[^\"]*\"|[^{0}])+", delimiter));
            var values =
                regCsv.Matches(value)
                .Cast<Match>()
                .Select(m => m.Value.Trim())
                .Where(v => !string.IsNullOrWhiteSpace(v));
            return values;
        }
    }
}
