using Simulador.Models;
using SistemaSolar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Simulador
{
    class Program
    {
        public static void Main(string[] args)
        {
            Simular(3652, 1, 0, 0);
        }

        public static void Simular(
            int dias,
            double angulo1 = 0,
            double angulo2 = 0,
            double angulo3 = 0,
            int delay = 0,
            bool printSequia = false, 
            bool printLluvia = false, 
            bool printLluviaPico = false, 
            bool printCOPT = false,
            bool printNormal = false)
        {
            var espacio = new Espacio(new List<Planeta>
            {
                new Planeta(500, 1, true, "Ferengi", angulo1),
                new Planeta(1000, 5, false, "Vulcano", angulo2),
                new Planeta(2000, 3, true, "Betasoide", angulo3)           
            });

            var intensidadMaximaLluvia = 0d;
            var periodoSequia = false;
            var periodoLluvia = false;
            var periodoCOPT = false;
            var periodos = new List<Periodo>();

            for (var i = 0; i < dias; ++i)
            {
                espacio.CalcularClima();
                if (espacio.Sequia)
                {
                    if (!periodoSequia)
                    {
                        // Cerrar período anterior si estaba abierto.
                        if (periodos.Count > 0 && !periodos[periodos.Count - 1].DiaFinal.HasValue)
                        {
                            periodos[periodos.Count - 1].DiaFinal = espacio.DiaActual - 1;
                            periodoLluvia = false;
                            periodoCOPT = false;
                            intensidadMaximaLluvia = 0;
                        }
                        
                        // Comienza período de sequía
                        periodoSequia = true;
                        periodos.Add(new Periodo(espacio.DiaActual, TipoPeriodo.Sequia));
                    }
                    // Si ya estaba en período de sequía, sigue acumulando días.

                    if (printSequia) { Console.WriteLine(espacio.Reportar()); }
                }
                else if (espacio.Lluvia)
                {
                    if (!periodoLluvia)
                    {
                        // Cerrar período anterior si estaba abierto.
                        if (periodos.Count > 0 && !periodos[periodos.Count - 1].DiaFinal.HasValue)
                        {
                            periodos[periodos.Count - 1].DiaFinal = espacio.DiaActual - 1;
                            periodoSequia= false;
                            periodoCOPT = false;
                        }

                        // Comienza período de lluvia
                        periodoLluvia = true;
                        periodos.Add(new Periodo(espacio.DiaActual, TipoPeriodo.Lluvia));                        
                    }

                    // Se va actualizando la intensidad pico día a día.
                    if (espacio.IntensidadLluvia > intensidadMaximaLluvia)
                    {
                        intensidadMaximaLluvia = espacio.IntensidadLluvia;
                        periodos[periodos.Count - 1].DiaPico = espacio.DiaActual;
                        periodos[periodos.Count - 1].IntensidadLluvia = intensidadMaximaLluvia;
                    }

                    // Si ya estaba en período de lluvia, sigue acumulando días.

                    if (printLluvia) { Console.WriteLine(espacio.Reportar()); }
                }
                else if (espacio.CondicionesOptimasPyT)
                {
                    if (!periodoCOPT)
                    {
                        // Cerrar período anterior si estaba abierto.
                        if (periodos.Count > 0 && !periodos[periodos.Count - 1].DiaFinal.HasValue)
                        {
                            periodos[periodos.Count - 1].DiaFinal = espacio.DiaActual - 1;
                            periodoSequia = false;
                            periodoLluvia = false;
                            intensidadMaximaLluvia = 0;
                        }

                        // Comienza período de COPT
                        periodoCOPT = true;
                        periodos.Add(new Periodo(espacio.DiaActual, TipoPeriodo.COPT));
                    }
                    // Si ya estaba en período de COPT, sigue acumulando días.

                    if (printCOPT) { Console.WriteLine(espacio.Reportar()); }
                }
                else
                {
                    // Cerrar período anterior si estaba abierto.
                    if (periodos.Count > 0 && !periodos[periodos.Count - 1].DiaFinal.HasValue)
                    {
                        periodos[periodos.Count - 1].DiaFinal = espacio.DiaActual - 1;
                        periodoSequia = false;
                        periodoLluvia = false;
                        periodoCOPT = false;
                        intensidadMaximaLluvia = 0;
                    }

                    if (printNormal) { Console.WriteLine(espacio.Reportar()); }
                }

                espacio.AvanzarDia();

                if (delay > 0) { Thread.Sleep(delay); }
            }

            Print(periodos);
        }

        public static void Print(List<Periodo> periodos)
        {
            Console.WriteLine($"Períodos de sequía: {periodos.Count(x => x.Tipo == TipoPeriodo.Sequia)}");
            Console.WriteLine($"Períodos de lluvia: {periodos.Count(x => x.Tipo == TipoPeriodo.Lluvia)}");
            Console.WriteLine($"Períodos de COPT: {periodos.Count(x => x.Tipo == TipoPeriodo.COPT)}");
            
            var maximaIntensidad = periodos.Where(x => x.Tipo == TipoPeriodo.Lluvia).Max(x => x.IntensidadLluvia);
            Console.Write($"Día(s) con pico máximo de lluvia ({maximaIntensidad:n}): ");
            var diasPico = new StringBuilder();
            foreach (var periodo in periodos.Where(x => x.Tipo == TipoPeriodo.Lluvia))
            {
                if (periodo.IntensidadLluvia == maximaIntensidad)
                {
                    diasPico.Append(periodo.DiaPico);
                    diasPico.Append(",");
                }
            }
            Console.WriteLine(diasPico.Length > 0 ? diasPico.ToString().TrimEnd(',') : string.Empty);
            
            Console.WriteLine();
            Console.WriteLine("---------------------- DETALLES -------------------------");

            Console.WriteLine($"Sequía");
            Console.WriteLine("---------------------------------------------------------");
            foreach (var periodo in periodos.Where(x => x.Tipo == TipoPeriodo.Sequia))
            {
                Console.WriteLine($"Desde {periodo.DiaInicial} hasta {periodo.DiaFinal}");
            }
            Console.WriteLine("---------------------------------------------------------");
            
            Console.WriteLine($"Lluvia");
            Console.WriteLine("---------------------------------------------------------");
            foreach (var periodo in periodos.Where(x => x.Tipo == TipoPeriodo.Lluvia))
            {
                Console.Write($"Desde {periodo.DiaInicial} hasta {periodo.DiaFinal}");
                Console.WriteLine($" con pico en día {periodo.DiaPico} ({periodo.IntensidadLluvia:n})");
            }
            Console.WriteLine("---------------------------------------------------------");
            
            Console.WriteLine($"COPT");
            Console.WriteLine("---------------------------------------------------------");
            foreach (var periodo in periodos.Where(x => x.Tipo == TipoPeriodo.COPT))
            {
                Console.WriteLine($"Desde {periodo.DiaInicial} hasta {periodo.DiaFinal}");
            }
            Console.WriteLine("---------------------------------------------------------");
            
            Console.ReadKey();
        }
    }
}
