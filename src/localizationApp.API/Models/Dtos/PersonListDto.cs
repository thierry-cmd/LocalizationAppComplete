namespace localizationApp.API.Models.Dtos;

public class PersonListDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;  // Combinaison FirstName + LastName
    public string Email { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;    // Enum converti en string
}