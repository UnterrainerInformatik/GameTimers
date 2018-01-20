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
    [Category("Fader")]
    public class TimerTests
    {
        private readonly Timer.Timer t = Timer.Timer.Builder(10).Build();

        [Test]
        public void TriggeredTimerInvokesEventsCorrectlyAndInOrder()
        {
            var index = 1;
            var updatingTriggered = -1;
            var updatedTriggered = -1;
            var firingTriggered = -1;
            var firedTriggered = -1;
            
            t.TimerUpdating += (sender, args) =>
            {
                updatingTriggered = index;
                index++;
            };
            t.TimerUpdated += (sender, args) =>
            {
                updatedTriggered = index;
                index++;
            };
            t.TimerFiring += (sender, args) =>
            {
                updatedTriggered = index;
                index++;
            };
            t.TimerFired += (sender, args) =>
            {
                firedTriggered = index;
                index++;
            };
            t.TimerFiring += (sender, args) =>
            {
                firingTriggered = index;
                index++;
            };

            var isTriggered = t.Update(1000f);

            Assert.IsTrue(isTriggered);
            Assert.AreEqual(updatingTriggered, 1);
            Assert.AreEqual(firingTriggered, 2);
            Assert.AreEqual(firedTriggered, 3);
            Assert.AreEqual(updatedTriggered, 4);
        }


        /*
        [Test]
        public void Test()
        {
        }
         */
    }
}