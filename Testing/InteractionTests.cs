namespace Testing;

public class InteractionTests
{
    private const string TestFileName = "BusinessLogicTest.json";

    [TearDown]
    public void CleanUp()
    {
        File.Delete(TestFileName);
    }

    [Test]
    public void BusinessLogicTests()
    {
        FileStream stream = new FileStream(TestFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
        stream.SetLength(0);
        InteractLogic interact = new(stream);

        Singer singer = interact.CreateSinger("TestName");
        Assert.That(interact.GetSingers(), Is.EqualTo(new List<Singer> { singer }));
        interact.ChangeName(singer, "TestName2");
        Assert.That(singer.Name, Is.EqualTo("TestName2"));

        Song song = interact.CreateSong("TestSong", "TestGenre", Guid.Empty);
        Assert.That(interact.GetSongs(), Is.EqualTo(new List<Song> { song }));
        interact.ChangeName(song, "TestSong2");
        Assert.That(song.Name, Is.EqualTo("TestSong2"));
        interact.ChangeGenre(song, "TestGenre2");
        Assert.That(song.Genre, Is.EqualTo("TestGenre2"));
        interact.ChangeArtist(song, singer.ID);
        Assert.That(song.ArtistID, Is.EqualTo(singer.ID));

        Disk disk = interact.CreateDisk("TestDisk");
        Assert.That(interact.GetDisks(), Is.EqualTo(new List<Disk> { disk }));
        interact.ChangeName(disk, "TestDisk2");
        Assert.That(disk.Name, Is.EqualTo("TestDisk2"));
        interact.AddSongToDisk(song, disk);
        Assert.That(disk.SongID, Is.EqualTo(new List<Guid> { song.ID }));
        interact.RemoveSongFromDisk(song, disk);
        Assert.That(disk.SongID, Is.EqualTo(new List<Guid> { }));

        Song song2 = interact.CreateSong("TestSong2", "TestGenre2", singer.ID);
        Assert.That(interact.GetSongs(), Is.EqualTo(new List<Song> { song, song2 }));
        interact.ChangeArtist(song2, singer.ID);
        Assert.That(song2.ArtistID, Is.EqualTo(singer.ID));
        Assert.That(interact.GetSongs(song => song.ArtistID == singer.ID), Is.EqualTo(new List<Song>{ song, song2 }));
        Assert.That(interact.GetSingers(), Is.EqualTo(new List<Singer> { singer }));
        Assert.That(interact.GetSinger(singer.ID), Is.EqualTo(singer));
        Assert.That(interact.GetDisks(), Is.EqualTo(new List<Disk> { disk }));
        Assert.That(interact.GetDisks(disk => disk.SongID.Contains(song.ID)), Is.EqualTo(new List<Disk> { }));
        Assert.That(interact.GetDisks(disk => disk.SongID.Contains(song2.ID)), Is.EqualTo(new List<Disk> { }));

        stream.Close();

        interact = new(TestFileName);

        Assert.That(interact.GetSongs(), Is.EqualTo(new List<Song> { song, song2 }));
        Assert.That(interact.GetSongs(song => song.ArtistID == singer.ID), Is.EqualTo(new List<Song> { song, song2 }));
        Assert.That(interact.GetSingers(), Is.EqualTo(new List<Singer> { singer }));
        Assert.That(interact.GetSinger(singer.ID), Is.EqualTo(singer));
        Assert.That(interact.GetDisks(), Is.EqualTo(new List<Disk> { disk }));
        Assert.That(interact.GetDisks(disk => disk.SongID.Contains(song.ID)), Is.EqualTo(new List<Disk> { }));
        Assert.That(interact.GetDisks(disk => disk.SongID.Contains(song2.ID)), Is.EqualTo(new List<Disk> { }));

        song2 = interact.CreateSong("TestSong3", "TestGenre3", singer.ID);
        interact.AddSongToDisk(interact.GetSongs()[1], interact.GetDisks()[0]);
        interact.DeleteSong(interact.GetSongs()[1]);
        Assert.That(interact.GetSongs(), Is.EqualTo(new List<Song> { song, song2 }));
        Assert.That(interact.GetSongs(song => song.ArtistID == singer.ID), Is.EqualTo(new List<Song> { song, song2 }));
        interact.AddSongToDisk(interact.GetSongs()[0], interact.GetDisks()[0]);
        interact.AddSongToDisk(song2, interact.GetDisks()[0]);
        interact.DeleteSinger(interact.GetSinger(singer.ID));
        Assert.That(interact.GetSingers(), Is.EqualTo(new List<Singer> { }));
        Assert.That(interact.GetSinger(singer.ID), Is.EqualTo(null));
        interact.DeleteDisk(interact.GetDisks()[0]);
        Assert.That(interact.GetDisks(), Is.EqualTo(new List<Disk> { }));
        interact.DeleteSong(interact.GetSongs()[0]);
        Assert.That(interact.GetSongs(), Is.EqualTo(new List<Song> { song2 }));
        Assert.That(interact.GetSongs(song => song.ArtistID == singer.ID), Is.EqualTo(new List<Song> { }));

        interact.Dispose();
    }
}