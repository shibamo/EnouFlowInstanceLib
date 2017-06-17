using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using EnouFlowTemplateLib.Tests.Attributes;
using NSubstitute;

using EnouFlowOrgMgmtLib;
using EnouFlowTemplateLib;

namespace EnouFlowInstanceLib.Tests.Integration
{
  [TestFixture]
  public class Template_TestsI
  {
    private EnouFlowInstanceContext db = null;

    [SetUp]
    public void Setup()
    {
      db = new EnouFlowInstanceContext();
    }

    [TearDown]
    public void TearDown()
    {
      db.Dispose();
    }
  }
}
