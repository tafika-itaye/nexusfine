namespace NexusFine.Core.Entities;

/// <summary>
/// Authenticated user of the NexusFine system. Back-office staff, supervisors,
/// admins, and officers all share this table; an Officer row is linked via
/// OfficerId when the user is a field officer.
/// </summary>
public class AppUser
{
    public int Id { get; set; }

    public string UserName { get; set; } = string.Empty;    // login handle
    public string Email    { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;
    public string PasswordSalt { get; set; } = string.Empty;

    // Comma-separated role list: "Admin,Supervisor" etc.
    public string Roles { get; set; } = string.Empty;

    public int? OfficerId { get; set; }
    public Officer? Officer { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public static class AppRoles
{
    public const string Admin      = "Admin";
    public const string Supervisor = "Supervisor";
    public const string Officer    = "Officer";
}
