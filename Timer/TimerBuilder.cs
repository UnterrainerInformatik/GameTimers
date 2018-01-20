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
using JetBrains.Annotations;

namespace Timer
{
    [PublicAPI]
    public class TimerBuilder
    {
        private event EventHandler<TimerArgs> TimerFired;
        private event EventHandler<TimerArgs> TimerFiring;
        private event EventHandler<TimerArgs> TimerUpdating;
        private event EventHandler<TimerArgs> TimerUpdated;

        private float value;
        private bool isActive = true;
        private Timer next;

        public TimerBuilder(float intervalInMilliseconds)
        {
            value = intervalInMilliseconds;
        }

        public Timer Build()
        {
            var t = new Timer(value);
            t.Next = next ?? t;
            t.IsActive = isActive;

            CopyEvents(TimerFired, t);
            CopyEvents(TimerFiring, t);
            CopyEvents(TimerUpdating, t);
            CopyEvents(TimerUpdated, t);

            return t;
        }

        private void CopyEvents<T>(EventHandler<T> source, Timer target)
        {
            if (source == null) return;
            foreach (var handler in source.GetInvocationList())
            {
                target.Fired((EventHandler<TimerArgs>) handler);
            }
        }

        /// <summary>
        ///     Sets the next timer that is chained after this one. If none is set, the new timer will reference itself with next.
        /// </summary>
        public TimerBuilder Next(Timer timer)
        {
            next = timer;
            return this;
        }

        /// <summary>
        ///     Sets a value indicating whether the Timer is active.
        ///     When the Timer is inactive updates have no effect and triggers are not called.
        /// </summary>
        public TimerBuilder Active(bool v)
        {
            isActive = v;
            return this;
        }

        /// <summary>
        ///     Sets the timer's value (not the upper bound) in milliseconds.
        /// </summary>
        public TimerBuilder ValueInMillis(float v)
        {
            value = v;
            return this;
        }

        /// <summary>
        ///     Registers a timer-firing event-handler. You can do this multiple times in one builder as well.
        /// </summary>
        public TimerBuilder Firing(EventHandler<TimerArgs> eventHandler)
        {
            TimerFiring += eventHandler;
            return this;
        }

        /// <summary>
        ///     Registers a timer-fired event-handler. You can do this multiple times in one builder as well.
        /// </summary>
        public TimerBuilder Fired(EventHandler<TimerArgs> eventHandler)
        {
            TimerFired += eventHandler;
            return this;
        }

        /// <summary>
        ///     Registers a timer-updating event-handler. You can do this multiple times in one builder as well.
        /// </summary>
        public TimerBuilder Updating(EventHandler<TimerArgs> eventHandler)
        {
            TimerUpdating += eventHandler;
            return this;
        }

        /// <summary>
        ///     Registers a timer-updated event-handler. You can do this multiple times in one builder as well.
        /// </summary>
        public TimerBuilder Updated(EventHandler<TimerArgs> eventHandler)
        {
            TimerUpdated += eventHandler;
            return this;
        }
    }
}