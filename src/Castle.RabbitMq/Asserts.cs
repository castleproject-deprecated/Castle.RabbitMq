namespace Castle.RabbitMq
{
    using System;

    internal static class Argument
    {
        public static void NotNull(object argValue, string argName, string message = null)
        {
            if (argValue == null)
            {
                throw new ArgumentNullException(argName, (message ?? "Argument cannot be null"));
            }
        }
    }

    internal static class Assert
    {
        public static void AssertNotNullOrEmpty(this string source, string message = null)
        {
            if (string.IsNullOrEmpty(source))
            {
                throw new Exception("Assertion failed: " + (message ?? "expected to not be null or empty"));
            }
        }
    }
}
