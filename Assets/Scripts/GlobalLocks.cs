using System.Threading;

public static class GlobalLocks
{
    public static int uiLock = 0;

    public static bool TryLock(ref int flag)
    {
        return Interlocked.CompareExchange(ref flag, 1, 0) == 0;
    }

    public static void UnLock(ref int flag)
    {
        Interlocked.CompareExchange(ref flag, 0, 1);
    }
}