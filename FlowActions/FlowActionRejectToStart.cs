﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EnouFlowTemplateLib;
using System.Dynamic;
using Newtonsoft.Json;

namespace EnouFlowInstanceLib.Actions
{
  // 拒绝操作,回到起始节点状态
  public class FlowActionRejectToStart : FlowAction
  {
    private static EnumFlowActionRequestType requestTypeSpecialized =
      EnumFlowActionRequestType.rejectToStart;
    public string currentActivityGuid { get; set; } // 当前所处的活动状态
    public string startActivityGuid { get; set; }   // 退回到的开始状态
    public List<Paticipant> roles { get; set; }     // 接办人选择的待办角色/人员列表

    public FlowActionRejectToStart(
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
      string code,                // documentNo
      string currentActivityGuid, // 当前所处的活动状态(也许流程有多个入口)
      string startActivityGuid,   // 退回到的开始状态
      List<Paticipant> roles,     // 接办人选择的下一个活动状态待办角色/人员列表
      int? delegateeUserId,
      string delegateeUserGuid
      ) 
      : base(requestTypeSpecialized, flowInstanceId, flowInstanceGuid, 
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
      concreteMetaObj.startActivityGuid = startActivityGuid;
      concreteMetaObj.roles = roles;
      concreteMetaObj.delegateeUserId = delegateeUserId;
      concreteMetaObj.delegateeUserGuid = delegateeUserGuid;

      // Dynamic properties
      concreteFlowActionMetaJson =
        JsonConvert.SerializeObject(concreteMetaObj);
    }

    public FlowActionRejectToStart(FlowActionRequest dbObj) : base(dbObj)
    {
      this.currentActivityGuid = concreteMetaObj.currentActivityGuid;
      this.startActivityGuid = concreteMetaObj.startActivityGuid;
      this.roles = JsonConvert.DeserializeObject(
        concreteMetaObj.roles.ToString(), typeof(List<Paticipant>));
    }

    private FlowActionRejectToStart() { }
  }
}
