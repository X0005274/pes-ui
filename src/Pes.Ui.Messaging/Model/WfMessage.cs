using System.Collections.Generic;

namespace Pes.Ui.Messaging.Model
{
    /// <summary>WF 도메인 메시지(entityType 은 항상 "WF"). Java WfMessage 와 동일 JSON 계약.</summary>
    public class WfMessage
    {
        public const string EntityTypeWf = "WF";

        public string EntityType { get; set; }
        public string WfId { get; set; }
        public string LotId { get; set; }
        public List<WfWorkflowStep> Workflow { get; set; }
        public PesMeta Meta { get; set; }

        public WfMessage()
        {
            this.EntityType = EntityTypeWf;
            this.Workflow = new List<WfWorkflowStep>();
        }
    }
}
