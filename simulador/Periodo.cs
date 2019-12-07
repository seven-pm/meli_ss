using System;
using System.Collections.Generic;
using System.Text;

namespace Simulador.Models
{
    public enum TipoPeriodo
    {
        Sequia,
        Lluvia,
        COPT,
        Normal
    };

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
        public int DiaPico { get; set; }
        public double IntensidadLluvia { get; set; }

        public Periodo (int diaInicial, TipoPeriodo tipo)
        {
            DiaInicial = diaInicial;
            Tipo = tipo;
        }
    }
}
