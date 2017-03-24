using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnouFlowInstanceLib
{
  public class FlowInstanceFriendlyLogDTO 
  {
    public int flowInstanceFriendlyLogId { get; set; }
    public string guid { get; set; }
    public int flowInstanceId { get; set; }
    public int flowActionRequestId { get; set; }
    public string currentActivityName { get; set; }
    public string paticipantName { get; set; }
    public string delegateeName { get; set; }
    public string actionName { get; set; }
    public string paticipantMemo { get; set; }
    public string createTime { get; set; }
  }
}
