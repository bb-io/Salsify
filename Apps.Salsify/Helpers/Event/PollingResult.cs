using Apps.Salsify.Events.Polling.Models;
using Blackbird.Applications.Sdk.Common.Polling;

namespace Apps.Salsify.Helpers.Event;

public static class PollingResult
{
    public static PollingEventResponse<PollingMemory, TResult> Baseline<TResult>(DateTime startPollingTime)
        where TResult : class
    {
        return new()
        {
            FlyBird = false, 
            Memory = new PollingMemory { LastPollingTime = startPollingTime }, 
            Result = null
        };
    }

    public static PollingEventResponse<PollingMemory, TResult> NoChanges<TResult>(DateTime startPollingTime)
        where TResult : class
    { 
        return new()
        {
            FlyBird = false, 
            Memory = new PollingMemory { LastPollingTime = startPollingTime }, 
            Result = null
        };
    }

    public static PollingEventResponse<PollingMemory, TResult> Fly<TResult>(TResult result, DateTime startPollingTime)
        where TResult : class
    {
        return new()
        {
            FlyBird = true, 
            Memory = new PollingMemory { LastPollingTime = startPollingTime }, 
            Result = result
        };
    }
}