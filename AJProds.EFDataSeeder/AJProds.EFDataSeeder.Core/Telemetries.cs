using System.Diagnostics;

namespace AJProds.EFDataSeeder.Core
{
    /// <summary>
    /// The default root instances for traces and metrics
    /// </summary>
    public static class Telemetries
    {
        /// <summary>
        /// The default or root <see cref="ActivitySource"/>
        /// </summary>
        public static ActivitySource? DefaultActivitySource { get; set; }
    }
}