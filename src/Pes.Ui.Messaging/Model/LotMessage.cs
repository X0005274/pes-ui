using System.Collections.Generic;

namespace Pes.Ui.Messaging.Model
{
    /// <summary>LOT 도메인 메시지(entityType 은 항상 "LOT"). Java LotMessage 와 동일 JSON 계약.</summary>
    public class LotMessage
    {
        public const string EntityTypeLot = "LOT";

        public string EntityType { get; set; }
        public string LotId { get; set; }
        public string WfId { get; set; }
        public string DurableId { get; set; }
        public List<LotWorkflowStep> Workflow { get; set; }
        public PesMeta Meta { get; set; }

        public LotMessage()
        {
            this.EntityType = EntityTypeLot;
            this.Workflow = new List<LotWorkflowStep>();
        }
    }
}
