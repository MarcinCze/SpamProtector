using System;

namespace ProtectorLib.Providers
{
    public interface IDateTimeProvider
    {
        DateTime CurrentTime { get; }
    }
}
