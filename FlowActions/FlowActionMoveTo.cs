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
  // 从当前活动节点根据选定的有向连接移动到目的活动节点
  public class FlowActionMoveTo : FlowAction
  {
    private static EnumFlowActionRequestType requestTypeSpecialized =
      EnumFlowActionRequestType.moveTo;
    public int userId { get; set; } // 执行操作的普通用户
    public string userGuid { get; set; }
    public string currentActivityGuid { get; set; } // 当前所处的活动状态
    public string connectionGuid { get; set; } // 接办人选择的Connection
    public List<Paticipant> roles { get; set; } // 接办人选择的待办角色/人员列表

    public FlowActionMoveTo(
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
      string connectionGuid,      // 接办人选择的Connection
      string nextActivityGuid,    // 接办人选择的Connection指向的活动(理论上应该由FlowTemplate算出)
      List<Paticipant> roles      // 接办人选择的下一个活动状态待办角色/人员列表
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
      concreteMetaObj.nextActivityGuid = nextActivityGuid;
      concreteMetaObj.roles = roles;

      // Dynamic properties
      concreteFlowActionMetaJson =
        JsonConvert.SerializeObject(concreteMetaObj);
    }

    public FlowActionMoveTo(FlowActionRequest dbObj) : base(dbObj)
    {
      this.userId = concreteMetaObj.userId;
      this.userGuid = concreteMetaObj.userGuid;
      this.currentActivityGuid = concreteMetaObj.currentActivityGuid;
      this.roles = JsonConvert.DeserializeObject(
        concreteMetaObj.roles.ToString(), typeof(List<Paticipant>));
      this.connectionGuid = concreteMetaObj.connectionGuid;
    }

    private FlowActionMoveTo() { }
  }
}
