using System;
using System.Collections.Generic;
using System.Linq;

namespace SistemaSolar
{
    public enum TipoPeriodo
    {
        Sequia = 1,
        Lluvia,
        COPT,
        Normal
    };

    /// <summary>
    /// Representa al sistema solar.
    /// </summary>
    public class Espacio
    {
        public IList<Planeta> Planetas { get; private set; }
        public int DiaActual { get; private set; }
        public bool Sequia { get; private set; }
        public bool Lluvia { get; private set; }
        public bool COPT { get; private set; }
        public double IntensidadLluvia { get; private set; }
        public TipoPeriodo TipoPeriodo
        {
            get
            {
                if (Sequia) return TipoPeriodo.Sequia;
                else if (Lluvia) return TipoPeriodo.Lluvia;
                else if (COPT) return TipoPeriodo.COPT;
                else return TipoPeriodo.Normal;
            }
        }

        public Espacio(IList<Planeta> planetas)
        {
            Planetas = planetas;
            DiaActual = 0;
        }

        public void CalcularClima()
        {
            CalcularSequia();
            if (!Sequia)
            {
                CalcularLluvia();
                CalcularPerimetro();
                if (!Lluvia)
                {
                    CalcularCOPT();
                }
            }
        }

        public void AvanzarDia()
        {
            DiaActual++;
            foreach (var planeta in Planetas)
            {
                planeta.AvanzarDia();
            }
        }

        /// <summary>
        /// Calcula el perímetro del triángulo formado por los 3 planetas.
        /// El resultado se guarda como representación de la intensidad de la lluvia.
        /// </summary>
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

        /// <summary>
        /// Determina si el día actual es de tipo Sequia.
        /// </summary>
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

            // Si entró acá, no hay lluvia.
            IntensidadLluvia = 0;
        }

        /// <summary>
        /// Determina si el día actual es de tipo Lluvia.
        /// </summary>
        private void CalcularLluvia()
        {
            // Es tipo lluvia si el sol está dentro del triángulo formado por los 3 planetas.
            // Se considera al sol como un punto en las coordenadas (0;0).
            // Para determinar si un punto está dentro de un triángulo, se pasa de coordenadas
            // cartesianas a baricéntricas.
            // Transformación de coordenadas:
            // Coordenadas del sol, el punto que se quiere probar si pertenece al triángulo.
            var solX = 0;
            var solY = 0;
            var lambda1 = ((Planetas[1].Y - Planetas[2].Y) * (solX - Planetas[2].X) + (Planetas[2].X - Planetas[1].X) * (solY - Planetas[2].Y)) / ((Planetas[1].Y - Planetas[2].Y) * (Planetas[0].X - Planetas[2].X) + (Planetas[2].X - Planetas[1].X) * (Planetas[0].Y - Planetas[2].Y));
            var lambda2 = ((Planetas[2].Y - Planetas[0].Y) * (solX - Planetas[2].X) + (Planetas[0].X - Planetas[2].X) * (solY - Planetas[2].Y)) / ((Planetas[1].Y - Planetas[2].Y) * (Planetas[0].X - Planetas[2].X) + (Planetas[2].X - Planetas[1].X) * (Planetas[0].Y - Planetas[2].Y));
            // La suma de las coordenadas baricéntricas debe ser 1.
            var lambda3 = 1 - lambda1 - lambda2;

            // Condición de pertenencia al triángulo: las 3 coordenadas deben estar entre 0 y 1.
            Lluvia = lambda1.Between(0, 1) && lambda2.Between(0, 1) && lambda3.Between(0, 1);
        }

        /// <summary>
        /// Determina si el día actual es de tipo COPT.
        /// </summary>
        private void CalcularCOPT()
        {
            // Previamente se tiene que haber verificado que los planetas no estaban alineados al sol.
            var distancias = new double[]
            {
                CalcularDistancia(0, 1),
                CalcularDistancia(1, 2),
                CalcularDistancia(0, 2)
            };

            // Condición de planetas alineados:
            // Por una propiedad de los triángulos, la suma de 2 lados siempre es mayor al lado restante.
            // Cuando hay una igualdad, es que el triángulo no tiene área, es decir es una línea recta.
            var distanciaMayor = distancias.Max();
            var distanciasSuma = distancias.Where(x => x != distanciaMayor).Sum();
            var dif = Math.Abs(distanciaMayor - distanciasSuma);

            // Si entró acá, no hay lluvia.
            IntensidadLluvia = 0;

            var delta = 1;
            COPT = dif.Between(-delta, delta);
        }

        /// <summary>
        /// Calcula la distancia entre 2 planetas.
        /// </summary>
        /// <param name="i">Índice de uno de los planeta.</param>
        /// <param name="j">Índice del otro planeta.</param>
        /// <returns>Distancia entre los planetas.</returns>
        private double CalcularDistancia(int i, int j)
        {
            // Se calcula por Pitágoras, es la hipotenusa del triángulo rectángulo con vértices en ambos planetas.
            var d = Math.Sqrt(Math.Pow(Planetas[i].X - Planetas[j].X, 2) + Math.Pow(Planetas[i].Y - Planetas[j].Y, 2));
            return d;
        }
    }
}
