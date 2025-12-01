using MediatR;
using CleanArchitectureTemplate.Application.Common.DTOs;

namespace CleanArchitectureTemplate.Application.Common.Models;

/// <summary>
/// Paginated result model
/// </summary>
/// <typeparam name="T">Data type</typeparam>
public class PaginatedResult<T>
{
    /// <summary>
    /// List of items
    /// </summary>
    public List<T> Items { get; set; } = new();
    
    /// <summary>
    /// Total count of items
    /// </summary>
    public int TotalCount { get; set; }
    
    /// <summary>
    /// Current page number
    /// </summary>
    public int PageNumber { get; set; }
    
    /// <summary>
    /// Page size
    /// </summary>
    public int PageSize { get; set; }
    
    /// <summary>
    /// Total pages
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    
    /// <summary>
    /// Whether there is a previous page
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;
    
    /// <summary>
    /// Whether there is a next page
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;
}

/// <summary>
/// Base query for pagination
/// </summary>
public abstract class PaginatedQuery<T> : IRequest<PaginatedResult<T>>
{
    /// <summary>
    /// Page number (1-based)
    /// </summary>
    public int PageNumber { get; set; } = 1;
    
    /// <summary>
    /// Page size
    /// </summary>
    public int PageSize { get; set; } = 10;
    
    /// <summary>
    /// Search term
    /// </summary>
    public string? SearchTerm { get; set; }
}
