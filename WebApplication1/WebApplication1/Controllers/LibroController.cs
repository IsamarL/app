using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LibroController : ControllerBase
    {
        //configurando el entorno para usar la cadena de coneccion , _config es la llave para usar la cadena de conexion
        private IConfiguration _Config;

        public LibroController(IConfiguration Config)
        {
            _Config = Config;
        }

        [HttpGet]
        public async Task<ActionResult<List<Libro>>> GetAllLibros()
        {
            using var conexion = new SqlConnection(_Config.GetConnectionString("DefaultConnection"));
            conexion.Open();
            var oLibros = conexion.Query<Libro>("LeerLibros", commandType: System.Data.CommandType.StoredProcedure);
            return Ok(oLibros);
        }

        [HttpGet("{ID}")]
        public async Task<ActionResult<List<Libro>>> GetLibrobyID(int ID)
        {
            using var conexion = new SqlConnection(_Config.GetConnectionString("DefaultConnection"));
            conexion.Open();
            var parametro = new DynamicParameters();
            parametro.Add("@ID", ID);
            var oLibros = conexion.Query<Libro>("BuscarLibroPorID", parametro, commandType: System.Data.CommandType.StoredProcedure);
            return Ok(oLibros);
        }

        [HttpPost]
        // obteniendo el objeto de usuario de la informacion proporcionada por Swagger
        public async Task<ActionResult<Libro>> CreateLibro(Libro lb)
        {
            try
            {
                using var conexion = new SqlConnection(_Config.GetConnectionString("DefaultConnection"));
                conexion.Open();
                var parametro = new DynamicParameters();
                parametro.Add("@Titulo", lb.Titulo);
                parametro.Add("@Autor", lb.Autor);
                parametro.Add("@Genero", lb.Genero);

                var oLibro = conexion.Query<Libro>("CrearLibro", parametro, commandType: System.Data.CommandType.StoredProcedure);

                // Verificar si la operación fue exitosa (por ejemplo, si oLibro no es nulo)
                if (oLibro != null)
                {

                    var mensaje = "Cliente creado exitosamente.";
                    return Ok(mensaje);
                }
                else
                {

                    var mensaje = "No se pudo crear el cliente.";
                    return BadRequest(new { mensaje });
                }
            }
            catch (Exception ex)
            {

                var mensaje = "Se produjo un error al crear el cliente: " + ex.Message;
                return StatusCode(500, new { mensaje });
            }
        }

        [HttpDelete("{ID}")]
        // obteniendo id del libro a eliminar (este id es de la clase Libros)
        public async Task<ActionResult> DeleteLibrobyID(int ID)
        {   
            //manejo de errores con trycach
            try
            {
                using (var conexion = new SqlConnection(_Config.GetConnectionString("DefaultConnection")))
                {
                    await conexion.OpenAsync();

                    var parametro = new DynamicParameters();
                    parametro.Add("@ID", ID);
                    await conexion.ExecuteAsync("EliminarLibro", parametro, commandType: CommandType.StoredProcedure);

                    return Ok("Libro eliminado correctamente.");
                }
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Error al eliminar el Libro: {ex.Message}");
            }
        }
    }
}
