using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SistemaSolar
{
    public class Espacio
    {
        public IList<Planeta> Planetas { get; private set; }
        public int DiaActual { get; private set; }
        public bool Sequia { get; private set; }
        public bool Lluvia { get; private set; }
        public bool CondicionesOptimasPyT { get; private set; }
        public double IntensidadLluvia { get; private set; }
        
        public Espacio(IList<Planeta> planetas)
        {
            Planetas = planetas;
            DiaActual = 0;
            //CalcularPerimetroMaximo();
        }

        public void CalcularClima()
        {
            Calcular();
        }

        public void AvanzarDia()
        {
            DiaActual++;
            foreach (var planeta in Planetas)
            {
                planeta.AvanzarDia();
            }
        }

        public string Reportar()
        {
            var reporte = new StringBuilder();
            reporte.AppendLine($"Día: {DiaActual}");
            if (Sequia)
            {
                reporte.AppendLine("Clima: Sequia");
            }
            else if (Lluvia)
            {
                reporte.Append($"Clima: Lluvia - Intensidad: {IntensidadLluvia}");
            }
            else if (CondicionesOptimasPyT)
            {
                reporte.AppendLine("Clima: Condiciones óptimas de P y T");
            }
            else
            {
                reporte.AppendLine("Clima: Normal");
            }

            foreach (var planeta in Planetas)
            {
                reporte.AppendLine($"Planeta {planeta.Nombre.PadRight(12)}--> ({planeta.Angulo:000};{planeta.Distancia:0000}) ({planeta.X:n};{planeta.Y:n})");
            }

            return reporte.ToString();
        }

        // Gen
        private void CalcularPerimetro()
        {
            var p = 0d;
            for (var i = 0; i < Planetas.Count() - 1; ++i)
            {
                p += CalcularDistancia(i, i + 1);
            }
            p += CalcularDistancia(0, Planetas.Count() - 1);

            IntensidadLluvia = p;
        }

        private void Calcular()
        {
            CalcularPerimetro();
            CalcularSequia();
            if (!Sequia)
            {
                CalcularLluvia();
                if (!Lluvia)
                {
                    CalcularCondicionOptimaPyT();
                }
            }
        }

        private void CalcularSequia()
        {
            // Los planetas están alineados con el sol cuando entre ellos están todos en el mismo ángulo a a 180°.
            Sequia = true;
            for (var i = 0; i < Planetas.Count() - 1; ++i)
            {
                if ((Planetas[i].Angulo - Planetas[i + 1].Angulo != 0) &&
                 (Math.Abs(Planetas[i].Angulo - Planetas[i + 1].Angulo) != 180))
                {
                    Sequia = false;
                    break;
                }
            }
        }

        private void CalcularLluvia()
        {
            // https://en.wikipedia.org/wiki/Barycentric_coordinate_system
            var lambda1 = ((Planetas[1].Y - Planetas[2].Y) * (0 - Planetas[2].X) + (Planetas[2].X - Planetas[1].X) * (0 - Planetas[2].Y)) / ((Planetas[1].Y - Planetas[2].Y) * (Planetas[0].X - Planetas[2].X) + (Planetas[2].X - Planetas[1].X) * (Planetas[0].Y - Planetas[2].Y));
            var lambda2 = ((Planetas[2].Y - Planetas[0].Y) * (0 - Planetas[2].X) + (Planetas[0].X - Planetas[2].X) * (0 - Planetas[2].Y)) / ((Planetas[1].Y - Planetas[2].Y) * (Planetas[0].X - Planetas[2].X) + (Planetas[2].X - Planetas[1].X) * (Planetas[0].Y - Planetas[2].Y));
            var lambda3 = 1 - lambda1 - lambda2;

            Lluvia = (0 <= lambda1 && lambda1 <= 1) && (0 <= lambda2 && lambda2 <= 1) && (0 <= lambda3 && lambda3 <= 1);
        }

        private void CalcularCondicionOptimaPyT()
        {
            // Ya se verificó que no estaban alineados al sol.
            var distancias = new double[]
            {
                CalcularDistancia(0, 1),
                CalcularDistancia(1, 2),
                CalcularDistancia(0, 2)
            };

            var distanciaMayor = distancias.Max();
            var distanciasSuma = distancias.Where(x => x != distanciaMayor).Sum();
            var dif = Math.Abs(distanciaMayor - distanciasSuma);

            var delta = 1;
            CondicionesOptimasPyT = dif.Between(-delta, delta);
        }

        private double CalcularDistancia(int i, int j)
        {
            var d = Math.Sqrt(Math.Pow(Planetas[i].X - Planetas[j].X, 2) + Math.Pow(Planetas[i].Y - Planetas[j].Y, 2));
            return d;
        }
    }
}
