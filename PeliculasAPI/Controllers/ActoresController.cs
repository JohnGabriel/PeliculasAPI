using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasAPI.DTO;
using PeliculasAPI.Entidades;
using PeliculasAPI.Services;

namespace PeliculasAPI.Controllers
{
    [ApiController]
    [Route("api/actores")]
    public class ActoresController : Controller
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly string contenedor = "actores";

        public ActoresController(ApplicationDBContext context,IMapper mapper,
            IAlmacenadorArchivos almacenadorArchivos)
        {
            this.context = context;
            this.mapper = mapper;
            this.almacenadorArchivos = almacenadorArchivos;
        }
        [HttpGet]
        public async Task<ActionResult<List<ActorDTO>>> Get() {
            var listaAurores = await context.Actores.ToListAsync();
            return mapper.Map<List<ActorDTO>>(listaAurores);
        }
        [HttpGet("{id}",Name ="obtenerActor")]
        public async Task<ActionResult<ActorDTO>> Get(int id) 
        {
            var entidad = await context.Actores.FirstOrDefaultAsync(a => a.Id == id);
            if (entidad == null)
            {
                return NotFound();
            }
            return mapper.Map<ActorDTO>(entidad);
        }
        [HttpPost]
        public async Task<ActionResult> Post([FromForm] ActorCreacionDTO actorCreacionDTO) 
        {
            var actor = mapper.Map<Actor>(actorCreacionDTO);

            if (actorCreacionDTO.Foto != null) {
                using (var memoryStream = new MemoryStream())
                {
                    await actorCreacionDTO.Foto.CopyToAsync(memoryStream);
                    var contenido = memoryStream.ToArray();
                    var extencion = Path.GetExtension(actorCreacionDTO.Foto.FileName);
                    actor.Foto = await almacenadorArchivos.GuardarArchivo(contenido, extencion, contenedor,
                        actorCreacionDTO.Foto.ContentType);
                }
            }
            context.Add(actor);
            await context.SaveChangesAsync();
            var actorDto = mapper.Map<ActorDTO>(actor);

            return new CreatedAtRouteResult("obtenerActor", new { id = actor.Id  }, actorDto);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<ActorDTO>> Put(int id, [FromForm] ActorCreacionDTO actorCreacionDTO)
        {
            var actorDB = await context.Actores.FirstOrDefaultAsync(s => s.Id == id);
            if (actorDB == null) { return NotFound(); }

            actorDB = mapper.Map(actorCreacionDTO, actorDB);

            if (actorCreacionDTO.Foto != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await actorCreacionDTO.Foto.CopyToAsync(memoryStream);
                    var contenido = memoryStream.ToArray();
                    var extencion = Path.GetExtension(actorCreacionDTO.Foto.FileName);
                    actorDB.Foto = await almacenadorArchivos.GuardarArchivo(contenido, extencion, contenedor,
                        actorCreacionDTO.Foto.ContentType);
                }
            }
            //var entidad = mapper.Map<Actor>(actorCreacionDTO);
            //entidad.Id = id;
            //context.Entry(entidad).State = EntityState.Modified;

            await context.SaveChangesAsync();

            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var reg = await context.Actores.AnyAsync(d => d.Id == id);
            if (!reg)
            {
                return NotFound();
            }
            context.Remove(new Actor() { Id = id });
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}
