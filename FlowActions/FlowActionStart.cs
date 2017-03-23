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
  // 流程开始Action
  public class FlowActionStart : FlowAction
  {
    private static EnumFlowActionRequestType requestTypeSpecialized =
      EnumFlowActionRequestType.start;
    public bool needGenerateTaskForUser { get; set; } = false;
    public List<Paticipant> roles { get; set; } = null; // 待办角色/人员列表

    public FlowActionStart(
      string clientRequestGuid,
      string bizDocumentGuid,
      string bizDocumentTypeCode,
      string userMemo,
      string bizDataPayloadJson,
      string optionalFlowActionDataJson,
      int userId, // 流程启动人员
      string userGuid,
      int flowTemplateId,
      string flowTemplateGuid,
      string flowTemplateJson,  // 目前不知有无必要重复存在该数据
      string code,
      string currentActivityGuid, // 当前所处的活动状态(也许流程有多个入口)
      bool needGenerateTaskForUser = false,
      List<Paticipant> roles = null,
      int? delegateeUserId = null,
      string delegateeUserGuid = null
      )
      : base(requestTypeSpecialized, clientRequestGuid, 
          bizDocumentGuid, bizDocumentTypeCode, userMemo, 
          bizDataPayloadJson, optionalFlowActionDataJson,
          userId, userGuid, delegateeUserId, delegateeUserGuid)
    {
      dynamic concreteMetaObj = new ExpandoObject();
      concreteMetaObj.userId = userId;
      concreteMetaObj.userGuid = userGuid;
      concreteMetaObj.flowTemplateId = flowTemplateId;
      concreteMetaObj.flowTemplateGuid = flowTemplateGuid;
      concreteMetaObj.flowTemplateJson = flowTemplateJson;
      concreteMetaObj.code = code;
      concreteMetaObj.currentActivityGuid = currentActivityGuid;
      concreteMetaObj.needGenerateTaskForUser = needGenerateTaskForUser;
      concreteMetaObj.roles = roles;
      concreteMetaObj.delegateeUserId = delegateeUserId;
      concreteMetaObj.delegateeUserGuid = delegateeUserGuid;

      // Dynamic properties
      concreteFlowActionMetaJson =
        JsonConvert.SerializeObject(concreteMetaObj);
    }

    public FlowActionStart(FlowActionRequest dbObj) : base(dbObj)
    {
      this.needGenerateTaskForUser = concreteMetaObj.needGenerateTaskForUser;
      this.roles = concreteMetaObj.roles;
    }

    private FlowActionStart() { }
  }
}
