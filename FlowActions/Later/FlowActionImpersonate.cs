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
  public class FlowActionImpersonate : FlowAction //Impersonate跟代理可能不同,等待出现合适的业务场景再实现
  {
    private static EnumFlowActionRequestType requestTypeSpecialized =
      EnumFlowActionRequestType.impersonate;
  }
}
