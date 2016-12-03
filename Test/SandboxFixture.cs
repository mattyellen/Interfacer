using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework.Internal;
using NUnit.Framework;

namespace Test
{
    public static class Static
    {
        public static void Method<T>(out T result) where T : new()
        {
            result = new T();
        }
    }
    public class Dyno : DynamicObject
    {
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            typeof(Static).GetMethod(binder.Name).Invoke(null, args);
            result = null;
            return true;
        }
    }

    public class Foo
    {
        
    }

    [TestFixture]
    public class SandboxFixture
    {
        [Test]
        public void Test()
        {
            int val;

            dynamic foo = new Dyno();
            foo.Method<Foo>(out val);

            Assert.That(val, Is.InstanceOf<Foo>());
        }
    }
}
