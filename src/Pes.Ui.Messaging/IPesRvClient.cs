using System;
using Pes.Ui.Messaging.Model;

namespace Pes.Ui.Messaging
{
    /// <summary>PES Biz 레이어로 RV 메시지를 송신하는 UI 클라이언트.</summary>
    public interface IPesRvClient : IDisposable
    {
        /// <summary>
        /// 지정 subject 로 도메인 메시지(LotMessage/WfMessage/DurableMessage)를 보내고
        /// Reply(PesProcessResult)를 받는다(Request/Reply). 타임아웃 시 null.
        /// </summary>
        PesProcessResult SendRequest(string subject, object message, double timeoutSeconds);
    }
}
