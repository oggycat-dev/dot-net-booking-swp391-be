namespace CleanArchitectureTemplate.Domain.Commons;

/// <summary>
/// Extension methods for BaseEntity
/// </summary>
public static class BaseEntityExtensions
{
    /// <summary>
    /// Marks an entity as deleted (soft delete)
    /// </summary>
    /// <param name="entity">The entity to mark as deleted</param>
    /// <param name="deletedBy">ID of user who deleted the entity</param>
    public static void MarkAsDeleted(this BaseEntity entity, Guid? deletedBy = null)
    {
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        entity.DeletedBy = deletedBy;
    }

    /// <summary>
    /// Updates the modification timestamp and user
    /// </summary>
    /// <param name="entity">The entity to update</param>
    /// <param name="modifiedBy">ID of user who modified the entity</param>
    public static void MarkAsModified(this BaseEntity entity, Guid? modifiedBy = null)
    {
        entity.ModifiedAt = DateTime.UtcNow;
        entity.ModifiedBy = modifiedBy;
    }

    /// <summary>
    /// Sets the creation timestamp and user
    /// </summary>
    /// <param name="entity">The entity to initialize</param>
    /// <param name="createdBy">ID of user who created the entity</param>
    public static void MarkAsCreated(this BaseEntity entity, Guid? createdBy = null)
    {
        entity.CreatedAt = DateTime.UtcNow;
        entity.CreatedBy = createdBy;
    }
}
