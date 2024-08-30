using FluentAssertions;
using Tests.Src.UnitTests.Examples;
using Xunit.Abstractions;

namespace Tests.UnitTests.Example5;

public class CallGroupTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public CallGroupTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;

    }

    [Fact]
    public void Constractor_ShouldThrowArgumentException_WhenParticipantIsZeroOrNegative()
    {
        Func<IReadOnlyCollection<string>, Task> func = _ => Task.FromResult(0);

        var funcWithZero = () => new CallGroup<string>(0, func, TimeSpan.FromSeconds(1));
        var funcWithNegative = () => new CallGroup<string>(-1, func, TimeSpan.FromSeconds(1));

        funcWithZero.Should().Throw<ArgumentException>().WithMessage("Value 0 should be greater than zero! (Parameter 'participantCount')");
        funcWithNegative.Should().Throw<ArgumentException>().WithMessage("Value -1 should be greater than zero! (Parameter 'participantCount')");
    }


    [Fact]
    public async Task Join_ToMAny()
    {
        var students = new List<string>();
        var func = new Func<IReadOnlyCollection<string>, Task>((requests) =>
        {
            students.AddRange(requests);
            return Task.CompletedTask;
        });

        var group = new CallGroup<string>(3, func, TimeSpan.FromSeconds(3));

        var t1 = group.Join("Helia");
        var t2 = group.Join("Samane");
        var t3 = group.Join("Nader");
    
        var action = async () => await group.Join("Sheeva");;
        await action.Should().ThrowAsync<Exception>().WithMessage("Too many participants!");

    }


    [Fact]
    public async Task Join()
    {
        var students = new List<string>();
        var expectedStudents = new List<string>() { "Helia", "Samane", "Nader" };
        var func = new Func<IReadOnlyCollection<string>, Task>((requests) =>
        {
            students.AddRange(requests);
            return Task.CompletedTask;
        });

        var group = new CallGroup<string>(3, func, TimeSpan.FromSeconds(3));

        var t1 = group.Join("Helia");
        var t2 = group.Join("Samane");
        var t3 = group.Join("Nader");

       await Task.WhenAll(t1, t2, t3);

        students.Should().HaveCount(3);
        students.Should().BeEquivalentTo(expectedStudents);
    }



    [Fact]
    public async Task Timeout()
    {
        var students = new List<string>();
        var expectedStudents = new List<string>() { "Helia", "Samane", "Nader" };
        var func = new Func<IReadOnlyCollection<string>, Task>((requests) =>
        {
            students.AddRange(requests);
            return Task.CompletedTask;
        });

        var callGroup = new CallGroup<string>(3, func, TimeSpan.FromMilliseconds(10));

        await Task.Delay(TimeSpan.FromSeconds(1));
       
        var action = async () => await callGroup.Join("Nabi");
          
        await action.Should().ThrowAsync<Exception>().WithMessage("The operation has timed out.");

    }



}
