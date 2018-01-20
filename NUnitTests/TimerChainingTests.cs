// *************************************************************************** 
// This is free and unencumbered software released into the public domain.
// 
// Anyone is free to copy, modify, publish, use, compile, sell, or
// distribute this software, either in source code form or as a compiled
// binary, for any purpose, commercial or non-commercial, and by any
// means.
// 
// In jurisdictions that recognize copyright laws, the author or authors
// of this software dedicate any and all copyright interest in the
// software to the public domain. We make this dedication for the benefit
// of the public at large and to the detriment of our heirs and
// successors. We intend this dedication to be an overt act of
// relinquishment in perpetuity of all present and future rights to this
// software under copyright law.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
// OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// 
// For more information, please refer to <http://unlicense.org>
// ***************************************************************************

using NUnit.Framework;

namespace NUnitTests
{
    [TestFixture]
    [Category("Timer.Chaining")]
    public class TimerChainingTests
    {
        private const float EPSILON = float.Epsilon;
        private Timer.Timer t1;
        private Timer.Timer t2;

        [Test]
        public void NewTimerIsChainedToItself()
        {
            t1 = Timer.Timer.Builder(10).Build();
            Assert.AreEqual(t1, t1.Next);
        }

        [Test]
        public void ChainedTimersAreInARingList()
        {
            t2 = Timer.Timer.Builder(20).Build();
            t1 = Timer.Timer.Builder(10).Build();
            t1.Connect(t2);
            Assert.AreEqual(t2, t1.Next);
            Assert.AreEqual(t1, t2.Next);
        }

        [Test]
        public void ChainedTimersAreNotActivatedWhenParentWasNotTriggered()
        {
            var t1Fired = false;
            var t2Fired = false;
            t1 = Timer.Timer.Builder(10).Fired((sender, args) => t1Fired = true).Build(); // active
            t2 = Timer.Timer.Builder(20).Fired((sender, args) => t2Fired = true).InActive().Build(); // inactive
            t1.Connect(t2);

            // You would simply call 'Update' on all timers.
            // T1 will deactivate and activate t2 automatically if triggered.
            t1.Update(5f);
            t2.Update(5f);
            Assert.IsFalse(t1Fired);
            Assert.IsFalse(t2Fired);
            Assert.AreEqual(5f, t1.ValueInMillis, EPSILON);
            Assert.AreEqual(0f, t2.ValueInMillis, EPSILON);
            Assert.IsTrue(t1.IsActive);
            Assert.IsFalse(t2.IsActive);
        }

        [Test]
        public void ChainedTimersAreActivatedWhenParentWasTriggered()
        {
            var t1Fired = false;
            var t2Fired = false;
            t1 = Timer.Timer.Builder(10).Fired((sender, args) => t1Fired = true).Build(); // active
            t2 = Timer.Timer.Builder(20).Fired((sender, args) => t2Fired = true).InActive().Build(); // inactive
            t1.Connect(t2);

            // You would simply call 'Update' on all timers.
            // T1 will deactivate and activate t2 automatically if triggered.
            t1.Update(15f);
            t2.Update(15f);
            Assert.IsTrue(t1Fired);
            Assert.IsFalse(t2Fired);
            Assert.AreEqual(0f, t1.ValueInMillis, EPSILON);
            Assert.AreEqual(15f, t2.ValueInMillis, EPSILON);
            Assert.IsFalse(t1.IsActive);
            Assert.IsTrue(t2.IsActive);
        }
    }
}