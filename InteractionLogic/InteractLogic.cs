using System.Reflection.Metadata;
using Database;

namespace InteractionLogic;

public class InteractLogic : IDisposable
{
    public InteractLogic(string path)
    {
        readWriter = new ReadWriter<Tuple<List<Singer>, List<Song>, List<Disk>>>(path);
        ReadOrCreateData();
    }

    public InteractLogic(FileStream stream)
    {
        readWriter = new ReadWriter<Tuple<List<Singer>, List<Song>, List<Disk>>>(stream);
        ReadOrCreateData();
    }

    public void Dispose()
    {
        SaveChanges();
        readWriter.Dispose();
    }

    public Singer CreateSinger(string name)
    {
        Singer singer = new Singer()
        {
            ID = Guid.NewGuid(),
            Name = name,
            SongID = new List<Guid>()
        };
        singers.Add(singer);
        SaveChanges();
        return singer;
    }

    public Disk CreateDisk(string name)
    {
        Disk disk = new Disk()
        {
            ID = Guid.NewGuid(),
            Name = name,
            SongID = new List<Guid>()
        };
        disks.Add(disk);
        SaveChanges();
        return disk;
    }

    public Song CreateSong(string name, string genre, Guid artistID)
    {
        Song song = new Song()
        {
            ID = Guid.NewGuid(),
            Name = name,
            Genre = genre,
            ArtistID = artistID,
            DiskID = new List<Guid>()
        };
        Singer? artist = singers.Find(singer => singer.ID == artistID);
        if (artist is not null)
            artist.SongID.Add(song.ID);
        else
            song.ArtistID = Guid.Empty;
        songs.Add(song);
        SaveChanges();
        return song;
    }

    public void DeleteSinger(Singer singer)
    {
        singers.Remove(singer);
        foreach (var song in songs)
            if (song.ArtistID == singer.ID)
                song.ArtistID = Guid.Empty;
        SaveChanges();
    }

    public void DeleteDisk(Disk disk)
    {
        foreach (var song in songs)
            if (song.DiskID.Contains(disk.ID))
                song.DiskID.Remove(disk.ID);
        disks.Remove(disk);
        SaveChanges();
    }

    public void DeleteSong(Song song)
    {
        foreach (var disk in disks)
            if (disk.SongID.Contains(song.ID))
                disk.SongID.Remove(song.ID);
        if (song.ArtistID != Guid.Empty)
            singers.Find(singer => singer.ID == song.ArtistID)?.SongID.Remove(song.ID);
        songs.Remove(song);
        SaveChanges();
    }

    public void ChangeName(Song song, string name)
    {
        song.Name = name;
        SaveChanges();
    }

    public void ChangeName(Singer singer, string name)
    {
        singer.Name = name;
        SaveChanges();
    }

    public void ChangeName(Disk disk, string name)
    {
        disk.Name = name;
        SaveChanges();
    }

    public void ChangeGenre(Song song, string genre)
    {
        song.Genre = genre;
        SaveChanges();
    }

    public void ChangeArtist(Song song, Guid artistID)
    {
        if (song.ArtistID != Guid.Empty)
            singers.Find(singer => singer.ID == song.ArtistID)?.SongID.Remove(song.ID);
        song.ArtistID = artistID;
        singers.Find(singer => singer.ID == artistID)?.SongID.Add(song.ID);
        SaveChanges();
    }

    public List<Song> GetSongs()
    {
        return songs;
    }

    public List<Song> GetSongs(Predicate<Song> match)
    {
        return songs.FindAll(match);
    }

    public List<Singer> GetSingers()
    {
        return singers;
    }

    public Singer? GetSinger(Guid id)
    {
        return singers.Find(singer => singer.ID == id);
    }

    public List<Disk> GetDisks()
    {
        return disks;
    }

    public List<Disk> GetDisks(Predicate<Disk> match)
    {
        return disks.FindAll(match);
    }

    public void AddSongToDisk(Song song, Disk disk)
    {
        disk.SongID.Add(song.ID);
        song.DiskID.Add(disk.ID);
        SaveChanges();
    }

    public void RemoveSongFromDisk(Song song, Disk disk)
    {
        disk.SongID.Remove(song.ID);
        song.DiskID.Remove(disk.ID);
        SaveChanges();
    }

    private void ReadOrCreateData()
    {
        var fileData = readWriter.Read();
        if (fileData is null)
        {
            singers = new List<Singer>();
            songs = new List<Song>();
            disks = new List<Disk>();
            data = new Tuple<List<Singer>, List<Song>, List<Disk>>(singers, songs, disks);
        }
        else
        {
            singers = fileData.Item1;
            songs = fileData.Item2;
            disks = fileData.Item3;
            data = fileData;
        }
    }

    private void SaveChanges()
    {
        readWriter.Save(data);
    }

    private ReadWriter<Tuple<List<Singer>, List<Song>, List<Disk>>> readWriter;
    private Tuple<List<Singer>, List<Song>, List<Disk>> data;
    private List<Singer> singers;
    private List<Song> songs;
    private List<Disk> disks;
}