using System.Collections.Generic;

namespace Pes.Ui.Messaging.Model
{
    /// <summary>DURABLE 도메인 메시지(entityType 은 항상 "DURABLE"). Java DurableMessage 와 동일.</summary>
    public class DurableMessage
    {
        public const string EntityTypeDurable = "DURABLE";

        public string EntityType { get; set; }
        public string DurableId { get; set; }
        public string LotId { get; set; }
        public List<DurableWorkflowStep> Workflow { get; set; }
        public PesMeta Meta { get; set; }

        public DurableMessage()
        {
            this.EntityType = EntityTypeDurable;
            this.Workflow = new List<DurableWorkflowStep>();
        }
    }
}
