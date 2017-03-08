using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Dynamic;

using EnouFlowTemplateLib;
using EnouFlowOrgMgmtLib;

namespace EnouFlowInstanceLib
{
  public class EnouFlowInstanceContext : DbContext
  {
    #region Some tedious configuration
    public EnouFlowInstanceContext()
      : base("name=EnouFlowPlatformDatabase") // DB connection name
    {

    }
    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
      //去掉系统自带的级联删除
      modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
      modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
    }
    #endregion

    public DbSet<FlowInstance> flowInstances { get; set; }
    public DbSet<FlowActionRequest> flowActionRequests { get; set; }
    public DbSet<FlowTaskForUser> flowTaskForUsers { get; set; }
    public DbSet<FlowInstanceFriendlyLog> flowFriendlyLogs { get; set; }
    public DbSet<FlowInstanceTechLog> flowTechLogs { get; set; }
    public DbSet<FlowInstanceTracerLog> flowTracerLogs { get; set; }

  }

  [Table("Enou_FlowInstance")]
  public class FlowInstance // 流程实例
  {
    [Key]
    public int flowInstanceId { get; set; }
    public string guid { get; set; } = Guid.NewGuid().ToString();
    public int flowTemplateId { get; set; }
    [Required]
    public string flowTemplateJson { get; set; } // 当前的流程模板Json
    public DateTime createTime { get; set; } = DateTime.Now;
    public bool isVisible { get; set; } = true;
    public int creatorId { get; set; }  // 创建该实例的用户
    public string code { get; set; } // 流程实例代码,一般为产生的表单编码
    public string bizDocumentGuid { get; set; } // 对应的业务数据Guid,用于初始阶段创建流程RequestAction时无法获取flowInstanceId和guid的替代查找
    public string bizDocumentTypeCode { get; set; } // 对应的业务数据类型代码
    public EnumFlowInstanceProcessingState processingState { get; set; }
    public EnumFlowInstanceLifeState lifeState { get; set; }
    public DateTime bizTimeStamp { get; set; } // 当前业务时间戳
    public DateTime mgmtTimeStamp { get; set; } // 当前管理时间戳
    public string startActivityGuid { get; set; } // 起始的活动状态,用于被直接拒绝回发起人时需要到达的目标状态
    public string currentActivityGuid { get; set; } // 当前所处的活动状态
    public string currentActivityName { get; set; } // 当前所处的活动状态名
    public string previousActivityGuid { get; set; } // 上一个活动状态,用于退回?
    public string previousActivityName { get; set; } // 上一个活动状态名
    public string bizDataPayloadJson { get; set; } // 具体的业务数据Json,考虑给动态加载的脚本使用来访问业务数据

    [NotMapped]
    public dynamic bizDataPayloadObj
    {
      get
      {
        if (string.IsNullOrEmpty(bizDataPayloadJson))
        {
          return (dynamic)new ExpandoObject();
        }
        else
        {
          try
          {
            return JsonHelper.DeserializeJsonToObject<dynamic>(
            bizDataPayloadJson);
          }
          catch (Exception ex)
          {
            return (dynamic)new ExpandoObject();
          }
        }
      }
    }
    // 前序流程实例, 可能额外需要记录一些辅助信息,因此考虑不直接自连接,使用中间连接表
    // public FlowInstance prevFlowInstance { get; set; } 
    // public virtual List<FlowInstance> parents { get; set; }
  }

  [Table("Enou_FlowActionRequest")]
  public class FlowActionRequest // 对流程实例的处理请求对象
  {
    [Key]
    public int flowActionRequestId { get; set; }
    public string guid { get; set; } = Guid.NewGuid().ToString();
    public virtual FlowInstance flowInstance { get; set; } // 在新创建流程实例的请求时为空,后期会回填该值
    public string flowInstanceGuid { get; set; } // 在新创建流程实例的请求时为空,后期会回填该值
    [Required]
    public string clientRequestGuid { get; set; } // 客户方产生的Request标识,用于避免重复提交
    [Required]
    public EnumFlowActionRequestType requestType { get; set; }
    public string bizDocumentGuid { get; set; } // 对应的业务数据Guid,用于初始阶段创建流程RequestAction时无法获取flowInstanceId和guid的替代查找
    public string bizDocumentTypeCode { get; set; } // 对应的业务数据类型代码
    public string userMemo { get; set; }  // 用户附言或说明
    public string bizDataPayloadJson { get; set; }
    public string concreteFlowActionMetaJson { get; set; } // 用于提供具体的FlowAction对象其特定的属性集
    public string optionalFlowActionDataJson { get; set; } // 用于给FlowAction对象提供可选的流程属性数据
    [Index("IX_GetUnprocessedRecord", 1)]
    public DateTime createTime { get; set; } = DateTime.Now;
    [Index("IX_GetUnprocessedRecord", 2)]
    public bool isProcessed { get; set; } = false; // 是否已被处理过
    public DateTime? finishTime { get; set; }
    public EnumFlowActionRequestResultType resultType { get; set; }
      = EnumFlowActionRequestResultType.notAvailable;
    public string failReason { get; set; }
  }

  [Table("Enou_FlowTaskForUser")]
  public class FlowTaskForUser // 用户的流程实例任务对象
  {
    [Key]
    public int flowTaskForUserId { get; set; }
    public string guid { get; set; } = Guid.NewGuid().ToString();
    public virtual FlowInstance flowInstance { get; set; } // 对应的FlowInstance
    public string bizDocumentGuid { get; set; } // 对应的业务数据Guid
    public string bizDocumentTypeCode { get; set; } // 对应的业务数据类型代码
    public int userId { get; set; }
    public string userGuid { get; set; }
    public string currentActivityGuid { get; set; }
    public DateTime bizTimeStamp { get; set; } // 收到任务时的流程实例业务时间戳
    public EnumFlowTaskType taskType { get; set; } = EnumFlowTaskType.normal;
    // 相关的任务Id,taskType为invitation,delegation时,设为对应相关任务的Id
    public int? relativeFlowTaskForUserId { get; set; } 
    public EnumFlowTaskState taskState { get; set; } = EnumFlowTaskState.initial;
    public EnumFlowTaskNotifyState taskNotifyState { get; set; } = EnumFlowTaskNotifyState.initial;
    public DateTime createTime { get; set; } = DateTime.Now;
    public DateTime? finishTime { get; set; } // 用户完成该任务的提交时间
    // 以下为任务的自定义字段,不同任务类型(taskType)将利用这些字段
    public int? intField_1 { get; set; }
    public int? intField_2 { get; set; }
    public int? intField_3 { get; set; }
    public int? intField_4 { get; set; }
    public int? intField_5 { get; set; }
    public int? intField_6 { get; set; }
    public int? intField_7 { get; set; }
    public int? intField_8 { get; set; }
    public int? intField_9 { get; set; }
    public int? intField_10 { get; set; }
    public string stringField_1 { get; set; }
    public string stringField_2 { get; set; }
    public string stringField_3 { get; set; }
    public string stringField_4 { get; set; }
    public string stringField_5 { get; set; }
    public string stringField_6 { get; set; }
    public string stringField_7 { get; set; }
    public string stringField_8 { get; set; }
    public string stringField_9 { get; set; }
    public string stringField_10 { get; set; }
    public decimal? decimalField_1 { get; set; }
    public decimal? decimalField_2 { get; set; }
    public decimal? decimalField_3 { get; set; }
    public decimal? decimalField_4 { get; set; }
    public decimal? decimalField_5 { get; set; }
    public decimal? decimalField_6 { get; set; }
    public decimal? decimalField_7 { get; set; }
    public decimal? decimalField_8 { get; set; }
    public decimal? decimalField_9 { get; set; }
    public decimal? decimalField_10 { get; set; }
    public DateTime? dateTimeField_1 { get; set; }
    public DateTime? dateTimeField_2 { get; set; }
    public DateTime? dateTimeField_3 { get; set; }
    public DateTime? dateTimeField_4 { get; set; }
    public DateTime? dateTimeField_5 { get; set; }

    public bool isValidToProcess()
    {
      return taskState != EnumFlowTaskState.done &&
          taskState != EnumFlowTaskState.deletedByUser &&
          taskState != EnumFlowTaskState.obsoleted;
    }
  }

  [Table("Enou_FlowInstanceFriendlyLog")]
  public class FlowInstanceFriendlyLog // 
  {
    [Key]
    public int flowInstanceFriendlyLogId { get; set; }
    public string guid { get; set; } = Guid.NewGuid().ToString();
    public virtual FlowInstance flowInstance { get; set; }
    public string flowInstanceGuid { get; set; }
    public int flowActionRequestId { get; set; }
    public DateTime createTime { get; set; } = DateTime.Now;

  }

  [Table("Enou_FlowInstanceTechLog")]
  public class FlowInstanceTechLog // 
  {
    [Key]
    public int flowInstanceTechLogId { get; set; }
    public string guid { get; set; } = Guid.NewGuid().ToString();
    public virtual FlowInstance flowInstance { get; set; }
    public string flowInstanceGuid { get; set; }
    public int flowActionRequestId { get; set; }
    public DateTime createTime { get; set; } = DateTime.Now;

  }

  [Table("Enou_FlowInstanceTracerLog")]
  public class FlowInstanceTracerLog // 
  {
    [Key]
    public int flowInstanceTracerLogId { get; set; }
    public string guid { get; set; } = Guid.NewGuid().ToString();
    public virtual FlowInstance flowInstance { get; set; }
    public string flowInstanceGuid { get; set; }
    public int flowActionRequestId { get; set; }
    public DateTime createTime { get; set; } = DateTime.Now;

  }

  [Table("Enou_FlowInstanceInviteOther")]
  public class FlowInstanceInviteOther // 
  {
    [Key]
    public int flowInstanceInviteOtherId { get; set; }



  }

  /// <summary>
  /// 下面是将来需要完善实现业务功能的表
  /// </summary>
  [Table("Enou_FlowTemplateOfInstance")]
  public class FlowTemplateOfInstance // 流程实例的历史流程模板Json
  {
    [Key]
    public int flowTemplateOfInstanceId { get; set; }
    public string oldFlowTemplateJson { get; set; } // 对应的老流程模板Json
    public string newFlowTemplateJson { get; set; } // 对应的新流程模板Json
    public string reason { get; set; } // 变更原因
    public DateTime createTime { get; set; } = DateTime.Now;
    public int systemManagerId { get; set; } //Creator
  }

}
