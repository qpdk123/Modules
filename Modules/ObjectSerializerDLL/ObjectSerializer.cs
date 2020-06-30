using SingletonDLL;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace ObjectSerializerDLL
{
    public class ObjectSerializer : Singleton<ObjectSerializer>
    {
        protected ObjectSerializer()
        {
        }

        public void Save<T>(string path, T arg) where T : class
        {
            FileInfo info = new FileInfo(path);

            if (Directory.Exists(info.DirectoryName) == false)
            {
                Directory.CreateDirectory(info.DirectoryName);
            }

            using (FileStream fs = new FileStream(path, (info.Exists) ? FileMode.Truncate : FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    XmlSerializer serializer = new XmlSerializer(arg.GetType());
                    serializer.Serialize(sw, arg);
                }
            }
        }

        public T Load<T>(string path) where T : class
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    return (T)serializer.Deserialize(sr);
                }
            }
        }
    }
}
