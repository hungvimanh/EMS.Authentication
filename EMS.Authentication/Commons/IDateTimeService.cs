using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TF.Authentication.Commons
{
    public interface IDateTimeService
    {
        DateTime UtcNow { get; }
    }
    public class DateTimeService : IDateTimeService
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
