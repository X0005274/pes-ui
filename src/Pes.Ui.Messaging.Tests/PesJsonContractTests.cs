using Newtonsoft.Json.Linq;
using Pes.Ui.Messaging;
using Pes.Ui.Messaging.Model;
using Xunit;

namespace Pes.Ui.Messaging.Tests
{
    /// <summary>Java Biz 레이어와의 JSON 메시지 계약(camelCase, null 생략, 필드명) 검증.</summary>
    public class PesJsonContractTests
    {
        [Fact]
        public void LotMessage_Serializes_CamelCase_WithOptions_AndOmitsNulls()
        {
            LotMessage message = new LotMessage
            {
                LotId = "LOT12345",
                WfId = "WF56789"
                // DurableId 는 null → 직렬화에서 생략되어야 함
            };
            LotWorkflowStep created = new LotWorkflowStep { Method = "created" };
            created.Options["createWf"] = true;
            created.Event = new PesEventInfo { EventCd = "CREATED", StatTyp = "NEW" };
            message.Workflow.Add(created);
            message.Meta = new PesMeta { SrcSystem = "UI", UserId = "UIUSER01" };

            string json = PesJson.Serialize(message);
            JObject root = JObject.Parse(json);

            Assert.Equal("LOT", (string)root["entityType"]);
            Assert.Equal("LOT12345", (string)root["lotId"]);
            Assert.Equal("WF56789", (string)root["wfId"]);
            Assert.Null(root["durableId"]);          // null 생략
            Assert.Null(root["meta"]["locale"]);      // null 생략

            JToken step = root["workflow"][0];
            Assert.Equal("created", (string)step["method"]);
            Assert.True((bool)step["options"]["createWf"]);  // 옵션 키는 원형 유지
            Assert.Equal("CREATED", (string)step["event"]["eventCd"]);
            Assert.Equal("NEW", (string)step["event"]["statTyp"]);
            Assert.Equal("UI", (string)root["meta"]["srcSystem"]);
        }

        [Fact]
        public void ProcessResult_Deserializes_FromJavaStyleJson()
        {
            string reply =
                "{\"entityType\":\"LOT\",\"entityId\":\"LOT12345\",\"correlationId\":\"TX-1\"," +
                "\"status\":\"SUCCESS\",\"steps\":[" +
                "{\"method\":\"created\",\"status\":\"SUCCESS\",\"message\":\"ok\"}," +
                "{\"method\":\"released\",\"status\":\"SKIPPED\",\"message\":\"skip\"}]}";

            PesProcessResult result = PesJson.Deserialize<PesProcessResult>(reply);

            Assert.Equal("LOT", result.EntityType);
            Assert.Equal("LOT12345", result.EntityId);
            Assert.Equal("TX-1", result.CorrelationId);
            Assert.True(result.IsSuccess());
            Assert.Equal(2, result.Steps.Count);
            Assert.Equal("created", result.Steps[0].Method);
            Assert.Equal("SKIPPED", result.Steps[1].Status);
        }

        [Fact]
        public void FailedResult_IsSuccess_ReturnsFalse()
        {
            PesProcessResult result =
                PesJson.Deserialize<PesProcessResult>("{\"status\":\"FAILED\"}");
            Assert.False(result.IsSuccess());
        }

        [Fact]
        public void LotMessage_RoundTrip_PreservesContractFields()
        {
            LotMessage original = new LotMessage { LotId = "LOT-RT", DurableId = "DUR-RT" };
            LotWorkflowStep step = new LotWorkflowStep { Method = "changeSpec" };
            step.Options["lotSpec"] = "SPEC-A";
            original.Workflow.Add(step);
            original.Meta = new PesMeta { SrcSystem = "HUB", CorrelationId = "C-1" };

            LotMessage copy = PesJson.Deserialize<LotMessage>(PesJson.Serialize(original));

            Assert.Equal("LOT", copy.EntityType);
            Assert.Equal("LOT-RT", copy.LotId);
            Assert.Equal("DUR-RT", copy.DurableId);
            Assert.Single(copy.Workflow);
            Assert.Equal("changeSpec", copy.Workflow[0].Method);
            Assert.Equal("SPEC-A", copy.Workflow[0].Options["lotSpec"].ToString());
            Assert.Equal("HUB", copy.Meta.SrcSystem);
            Assert.Equal("C-1", copy.Meta.CorrelationId);
        }
    }
}
