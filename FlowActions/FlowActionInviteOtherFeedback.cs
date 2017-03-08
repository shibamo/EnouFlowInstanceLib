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
  // 对征求意见的回复, 与FlowActionInviteOther对应
  public class FlowActionInviteOtherFeedback : FlowAction
  {
    private static EnumFlowActionRequestType requestTypeSpecialized =
      EnumFlowActionRequestType.inviteOtherFeedback;
    public int userId { get; set; } // 执行操作的普通用户
    public string userGuid { get; set; }
    public string currentActivityGuid { get; set; } // 当前所处的活动状态
    public string connectionGuid { get; set; } // 被征求意见人选择的Connection
    public List<Paticipant> roles { get; set; } // 被征求意见人选择的角色/人员列表
    public int relativeFlowTaskForUserId { get; set; } // 被邀请者的task

    public FlowActionInviteOtherFeedback(
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
      string connectionGuid,  // 被征求意见人建议的connection
      List<Paticipant> roles,      // 被征求意见人选择的角色/人员列表
      int relativeFlowTaskForUserId // 被邀请者的taskid
      ) : base(requestTypeSpecialized, flowInstanceId, flowInstanceGuid, clientRequestGuid, bizDocumentGuid, bizDocumentTypeCode, userMemo, bizDataPayloadJson, optionalFlowActionDataJson)
    {
      dynamic concreteMetaObj = new ExpandoObject();
      concreteMetaObj.bizTimeStamp = bizTimeStamp;
      concreteMetaObj.userId = userId;
      concreteMetaObj.userGuid = userGuid;
      concreteMetaObj.flowInstanceId = flowInstanceId;
      concreteMetaObj.flowInstanceGuid = flowInstanceGuid;
      concreteMetaObj.code = code;
      concreteMetaObj.currentActivityGuid = currentActivityGuid;
      concreteMetaObj.connectionGuid = connectionGuid;
      concreteMetaObj.roles = roles;
      concreteMetaObj.relativeFlowTaskForUserId = relativeFlowTaskForUserId;

      // Dynamic properties
      concreteFlowActionMetaJson =
        JsonConvert.SerializeObject(concreteMetaObj);
    }

    public FlowActionInviteOtherFeedback(FlowActionRequest dbObj) : base(dbObj)
    {
      this.userId = concreteMetaObj.userId;
      this.userGuid = concreteMetaObj.userGuid;
      this.currentActivityGuid = concreteMetaObj.currentActivityGuid;
      this.roles = JsonConvert.DeserializeObject(
        concreteMetaObj.roles.ToString(), typeof(List<Paticipant>));
      this.relativeFlowTaskForUserId = concreteMetaObj.relativeFlowTaskForUserId;
      this.connectionGuid = concreteMetaObj.connectionGuid;
    }

    private FlowActionInviteOtherFeedback() { }
  }
}
