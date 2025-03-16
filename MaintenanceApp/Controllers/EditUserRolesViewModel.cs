using System.Collections.Generic;

public class EditUserRolesViewModel
{
    public string UserId { get; set; }
    public string Email { get; set; }
    public List<string> AvailableRoles { get; set; }
    public List<string> AssignedRoles { get; set; }
}