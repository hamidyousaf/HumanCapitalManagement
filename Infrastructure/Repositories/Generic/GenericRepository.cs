using Domain.Abstractions.GenericRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Repositories.Generic;

public class GenericRepository<TEntity> : IGenericRepository<TEntity>
     where TEntity : class
{

    private readonly DbContext _dbContext;

    public GenericRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public virtual IQueryable<TEntity> GetWithCondition(Expression<Func<TEntity, bool>> expression)
    {
        return _dbContext.Set<TEntity>().Where(expression);
    }

    public virtual IQueryable<TEntity> GetWithConditionReadOnly(Expression<Func<TEntity, bool>> expression)
    {
        return _dbContext.Set<TEntity>().AsNoTracking().Where(expression);
    }

    public IQueryable<TEntity> GetAll()
    {
        return _dbContext.Set<TEntity>();
    }

    public IQueryable<TEntity> GetAllReadOnly()
    {
        return _dbContext.Set<TEntity>().AsNoTracking();
    }

    public async Task<TEntity> GetById<T>(T id)
    {
        return await _dbContext.Set<TEntity>().FindAsync(id);
    }

    public async Task Add(TEntity entity)
    {
        SetCreateAnalysisValue(entity);
        await _dbContext.Set<TEntity>().AddAsync(entity);
    }

    public void Change(TEntity entity)
    {
        SetUpdateAnalysisValue(entity, false);
        _dbContext.Set<TEntity>().Update(entity);
    }

    public async Task DeleteById<T>(T id)
    {
        var entity = await GetById(id);
        if (entity != null)
        {
            _dbContext.Set<TEntity>().Remove(entity);
        }
    }

    public void DeleteByEntity<T>(TEntity entity)
    {
        if (entity != null)
        {
            _dbContext.Set<TEntity>().Remove(entity);
        }
    }

    public async Task DeleteRange<T>(List<T> ids)
    {
        foreach (var item in ids)
        {
            await DeleteById(item);
        }
    }

    public async Task SoftDeleteById<T>(T id)
    {
        var entity = await GetById(id);
        if (entity != null)
        {
            var entityProperties = entity.GetType().GetProperties();
            var property = entityProperties.FirstOrDefault(x => x.Name.ToLower() == "isdeleted");
            if (property != null)
                property.SetValue(entity, true);
        }
    }

    public IQueryable<TEntity> GetNotDeleted()
    {
        object deleted = true;
        return GetAll().Where(x => typeof(TEntity).GetProperty("IsDeleted").GetValue(x) != deleted);
    }

    public void ChangeRange(List<TEntity> entities)
    {
        foreach (var item in entities)
            SetUpdateAnalysisValue(item, false);
        _dbContext.Set<TEntity>().UpdateRange(entities);
    }

    public void RemoveRange(List<TEntity> entities)
    {
        _dbContext.RemoveRange(entities);
    }

    public async Task AddRangeAsync(List<TEntity> entities)
    {
        foreach (var item in entities)
            SetCreateAnalysisValue(item);
        await _dbContext.AddRangeAsync(entities);
    }

    private void SetCreateAnalysisValue(TEntity entity)
    {
        var entityProperties = entity.GetType().GetProperties();
        var createdDateProperty = entityProperties.FirstOrDefault(x => x.Name.ToLower() == "createddate");
        var isActiveProperty = entityProperties.FirstOrDefault(x => x.Name.ToLower() == "isactive");
        if (createdDateProperty != null)
            createdDateProperty.SetValue(entity, DateTime.UtcNow);
        if (isActiveProperty != null)
            isActiveProperty.SetValue(entity, true);
    }

    private void SetUpdateAnalysisValue(TEntity entity, bool isSoftDelete)
    {
        var entityProperties = entity.GetType().GetProperties();
        var property = entityProperties.FirstOrDefault(x => x.Name.ToLower() == "modifieddate");
        if (property != null)
            property.SetValue(entity, DateTime.UtcNow);

        if (isSoftDelete)
        {
            property = entityProperties.FirstOrDefault(x => x.Name.ToLower() == "isdeleted");
            if (property != null)
                property.SetValue(entity, true);
        }
    }
}
