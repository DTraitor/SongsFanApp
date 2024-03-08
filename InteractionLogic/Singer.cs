namespace InteractionLogic;

public class Singer
{
    public Guid ID { get; set; }
    public string Name { get; set; }
    public List<Guid> SongID { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is Singer singer &&
                ID.Equals(singer.ID) &&
                Name == singer.Name &&
                SongID.SequenceEqual(singer.SongID);
    }
}