using System;
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
