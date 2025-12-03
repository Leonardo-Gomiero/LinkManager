namespace LinkManager.Api.Entities;

public class Link {

    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;

    public int UserId { get; set; }

    public User? User { get; set; }

}