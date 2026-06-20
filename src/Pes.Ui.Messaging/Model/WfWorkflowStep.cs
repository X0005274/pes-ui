using System.Collections.Generic;

namespace Pes.Ui.Messaging.Model
{
    /// <summary>WF workflow step. Java WfWorkflowStep / WfStepOptions 와 호환.</summary>
    public class WfWorkflowStep
    {
        public string Method { get; set; }
        public Dictionary<string, object> Options { get; set; }
        public PesEventInfo Event { get; set; }

        public WfWorkflowStep()
        {
            this.Options = new Dictionary<string, object>();
        }
    }
}
