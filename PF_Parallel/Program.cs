using System;
using System.Diagnostics;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;

namespace PF_Parallel
{
    internal class Program
    {
        static string rutaEscanear = "C:\\DataPort\\sops\\Programs\\Windows";
        static string[] archivos = Directory.GetFiles(rutaEscanear, "*.*", SearchOption.AllDirectories);

        static void Main(string[] args)
        {
            Console.WriteLine($"Se leyeron {archivos.Length} en el directorio {rutaEscanear}.");
            
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

                        Console.Write($"{fileInfoArchivo.Name}: ");
                        ImprimirByteArrayHexadecimal(hashValue);
                    }
                    catch (IOException ioe)
                    {
                        Console.WriteLine($"Excepción en E/S: {ioe.Message}");
                    }
                    catch (UnauthorizedAccessException uae)
                    {
                        Console.WriteLine($"Excepción de acceso: {uae.Message}");
                    }
                }
            }
        }

        public static void ImprimirByteArrayHexadecimal(byte[] byteArray)
        {
            for (int i = 0; i < byteArray.Length; i++)
            {
                Console.Write($"{byteArray[i]:X2}");
                if ((i % 4) == 3) Console.Write(" ");
            }

            Console.WriteLine();
        }


        public static void ParallelForEach()
        {
            Console.WriteLine($"Ejecutando ParallelForEach(), fecha y hora de inicio: {DateTime.Now.ToString().ToUpperInvariant()}");
            
            Stopwatch cronometro = new Stopwatch();
            
            cronometro.Start();
            Parallel.ForEach(archivos, CalcularSHA256);
            cronometro.Stop();
            
            TimeSpan espacioDeTiempo = cronometro.Elapsed;
            
            string tiempoTranscurridoString = String.Format("{0:00}:{1:00}:{2:00}.{3:000}",
                espacioDeTiempo.Hours, espacioDeTiempo.Minutes, espacioDeTiempo.Seconds, espacioDeTiempo.Milliseconds);
            
            Console.WriteLine($"Tiempo transcurrido: {tiempoTranscurridoString}");
            
            Console.WriteLine($"Finalizado ParallelForEach(), fecha y hora de finalización: {DateTime.Now.ToString().ToUpperInvariant()}");

            Console.WriteLine(Environment.NewLine);
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

            TimeSpan espacioDeTiempo = cronometro.Elapsed;

            string tiempoTranscurridoString = String.Format("{0:00}:{1:00}:{2:00}.{3:000}",
                espacioDeTiempo.Hours, espacioDeTiempo.Minutes, espacioDeTiempo.Seconds, espacioDeTiempo.Milliseconds);

            Console.WriteLine($"Tiempo transcurrido: {tiempoTranscurridoString}");

            Console.WriteLine($"Finalizado SequentialForEach(), fecha y hora de finalización: {DateTime.Now.ToString().ToUpperInvariant()}");

            Console.WriteLine(Environment.NewLine);
        }
    }
}