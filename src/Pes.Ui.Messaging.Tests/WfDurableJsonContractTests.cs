using Newtonsoft.Json.Linq;
using Pes.Ui.Messaging;
using Pes.Ui.Messaging.Model;
using Xunit;

namespace Pes.Ui.Messaging.Tests
{
    /// <summary>WF/DURABLE 메시지의 JSON 계약(entityType 고정, 옵션 키) 검증.</summary>
    public class WfDurableJsonContractTests
    {
        [Fact]
        public void WfMessage_ChangeSpec_WithApplyToLot_Serializes()
        {
            WfMessage wf = new WfMessage { WfId = "WF56789", LotId = "LOT12345" };
            WfWorkflowStep step = new WfWorkflowStep { Method = "changeSpec" };
            step.Options["wfSpec"] = "WSPEC-3";
            step.Options["applyToLot"] = true;
            step.Event = new PesEventInfo { EventCd = "CHG", StatTyp = "NEW" };
            wf.Workflow.Add(step);
            wf.Meta = new PesMeta { SrcSystem = "UI", CorrelationId = "TX-WF-2" };

            JObject root = JObject.Parse(PesJson.Serialize(wf));

            Assert.Equal("WF", (string)root["entityType"]);
            Assert.Equal("WF56789", (string)root["wfId"]);
            Assert.Equal("LOT12345", (string)root["lotId"]);

            JToken s = root["workflow"][0];
            Assert.Equal("changeSpec", (string)s["method"]);
            Assert.Equal("WSPEC-3", (string)s["options"]["wfSpec"]);
            Assert.True((bool)s["options"]["applyToLot"]);
            Assert.Equal("CHG", (string)s["event"]["eventCd"]);
        }

        [Fact]
        public void DurableMessage_MakeInUse_WithBindLot_Serializes()
        {
            DurableMessage durable = new DurableMessage { DurableId = "DUR001", LotId = "LOT12345" };
            DurableWorkflowStep step = new DurableWorkflowStep { Method = "makeInUse" };
            step.Options["bindLot"] = true;
            step.Event = new PesEventInfo { EventCd = "INUSE", StatTyp = "IN_USE" };
            durable.Workflow.Add(step);
            durable.Meta = new PesMeta { SrcSystem = "UI" };

            JObject root = JObject.Parse(PesJson.Serialize(durable));

            Assert.Equal("DURABLE", (string)root["entityType"]);
            Assert.Equal("DUR001", (string)root["durableId"]);
            Assert.Equal("LOT12345", (string)root["lotId"]);

            JToken s = root["workflow"][0];
            Assert.Equal("makeInUse", (string)s["method"]);
            Assert.True((bool)s["options"]["bindLot"]);
            Assert.Equal("IN_USE", (string)s["event"]["statTyp"]);
        }

        [Fact]
        public void WfMessage_RoundTrip_PreservesContract()
        {
            WfMessage original = new WfMessage { WfId = "WF-RT", LotId = "LOT-RT" };
            WfWorkflowStep step = new WfWorkflowStep { Method = "created" };
            step.Options["inheritLotSpec"] = true;
            original.Workflow.Add(step);
            original.Meta = new PesMeta { SrcSystem = "HUB" };

            WfMessage copy = PesJson.Deserialize<WfMessage>(PesJson.Serialize(original));

            Assert.Equal("WF", copy.EntityType);
            Assert.Equal("WF-RT", copy.WfId);
            Assert.Equal("LOT-RT", copy.LotId);
            Assert.Single(copy.Workflow);
            Assert.Equal("created", copy.Workflow[0].Method);
            Assert.True(bool.Parse(copy.Workflow[0].Options["inheritLotSpec"].ToString()));
        }
    }
}
