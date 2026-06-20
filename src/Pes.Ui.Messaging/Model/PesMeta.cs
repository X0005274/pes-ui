namespace Pes.Ui.Messaging.Model
{
    /// <summary>공통 메타데이터. Java PesMeta 와 동일 JSON 계약(camelCase).</summary>
    public class PesMeta
    {
        public string SrcSystem { get; set; }
        public string UserId { get; set; }
        public string CorrelationId { get; set; }
        public string RequestTm { get; set; }
        public string Locale { get; set; }
    }
}
