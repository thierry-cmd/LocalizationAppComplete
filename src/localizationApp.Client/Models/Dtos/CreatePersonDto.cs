namespace localizationApp.Client.Models.Dtos;

public class CreatePersonDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime? BirthDate { get; set; }
    public Gender Gender { get; set; } = Gender.Other;
    public PersonStatus Status { get; set; } = PersonStatus.Pending;
}

