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
  // 直接跳转到指定活动节点(类型可以是开始/自动/结束?)
  public class FlowActionJumpTo : FlowAction
  {
    private static EnumFlowActionRequestType requestTypeSpecialized =
      EnumFlowActionRequestType.jumpTo;
    public string currentActivityGuid { get; set; } // 当前所处的活动
    public List<Paticipant> roles { get; set; } // 执行人选择的待办角色/人员列表
    public string nextActivityGuid { get; set; } // 需要跳转到的活动
    public bool forceJump { get; set; } = false; // 是否强制跳转,不做时间戳有效判定

    public FlowActionJumpTo(
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
      List<Paticipant> roles,     // 接办人选择的下一个活动状态待办角色/人员列表
      bool forceJump,              // 是否强制跳转, 不做时间戳有效判定
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
      concreteMetaObj.roles = roles;
      concreteMetaObj.forceJump = forceJump;
      concreteMetaObj.delegateeUserId = delegateeUserId;
      concreteMetaObj.delegateeUserGuid = delegateeUserGuid;

      // Dynamic properties
      concreteFlowActionMetaJson =
        JsonConvert.SerializeObject(concreteMetaObj);
    }

    public FlowActionJumpTo(FlowActionRequest dbObj) : base(dbObj)
    {
      this.currentActivityGuid = concreteMetaObj.currentActivityGuid;
      this.roles = JsonConvert.DeserializeObject(
        concreteMetaObj.roles.ToString(), typeof(List<Paticipant>));
      this.nextActivityGuid = concreteMetaObj.nextActivityGuid;
      this.forceJump = concreteMetaObj.forceJump;
    }

    private FlowActionJumpTo() { }

  }
}
