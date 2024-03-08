using System.Text.Json.Serialization;

namespace InteractionLogic;

public class Song
{
    public Guid ID { get; set; }
    public string Name { get; set; }
    public string Genre { get; set; }
    public Guid ArtistID { get; set; }
    public List<Guid> DiskID { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is Song song &&
                ID.Equals(song.ID) &&
                Name == song.Name &&
                Genre == song.Genre &&
                ArtistID.Equals(song.ArtistID) &&
                DiskID.SequenceEqual(song.DiskID);
    }
}