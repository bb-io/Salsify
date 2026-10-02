using Apps.Salsify.Events.Polling.Models.Memory;
using Blackbird.Applications.Sdk.Common.Polling;

namespace Apps.Salsify.Helpers.Event;

public static class PollingResult
{
    public static PollingEventResponse<DateMemory, TResult> DoNotFly<TResult>(DateTime startPollingTime)
        where TResult : class
    {
        return new()
        {
            FlyBird = false, 
            Memory = new DateMemory { LastPollingTime = startPollingTime }, 
            Result = null
        };
    }
    
    public static PollingEventResponse<TMemory, TResult> DoNotFly<TMemory, TResult>(TMemory memory)
        where TResult : class
    {
        return new()
        {
            FlyBird = false, 
            Memory = memory, 
            Result = null
        };
    }

    public static PollingEventResponse<DateMemory, TResult> Fly<TResult>(TResult result, DateTime startPollingTime)
        where TResult : class
    {
        return new()
        {
            FlyBird = true, 
            Memory = new DateMemory { LastPollingTime = startPollingTime }, 
            Result = result
        };
    }

    public static PollingEventResponse<TMemory, TResult> Fly<TMemory, TResult>(TMemory memory, TResult result)
        where TResult : class
    {
        return new()
        {
            FlyBird = true, 
            Memory = memory, 
            Result = result
        };
    }
}