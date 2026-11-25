using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SistemaHotelero.Dominio.InterfazAD;
using SistemaHotelero.Utilitarios;
using System.Linq.Expressions;

namespace SistemaHotelero.AccesoDatos.Implementaciones;

public class RepositorioAD<TEntity> : IRepositorioAD<TEntity>
    where TEntity : class
{
    protected readonly DbContext _context;

    public RepositorioAD(DbContext context)
    {
        _context = context;
    }

    public async Task<Respuesta<TEntity>> InsertarAsync(TEntity objEntidad)
    {
        Respuesta<TEntity> objRespuesta = new();

        try
        {
            await _context.Set<TEntity>().AddAsync(objEntidad);
            await _context.SaveChangesAsync();
            objRespuesta.Data = objEntidad;
        }
        catch (Exception ex)
        {
            // Si queda en el contexto, el siguiente SaveChanges vuelve a fallar con ella
            _context.Entry(objEntidad).State = EntityState.Detached;
            LlenarError(objRespuesta, ex);
        }

        return objRespuesta;
    }

    public async Task<Respuesta<TEntity>> ModificarAsync(TEntity objEntidad)
    {
        Respuesta<TEntity> objRespuesta = new();

        try
        {
            _context.Set<TEntity>().Update(objEntidad);
            await _context.SaveChangesAsync();
            objRespuesta.Data = objEntidad;
        }
        catch (Exception ex)
        {
            _context.Entry(objEntidad).State = EntityState.Detached;
            LlenarError(objRespuesta, ex);
        }

        return objRespuesta;
    }

    public async Task<Respuesta<bool>> EliminarAsync(TEntity objEntidad)
    {
        Respuesta<bool> objRespuesta = new();

        try
        {
            _context.Entry(objEntidad).State = EntityState.Deleted;
            await _context.SaveChangesAsync();
            objRespuesta.Data = true;
        }
        catch (Exception ex)
        {
            _context.Entry(objEntidad).State = EntityState.Unchanged;
            LlenarError(objRespuesta, ex);
            objRespuesta.Data = false;
        }

        return objRespuesta;
    }

    public async Task<Respuesta<IEnumerable<TEntity>>> ListarAsync(
        List<string>? objIncludes = null)
    {
        Respuesta<IEnumerable<TEntity>> objRespuesta = new();

        try
        {
            IQueryable<TEntity> consulta = _context.Set<TEntity>();

            if (objIncludes != null)
            {
                objIncludes.ForEach(
                    include => consulta = consulta.Include(include));
            }

            objRespuesta.Data = await consulta.ToListAsync();
        }
        catch (Exception ex)
        {
            LlenarError(objRespuesta, ex);
        }

        return objRespuesta;
    }

    public async Task<Respuesta<IEnumerable<TEntity>>> BuscarAsync(
        Expression<Func<TEntity, bool>> objPredicado,
        List<string>? objIncludes = null)
    {
        Respuesta<IEnumerable<TEntity>> objRespuesta = new();

        try
        {
            IQueryable<TEntity> consulta = _context.Set<TEntity>();

            if (objIncludes != null)
            {
                objIncludes.ForEach(
                    include => consulta = consulta.Include(include));
            }

            objRespuesta.Data = await consulta
                .Where(objPredicado)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            LlenarError(objRespuesta, ex);
        }

        return objRespuesta;
    }

    public async Task<Respuesta<TEntity>> ObtenerEntidadAsync(
        Expression<Func<TEntity, bool>> objPredicado,
        List<string>? objIncludes = null)
    {
        Respuesta<TEntity> objRespuesta = new();

        try
        {
            IQueryable<TEntity> consulta = _context.Set<TEntity>();

            if (objIncludes != null)
            {
                objIncludes.ForEach(
                    include => consulta = consulta.Include(include));
            }

            objRespuesta.Data = await consulta
                .FirstOrDefaultAsync(objPredicado);
        }
        catch (Exception ex)
        {
            LlenarError(objRespuesta, ex);
        }

        return objRespuesta;
    }

    public async Task<Respuesta<int?>> ContarAsync(
        Expression<Func<TEntity, bool>> objPredicado,
        List<string>? objIncludes = null)
    {
        Respuesta<int?> objRespuesta = new();

        try
        {
            objRespuesta.Data = await _context.Set<TEntity>()
                .CountAsync(objPredicado);
        }
        catch (Exception ex)
        {
            LlenarError(objRespuesta, ex);
        }

        return objRespuesta;
    }

    // Traduce los errores de SQL Server a los códigos que entiende la API
    private static void LlenarError<T>(Respuesta<T> objRespuesta, Exception ex)
    {
        objRespuesta.Success = false;
        objRespuesta.Data = default;
        objRespuesta.Error = ex.GetBaseException().Message;
        objRespuesta.Codigo = CodigosRespuesta.Error;

        if (ex is DbUpdateConcurrencyException)
        {
            objRespuesta.Codigo = CodigosRespuesta.Conflicto;
            objRespuesta.Error = "El registro fue modificado por otro usuario. Vuelva a cargarlo.";
            return;
        }

        if (ex.GetBaseException() is SqlException sql)
        {
            switch (sql.Number)
            {
                case 2601: // índice único
                case 2627: // restricción UNIQUE o PK
                    objRespuesta.Codigo = CodigosRespuesta.Conflicto;
                    break;
                case 547:  // FK o CHECK
                    objRespuesta.Codigo = CodigosRespuesta.Validacion;
                    break;
            }
        }
    }
}
