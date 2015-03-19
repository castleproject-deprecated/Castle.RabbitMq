namespace Castle.RabbitMq.WindsorIntegration
{
    using System;

    static class Exts
    {
        public static string ExtendedName(this Type source)
        {
            var fullname = source.AssemblyQualifiedName;

            // TopNamespace.SubNameSpace.ContainingClass+NestedClass, MyAssembly, Version=1.3.0.0, Culture=neutral, PublicKeyToken=b17a5c561934e089

            var sndComma = fullname.IndexOf("Version=", source.FullName.Length, StringComparison.Ordinal);

            return fullname.Substring(0, sndComma - 2);
        }
    }
}