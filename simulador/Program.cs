using SistemaSolar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simulador
{
    class Program
    {
        public static void Main(string[] args)
        {
            // 3652 días = 10 años.
            Simular(3652, 0, 0, 0);
        }

        /// <summary>
        /// Simula la evolución del sistema solar por una cantidad de días, y registra datos
        /// sobre los períodos climáticos.
        /// </summary>
        /// <param name="dias">Cantidad de días de la simulación. Se parte desde el día 0.</param>
        /// <param name="angulo1">Ángulo inicial del primer planeta (500km)</param>
        /// <param name="angulo2">Ángulo inicial del segundo planeta (1000km)</param>
        /// <param name="angulo3">Ángulo inicial del tercer planeta (2000km)</param>
        public static void Simular(
            int dias,
            double angulo1 = 0,
            double angulo2 = 0,
            double angulo3 = 0)
        {
            if (dias <= 0)
            {
                throw new ArgumentException("La cantidad de días debe ser mayor a 0.");
            }

            var espacio = new Espacio(new List<Planeta>
            {
                new Planeta(500, 1, true, "Ferengi", angulo1),
                new Planeta(1000, 5, false, "Vulcano", angulo2),
                new Planeta(2000, 3, true, "Betasoide", angulo3)
            });

            // Para registrar el valor máximo de intensidad de lluvia en toda la simulación.
            var intensidadMaximaLluvia = 0d;
            // Flags para determinar en qué período climático se encuentra la simulación.
            var periodoSequia = false;
            var periodoLluvia = false;
            var periodoCOPT = false;
            // Lista de períodos donde se registra toda la información que se imprime al final de la simulación.
            var periodos = new List<Periodo>();

            // Por cada día, se analiza si hay que abrir o cerrar un período climático.
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
                        periodos[periodos.Count - 1].DiaPicoIntensidadLluvia = espacio.DiaActual;
                        periodos[periodos.Count - 1].IntensidadLluvia = intensidadMaximaLluvia;
                    }
                    // Si ya estaba en período de lluvia, sigue acumulando días.
                }
                else if (espacio.COPT)
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
                }

                espacio.AvanzarDia();
            }

            Print(periodos);
        }

        /// <summary>
        /// Se imprimen:
        /// La cantidad de períodos de cada tipo de clima.
        /// El/los día/s con pico de intensidad de lluvia (independientes del período).
        /// Detalles de cada período, con día inicial y día final, agrupados por tipo de clima.
        /// Para los períodos de lluvia, el día con pico de intensidad para ese período.
        /// </summary>
        /// <param name="periodos">Lista de períodos climáticos.</param>
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
                    diasPico.Append(periodo.DiaPicoIntensidadLluvia);
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
                Console.WriteLine($" con pico en día {periodo.DiaPicoIntensidadLluvia} ({periodo.IntensidadLluvia:n})");
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
