namespace localizationApp.Client.Models;

public class Person
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime? BirthDate { get; set; }
    public PersonStatus Status { get; set; } = PersonStatus.Pending;
    public Gender Gender { get; set; } = Gender.Other;


}