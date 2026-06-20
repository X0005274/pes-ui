using System.Collections.Generic;

namespace Pes.Ui.Messaging.Model
{
    /// <summary>DURABLE workflow step. Java DurableWorkflowStep / DurableStepOptions 와 호환.</summary>
    public class DurableWorkflowStep
    {
        public string Method { get; set; }
        public Dictionary<string, object> Options { get; set; }
        public PesEventInfo Event { get; set; }

        public DurableWorkflowStep()
        {
            this.Options = new Dictionary<string, object>();
        }
    }
}
