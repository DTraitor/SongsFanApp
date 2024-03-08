using System.Text.Json;

namespace Database;

public class ReadWriter<T> : IDisposable
{
    public ReadWriter(string file)
    {
        stream = new FileStream(file, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
        Read();
    }

    public ReadWriter(FileStream stream)
    {
        streamOwned = false;
        this.stream = stream;
    }

    public void Dispose()
    {
        if (streamOwned)
            stream.Dispose();
    }

    public T? Read()
    {
        stream.Position = 0;
        using StreamReader reader = new StreamReader(stream, leaveOpen: true);
        try
        {
            return JsonSerializer.Deserialize<T>(reader.ReadToEnd());
        }
        catch (JsonException e)
        {
            return default;
        }
    }

    public void Save(T obj)
    {
        stream.SetLength(0);
        using StreamWriter writer = new StreamWriter(stream, leaveOpen: true);
        writer.Write(JsonSerializer.Serialize(obj));
    }

    private bool streamOwned = true;
    private readonly FileStream stream;
}