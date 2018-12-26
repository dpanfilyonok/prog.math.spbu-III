using System;

namespace Source
{
    public enum Results
    {
        Succeeded,
        Failed,
        Ignored
    }

    public class TestInfo
    {
        public string Name { get; set; }
        public Results Result { get; set; }
        public string IgnoreReason { get; set; }
        public long CompletionTime { get; set; }

        public TestInfo(
            string name, 
            Results result, 
            long completionTime = default(long), 
            string ignoreReason = null)
        {
            this.Name = name;
            this.Result = result;
            this.IgnoreReason = ignoreReason;
            this.CompletionTime = completionTime;
        }
    }
}