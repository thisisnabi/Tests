namespace Tests.Src.UnitTests.Examples;

public class CallGroup<TOperation>
{
    private readonly int initialParticipantCount;
    private readonly Func<IReadOnlyCollection<TOperation>, Task> groupCallDelegate;
    private readonly TimeSpan timeout;
    private readonly List<TOperation> requests;
    private readonly Task task;
    private readonly object @lock = new();
    private readonly TaskCompletionSource barrier;
    private int joinedParticipantCount;
    public CallGroup(int participantCount,
    Func<IReadOnlyCollection<TOperation>, Task> groupCallDelegate,
    TimeSpan timeout)
    {
        if (participantCount <= 0)
        {
            throw new ArgumentException($"Value {participantCount} should be greater than zero!", nameof(participantCount));
        }
        joinedParticipantCount = 0;
        initialParticipantCount = participantCount;
        this.groupCallDelegate = groupCallDelegate;
        this.timeout = timeout;
        requests = new();
        barrier = new();
        task = Task.Run(WaitParticipants);
    }
    public Task Join(TOperation value)
    {
        ProcessNewJoiner(() => requests.Add(value));
        return task;
    }
    private async Task WaitParticipants()
    {
        try
        {
            await barrier.Task.WaitAsync(timeout);
        }
        catch (Exception err)
        {
            throw new Exception(err.Message, err);
        }
        await groupCallDelegate(requests);
    }
    public void Leave()
    {
        ProcessNewJoiner(() => { });
    }
    private void ProcessNewJoiner(Action addState)
    {
        lock (@lock)
        {
            var leftToJoin = initialParticipantCount - joinedParticipantCount;
            if (leftToJoin <= 0)
            {
                throw new Exception("Too many participants!");
            }
            joinedParticipantCount++;
            addState();
            if (leftToJoin == 1)
            {
                barrier.SetResult();
            }
        }
    }
}
