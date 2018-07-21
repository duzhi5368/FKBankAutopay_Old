using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
//------------------------------------------------------------
namespace FKVerifyLib
{
    public static class FKDeepObjCopier
    {
        public static T Clone<T>(T source)
        {
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException("拷贝对象必须可以被流式化.", "source");
            }
            if(Object.ReferenceEquals(source, null))
            {
                return default(T);
            }
            IFormatter myFormatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                myFormatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)myFormatter.Deserialize(stream);
            }
        }
    }
}
