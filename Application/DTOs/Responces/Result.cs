namespace Domain.DTOs.Responces;

public class Result<T>
{
    public bool Succeeded { get; private set; }
    public string Message { get; private set; }
    public T Data { get; private set; }

    private Result(bool succeeded, string message, T data)
    {
        Succeeded = succeeded;
        Message = message;
        Data = data;
    }

    public static Result<T> Success(T data, string message = "")
    {
        return new Result<T>(true, message, data);
    }

    public static Result<T> Fail(string message, T data = default)
    {
        return new Result<T>(false, message, data);
    }
}
