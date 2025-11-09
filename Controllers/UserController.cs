using app_congreso.Data;
using app_congreso.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace app_congreso.Controllers
{
    [Route("api")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsuariosController(AppDbContext context)
        {
            _context = context;
        }

        // 1️⃣ GET: /api/listado
        // Devuelve todos los usuarios registrados
        [HttpGet("listado")]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetListado()
        {
            var usuarios = await _context.Usuarios
                .OrderBy(u => u.Id)
                .ToListAsync();

            return Ok(usuarios);
        }

        // 2️⃣ GET: /api/listado/buscar?q=texto
        // Devuelve usuarios que coincidan con el texto de búsqueda en cualquier campo
        [HttpGet("listado/buscar")]
        public async Task<ActionResult<IEnumerable<Usuario>>> BuscarUsuarios([FromQuery] string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return BadRequest(new { message = "Debe proporcionar un texto de búsqueda (?q=...)" });

            q = q.ToLower();

            var resultados = await _context.Usuarios
                .Where(u =>
                    (u.Nombre != null && u.Nombre.ToLower().Contains(q)) ||
                    (u.Apellidos != null && u.Apellidos.ToLower().Contains(q)) ||
                    (u.Email != null && u.Email.ToLower().Contains(q)) ||
                    (u.Twitter != null && u.Twitter.ToLower().Contains(q)) ||
                    (u.Ocupacion != null && u.Ocupacion.ToLower().Contains(q)) ||
                    (u.Avatar != null && u.Avatar.ToLower().Contains(q))
                )
                .OrderBy(u => u.Id)
                .ToListAsync();

            return Ok(resultados);
        }

        // 3️⃣ GET: /api/usuario/{id}
        // Devuelve un usuario por su ID
        [HttpGet("usuario/{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound(new { message = "Usuario no encontrado" });

            return Ok(usuario);
        }

        // 4️⃣ POST: /api/registro
        // Registra un nuevo usuario
        [HttpPost("registro")]
        public async Task<ActionResult<Usuario>> RegistrarUsuario([FromBody] Usuario usuario)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUsuario), new { id = usuario.Id }, usuario);
        }
    }
}
