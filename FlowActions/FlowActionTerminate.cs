using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EnouFlowTemplateLib;
using System.Dynamic;
using Newtonsoft.Json;

namespace EnouFlowInstanceLib.Actions
{
  // 强制流程终止, 流程实例的processingState将会被置为
  // EnumFlowInstanceLifeState.terminated
  public class FlowActionTerminate : FlowAction
  {
    private static EnumFlowActionRequestType requestTypeSpecialized =
      EnumFlowActionRequestType.terminate;
    public string currentActivityGuid { get; set; } // 当前所处的活动
    public string nextActivityGuid { get; set; } // 需要终止到的活动

    public FlowActionTerminate(
      string clientRequestGuid,
      string bizDocumentGuid,
      string bizDocumentTypeCode,
      DateTime bizTimeStamp,
      string userMemo,
      string bizDataPayloadJson,
      string optionalFlowActionDataJson,
      int userId,
      string userGuid,
      int flowInstanceId,
      string flowInstanceGuid,
      string code,
      string currentActivityGuid, // 当前所处的活动状态
      string nextActivityGuid,    // 接办人选择跳转到的活动
      int? delegateeUserId,
      string delegateeUserGuid
      ) : base(requestTypeSpecialized, flowInstanceId, flowInstanceGuid, 
        clientRequestGuid, bizDocumentGuid, bizDocumentTypeCode, 
        userMemo, bizDataPayloadJson, optionalFlowActionDataJson,
        userId, userGuid, delegateeUserId, delegateeUserGuid)
    {
      dynamic concreteMetaObj = new ExpandoObject();
      concreteMetaObj.bizTimeStamp = bizTimeStamp;
      concreteMetaObj.userId = userId;
      concreteMetaObj.userGuid = userGuid;
      concreteMetaObj.flowInstanceId = flowInstanceId;
      concreteMetaObj.flowInstanceGuid = flowInstanceGuid;
      concreteMetaObj.code = code;
      concreteMetaObj.currentActivityGuid = currentActivityGuid;
      concreteMetaObj.nextActivityGuid = nextActivityGuid;
      concreteMetaObj.delegateeUserId = delegateeUserId;
      concreteMetaObj.delegateeUserGuid = delegateeUserGuid;

      // Dynamic properties
      concreteFlowActionMetaJson =
        JsonConvert.SerializeObject(concreteMetaObj);
    }

    public FlowActionTerminate(FlowActionRequest dbObj) : base(dbObj)
    {
      this.currentActivityGuid = concreteMetaObj.currentActivityGuid;
      this.nextActivityGuid = concreteMetaObj.nextActivityGuid;
    }

    private FlowActionTerminate() {}
  }
}
