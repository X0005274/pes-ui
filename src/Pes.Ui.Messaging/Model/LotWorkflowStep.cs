using System.Collections.Generic;

namespace Pes.Ui.Messaging.Model
{
    /// <summary>LOT workflow step. options 는 자유 key/value(Java LotStepOptions 와 호환).</summary>
    public class LotWorkflowStep
    {
        public string Method { get; set; }
        public Dictionary<string, object> Options { get; set; }
        public PesEventInfo Event { get; set; }

        public LotWorkflowStep()
        {
            this.Options = new Dictionary<string, object>();
        }
    }
}
