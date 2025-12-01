namespace CleanArchitectureTemplate.Application.Common.DTOs;

/// <summary>
/// Base response DTO with standard structure
/// </summary>
/// <typeparam name="T">Data type</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// HTTP status code
    /// </summary>
    public int StatusCode { get; set; }
    
    /// <summary>
    /// Indicates if the operation was successful
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// Response message
    /// </summary>
    public string Message { get; set; } = string.Empty;
    
    /// <summary>
    /// Response data
    /// </summary>
    public T? Data { get; set; }
    
    /// <summary>
    /// List of errors
    /// </summary>
    public List<string>? Errors { get; set; }
    
    /// <summary>
    /// Timestamp of the response
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Creates a successful response with 200 OK
    /// </summary>
    /// <param name="data">Response data</param>
    /// <param name="message">Success message</param>
    /// <returns>Successful API response</returns>
    public static ApiResponse<T> Ok(T data, string message = "Operation completed successfully")
    {
        return new ApiResponse<T>
        {
            StatusCode = 200,
            Success = true,
            Message = message,
            Data = data,
            Timestamp = DateTime.UtcNow
        };
    }
    
    /// <summary>
    /// Creates a successful response with 201 Created
    /// </summary>
    /// <param name="data">Response data</param>
    /// <param name="message">Success message</param>
    /// <returns>Successful API response</returns>
    public static ApiResponse<T> Created(T data, string message = "Resource created successfully")
    {
        return new ApiResponse<T>
        {
            StatusCode = 201,
            Success = true,
            Message = message,
            Data = data,
            Timestamp = DateTime.UtcNow
        };
    }
    
    /// <summary>
    /// Creates a successful response with 204 No Content
    /// </summary>
    /// <param name="message">Success message</param>
    /// <returns>Successful API response</returns>
    public static ApiResponse<T> NoContent(string message = "Operation completed successfully")
    {
        return new ApiResponse<T>
        {
            StatusCode = 204,
            Success = true,
            Message = message,
            Timestamp = DateTime.UtcNow
        };
    }
    
    /// <summary>
    /// Creates a failed response with 400 Bad Request
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="errors">List of errors</param>
    /// <returns>Failed API response</returns>
    public static ApiResponse<T> BadRequest(string message = "Bad request", List<string>? errors = null)
    {
        return new ApiResponse<T>
        {
            StatusCode = 400,
            Success = false,
            Message = message,
            Errors = errors ?? new List<string> { message },
            Timestamp = DateTime.UtcNow
        };
    }
    
    /// <summary>
    /// Creates a failed response with 401 Unauthorized
    /// </summary>
    /// <param name="message">Error message</param>
    /// <returns>Failed API response</returns>
    public static ApiResponse<T> Unauthorized(string message = "Unauthorized access")
    {
        return new ApiResponse<T>
        {
            StatusCode = 401,
            Success = false,
            Message = message,
            Errors = new List<string> { message },
            Timestamp = DateTime.UtcNow
        };
    }
    
    /// <summary>
    /// Creates a failed response with 403 Forbidden
    /// </summary>
    /// <param name="message">Error message</param>
    /// <returns>Failed API response</returns>
    public static ApiResponse<T> Forbidden(string message = "Access forbidden")
    {
        return new ApiResponse<T>
        {
            StatusCode = 403,
            Success = false,
            Message = message,
            Errors = new List<string> { message },
            Timestamp = DateTime.UtcNow
        };
    }
    
    /// <summary>
    /// Creates a failed response with 404 Not Found
    /// </summary>
    /// <param name="message">Error message</param>
    /// <returns>Failed API response</returns>
    public static ApiResponse<T> NotFound(string message = "Resource not found")
    {
        return new ApiResponse<T>
        {
            StatusCode = 404,
            Success = false,
            Message = message,
            Errors = new List<string> { message },
            Timestamp = DateTime.UtcNow
        };
    }
    
    /// <summary>
    /// Creates a failed response with 422 Unprocessable Entity (validation errors)
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="errors">List of validation errors</param>
    /// <returns>Failed API response</returns>
    public static ApiResponse<T> ValidationError(string message = "Validation failed", List<string>? errors = null)
    {
        return new ApiResponse<T>
        {
            StatusCode = 422,
            Success = false,
            Message = message,
            Errors = errors ?? new List<string> { message },
            Timestamp = DateTime.UtcNow
        };
    }
    
    /// <summary>
    /// Creates a failed response with 500 Internal Server Error
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="errors">List of errors</param>
    /// <returns>Failed API response</returns>
    public static ApiResponse<T> InternalServerError(string message = "An internal server error occurred", List<string>? errors = null)
    {
        return new ApiResponse<T>
        {
            StatusCode = 500,
            Success = false,
            Message = message,
            Errors = errors ?? new List<string> { message },
            Timestamp = DateTime.UtcNow
        };
    }
    
    /// <summary>
    /// Creates a custom response with specified status code
    /// </summary>
    /// <param name="statusCode">HTTP status code</param>
    /// <param name="success">Success flag</param>
    /// <param name="message">Response message</param>
    /// <param name="data">Response data</param>
    /// <param name="errors">List of errors</param>
    /// <returns>API response</returns>
    public static ApiResponse<T> Custom(int statusCode, bool success, string message, T? data = default, List<string>? errors = null)
    {
        return new ApiResponse<T>
        {
            StatusCode = statusCode,
            Success = success,
            Message = message,
            Data = data,
            Errors = errors,
            Timestamp = DateTime.UtcNow
        };
    }
    
    // Legacy methods for backward compatibility
    [Obsolete("Use Ok() instead")]
    public static ApiResponse<T> SuccessResult(T data, string message = "Operation completed successfully") 
        => Ok(data, message);
    
    [Obsolete("Use BadRequest() instead")]
    public static ApiResponse<T> ErrorResult(string message, List<string>? errors = null) 
        => BadRequest(message, errors);
}
