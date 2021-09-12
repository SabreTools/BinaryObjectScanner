using System.Collections.Generic;
using System.IO;

namespace BurnOutSharp.ExecutableType.Microsoft.Resources
{
    public class Var : Resource
    {
        /// <summary>
        /// An array of one or more values that are language and code page identifier pairs.
        /// 
        /// If you use the Var structure to list the languages your application or DLL supports instead of using multiple version resources,
        /// use the Value member to contain an array of DWORD values indicating the language and code page combinations supported by this file.
        /// The low-order word of each DWORD must contain a Microsoft language identifier, and the high-order word must contain the IBM code page number.
        /// Either high-order or low-order word can be zero, indicating that the file is language or code page independent.
        /// If the Var structure is omitted, the file will be interpreted as both language and code page independent.
        /// </summary>
        public LanguageCodePage[] Value;

        public Var(Resource resource)
        {
            this.Length = resource?.Length ?? default;
            this.ValueLength = resource?.ValueLength ?? default;
            this.Type = resource?.Type ?? default;
            this.Key = resource?.Key ?? default;
        }

        public static new Var Deserialize(Stream stream)
        {
            long originalPosition = stream.Position;
            Resource resource = Resource.Deserialize(stream);
            if (resource.Key != "Translation")
                return null;

            Var v = new Var(resource);

            var tempValue = new List<LanguageCodePage>();
            while (stream.Position - originalPosition < v.Length)
            {
                tempValue.Add(LanguageCodePage.Deserialize(stream));
            }

            v.Value = tempValue.ToArray();

            return v;
        }

        public static new Var Deserialize(byte[] content, ref int offset)
        {
            int originalPosition = offset;
            Resource resource = Resource.Deserialize(content, ref offset);
            if (resource.Key != "Translation")
                return null;

            Var v = new Var(resource);

            var tempValue = new List<LanguageCodePage>();
            while (offset - originalPosition < v.Length)
            {
                tempValue.Add(LanguageCodePage.Deserialize(content, ref offset));
            }

            v.Value = tempValue.ToArray();

            return v;
        }
    }
}