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

using System;
using Microsoft.Xna.Framework;
using NUnit.Framework;

namespace NUnitTests
{
    [TestFixture]
    [Category("Timer")]
    public class TimerTests
    {
        private const float EPSILON = float.Epsilon;
        private Timer.Timer t;

        [Test]
        public void TriggeredTimerInvokesEventsCorrectlyAndInOrder()
        {
            t = Timer.Timer.Builder(10).Build();
            var index = 1;
            var updatingTriggered = 0;
            var updatedTriggered = 0;
            var firingTriggered = 0;
            var firedTriggered = 0;

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
                firingTriggered = index;
                index++;
            };
            t.TimerFired += (sender, args) =>
            {
                firedTriggered = index;
                index++;
            };

            var isTriggered = t.Update(1000f);

            Assert.IsTrue(isTriggered);
            Assert.AreEqual(1, updatingTriggered);
            Assert.AreEqual(2, firingTriggered);
            Assert.AreEqual(3, firedTriggered);
            Assert.AreEqual(4, updatedTriggered);
        }

        [Test]
        public void NoOverlapIsWorkingAfterUpdating()
        {
            t = Timer.Timer.Builder(10).Build();
            t.Update(30f);
            Assert.AreEqual(0f, t.ValueInMillis, EPSILON);
        }

        [Test]
        public void MultipleOverlapsIsWorkingAfterUpdating()
        {
            t = Timer.Timer.Builder(10).Build(); 
            t.Update(34f, true);
            Assert.AreEqual(4f, t.ValueInMillis, EPSILON);
        }
        
        [Test]
        public void InactiveTimerDoesntFire()
        {
            t = Timer.Timer.Builder(10).Active(false).Build();
            t.Update(4f, true);
            Assert.AreEqual(0f, t.ValueInMillis, EPSILON);
        }

        [Test]
        public void IfTimeElapsedIsTooSmallOnlyUpdatingAndUpdatedAreTriggered()
        {
            t = Timer.Timer.Builder(10).Build();
            var index = 1;
            var updatingTriggered = 0;
            var updatedTriggered = 0;
            var firingTriggered = 0;
            var firedTriggered = 0;

            t.Updating((sender, args) =>
            {
                updatingTriggered = index;
                index++;
            });
            t.Updated((sender, args) =>
            {
                updatedTriggered = index;
                index++;
            });
            t.Firing((sender, args) =>
            {
                firingTriggered = index;
                index++;
            });
            t.Fired((sender, args) =>
            {
                firedTriggered = index;
                index++;
            });

            // Update 4ms by creating a GameTime object from scratch.
            var isTriggered = t.Update(new GameTime(TimeSpan.FromMilliseconds(1004f), TimeSpan.FromMilliseconds(4f)));

            Assert.AreEqual(6f, t.TimeLeftToGoUntilReset, EPSILON);
            Assert.IsFalse(isTriggered);
            Assert.AreEqual(1, updatingTriggered);
            Assert.AreEqual(0, firingTriggered);
            Assert.AreEqual(0, firedTriggered);
            Assert.AreEqual(2, updatedTriggered);
        }

        [Test]
        public void ResettingTriggersOnNextUpdateCycleEvenIfTimeIsZero()
        {
            t = Timer.Timer.Builder(10).Build();

            var triggered = t.Update(4f);
            Assert.IsFalse(triggered);
            Assert.AreEqual(4f, t.ValueInMillis, EPSILON);

            t.Set();

            triggered = t.Update();
            Assert.IsTrue(triggered);
            Assert.AreEqual(0f, t.ValueInMillis, EPSILON);
        }

        [Test]
        public void ResetDoesntEnablePreviouslyDisabledTimer()
        {
            t = Timer.Timer.Builder(10).Active(false).Build();
            t.SetTo(2f);
            t.Reset();
            Assert.AreEqual(0f, t.ValueInMillis);
            Assert.IsFalse(t.IsActive);
        }

        [Test]
        public void ResetResetsTimerToZero()
        {
            t = Timer.Timer.Builder(10).Build();
            t.Update(4f);
            t.Reset();
            Assert.AreEqual(0f, t.ValueInMillis);
        }

        [Test]
        public void ResetToResetsTimerMaxToAValueAndTimerToZero()
        {
            t = Timer.Timer.Builder(10).Build();
            t.Update(4f);
            t.ResetTo(6f);
            Assert.AreEqual(0f, t.ValueInMillis);
            Assert.AreEqual(6f, t.MaxValue);
        }
    }
}