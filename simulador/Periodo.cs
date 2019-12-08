using SistemaSolar;
using System;

namespace Simulador
{
    /// <summary>
    /// Representa un período de días del mismo tipo de clima
    /// </summary>
    public class Periodo
    {
        public int DiaInicial { get; set; }
        private int? _diaFinal;
        public int? DiaFinal
        {
            get { return _diaFinal; }
            set
            {
                if (value < DiaInicial)
                {
                    throw new Exception("DiaFinal debe ser mayor o igual al día inicial.");
                }
                _diaFinal = value;
            }
        }
        public TipoPeriodo Tipo { get; set; }
        public int DiaPicoIntensidadLluvia { get; set; }
        public double IntensidadLluvia { get; set; }

        public Periodo (int diaInicial, TipoPeriodo tipo)
        {
            DiaInicial = diaInicial;
            Tipo = tipo;
        }
    }
}
