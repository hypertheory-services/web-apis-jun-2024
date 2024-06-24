﻿namespace SoftwareCatalog.Api.Techs;

public class TechEntity
{

    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateTimeOffset DateAdded { get; set; }
    public string? AddedBy { get; set; }
}