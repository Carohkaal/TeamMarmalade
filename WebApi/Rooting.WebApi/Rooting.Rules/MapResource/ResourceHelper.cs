using System.Reflection;

namespace Rooting.Rules
{
    public static class ResourceHelper
    {
        /// <summary>
        /// Read embedded resource as Stream.
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="folder"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static Stream? ReadResource(string folder, string fileName)
        {
            string resourcePath;
            var assembly = Assembly.GetAssembly(typeof(ResourceHelper));
            var assemblyName = assembly?.GetName().Name;
            if (folder != null)
                resourcePath = $"{assemblyName}.{folder}.{fileName}";
            else
                resourcePath = $"{assemblyName}.{fileName}";

            return assembly?.GetManifestResourceStream(resourcePath);
        }

        /// <summary>
        /// Read embedded resource as String.
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="folder"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string? ReadResourceAsString(Assembly assembly, string folder, string fileName)
        {
            using var stream = ReadResource(folder, fileName);
            if (stream == null)
                return null;
            using var streamReader = new StreamReader(stream);
            return streamReader.ReadToEnd();
        }
    }
}