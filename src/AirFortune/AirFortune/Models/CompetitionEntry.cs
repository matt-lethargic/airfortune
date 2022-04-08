namespace AirFortune.Models;

public class CompetitionEntry
{
    public string Name { get; }
    public string Email { get; }
    public string Telephone { get; }
    public bool Selected { get; set; }

    public CompetitionEntry(string name)
    {
        Name = name;
    }
}