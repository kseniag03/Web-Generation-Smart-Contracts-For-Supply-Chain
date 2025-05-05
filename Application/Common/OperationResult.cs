namespace Application.Common
{
    public record OperationResult<T>(
        bool Succeeded,
        T? Payload = default,
        string? Error = null)
    {
        public static OperationResult<T> Ok(T? payload = default)
            => new(true, payload, null);

        public static OperationResult<T> Fail(string error)
            => new(false, default, error);
    }
}
