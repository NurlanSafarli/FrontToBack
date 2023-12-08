namespace FronyToBack.Utilities.Extentions
{
    public class StringValidator
    {
        public static string Capitalize(this string name)
        {
            name = name.Substring(0, 1).ToUpper() + name.Substring(1).ToLower();
            return name;
        }
        public static bool IsDigit(this string name)
        {
            if ((name.Any(char.IsDigit)))
            {
                return true;

            }
            return false;
        }
    }
}
