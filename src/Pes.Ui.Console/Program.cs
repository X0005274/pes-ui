using System;
using Pes.Ui.Messaging;
using Pes.Ui.Messaging.Model;
using Pes.Ui.Rv;

namespace Pes.Ui.Console
{
    /// <summary>
    /// LOT 요청 송신 샘플. RVD 접속 파라미터는 환경변수(PES_RV_*) 또는 인자로 받는다.
    /// 사용: Pes.Ui.Console.exe [subject]
    ///   기본 subject = PES.UI.LOT.REQUEST (PES.UI 포워더 경유).
    ///   현재 Java Biz 리스너로 직접 보내려면 PES.BIZ.LOT.EVENT 사용.
    /// </summary>
    public static class Program
    {
        public static int Main(string[] args)
        {
            string service = Env("PES_RV_SERVICE", "7500");
            string network = Env("PES_RV_NETWORK", ";");
            string daemon = Env("PES_RV_DAEMON", "tcp:7500");
            string subject = args.Length > 0 ? args[0] : PesSubjects.LotUiRequest;

            LotMessage message = BuildSampleLot();
            System.Console.WriteLine("Sending to " + subject + ":");
            System.Console.WriteLine(PesJson.Serialize(message));

            using (IPesRvClient client = new TibrvPesClient(service, network, daemon))
            {
                PesProcessResult result = client.SendRequest(subject, message, 10.0);
                if (result == null)
                {
                    System.Console.WriteLine("No reply (timeout).");
                    return 2;
                }

                System.Console.WriteLine("Reply: status=" + result.Status
                    + ", entityId=" + result.EntityId
                    + ", correlationId=" + result.CorrelationId);
                if (result.Steps != null)
                {
                    foreach (PesStepResult step in result.Steps)
                    {
                        System.Console.WriteLine("  - " + step.Method + ": " + step.Status
                            + " (" + step.Message + ")");
                    }
                }
                return result.IsSuccess() ? 0 : 1;
            }
        }

        private static LotMessage BuildSampleLot()
        {
            LotMessage message = new LotMessage();
            message.LotId = "LOT-UI-" + DateTime.Now.ToString("yyyyMMddHHmmss");
            message.WfId = "WF-UI-1";
            message.DurableId = "DUR-UI-1";

            LotWorkflowStep created = new LotWorkflowStep();
            created.Method = "created";
            created.Options["createWf"] = true;
            created.Event = new PesEventInfo { EventCd = "CREATED", EventDesc = "from C# UI", StatTyp = "NEW" };
            message.Workflow.Add(created);

            LotWorkflowStep released = new LotWorkflowStep();
            released.Method = "released";
            released.Options["makeDurableInUse"] = true;
            released.Event = new PesEventInfo { EventCd = "RELEASED", StatTyp = "REL" };
            message.Workflow.Add(released);

            message.Meta = new PesMeta { SrcSystem = "UI", UserId = "UIUSER01" };
            return message;
        }

        private static string Env(string name, string fallback)
        {
            string value = System.Environment.GetEnvironmentVariable(name);
            return string.IsNullOrEmpty(value) ? fallback : value;
        }
    }
}
