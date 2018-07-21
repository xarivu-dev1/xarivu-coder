using System.IO;
using System.Reflection;

namespace Xarivu.Coder.Utilities
{
    public static class ResourceUtilities
    {
        /// <summary>
        /// Read an embedded text resource file.
        /// The file should be embedded in the executing assembly.
        /// The Build Action on the file should be "Embedded Resource".
        /// </summary>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        public static string ReadResource(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string result = reader.ReadToEnd();
                    return result;
                }
            }
        }
    }
}
