﻿namespace NotionBackend.Api.Domain.DTOs.Auth;

public class RequestUser
{
    public Guid UserId { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }
}