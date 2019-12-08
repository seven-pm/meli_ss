using System;

namespace SistemaSolar
{
    /// <summary>
    /// Representa un planeta con una distancia al sol y una velocidad y sentido de rotación.
    /// </summary>
    public class Planeta
    {
        // Se usa para resetear el ángulo.
        private double _anguloInicial;

        public double Angulo { get; private set; }
        public string Nombre { get; private set; }
        public int Distancia { get; private set; }
        public int Velocidad { get; private set; }
        public bool SentidoHorario { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="distancia">Distancia al sol en el origen (0;0).</param>
        /// <param name="velocidad">Velocidad de rotación en grados/día.</param>
        /// <param name="sentidoHorario">Indica si el planeta gira en sentido horario o anti-horario.</param>
        /// <param name="nombre">Nombre del planeta.</param>
        /// <param name="angulo">Ángulo inicial.</param>
        public Planeta(int distancia, int velocidad, bool sentidoHorario, string nombre, double angulo = 0)
        {
            if (distancia <= 0)
            {
                throw new ArgumentException("La distancia mayor a 0.");
            }

            // El ángulo se normaliza entre 0 y 359.
            while (angulo < 0)
            {
                angulo += 360;
            }
            
            if (angulo >= 360)
            {
                angulo = angulo % 360;
            }

            Distancia = distancia;
            Angulo = angulo;
            Velocidad = velocidad;
            SentidoHorario = sentidoHorario;
            Nombre = nombre;

            _anguloInicial = angulo;
        }

        /// <summary>
        /// Modifica el ángulo en un día, a partir de la velocidad de giro por día.
        /// </summary>
        public void AvanzarDia()
        {
            // En sentido horario, el ángulo decrece. 
            Angulo += Velocidad * (SentidoHorario ? -1 : 1);
            // El ángulo siempre tiene que quedar entre 0 y 359.
            if (Angulo >= 360)
            {
                Angulo -= 360;
            }
            else if (Angulo < 0)
            {
                Angulo += 360;
            }
        }

        /// <summary>
        /// Volver al ángulo inicial.
        /// </summary>
        public void Reset()
        {
            Angulo = _anguloInicial;
        }

        /// <summary>
        /// Coordenada cartesiana X.
        /// </summary>
        public double X
        {
            // La distancia al sol es la hipotenusa. Lado adyacente = X = coseno(angulo) * hipotenusa.
            get { return Math.Cos(Angulo * Math.PI / 180d) * Distancia; }
        }

        /// <summary>
        /// Coordenada cartesiana Y.
        /// </summary>
        public double Y
        {
            // La distancia al sol es la hipotenusa. Lado opuesto = Y = seno(angulo) * hipotenusa.
            get { return Math.Sin(Angulo * Math.PI / 180d) * Distancia; }
        }
    }
}
