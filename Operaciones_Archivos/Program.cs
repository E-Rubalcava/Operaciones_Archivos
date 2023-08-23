using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Operaciones_Archivos
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path;
            if (args.Length > 1)
            {
                Console.WriteLine("Demasiados argumentos");
                return;
            }

            if (args.Length == 1)
            {
                path = args[0];
            }
            else
            {
                Console.Write("Escriba el directorio: ");
                path = Console.ReadLine();
            }

            if (!Directory.Exists(path))
            {
                Console.WriteLine("El directorio no existe");
                return;
            }

            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] files = dir.GetFiles();

            Stopwatch globalStopwatch = Stopwatch.StartNew();

            Console.WriteLine("\nOperando usando Foreach");
            var inicioSecuencial = globalStopwatch.ElapsedMilliseconds;
            foreach (FileInfo file in files)
            {
                SHA(file);
            }
            var finSecuencial = globalStopwatch.ElapsedMilliseconds;

            Console.WriteLine("\nOperando usando Parallel.Foreach");
            var inicioAsincrono = globalStopwatch.ElapsedMilliseconds;
            Parallel.ForEach(files, file =>
            {
                SHA(file);
            });
            var finAsincrono = globalStopwatch.ElapsedMilliseconds;

            globalStopwatch.Stop();

            Console.WriteLine("\n\nSecuencial");
            Console.WriteLine($"Tiempo de inicio: {inicioSecuencial}ms");
            Console.WriteLine($"Tiempo de finalizacion: {finSecuencial}ms");
            Console.WriteLine($"Tiempo en procesamiento: {finSecuencial - inicioSecuencial}ms");

            Console.WriteLine("\n\nParalelo");
            Console.WriteLine($"Tiempo de inicio: {inicioAsincrono}ms");
            Console.WriteLine($"Tiempo de finalizacion: {finAsincrono}ms");
            Console.WriteLine($"Tiempo en procesamiento: {finAsincrono - inicioAsincrono}ms");


            Console.WriteLine($"\n\nTiempo total de ejecucion: {globalStopwatch.ElapsedMilliseconds}ms");

            Console.ReadKey();
        }

        private static void SHA(FileInfo file)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                using (FileStream fileStream = file.Open(FileMode.Open))
                {
                    try
                    {
                        fileStream.Position = 0;
                        byte[] hashValue = sha256.ComputeHash(fileStream);
                        // Console.WriteLine($"{file.Name}: {PrintByteArray(hashValue)}");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Ha ocurrido un error al intentar abrir el archivo {file.Name}");
                    }                    
                }
            }
        }

        private static string PrintByteArray(byte[] array)
        {
            string print = "";
            for (int i = 0; i < array.Length; i++)
            {
                print+= $"{array[i]:X2}";
                if ((i % 4) == 3) print+=" ";
            }
            return print;
        }

    }
}
