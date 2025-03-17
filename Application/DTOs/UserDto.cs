﻿using Core.Enums;

namespace Application.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
    }

}
