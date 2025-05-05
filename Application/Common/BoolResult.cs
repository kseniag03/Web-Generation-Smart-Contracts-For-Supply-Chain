namespace Application.Common
{
    public sealed record BoolResult(bool Succeeded, string? Error = null)
    {
        public static BoolResult Ok() => new(true, null);
        public static BoolResult Fail(string error) => new(false, error);
    }
}
