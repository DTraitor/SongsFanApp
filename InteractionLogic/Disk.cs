namespace InteractionLogic;

public class Disk
{
    public Guid ID { get; set; }
    public string Name { get; set; }
    public List<Guid> SongID { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is Disk disk &&
                ID.Equals(disk.ID) &&
                Name == disk.Name &&
                SongID.SequenceEqual(disk.SongID);
    }
}