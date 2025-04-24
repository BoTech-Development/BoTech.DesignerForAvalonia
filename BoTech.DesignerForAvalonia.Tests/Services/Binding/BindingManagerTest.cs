using BoTech.DesignerForAvalonia.Services.Binding;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BoTech.DesignerForAvalonia.Tests.Services.Binding;

[TestClass]
[TestSubject(typeof(BindingManager))]
public class BindingManagerTest
{

    [TestMethod]
    public void  TestNodeTree()
    {
        BindingManager.ParseBindingsFromSource(
            "Binding Title, RelativeSource={RelativeSource Tree=Logical, Mode=FindAncestor, AncestorType=Window}, TargetNullValue={Binding MyInt}");
    }
}