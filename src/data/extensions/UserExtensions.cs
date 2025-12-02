using talearc_backend.src.data.dto;
using talearc_backend.src.data.entities;

namespace talearc_backend.src.data.extensions;

public static class UserExtensions
{
    public static UserDto ToDto(this User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            CreateAt = user.CreateAt
        };
    }
}