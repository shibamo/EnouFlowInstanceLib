﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EnouFlowTemplateLib;
using EnouFlowOrgMgmtLib;
using EnouFlowInstanceLib.Actions;

namespace EnouFlowInstanceLib
{
  public static class FlowInstanceHelper
  {
    public static FlowAction GetFlowAction(int flowActionRequestId, bool onlyNotProcessed = true)
    {
      using (var db = new EnouFlowInstanceContext())
      {
        db.Configuration.LazyLoadingEnabled = true;
        var dbObj = db.flowActionRequests.Find(flowActionRequestId);

        if (dbObj==null || (dbObj.isProcessed && onlyNotProcessed)) return null;

        return generateFlowAction(dbObj);
      }
    }

    public static FlowAction GetFirtUnprocessedRequest()
    {
      using (var db = new EnouFlowInstanceContext())
      {
        db.Configuration.LazyLoadingEnabled = true;
        var dbObj = db.flowActionRequests
          .Where(rq => !rq.isProcessed)
          .OrderBy(rq => rq.createTime)
          .FirstOrDefault();

        return generateFlowAction(dbObj);
      }
    }

    public static FlowAction GetFirtUnprocessedRequest(
      int flowInstanceId, EnumFlowActionRequestType[] flowActionRequestTypes)
    {
      using (var db = new EnouFlowInstanceContext())
      {
        db.Configuration.LazyLoadingEnabled = true;
        var dbObjs = db.flowActionRequests
          .Where(rq => !rq.isProcessed && rq.flowInstance.flowInstanceId == flowInstanceId)
          .OrderBy(rq => rq.createTime).ToList();

        var dbObj = dbObjs.Where(rq => flowActionRequestTypes.Contains(rq.requestType)).ToList().FirstOrDefault();

        return generateFlowAction(dbObj);
      }
    }

    private static FlowAction generateFlowAction(FlowActionRequest dbObj)
    {
      if (dbObj == null) return null;

      switch (dbObj.requestType)
      {
        case EnumFlowActionRequestType.start:
          return new FlowActionStart(dbObj);

        case EnumFlowActionRequestType.moveTo:
          return new FlowActionMoveTo(dbObj);

        case EnumFlowActionRequestType.moveToAutoGenerated:
          return new FlowActionMoveToAutoGenerated(dbObj);

        case EnumFlowActionRequestType.rejectToStart:
          return new FlowActionRejectToStart(dbObj);

        case EnumFlowActionRequestType.inviteOther:
          return new FlowActionInviteOther(dbObj);

        case EnumFlowActionRequestType.inviteOtherFeedback:
          return new FlowActionInviteOtherFeedback(dbObj);

        case EnumFlowActionRequestType.jumpTo:
          return new FlowActionJumpTo(dbObj);

        default:
          throw new Exception("Some member of EnumFlowActionRequestType not implemented !");
      }
    }

    // 启动
    public static FlowActionRequest PostFlowActionStart(
      string clientRequestGuid,
      string bizDocumentGuid,
      string bizDocumentTypeCode,
      string userMemo,
      string bizDataPayloadJson,
      string optionalFlowActionDataJson,
      int userId,
      string userGuid,
      int flowTemplateId,
      string flowTemplateGuid,
      string flowTemplateJson,
      string code,
      string currentActivityGuid
      )
    {
      // 未通过合法性检查直接返回
      if (!preValidate(clientRequestGuid))
      {
        return null;
      }

      var incomingReq = new FlowActionStart(
        clientRequestGuid, bizDocumentGuid, bizDocumentTypeCode, userMemo, 
        bizDataPayloadJson, optionalFlowActionDataJson, userId, userGuid,
        flowTemplateId, flowTemplateGuid, flowTemplateJson, code,
        currentActivityGuid);

      return saveToDB(incomingReq);
    }

    // MoveTo
    public static FlowActionRequest PostFlowActionMoveTo(
      string clientRequestGuid,
      string bizDocumentGuid,
      string bizDocumentTypeCode,
      DateTime bizTimeStamp,
      string userMemo,
      string bizDataPayloadJson,
      string optionalFlowActionDataJson,
      int userId, // 执行人员
      string userGuid,
      int flowInstanceId,
      string flowInstanceGuid,
      string code,
      string currentActivityGuid, // 当前所处的活动状态(也许流程有多个入口)
      string connectionGuid,      // 接办人选择的Connection
      string nextActivityGuid,    // 接办人选择的Connection指向的活动(理论上应该由FlowTemplate算出)
      List<Paticipant> roles,     // 接办人选择的下一个活动状态待办角色/人员列表
      int? delegateeUserId,
      string delegateeUserGuid
      )
    {
      // 未通过合法性检查直接返回
      if (!preValidate(clientRequestGuid))
      {
        return null;
      }

      var incomingReq = new FlowActionMoveTo(
        clientRequestGuid, bizDocumentGuid, bizDocumentTypeCode,  bizTimeStamp, 
        userMemo, bizDataPayloadJson, optionalFlowActionDataJson, userId, userGuid,
        flowInstanceId, flowInstanceGuid, code,
        currentActivityGuid, connectionGuid, nextActivityGuid, roles,
        delegateeUserId, delegateeUserGuid);

      return saveToDB(incomingReq);
    }

    // RejectToStart
    public static FlowActionRequest PostFlowActionRejectToStart(
      string clientRequestGuid,
      string bizDocumentGuid,
      string bizDocumentTypeCode,
      DateTime bizTimeStamp,
      string userMemo,
      string bizDataPayloadJson,
      string optionalFlowActionDataJson,
      int userId, // 执行人员
      string userGuid,
      int flowInstanceId,
      string flowInstanceGuid,
      string code,
      string currentActivityGuid, // 当前所处的活动状态(也许流程有多个入口)
      string startActivityGuid,   // 退回到的开始状态
      List<Paticipant> roles,     // 接办人选择的下一个活动状态待办角色/人员列表
      int? delegateeUserId,
      string delegateeUserGuid
      )
    {
      // 未通过合法性检查直接返回
      if (!preValidate(clientRequestGuid))
      {
        return null;
      }

      var incomingReq = new FlowActionRejectToStart(
        clientRequestGuid, bizDocumentGuid, bizDocumentTypeCode, bizTimeStamp,
        userMemo, bizDataPayloadJson, optionalFlowActionDataJson, userId, userGuid,
        flowInstanceId, flowInstanceGuid, code, currentActivityGuid, startActivityGuid, 
        roles, delegateeUserId, delegateeUserGuid);

      return saveToDB(incomingReq);
    }

    // MoveToAutoGenerated
    public static FlowActionRequest PostFlowActionMoveToAutoGenerated(
      string clientRequestGuid,
      string bizDocumentGuid,
      string bizDocumentTypeCode,
      DateTime bizTimeStamp,
      string userMemo,
      string bizDataPayloadJson,
      string optionalFlowActionDataJson,
      int flowInstanceId,
      string flowInstanceGuid,
      string code,
      string currentActivityGuid, // 当前所处的活动状态(也许流程有多个入口)
      string connectionGuid,      // 接办人选择的Connection
      string nextActivityGuid,    // 接办人选择的Connection指向的活动(理论上应该由FlowTemplate算出)
      List<Paticipant> roles,      // 接办人选择的下一个活动状态待办角色/人员列表
      EnouFlowInstanceContext db  // 自动规则生成的活动需要等待前Request成功过完成处理才能一并保存,只在同一个Context上创建对象
      )
    {
      // 未通过合法性检查直接返回
      if (!preValidate(clientRequestGuid))
      {
        return null;
      }

      var incomingReq = new FlowActionMoveToAutoGenerated(
        clientRequestGuid, bizDocumentGuid, bizDocumentTypeCode, bizTimeStamp, 
        userMemo, bizDataPayloadJson, optionalFlowActionDataJson, flowInstanceId, 
        flowInstanceGuid, code, currentActivityGuid, connectionGuid, nextActivityGuid, roles);

      return createWoSave(incomingReq, db);
    }

    // InviteOther
    public static FlowActionRequest PostFlowActionInviteOther(
      string clientRequestGuid,
      string bizDocumentGuid,
      string bizDocumentTypeCode,
      DateTime bizTimeStamp,
      string userMemo,
      string bizDataPayloadJson,
      string optionalFlowActionDataJson,
      int userId, // 执行人员
      string userGuid,
      int flowInstanceId,
      string flowInstanceGuid,
      string code,
      string currentActivityGuid, // 当前所处的活动状态(也许流程有多个入口)
      List<Paticipant> roles,      // 接办人选择的下一个活动状态待办角色/人员列表
      int relativeFlowTaskForUserId,
      int? delegateeUserId,
      string delegateeUserGuid
      )
    {
      // 未通过合法性检查直接返回
      if (!preValidate(clientRequestGuid))
      {
        return null;
      }

      var incomingReq = new FlowActionInviteOther(
        clientRequestGuid, bizDocumentGuid, bizDocumentTypeCode, bizTimeStamp,
        userMemo, bizDataPayloadJson, optionalFlowActionDataJson, userId, userGuid,
        flowInstanceId, flowInstanceGuid, code, currentActivityGuid, roles, 
        relativeFlowTaskForUserId, delegateeUserId, delegateeUserGuid);

      return saveToDB(incomingReq);
    }

    //InviteOtherFeedback
    public static FlowActionRequest PostFlowActionInviteOtherFeedback(
      string clientRequestGuid,
      string bizDocumentGuid,
      string bizDocumentTypeCode,
      DateTime bizTimeStamp,
      string userMemo,
      string bizDataPayloadJson,
      string optionalFlowActionDataJson,
      int userId,                     // 执行人员
      string userGuid,
      int flowInstanceId,
      string flowInstanceGuid,
      string code,
      string currentActivityGuid,     // 当前所处的活动状态
      string connectionGuid,          // 被征求意见人建议的connection
      List<Paticipant> roles,         // 被征求意见人选择的角色/人员列表
      int relativeFlowTaskForUserId,  // 被邀请者的taskid
      int? delegateeUserId,
      string delegateeUserGuid
    )
    {
      // 未通过合法性检查直接返回
      if (!preValidate(clientRequestGuid))
      {
        return null;
      }

      var incomingReq = new FlowActionInviteOtherFeedback(
        clientRequestGuid, bizDocumentGuid, bizDocumentTypeCode, bizTimeStamp,
        userMemo, bizDataPayloadJson, optionalFlowActionDataJson, 
        userId, userGuid, flowInstanceId, flowInstanceGuid, code, 
        currentActivityGuid, connectionGuid, roles, relativeFlowTaskForUserId,
        delegateeUserId, delegateeUserGuid);

      return saveToDB(incomingReq);
    }

    // JumpTo
    public static FlowActionRequest PostFlowActionJumpTo(
      string clientRequestGuid,
      string bizDocumentGuid,
      string bizDocumentTypeCode,
      DateTime bizTimeStamp,
      string userMemo,
      string bizDataPayloadJson,
      string optionalFlowActionDataJson,
      int userId, // 执行人员
      string userGuid,
      int flowInstanceId,
      string flowInstanceGuid,
      string code,
      string currentActivityGuid, // 当前所处的活动状态
      string nextActivityGuid,    // 接办人选择的目标活动
      List<Paticipant> roles,     // 接办人选择的下一个活动状态待办角色/人员列表
      bool forceJump,             // 是否强制跳转, 不做时间戳有效判定
      int? delegateeUserId,
      string delegateeUserGuid
      )
    {
      // 未通过合法性检查直接返回
      if (!preValidate(clientRequestGuid))
      {
        return null;
      }

      var incomingReq = new FlowActionJumpTo(
        clientRequestGuid, bizDocumentGuid, bizDocumentTypeCode, bizTimeStamp,
        userMemo, bizDataPayloadJson, optionalFlowActionDataJson, userId, userGuid,
        flowInstanceId, flowInstanceGuid, code,
        currentActivityGuid,  nextActivityGuid, roles, forceJump,
        delegateeUserId, delegateeUserGuid);

      return saveToDB(incomingReq);
    }

    private static FlowActionRequest saveToDB(FlowAction incomingReq)
    {
      using (var db = new EnouFlowInstanceContext())
      {
        var dbReq = db.flowActionRequests.Create();
        dbReq.requestType = incomingReq.requestType;
        dbReq.clientRequestGuid = incomingReq.clientRequestGuid;
        dbReq.bizDocumentGuid =incomingReq.bizDocumentGuid;
        dbReq.bizDocumentTypeCode = incomingReq.bizDocumentTypeCode;
        if (incomingReq.flowInstanceId > 0) // 穿入了有效的flowInstanceId
        {
          dbReq.flowInstance = GetFlowInstance(incomingReq.flowInstanceId, db);
          dbReq.flowInstanceGuid = incomingReq.flowInstanceGuid;
        }
        dbReq.userMemo = incomingReq.userMemo;
        dbReq.bizDataPayloadJson = incomingReq.bizDataPayloadJson;
        dbReq.concreteFlowActionMetaJson = incomingReq.concreteFlowActionMetaJson;
        dbReq.optionalFlowActionDataJson = incomingReq.optionalFlowActionDataJson;
        dbReq.userId = incomingReq.userId;
        dbReq.userGuid = incomingReq.userGuid;
        dbReq.delegateeUserId = incomingReq.delegateeUserId;
        dbReq.delegateeUserGuid = incomingReq.delegateeUserGuid;

        db.flowActionRequests.Add(dbReq);
        db.SaveChanges();

        return dbReq;
      }
    }

    private static FlowActionRequest createWoSave(FlowAction incomingReq,
      EnouFlowInstanceContext db)
    {
      var dbReq = db.flowActionRequests.Create();
      dbReq.requestType = incomingReq.requestType;
      dbReq.clientRequestGuid = incomingReq.clientRequestGuid;
      dbReq.bizDocumentGuid = incomingReq.bizDocumentGuid;
      dbReq.bizDocumentTypeCode = incomingReq.bizDocumentTypeCode;
      if (incomingReq.flowInstanceId > 0) // 穿入了有效的flowInstanceId
      {
        dbReq.flowInstance = GetFlowInstance(incomingReq.flowInstanceId, db);
        dbReq.flowInstanceGuid = incomingReq.flowInstanceGuid;
      }
      dbReq.userMemo = incomingReq.userMemo;
      dbReq.bizDataPayloadJson = incomingReq.bizDataPayloadJson;
      dbReq.concreteFlowActionMetaJson = incomingReq.concreteFlowActionMetaJson;
      dbReq.optionalFlowActionDataJson = incomingReq.optionalFlowActionDataJson;

      db.flowActionRequests.Add(dbReq);
      //db.SaveChanges();

      return dbReq;
    }

    private static bool preValidate(string clientRequestGuid)
    {
      #region 验证是否为同一个处理请求
      using (var db = new EnouFlowInstanceContext())
      {
        if (db.flowActionRequests.Where(
          r => r.clientRequestGuid == clientRequestGuid).Count() > 0)
        {
          throw new FlowActionRequestException(
            string.Format("请不要重复提交同一个处理请求'{0}'!",
            clientRequestGuid));
        }
      }
      #endregion
      return true;
    }

    public static List<FlowActionRequest> GetUnprocessedRequests()
    {
      using (var db = new EnouFlowInstanceContext())
      {
        return (db.flowActionRequests as IQueryable<FlowActionRequest>)
          .Where(rq => !rq.isProcessed)
          .OrderBy(rq => rq.createTime)
          .ToList();
      }
    }

    public static FlowInstance GetFlowInstance(int id, 
      EnouFlowInstanceContext db)
    {
      return db.flowInstances.Find(id);
    }

    public static FlowInstance GetFlowInstance(string guid, 
      EnouFlowInstanceContext db)
    {
      return db.flowInstances.Where(
        obj => obj.guid == guid).FirstOrDefault();
    }

    public static FlowTaskForUser GetFlowTaskForUser(int id, 
      EnouFlowInstanceContext db)
    {
      return db.flowTaskForUsers.Find(id);
    }

    public static FlowTaskForUser GetFlowTaskForUser(string guid, 
      EnouFlowInstanceContext db)
    {
      return db.flowTaskForUsers.Where(task=>task.guid==guid).FirstOrDefault() ;
    }

    public static List<FlowTaskForUser> GetFlowTaskForUserListOfUser(
      string userGuid, EnouFlowInstanceContext db)
    {
      List<FlowTaskForUser> flowTaskForUsers = db.flowTaskForUsers.Where(
          task => task.userGuid == userGuid).ToList();
      if (flowTaskForUsers.Count() > 0)
      {
        flowTaskForUsers = flowTaskForUsers.Where(
          task => task.isValidToProcess()).ToList();
      }
      else
      {
        flowTaskForUsers = new List<FlowTaskForUser>();
      }

      return flowTaskForUsers;
    }

    public static List<FlowTaskForUser> GetDelegatableFlowTaskForUserListOfUser(
      string userGuid, EnouFlowInstanceContext db)
    {
      List<FlowTaskForUser> flowTaskForUsers = db.flowTaskForUsers.Where(
          task => task.userGuid == userGuid &&
          (task.taskType == EnumFlowTaskType.normal || 
            task.taskType == EnumFlowTaskType.invitationFeedback)
          ).ToList();
      if (flowTaskForUsers.Count() > 0)
      {
        flowTaskForUsers = flowTaskForUsers.Where(
          task => task.isValidToProcess()).ToList();
      }
      else
      {
        flowTaskForUsers = new List<FlowTaskForUser>();
      }

      return flowTaskForUsers;
    }

    public static List<FlowTaskForUser> GetWaitingFlowTaskForUserListOfFlowInstance(
      int flowInstanceId, EnouFlowInstanceContext db)
    {
      List<FlowTaskForUser> flowTaskForUsers = db.flowTaskForUsers.Where(
          task => task.flowInstanceId == flowInstanceId &&
            task.taskState== EnumFlowTaskState.initial && 
            (task.taskType == EnumFlowTaskType.normal ||
            task.taskType == EnumFlowTaskType.redraft)).ToList();
      return flowTaskForUsers;
    }

    public static IEnumerable<FlowInstanceFriendlyLog> GetFlowInstanceFriendlyLogs(
      int flowInstanceId, EnouFlowInstanceContext db)
    {
      return db.flowFriendlyLogs.Where(obj =>
        obj.flowInstanceId == flowInstanceId).ToList();
    }
  }
}
