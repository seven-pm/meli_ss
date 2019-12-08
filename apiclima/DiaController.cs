using Microsoft.AspNetCore.Mvc;

namespace ApiClima.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiaController : ControllerBase
    {
        /// <summary>
        /// Consulta la BD en Azure y devuelve un DTO con información del día y el clima.
        /// Si no encuentra el día, informa 'Sin datos'.
        /// </summary>
        /// <param name="diaId">Número de día (primer día = 0)</param>
        /// <returns>DTO con el tipo de clima para el día consultado.</returns>
        // GET api/dia/5
        [HttpGet("{diaId}")]
        [ProducesResponseType(200, Type = typeof(DiaDTO))]
        [ProducesResponseType(404)]
        public IActionResult Get(int diaId)
        {
            var dal = new DiaDAL();
            var dto = dal.Find(diaId);
            if (string.IsNullOrEmpty(dto.Clima))
            {
                dto.Clima = "Sin datos";
            }

            return Ok(dto);
        }
    }
}
