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
  public class FlowActionTake : FlowAction
  {
    public int userId { get; set; } // 接办人
    public string userGuid { get; set; }
    public string flowTaskForUserId { get; set; } // 当前所处的活动状态

    public FlowActionTake(
      string clientRequestGuid,
      string bizDocumentGuid,
      string bizDocumentTypeCode,
      int flowTaskForUserId,
      DateTime bizTimeStamp,
      string userMemo,
      string bizDataPayloadJson,
      string optionalFlowActionDataJson,
      int userId,
      string userGuid,
      int flowInstanceId,
      string flowInstanceGuid
      )
      : base(EnumFlowActionRequestType.take, flowInstanceId, flowInstanceGuid, 
          clientRequestGuid, bizDocumentGuid, bizDocumentTypeCode, userMemo, 
          bizDataPayloadJson, optionalFlowActionDataJson)
    {
      dynamic concreteMetaObj = new ExpandoObject();
      concreteMetaObj.bizTimeStamp = bizTimeStamp;
      concreteMetaObj.userId = userId;
      concreteMetaObj.userGuid = userGuid;
      concreteMetaObj.flowInstanceId = flowInstanceId;
      concreteMetaObj.flowInstanceGuid = flowInstanceGuid;
      concreteMetaObj.flowTaskForUserId = flowTaskForUserId;
      // Dynamic properties
      concreteFlowActionMetaJson =
        JsonConvert.SerializeObject(concreteMetaObj);
    }


    private FlowActionTake() { }
  }
}
