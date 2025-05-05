using Application.DTOs;

namespace Application.Common
{
    public sealed record UserResult(bool Succeeded, UserDto? Payload = default, string? Error = null)
        : OperationResult<UserDto>(Succeeded, Payload, Error)
    {
        public static new UserResult Ok(UserDto? user = default)
            => new(true, user, null);

        public static new UserResult Fail(string error)
            => new(false, null, error);
    }
}
