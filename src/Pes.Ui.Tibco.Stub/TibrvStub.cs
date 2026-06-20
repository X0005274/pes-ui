using System;
using System.Collections.Generic;

// TIBCO Rendezvous .NET API 최소 스텁(shim).
// 목적: 상용 TIBCO.Rendezvous.dll 없이 전체 솔루션을 컴파일/CI 하기 위함.
// 런타임 통신은 지원하지 않으며(SendRequest 호출 시 예외), 운영 빌드에서는
// 실제 DLL 을 참조한다(-p:UseRealTibrv=true). 어셈블리명은 TIBCO.Rendezvous 로 출력된다.
namespace TIBCO.Rendezvous
{
    public static class Environment
    {
        public static bool IsValid { get; private set; }

        public static void Open()
        {
            IsValid = true;
        }

        public static void Close()
        {
            IsValid = false;
        }
    }

    public class MessageField
    {
        public object Value { get; set; }
    }

    public class Message
    {
        private readonly Dictionary<string, object> fields = new Dictionary<string, object>();

        public string SendSubject { get; set; }

        public void AddField(string name, string value)
        {
            this.fields[name] = value;
        }

        public MessageField GetField(string name)
        {
            object value;
            return this.fields.TryGetValue(name, out value) ? new MessageField { Value = value } : null;
        }
    }

    public abstract class Transport : IDisposable
    {
        public abstract Message SendRequest(Message request, double timeout);

        public virtual void Dispose()
        {
        }
    }

    public sealed class NetTransport : Transport
    {
        public NetTransport(string service, string network, string daemon)
        {
        }

        public override Message SendRequest(Message request, double timeout)
        {
            throw new NotSupportedException(
                "TIBCO.Rendezvous 스텁입니다. 실제 통신은 상용 DLL 로 빌드해야 합니다 (-p:UseRealTibrv=true).");
        }
    }
}
