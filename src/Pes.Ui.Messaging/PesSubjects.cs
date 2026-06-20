namespace Pes.Ui.Messaging
{
    /// <summary>TIBCO RV Subject 규약(PES.&lt;layer&gt;.&lt;domain&gt;.&lt;type&gt;).</summary>
    public static class PesSubjects
    {
        // UI → PES.UI (요청). PES.UI 레이어가 PES.BIZ 로 전달.
        public const string LotUiRequest = "PES.UI.LOT.REQUEST";
        public const string WfUiRequest = "PES.UI.WF.REQUEST";
        public const string DurableUiRequest = "PES.UI.DURABLE.REQUEST";

        // Biz → UI (이벤트 push) 구독용.
        public const string LotUiEvent = "PES.UI.LOT.EVENT";
        public const string WfUiEvent = "PES.UI.WF.EVENT";
        public const string DurableUiEvent = "PES.UI.DURABLE.EVENT";

        // Biz 직접 수신 subject(현재 Java 인바운드 리스너 대상).
        public const string LotBizEvent = "PES.BIZ.LOT.EVENT";
        public const string WfBizEvent = "PES.BIZ.WF.EVENT";
        public const string DurableBizEvent = "PES.BIZ.DURABLE.EVENT";
    }

    /// <summary>RV 메시지 필드명(Java TibrvMessageTransport.FIELD_JSON 과 일치).</summary>
    public static class PesFields
    {
        public const string Json = "json";
    }
}
