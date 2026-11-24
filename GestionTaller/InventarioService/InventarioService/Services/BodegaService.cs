using InventarioService.Data;
using InventarioService.DTOs;
using InventarioService.Interfaces;
using InventarioService.Models;
using Microsoft.EntityFrameworkCore;

namespace InventarioService.Services
{
    public class BodegaService : IBodegaService
    {
        private readonly InventarioDbContext _context;

        public BodegaService(InventarioDbContext context)
        {
            _context = context;
        }

        public async Task<List<BodegaDto>> ConsultarTodos()
        {
            return await _context.Bodegas
                .AsNoTracking()
                .Select(x => new BodegaDto
                {
                    Id = x.Id,
                    Codigo = x.Codigo,
                    Nombre = x.Nombre,
                    Ubicacion = x.Ubicacion,
                    Activo = x.Activo
                })
                .ToListAsync();
        }

        public async Task<BodegaDto?> ConsultarPorId(int id)
        {
            return await _context.Bodegas
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new BodegaDto
                {
                    Id = x.Id,
                    Codigo = x.Codigo,
                    Nombre = x.Nombre,
                    Ubicacion = x.Ubicacion,
                    Activo = x.Activo
                })
                .FirstOrDefaultAsync();
        }

        public async Task<BodegaDto> Crear(CrearBodegaDto dto)
        {
            var bodega = new Bodega
            {
                Codigo = dto.Codigo.Trim(),
                Nombre = dto.Nombre.Trim(),
                Ubicacion = dto.Ubicacion,
                Activo = true
            };

            _context.Bodegas.Add(bodega);

            await _context.SaveChangesAsync();

            return new BodegaDto
            {
                Id = bodega.Id,
                Codigo = bodega.Codigo,
                Nombre = bodega.Nombre,
                Ubicacion = bodega.Ubicacion,
                Activo = bodega.Activo
            };
        }

        public async Task<bool> Actualizar(
            int id,
            ActualizarBodegaDto dto)
        {
            var bodega = await _context.Bodegas.FindAsync(id);

            if (bodega == null)
                return false;

            bodega.Codigo = dto.Codigo.Trim();
            bodega.Nombre = dto.Nombre.Trim();
            bodega.Ubicacion = dto.Ubicacion;
            bodega.Activo = dto.Activo;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> Eliminar(int id)
        {
            var bodega = await _context.Bodegas.FindAsync(id);

            if (bodega == null)
                return false;

            _context.Bodegas.Remove(bodega);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}