using System;

namespace SistemaSolar
{
    public class Planeta
    {
        private double _anguloInicial;

        public double Angulo { get; private set; }
        public string Nombre { get; private set; }
        public int Distancia { get; private set; }
        public int Velocidad { get; private set; }
        public bool SentidoHorario { get; private set; }

        public Planeta(int distancia, int velocidad, bool sentidoHorario, string nombre, double angulo = 0)
        {
            if (distancia <= 0)
            {
                throw new ArgumentException("La distancia mayor a 0.");
            }

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

        public void AvanzarDia()
        {
            // En sentido horario, el ángulo decrece. 
            Angulo += Velocidad * (SentidoHorario ? -1 : 1);
            if (Angulo >= 360)
            {
                Angulo -= 360;
            }
            else if (Angulo < 0)
            {
                Angulo += 360;
            }
        }

        public void Reset()
        {
            Angulo = _anguloInicial;
        }
   
        public double X
        {
            // La distancia al sol es la hipotenusa. Lado adyacente = X = coseno(angulo) * hipotenusa.
            get { return Math.Cos(Angulo * Math.PI / 180d) * Distancia; }
        }

        public double Y
        {
            // La distancia al sol es la hipotenusa. Lado opuesto = Y = seno(angulo) * hipotenusa.
            get { return Math.Sin(Angulo * Math.PI / 180d) * Distancia; }
        }
    }
}
