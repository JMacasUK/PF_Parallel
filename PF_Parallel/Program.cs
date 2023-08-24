using System;
using System.Diagnostics;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;

namespace PF_Parallel
{
    internal class Program
    {
        static string rutaEscanear = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        static string[] archivos = Directory.GetFiles(rutaEscanear, "*.*", SearchOption.AllDirectories);
        static TimeSpan espacioDeTiempoParallel;
        static TimeSpan espacioDeTiempoSequential;

        static void Main(string[] args)
        {
            Console.WriteLine($"Se detectaron {archivos.Length} archivos en el directorio {rutaEscanear}");
            Console.WriteLine("");

            ParallelForEach();

            SequentialForEach();

            Console.WriteLine("Demostración finalizada, pulse una tecla para salir...");
            Console.ReadLine();
        }

        static void CalcularSHA256(string rutaArchivo )
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                if (!File.Exists(rutaArchivo)) { return; }
                
                FileInfo fileInfoArchivo = new FileInfo( rutaArchivo );

                using (FileStream fileStream = fileInfoArchivo.Open(FileMode.Open))
                {
                    try
                    {
                        fileStream.Position = 0;
                        byte[] hashValue = sha256.ComputeHash(fileStream);

                        //Console.Write($"{fileInfoArchivo.Name}: ");
                        //ImprimirByteArrayHexadecimal(hashValue);
                    }
                    catch (IOException ioException)
                    {
                        Console.WriteLine($"Excepción en E/S: {ioException.Message}");
                    }
                    catch (UnauthorizedAccessException uaException)
                    {
                        Console.WriteLine($"Excepción de acceso: {uaException.Message}");
                    }
                }
            }
        }

        public static void ImprimirByteArrayHexadecimal(byte[] byteArray)
        {
            for (int i = 0; i < byteArray.Length; i++)
            {
                Console.Write($"{byteArray[i]:X2}".ToLowerInvariant());
            }

            Console.WriteLine();
        }


        public static void ParallelForEach()
        {
            Console.WriteLine($"Ejecutando ParallelForEach(), fecha y hora de inicio: {DateTime.Now:O}");
            
            Stopwatch cronometro = new Stopwatch();
            
            cronometro.Start();
            Parallel.ForEach(archivos, CalcularSHA256);
            cronometro.Stop();
            
            TimeSpan espacioDeTiempoParallel = cronometro.Elapsed;
            
            string tiempoTranscurridoString = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                espacioDeTiempoParallel.Hours, espacioDeTiempoSequential.Minutes, espacioDeTiempoSequential.Seconds, espacioDeTiempoSequential.Milliseconds);
            
            Console.WriteLine($"Finalizado ParallelForEach(), fecha y hora de finalización: {DateTime.Now:O}");
            Console.WriteLine($"Tiempo transcurrido: {tiempoTranscurridoString}");

            Console.Write(Environment.NewLine);
        }

        public static void SequentialForEach()
        {
            Console.WriteLine($"Ejecutando SequentialForEach(), fecha y hora de inicio: {DateTime.Now.ToString().ToUpperInvariant()}");

            Stopwatch cronometro = new Stopwatch();

            cronometro.Start();

            foreach (string archivo in archivos)
            {
                CalcularSHA256(archivo);
            }

            cronometro.Stop();

            TimeSpan espacioDeTiempoSequential = cronometro.Elapsed;

            string tiempoTranscurridoString = String.Format("{0:00}:{1:00}:{2:00}.{3:000}",
                espacioDeTiempoSequential.Hours, espacioDeTiempoSequential.Minutes, espacioDeTiempoSequential.Seconds, espacioDeTiempoSequential.Milliseconds);
            
            Console.WriteLine($"Finalizado SequentialForEach(), fecha y hora de finalización: {DateTime.Now.ToString().ToUpperInvariant()}");
            Console.WriteLine($"Tiempo transcurrido: {tiempoTranscurridoString}");

            Console.Write(Environment.NewLine);
        }
    }
}