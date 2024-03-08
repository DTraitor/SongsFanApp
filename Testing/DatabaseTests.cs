namespace Testing;

public class DatabaseTests
{
    private const string TestFileName = "DatabaseTest.json";

    [TearDown]
    public void CleanUp()
    {
        File.Delete(TestFileName);
    }

    [Test]
    public void DatabaseTest()
    {
        FileStream stream = new FileStream(TestFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
        Singer singer = new Singer()
        {
            ID = Guid.NewGuid(),
            Name = "TestName",
            SongID = new List<Guid>()
        };
        stream.SetLength(0);

        ReadWriter<Singer> database = new (stream);
        Assert.That(database.Read(), Is.EqualTo(null));
        database.Save(singer);
        database.Dispose();
        stream.Close();

        database = new ReadWriter<Singer>(TestFileName);
        Assert.That(database.Read(), Is.EqualTo(singer));
        database.Dispose();
    }
}