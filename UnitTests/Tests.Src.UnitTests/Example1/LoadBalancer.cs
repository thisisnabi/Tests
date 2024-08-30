
namespace Tests.Src.UnitTests.Example1;
public class LoadBalancer
{
    private readonly string[] _connections;
    private int currentIndex = -1;
    private const int charOffset = 97;

    public LoadBalancer(string[] connections)
    {
        _connections = connections;
    }

    public int TenantId => currentIndex;
    public string TenantConnectionString => _connections[currentIndex];

    public Tenant MoveNext()
    {
        currentIndex++;
        if (currentIndex > 2)
            currentIndex = 0;

        var currentChar = (char)(charOffset + currentIndex);
        return new Tenant(currentIndex + 1, _connections[currentIndex], currentChar);
    }

    internal string GetConnectionStringByPredicateId(char v)
    {
        var wordIndex = (int)v;
        var index = wordIndex - charOffset;

        //if (index >= _connections.Length)
        //    throw new IndexOutOfRangeException(nameof(_connections));

        return _connections[index];
    }

    public IEnumerable<string> GetConnections() => _connections;
}

public record Tenant(int Id, string ConnectionString, char Predicate);