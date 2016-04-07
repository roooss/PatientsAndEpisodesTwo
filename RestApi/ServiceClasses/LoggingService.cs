using RestApi.Models;
using System;
using System.IO;

namespace RestApi.ServiceClasses
{
    public class LoggingService
    {
        public static void WriteToXmlLog(RequestResponseLog item)
        {
            System.Xml.Serialization.XmlSerializer writer =
            new System.Xml.Serialization.XmlSerializer(typeof(RequestResponseLog));


            FileStream fileStream = null;
            string path = @"C:\weblogs\logs_" + DateTime.Now.ToString("yyyyMMdd") + ".xml";

            if (!File.Exists(path))
            {
                fileStream = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
            }
            else
            {
                fileStream = File.Open(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            }

            writer.Serialize(fileStream, item);
            fileStream.Close();
        }
    }
}