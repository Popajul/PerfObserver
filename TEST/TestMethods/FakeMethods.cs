using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TEST.TestMethods
{
    public static class FakeMethods
    {
        const int sleepTime = 2;
        public static void FakeMethod0()
        {
            Thread.Sleep(sleepTime);
            FakeMethod10();
            FakeMethod11();
        }
        private static void FakeMethod10()
        {
            Thread.Sleep(sleepTime);
        }

        private static void FakeMethod11()
        {
            Thread.Sleep(sleepTime);
            FakeMethod20();
        }

        private static void FakeMethod20()
        {
            Thread.Sleep(sleepTime);
        }

        public static void FakeMethodOUT(out string str)
        {
            str = "bonjour";
            Console.WriteLine(str);
        }
    }
}
