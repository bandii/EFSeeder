using System;

namespace AJProds.EFDataSeeder.Core.Db
{
    /// <summary>
    /// Stores information regarding when did a seed run
    /// </summary>
    public class SeederHistory
    {
        /// <summary>
        /// Unique identifier for this seed
        /// </summary>
        public int Id { get; set; }
    
        /// <summary>
        /// Stamp, when did the seed run for the first time
        /// </summary>
        public DateTime FirstRunAt { get; set; }
    
        /// <summary>
        /// Stamp, when did the seed run for the last time
        /// </summary>
        public DateTime LastRunAt { get; set; }
    
        /// <summary>
        /// Unique name for this seed
        /// </summary>
        public string SeedName { get; set; }
    
        /// <summary>
        /// Should this seed run everytime when the seed procedures got triggered?
        /// </summary>
        public bool AlwaysRun { get; set; }
    }
}