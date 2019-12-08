using SistemaSolar;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace GeneradorDatos
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var espacio = new Espacio(new List<Planeta>
                {
                    new Planeta(500, 1, true, "Ferengi"),
                    new Planeta(1000, 5, false, "Vulcano"),
                    new Planeta(2000, 3, true, "Betasoide")
                });

                var builder = new SqlConnectionStringBuilder();
                builder.DataSource = "servidor-meli-clima.database.windows.net";
                builder.UserID = "admin-meli";
                builder.Password = "Diciembre2019";
                builder.InitialCatalog = "clima";
                using (var connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand())
                    {
                        command.Connection = connection;
                        for (var i = 0; i < 3652; ++i)
                        {
                            espacio.CalcularClima();
                            var sql = new StringBuilder();
                            sql.Append("insert into dias(diaId, tipoClimaId) values(");
                            sql.Append(espacio.DiaActual);
                            sql.Append(",");
                            sql.Append((int)espacio.TipoPeriodo);
                            sql.Append(");");
                            command.CommandText = sql.ToString();
                            command.ExecuteNonQuery();

                            espacio.AvanzarDia();
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("Terminado.");
            Console.ReadLine();
        }
    }
}
