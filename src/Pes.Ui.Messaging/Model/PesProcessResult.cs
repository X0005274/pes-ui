using System.Collections.Generic;

namespace Pes.Ui.Messaging.Model
{
    /// <summary>step 처리 결과. Java PesStepResult 와 동일.</summary>
    public class PesStepResult
    {
        public string Method { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
    }

    /// <summary>메시지 처리 결과(Reply 본문). Java PesProcessResult 와 동일.</summary>
    public class PesProcessResult
    {
        public string EntityType { get; set; }
        public string EntityId { get; set; }
        public string CorrelationId { get; set; }
        public string Status { get; set; }
        public List<PesStepResult> Steps { get; set; }

        public bool IsSuccess()
        {
            return "SUCCESS".Equals(this.Status);
        }
    }
}
