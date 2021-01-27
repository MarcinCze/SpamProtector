using System;

namespace ProtectorLib.Providers
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime CurrentTime => DateTime.Now;
    }
}
