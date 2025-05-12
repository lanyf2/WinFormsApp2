using Cognex.VisionPro;
using Cognex.VisionPro.Implementation;
using Cognex.VisionPro.Implementation.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;
using System.Threading.Tasks;

namespace Ll
{
    public class LlSerializer
    {
        public static object DeepCopyObject(object obj)
        {
            return DeepCopyObject(obj, CogSerializationOptionsConstants.All);
        }
        public static object DeepCopyObject(object obj, CogSerializationOptionsConstants optionBits)
        {
            object result = null;
            MemoryStream memoryStream = null;
            try
            {
                memoryStream = new MemoryStream();
                if (memoryStream == null)
                {
                    throw new OutOfMemoryException();
                }
                Type typeFromHandle = typeof(BinaryFormatter);
                SaveObjectToStream(obj, memoryStream, typeFromHandle, optionBits, StreamingContextStates.Persistence);
                memoryStream.Position = 0L;
                Type typeFromHandle2 = typeof(BinaryFormatter);
                result = LoadObjectFromStream(memoryStream, typeFromHandle2, optionBits, StreamingContextStates.Persistence);
            }
            finally
            {
                if (memoryStream != null)
                {
                    memoryStream.Close();
                }
            }
            return result;
        }
        public static Type GetFileFormat(string Filename)
        {
            FileStream fileStream = File.OpenRead(Filename);
            Type streamFormat;
            try
            {
                streamFormat = GetStreamFormat(fileStream);
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }
            return streamFormat;
        }
        private static Type GetStreamFormat(Stream s)
        {
            if (s.CanSeek)
            {
                long position = s.Position;
                try
                {
                    char c = ReadNextNWChar(s);
                    if (c == '<')
                    {
                        char c2 = ReadNextNWChar(s);
                        if (c2 == 'S' || c2 == 's')
                        {
                            Type result = typeof(SoapFormatter);
                            return result;
                        }
                        if (c2 == '?')
                        {
                            Type result = null;
                            return result;
                        }
                    }
                }
                finally
                {
                    s.Position = position;
                }
            }
            return typeof(BinaryFormatter);
        }
        public static object LoadObjectFromFile(string path, Type formatterType, CogSerializationOptionsConstants optionBits)
        {
            object result;
            using (FileStream fileStream = File.OpenRead(path))
            {
                result = LoadObjectFromStream(fileStream, formatterType, optionBits, StreamingContextStates.File | StreamingContextStates.Persistence);
            }
            return result;
        }
        public static object LoadObjectFromFile(string path, Type formatterType)
        {
            return LoadObjectFromFile(path, formatterType, CogSerializationOptionsConstants.All);
        }

        public static object LoadObjectFromStream(Stream stream, Type formatterType, CogSerializationOptionsConstants optionBits, StreamingContextStates contextStates)
        {
            IFormatter formatter = (IFormatter)Activator.CreateInstance(formatterType);
            StreamingContext context = new StreamingContext(contextStates, CogSerializationOptionsContext.FromOptionBits(optionBits));
            formatter.Context = context;
            SerializationBinder binder = new CogSerializationBinder(formatter.Binder);
            formatter.Binder = binder;
            CogSerializationSurrogateSelector surrogateSelector = new CogSerializationSurrogateSelector(formatter.SurrogateSelector);
            formatter.SurrogateSelector = surrogateSelector;
            return formatter.Deserialize(stream);
        }
        public static object LoadObjectFromStream(Stream stream, Type formatterType, CogSerializationOptionsConstants optionBits)
        {
            return LoadObjectFromStream(stream, formatterType, optionBits, StreamingContextStates.Persistence);
        }
        public static object LoadObjectFromStream(Stream stream, Type formatterType)
        {
            return LoadObjectFromStream(stream, formatterType, CogSerializationOptionsConstants.All, StreamingContextStates.Persistence);
        }
        public static object LoadObjectFromStream(Stream stream)
        {
            Type streamFormat = GetStreamFormat(stream);
            if (streamFormat == null)
            {
                throw new SerializationException(CogLocalizer.GetString(typeof(CogSerializer), CogCoreResourceKeys.RkLegacyFormatNotSupported));
            }
            return LoadObjectFromStream(stream, streamFormat, CogSerializationOptionsConstants.All, StreamingContextStates.Persistence);
        }
        private static char ReadNextNWChar(Stream s)
        {
            char c = (char)s.ReadByte();
            while (c == '\0' || char.IsWhiteSpace(c))
            {
                c = (char)s.ReadByte();
            }
            return c;
        }

        public static void SaveObjectToFile(object obj, string path, Type formatterType, CogSerializationOptionsConstants optionBits)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj == null");
            }
            using (FileStream fileStream = File.Create(path))
            {
                SaveObjectToStream(obj, fileStream, formatterType, optionBits, StreamingContextStates.File | StreamingContextStates.Persistence);
            }
        }
        public static void SaveObjectToFile(object obj, string path, Type formatterType)
        {
            SaveObjectToFile(obj, path, formatterType, CogSerializationOptionsConstants.All);
        }
        public static void SaveObjectToFile(object obj, string path)
        {
            Type typeFromHandle = typeof(BinaryFormatter);
            SaveObjectToFile(obj, path, typeFromHandle, CogSerializationOptionsConstants.All);
        }

        public static void SaveObjectToStream(object obj, Stream stream, Type formatterType, CogSerializationOptionsConstants optionBits, StreamingContextStates contextStates)
        {
            IFormatter formatter = (IFormatter)Activator.CreateInstance(formatterType);
            StreamingContext context = new StreamingContext(contextStates, CogSerializationOptionsContext.FromOptionBits(optionBits));
            formatter.Context = context;
            CogSerializationSurrogateSelector surrogateSelector = new CogSerializationSurrogateSelector(formatter.SurrogateSelector);
            formatter.SurrogateSelector = surrogateSelector;
            formatter.Serialize(stream, obj);
        }
        public static void SaveObjectToStream(object obj, Stream stream, Type formatterType, CogSerializationOptionsConstants optionBits)
        {
            SaveObjectToStream(obj, stream, formatterType, optionBits, StreamingContextStates.Persistence);
        }
        public static void SaveObjectToStream(object obj, Stream stream, Type formatterType)
        {
            SaveObjectToStream(obj, stream, formatterType, CogSerializationOptionsConstants.All, StreamingContextStates.Persistence);
        }
        public static void SaveObjectToStream(object obj, Stream stream)
        {
            Type typeFromHandle = typeof(BinaryFormatter);
            SaveObjectToStream(obj, stream, typeFromHandle, CogSerializationOptionsConstants.All, StreamingContextStates.Persistence);
        }
    }
}
