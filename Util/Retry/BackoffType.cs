namespace Util.Retry
{
    public enum BackoffType
    {
        Fixed = 0,
        Linear = 1,
        Exponential = 2,
        Random = 3
    }
}
