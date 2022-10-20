using Newtonsoft.Json;
using System;
using System.IO;

namespace WebApiFox.Helpers
{
    public class LoggerError
    {
        public static void CrearArchivoError(Exception exception)
        {
            string path = "C:\\ErroresConsultaWeb";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string text = JsonConvert.SerializeObject(exception);
            string nombreArchivo = Guid.NewGuid().ToString();
            using(StreamWriter outputFile = new StreamWriter($"{path}\\{nombreArchivo}.json"))
            {
                outputFile.WriteLine(text);
            }
        }
    }
}