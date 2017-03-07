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
  // 征求他人意见
  public class FlowActionInviteOther : FlowAction
  {
    public int userId { get; set; } // 执行操作的普通用户
    public string userGuid { get; set; }
    public string currentActivityGuid { get; set; } // 当前所处的活动状态
    public List<Paticipant> roles { get; set; } // 接办人选择的征求处理意见的角色/人员列表

    public FlowActionInviteOther(
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
      string currentActivityGuid, // 当前所处的活动状态(也许流程有多个入口)
      List<Paticipant> roles      // 接办人选择的征求处理意见的角色/人员列表
      ) : base(EnumFlowActionRequestType.inviteOther, flowInstanceId, flowInstanceGuid, clientRequestGuid, bizDocumentGuid, bizDocumentTypeCode, userMemo, bizDataPayloadJson, optionalFlowActionDataJson)
    {
      dynamic concreteMetaObj = new ExpandoObject();
      concreteMetaObj.bizTimeStamp = bizTimeStamp;
      concreteMetaObj.userId = userId;
      concreteMetaObj.userGuid = userGuid;
      concreteMetaObj.flowInstanceId = flowInstanceId;
      concreteMetaObj.flowInstanceGuid = flowInstanceGuid;
      concreteMetaObj.code = code;
      concreteMetaObj.currentActivityGuid = currentActivityGuid;
      concreteMetaObj.roles = roles;

      // Dynamic properties
      concreteFlowActionMetaJson =
        JsonConvert.SerializeObject(concreteMetaObj);
    }

    public FlowActionInviteOther(FlowActionRequest dbObj) : base(dbObj)
    {
      this.userId = concreteMetaObj.userId;
      this.userGuid = concreteMetaObj.userGuid;
      this.currentActivityGuid = concreteMetaObj.currentActivityGuid;
      this.roles = JsonConvert.DeserializeObject(
        concreteMetaObj.roles.ToString(), typeof(List<Paticipant>));
    }

    private FlowActionInviteOther() { }
  }
}
