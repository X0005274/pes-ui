# PES UI (C#, .NET Framework 4.8)

PES 시스템의 **UI 클라이언트** 레이어. TIBCO Rendezvous 로 LOT/WF/DURABLE 요청을 보내고
Reply(`PesProcessResult`)를 받습니다. 메시지는 **Java Biz 레이어와 동일한 JSON 계약**을 사용합니다.

> 서버(Biz) 구현은 별도 저장소 [`pes-boot`](https://github.com/X0005274/pes-boot) (Java/Spring Boot).

## 솔루션 구성

```
Pes.Ui.sln
└── src/
    ├── Pes.Ui.Messaging/    메시지 DTO·JSON 규약·Subject 상수·클라이언트 인터페이스 (TIBCO 비의존)
    ├── Pes.Ui.Rv/           TIBCO RV 클라이언트 구현 (스텁 또는 실제 DLL)
    ├── Pes.Ui.Tibco.Stub/   TIBCO.Rendezvous 최소 스텁(상용 DLL 없이 빌드/CI 용)
    └── Pes.Ui.Console/      LOT 요청 송신 샘플
```

### TIBCO 스텁 vs 실제 DLL
- **기본(개발/CI)**: `UseRealTibrv=false` → `Pes.Ui.Tibco.Stub` 가 `TIBCO.Rendezvous` 어셈블리를
  대신 제공해 **상용 DLL 없이 전체 솔루션이 빌드**됩니다. (단, 런타임 `SendRequest` 는 예외 — 통신 불가)
- **운영**: 실제 `TIBCO.Rendezvous.dll` 로 빌드
  ```bat
  set TIBRV_HOME=C:\tibco\tibrv\8.4
  dotnet build Pes.Ui.sln -c Release -p:UseRealTibrv=true
  ```

- **Target**: `net48` (.NET Framework 4.8), Visual Studio 2026
- **코드 규칙**: `var` 미사용(명시적 타입), null 생략 JSON(camelCase) — Java 측과 정합
- **JSON**: Newtonsoft.Json + CamelCase 리졸버 → `entityType/lotId/eventCd/...` 그대로 매핑
- **RV 필드**: 페이로드는 메시지 `json` 필드(Java `TibrvMessageTransport.FIELD_JSON` 과 동일)

## 사전 준비 (TIBCO RV)

상용 `TIBCO.Rendezvous.dll` 이 필요합니다. 설치 후 환경변수로 경로 지정:
```bat
set TIBRV_HOME=C:\tibco\tibrv\8.4
```
`Pes.Ui.Rv.csproj` 가 `%TIBRV_HOME%\bin\TIBCO.Rendezvous.dll` 을 참조합니다.
네이티브 RV 라이브러리(`%TIBRV_HOME%\bin`)가 PATH 에 있어야 런타임 동작합니다.

## 빌드 & 실행

```bat
dotnet build Pes.Ui.sln -c Release
:: 또는 Visual Studio 2026 에서 Pes.Ui.sln 열기

set PES_RV_SERVICE=7500
set PES_RV_NETWORK=;
set PES_RV_DAEMON=tcp:7500
dotnet run --project src\Pes.Ui.Console            :: PES.UI.LOT.REQUEST 로 송신
dotnet run --project src\Pes.Ui.Console -- PES.BIZ.LOT.EVENT  :: 현재 Java Biz 리스너로 직접
```

## 사용 예 (라이브러리)

```csharp
using (IPesRvClient client = new TibrvPesClient("7500", ";", "tcp:7500"))
{
    LotMessage msg = new LotMessage { LotId = "LOT12345", WfId = "WF56789" };
    LotWorkflowStep created = new LotWorkflowStep { Method = "created" };
    created.Options["createWf"] = true;
    created.Event = new PesEventInfo { EventCd = "CREATED", StatTyp = "NEW" };
    msg.Workflow.Add(created);
    msg.Meta = new PesMeta { SrcSystem = "UI", UserId = "UIUSER01" };

    PesProcessResult result = client.SendLotRequest(PesSubjects.LotUiRequest, msg, 10.0);
    // result.Status == "SUCCESS" / "FAILED", result.Steps ...
}
```

## 메시징 경로

```
[C# UI] --PES.UI.LOT.REQUEST--> [PES.UI 포워더] --PES.BIZ.LOT.EVENT--> [Java Biz]
        <----------- RV INBOX Reply (PesProcessResult) -----------
```
- `correlationId` 미지정 시 서버가 발급(응답 `correlationId` 로 확인).
- 상태 변경은 `PES.UI.<domain>.EVENT` 로 push(구독 핸들러 확장 가능).

## 참고/제약

- 이 솔루션은 소스만 제공하며, 빌드에는 **.NET SDK(또는 VS2026)** 와 **TIBCO.Rendezvous.dll** 이 필요합니다.
- 현재 Java 인바운드 리스너는 `PES.BIZ.<domain>.EVENT` 구독입니다. `PES.UI.*.REQUEST` →
  `PES.BIZ.*.EVENT` **포워더**(PES.UI 레이어)는 아직 미구현이므로, 단독 통신 시 콘솔 인자로
  `PES.BIZ.LOT.EVENT` 를 지정해 직접 보낼 수 있습니다.
