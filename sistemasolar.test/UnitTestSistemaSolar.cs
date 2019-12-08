using System;
using System.Collections.Generic;
using Xunit;

namespace SistemaSolar.Test
{
    public class UnitTestSistemaSolar
    {
        [Fact]
        [Trait("Category", "Sequia")]
        public void SequiaConPlanetasMismoAngulo()
        {
            var angulo = new Random().Next(0, 359);
            var sistema = CrearSistema(angulo, angulo, angulo);
            sistema.CalcularClima();
            Assert.True(sistema.Sequia);
        }

        [Fact]
        [Trait("Category", "Sequia")]
        public void SequiaConPlanetasMismoAnguloUnoOpuesto1()
        {
            var angulo = new Random().Next(0, 359);
            var sistema = CrearSistema(angulo + 180, angulo, angulo);
            sistema.CalcularClima();
            Assert.True(sistema.Sequia);
        }

        [Fact]
        [Trait("Category", "Sequia")]
        public void SequiaConPlanetasMismoAnguloUnoOpuesto2()
        {
            var angulo = new Random().Next(0, 359);
            var sistema = CrearSistema(angulo, angulo + 180, angulo);
            sistema.CalcularClima();
            Assert.True(sistema.Sequia);
        }

        [Fact]
        [Trait("Category", "Sequia")]
        public void SequiaConPlanetasMismoAnguloUnoOpuesto3()
        {
            var angulo = new Random().Next(0, 359);
            var sistema = CrearSistema(angulo, angulo, angulo + 180);
            sistema.CalcularClima();
            Assert.True(sistema.Sequia);
        }

        [Fact]
        [Trait("Category", "Lluvia")]
        public void LluviaConSolEnPerimetro()
        {
            var sistema = CrearSistema(180, 0, 90);
            sistema.CalcularClima();
            Assert.True(sistema.Lluvia);
        }

        [Fact]
        [Trait("Category", "Lluvia")]
        public void LluviaConSolFueraPerimetro1()
        {
            var sistema = CrearSistema(179, 0, 90);
            sistema.CalcularClima();
            Assert.False(sistema.Lluvia);
        }

        [Fact]
        [Trait("Category", "Lluvia")]
        public void LluviaConSolFueraPerimetro2()
        {
            var sistema = CrearSistema(90, 0, 45);
            sistema.CalcularClima();
            Assert.False(sistema.Lluvia);
        }

        [Fact]
        [Trait("Category", "Lluvia")]
        public void LluviaConSolDentroTriangulo()
        {
            var sistema = CrearSistema(90, 250, 45);
            sistema.CalcularClima();
            Assert.True(sistema.Lluvia);
        }

        [Fact]
        [Trait("Category", "Lluvia")]
        public void LluviaConSolFueraTriangulo()
        {
            var sistema = CrearSistema(90, 0, 45);
            sistema.CalcularClima();
            Assert.False(sistema.Lluvia);
        }

        [Fact]
        [Trait("Category", "COPT")]
        public void COPTConPlanetasAlineados1()
        {
            var sistema = CrearSistema(313, 235, 219);
            sistema.CalcularClima();
            Assert.True(sistema.COPT);
        }

        [Fact]
        [Trait("Category", "COPT")]
        public void COPTConPlanetasAlineados2()
        {
            var sistema = CrearSistema(296, 320, 168);
            sistema.CalcularClima();
            Assert.True(sistema.COPT);
        }

        [Fact]
        [Trait("Category", "COPT")]
        public void COPTConPlanetasAlineados3()
        {
            var sistema = CrearSistema(244, 220, 12);
            sistema.CalcularClima();
            Assert.True(sistema.COPT);
        }

        [Fact]
        [Trait("Category", "COPT")]
        public void COPTConPlanetasAlineados4()
        {
            var sistema = CrearSistema(227, 305, 321);
            sistema.CalcularClima();
            Assert.True(sistema.COPT);
        }

        [Fact]
        [Trait("Category", "COPT")]
        public void SinCOPTConSequia()
        {
            var sistema = CrearSistema(0, 180, 180);
            sistema.CalcularClima();
            Assert.False(sistema.COPT);
        }

        [Fact]
        [Trait("Category", "Planeta")]
        public void AvanzarDiaReduceAnguloSentidoHorario()
        {
            var sistema = CrearSistema(180, 180, 180);
            // Planeta 2 gira horario.
            var anguloOriginal = sistema.Planetas[2].Angulo;
            var velocidad = sistema.Planetas[2].Velocidad;
            sistema.AvanzarDia();
            Assert.Equal(sistema.Planetas[2].Angulo, anguloOriginal - velocidad);
        }

        [Fact]
        [Trait("Category", "Planeta")]
        public void AvanzarDiaAumentaAnguloSentidoAntiHorario()
        {
            var sistema = CrearSistema(180, 180, 180);
            // Planeta 1 gira anti-horario.    
            var anguloOriginal = sistema.Planetas[1].Angulo;
            var velocidad = sistema.Planetas[1].Velocidad;
            sistema.AvanzarDia();
            Assert.Equal(sistema.Planetas[1].Angulo, anguloOriginal + velocidad);
        }

        [Fact]
        [Trait("Category", "Planeta")]
        public void AnguloNegativoSeNormaliza()
        {
            var angulo = new Random().Next(-9999, -1);
            var planeta = new Planeta(1, 1, true, "foo", angulo);
            Assert.True(planeta.Angulo > 0);
            Assert.True(planeta.Angulo < 360);
        }

        [Fact]
        [Trait("Category", "Planeta")]
        public void AnguloMayor360SeNormaliza()
        {
            var angulo = new Random().Next(360, 9999);
            var planeta = new Planeta(1, 1, true, "foo", angulo);
            Assert.True(planeta.Angulo > 0);
            Assert.True(planeta.Angulo < 360);
        }

        private Espacio CrearSistema(double angulo1, double angulo2, double angulo3)
        {
            return new Espacio(new List<Planeta>
            {
                new Planeta(500, 1, true, "Ferengi", angulo1),
                new Planeta(1000, 5, false, "Vulcano", angulo2),
                new Planeta(2000, 3, true, "Betasoide", angulo3)                
            });
        }
    }
}
