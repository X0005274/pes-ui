using System;
using Pes.Ui.Messaging;
using Pes.Ui.Messaging.Model;
using Pes.Ui.Rv;

namespace Pes.Ui.Console
{
    /// <summary>
    /// 도메인별 요청 송신 샘플. RVD 접속은 환경변수(PES_RV_*) 로 받는다.
    /// 사용: Pes.Ui.Console.exe [lot|wf|durable] [subject]
    ///   - 1번째 인자: 도메인(기본 lot)
    ///   - 2번째 인자: subject 직접 지정(기본 PES.UI.&lt;domain&gt;.REQUEST).
    ///                 현재 Java Biz 리스너로 직접 보내려면 PES.BIZ.&lt;domain&gt;.EVENT 사용.
    /// </summary>
    public static class Program
    {
        public static int Main(string[] args)
        {
            string service = Env("PES_RV_SERVICE", "7500");
            string network = Env("PES_RV_NETWORK", ";");
            string daemon = Env("PES_RV_DAEMON", "tcp:7500");

            string domain = (args.Length > 0 ? args[0] : "lot").ToLowerInvariant();
            string defaultSubject;
            object message;
            switch (domain)
            {
                case "wf":
                    defaultSubject = PesSubjects.WfUiRequest;
                    message = BuildSampleWf();
                    break;
                case "durable":
                    defaultSubject = PesSubjects.DurableUiRequest;
                    message = BuildSampleDurable();
                    break;
                case "lot":
                    defaultSubject = PesSubjects.LotUiRequest;
                    message = BuildSampleLot();
                    break;
                default:
                    System.Console.WriteLine("Unknown domain: " + domain + " (use lot|wf|durable)");
                    return 2;
            }
            string subject = args.Length > 1 ? args[1] : defaultSubject;

            System.Console.WriteLine("Sending [" + domain + "] to " + subject + ":");
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
            message.LotId = "LOT-UI-" + Stamp();
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

        private static WfMessage BuildSampleWf()
        {
            WfMessage message = new WfMessage();
            message.WfId = "WF-UI-" + Stamp();
            message.LotId = "LOT-UI-1";

            WfWorkflowStep created = new WfWorkflowStep();
            created.Method = "created";
            created.Event = new PesEventInfo { EventCd = "CREATED", StatTyp = "NEW" };
            message.Workflow.Add(created);

            WfWorkflowStep changeSpec = new WfWorkflowStep();
            changeSpec.Method = "changeSpec";
            changeSpec.Options["wfSpec"] = "WSPEC-1";
            changeSpec.Event = new PesEventInfo { EventCd = "CHG", StatTyp = "NEW" };
            message.Workflow.Add(changeSpec);

            message.Meta = new PesMeta { SrcSystem = "UI", UserId = "UIUSER01" };
            return message;
        }

        private static DurableMessage BuildSampleDurable()
        {
            DurableMessage message = new DurableMessage();
            message.DurableId = "DUR-UI-" + Stamp();
            message.LotId = "LOT-UI-1";

            DurableWorkflowStep created = new DurableWorkflowStep();
            created.Method = "created";
            created.Event = new PesEventInfo { EventCd = "CREATED", StatTyp = "NEW" };
            message.Workflow.Add(created);

            DurableWorkflowStep makeInUse = new DurableWorkflowStep();
            makeInUse.Method = "makeInUse";
            makeInUse.Options["bindLot"] = true;
            makeInUse.Event = new PesEventInfo { EventCd = "INUSE", StatTyp = "IN_USE" };
            message.Workflow.Add(makeInUse);

            message.Meta = new PesMeta { SrcSystem = "UI", UserId = "UIUSER01" };
            return message;
        }

        private static string Stamp()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        private static string Env(string name, string fallback)
        {
            string value = System.Environment.GetEnvironmentVariable(name);
            return string.IsNullOrEmpty(value) ? fallback : value;
        }
    }
}
