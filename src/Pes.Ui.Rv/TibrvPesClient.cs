using System;
using Pes.Ui.Messaging;
using Pes.Ui.Messaging.Model;
using TIBCO.Rendezvous;

namespace Pes.Ui.Rv
{
    /// <summary>
    /// TIBCO Rendezvous 기반 PES UI 클라이언트. Request/Reply(INBOX) 로 LOT 요청을 보낸다.
    /// 페이로드는 RV 메시지의 "json" 필드에 JSON 문자열로 싣는다(Java 측과 동일 계약).
    /// 사용에는 상용 TIBCO.Rendezvous.dll 이 필요하다.
    /// </summary>
    public sealed class TibrvPesClient : IPesRvClient
    {
        private readonly Transport transport;
        private bool disposed;

        public TibrvPesClient(string service, string network, string daemon)
        {
            TIBCO.Rendezvous.Environment.Open();
            this.transport = new NetTransport(service, network, daemon);
        }

        public PesProcessResult SendRequest(string subject, object message, double timeoutSeconds)
        {
            if (string.IsNullOrEmpty(subject))
            {
                throw new ArgumentException("subject is required", nameof(subject));
            }
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            Message request = new Message();
            request.SendSubject = subject;
            request.AddField(PesFields.Json, PesJson.Serialize(message));

            Message reply = this.transport.SendRequest(request, timeoutSeconds);
            if (reply == null)
            {
                return null; // 타임아웃
            }

            MessageField field = reply.GetField(PesFields.Json);
            string json = field != null && field.Value != null ? field.Value.ToString() : null;
            return string.IsNullOrEmpty(json) ? null : PesJson.Deserialize<PesProcessResult>(json);
        }

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }
            this.disposed = true;

            if (this.transport != null)
            {
                this.transport.Dispose();
            }
            if (TIBCO.Rendezvous.Environment.IsValid)
            {
                TIBCO.Rendezvous.Environment.Close();
            }
        }
    }
}
