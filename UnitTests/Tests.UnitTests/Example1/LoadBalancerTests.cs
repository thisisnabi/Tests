using FluentAssertions;
using Tests.Src.UnitTests.Example1;

namespace Tests.UnitTests.Example1;

public class LoadBalancerTests
{
    private readonly LoadBalancer _sut;

    // setup
    public LoadBalancerTests()
    {
        _sut = new LoadBalancer(["conn1", "conn2", "conn3"]);
    }


    [Fact]
    public void MoveNext_ShouldRotateThroughConnections()
    {
        // act
        var tenant1 = _sut.MoveNext();
        var tenant2 = _sut.MoveNext();
        var tenant3 = _sut.MoveNext();
        var tenant4 = _sut.MoveNext();

        // assert
        tenant1.ConnectionString.Should().Be("conn1");
        tenant1.Id.Should().Be(1);
        tenant1.Predicate.Should().Be('a');

        tenant2.ConnectionString.Should().Be("conn2");
        tenant2.Id.Should().Be(2);
        tenant2.Predicate.Should().Be('b');

        tenant3.ConnectionString.Should().Be("conn3");
        tenant3.Id.Should().Be(3);
        tenant3.Predicate.Should().Be('c');

        tenant4.ConnectionString.Should().Be("conn1");
        tenant4.Id.Should().Be(1);
        tenant4.Predicate.Should().Be('c');
    }

    [Theory]
    [InlineData('a', "conn1")]
    [InlineData('b', "conn2")]
    [InlineData('c', "conn3")]
    public void GetConnectionStringByPredicateId_ShouldReturnCorrectConnectionString(char act, string expected)
    {
        // act
        var conn = _sut.GetConnectionStringByPredicateId(act);

        // assert
        conn.Should().Be(expected);   
    }

    [Fact]
    public void GetConnectionStringByPredicateId_ShouldThrowExceptionInMaxIndex()
    {
        // act
        var connFunc = () => _sut.GetConnectionStringByPredicateId('d');

        // assert
        connFunc.Should().Throw<IndexOutOfRangeException>();
    }
}
