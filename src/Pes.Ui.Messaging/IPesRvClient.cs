using System;
using Pes.Ui.Messaging.Model;

namespace Pes.Ui.Messaging
{
    /// <summary>PES Biz 레이어로 RV 메시지를 송신하는 UI 클라이언트.</summary>
    public interface IPesRvClient : IDisposable
    {
        /// <summary>지정 subject 로 LOT 요청을 보내고 Reply(PesProcessResult)를 받는다(Request/Reply).</summary>
        PesProcessResult SendLotRequest(string subject, LotMessage message, double timeoutSeconds);
    }
}
