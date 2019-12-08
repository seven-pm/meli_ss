using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiClima
{
    public class DiaDAL
    {
        public DiaDTO Find(int id)
        {
            var dto = new DiaDTO();
            try
            {
                var builder = new SqlConnectionStringBuilder();
                builder.DataSource = "servidor-meli-clima.database.windows.net";
                builder.UserID = "admin-meli";
                builder.Password = "Diciembre2019";
                builder.InitialCatalog = "clima";
                var sql = $"SELECT d.diaId, t.nombre FROM dias d INNER JOIN tipos_clima t ON d.tipoClimaId = t.tipoClimaId WHERE diaId = {id}";
                using (var connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand(sql, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                dto.NumeroDia = reader.GetDecimal(0);
                                dto.Clima = reader.GetString(1);
                                break;
                            }
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }

            return dto;
        }
    }
}
