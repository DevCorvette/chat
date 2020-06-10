namespace Corvette.Chat.Logic.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Returns true when the string doesn't equal null or empty or white space.
        /// </summary>
        public static bool HasValue(this string? str)
        {
            return !string.IsNullOrWhiteSpace(str);
        }
    }
}