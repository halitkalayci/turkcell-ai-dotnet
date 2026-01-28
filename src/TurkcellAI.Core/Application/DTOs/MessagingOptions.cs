using System;

namespace TurkcellAI.Core.Application.DTOs
{
    /// <summary>
    /// Configuration options for messaging. Bind via IOptions and validate at startup.
    /// </summary>
    public sealed class MessagingOptions
    {
        public bool Enabled { get; set; } = true;

        public string? Host { get; set; }
        public string? VirtualHost { get; set; } = "/";
        public string? Username { get; set; }
        public string? Password { get; set; }

        public int Prefetch { get; set; } = 32;
        public bool PublisherConfirmEnabled { get; set; } = true;

        public bool OutboxEnabled { get; set; } = true;
        public bool InboxEnabled { get; set; } = true;

        public RetryOptions Retry { get; set; } = new();
        public DeadLetterOptions DeadLetter { get; set; } = new();

        public sealed class RetryOptions
        {
            public int MaxAttempts { get; set; } = 5;
            public TimeSpan MinBackoff { get; set; } = TimeSpan.FromSeconds(1);
            public TimeSpan MaxBackoff { get; set; } = TimeSpan.FromSeconds(30);
            public double IntervalFactor { get; set; } = 2.0;
        }

        public sealed class DeadLetterOptions
        {
            public string Exchange { get; set; } = "ex.tai.dlx";
            public string Suffix { get; set; } = ".dlq";
            public bool EnableParkQueue { get; set; } = true;
            public string ParkSuffix { get; set; } = ".park";
        }
    }
}
