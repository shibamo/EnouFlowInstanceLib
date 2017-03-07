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
  public abstract class FlowAction
  {
    public int flowActionRequestId { get; set; }
    public int flowInstanceId { get; set; } = int.MinValue; // 流程实例
    public string flowInstanceGuid { get; set; }
    public string clientRequestGuid { get; set; }
    public string bizDocumentGuid { get; set; }
    public string bizDocumentTypeCode { get; set; } // 对应的业务数据类型代码
    public EnumFlowActionRequestType requestType { get; set; }
    public string userMemo { get; set; }  // 用户附言或说明
    public string bizDataPayloadJson { get; set; }
    public string concreteFlowActionMetaJson { get; set; }
    public dynamic concreteMetaObj { get; set; }
    public string optionalFlowActionDataJson { get; set; } // 用于提供具体的FlowAction对象
    public DateTime createTime { get; set; }

    public FlowAction(EnumFlowActionRequestType requestType,
                      string clientRequestGuid,
                      string bizDocumentGuid,
                      string bizDocumentTypeCode,
                      string userMemo,
                      string bizDataPayloadJson,
                      string optionalFlowActionDataJson)
    {
      this.requestType = requestType;
      this.userMemo = userMemo;
      this.bizDataPayloadJson = bizDataPayloadJson;
      this.optionalFlowActionDataJson = optionalFlowActionDataJson;
      this.clientRequestGuid = clientRequestGuid;
      this.bizDocumentGuid = bizDocumentGuid;
      this.bizDocumentTypeCode = bizDocumentTypeCode;
    }

    public FlowAction(EnumFlowActionRequestType requestType,
                      int flowInstanceId,
                      string flowInstanceGuid,
                      string clientRequestGuid,
                      string bizDocumentGuid,
                      string bizDocumentTypeCode,
                      string userMemo,
                      string bizDataPayloadJson,
                      string optionalFlowActionDataJson)
    {
      this.requestType = requestType;
      this.userMemo = userMemo;
      this.bizDataPayloadJson = bizDataPayloadJson;
      this.optionalFlowActionDataJson = optionalFlowActionDataJson;
      this.clientRequestGuid = clientRequestGuid;
      this.bizDocumentGuid = bizDocumentGuid;
      this.bizDocumentTypeCode = bizDocumentTypeCode;
      this.flowInstanceId = flowInstanceId;
      this.flowInstanceGuid = flowInstanceGuid;
    }

    public FlowAction(FlowActionRequest dbObj)
    {
      if (dbObj.flowInstance != null)
      {
        flowInstanceId = dbObj.flowInstance.flowInstanceId;
        flowInstanceGuid = dbObj.flowInstance.guid;
      }
      flowActionRequestId = dbObj.flowActionRequestId;
      requestType = dbObj.requestType;
      bizDocumentGuid = dbObj.bizDocumentGuid;
      bizDocumentTypeCode = dbObj.bizDocumentTypeCode;
      userMemo = dbObj.userMemo;
      bizDataPayloadJson = dbObj.bizDataPayloadJson;
      concreteFlowActionMetaJson = dbObj.concreteFlowActionMetaJson;
      optionalFlowActionDataJson = dbObj.optionalFlowActionDataJson;
      createTime = dbObj.createTime;

      // Dynamic properties
      concreteMetaObj = JsonConvert.DeserializeObject(
        dbObj.concreteFlowActionMetaJson);
    }

    public FlowAction()
    {
      throw new Exception("不能直接创建FlowAction对象!");
    }
  }

}
