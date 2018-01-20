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
using Faders;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace Timer
{
    /// <summary>
    ///     This is a helper class that represents a millisecond-timer.
    ///     You may change its value (timer) or interval even when it's running.<br />
    ///     When updated a timer essentially does a countdown and when it reaches zero, it fires. After that it starts again
    ///     if you don't stop it.
    ///     Timers may be chained together, resulting in the previous one triggering the next one when it fires.
    /// </summary>
    [PublicAPI]
    public class Timer : Fader
    {
        /// <summary>
        ///     Occurs when the timer triggers.
        /// </summary>
        public event EventHandler<TimerArgs> TimerFired;

        /// <summary>
        ///     Occurs after the value is updated but before the trigger is fired.
        /// </summary>
        public event EventHandler<TimerArgs> TimerFiring;

        /// <summary>
        ///     Occurs before the update-operations are performed, but only if the timer is active.
        /// </summary>
        public event EventHandler<TimerArgs> TimerUpdating;

        /// <summary>
        ///     Occurs after the update-operations are performed, but only if the timer is active.
        /// </summary>
        public event EventHandler<TimerArgs> TimerUpdated;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Timer" /> class.
        /// </summary>
        /// <param name="intervalInMillis">The interval in milliseconds.</param>
        public Timer(float intervalInMillis)
            : base(0.0f, intervalInMillis, 0.0f)
        {
            IsActive = true;
            Next = this;
        }

        public static TimerBuilder Builder(float value)
        {
            return new TimerBuilder(value);
        }

        /// <summary>
        ///     Gets or sets the next timer that is chained after this one.
        /// </summary>
        /// <value>
        ///     The next.
        /// </value>
        public Timer Next { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the Timer is active.
        ///     When the Timer is inactive updates have no effect and triggers are not called.
        /// </summary>
        /// <value>
        ///     <c>true</c> if it is active; otherwise, <c>false</c>.
        /// </value>
        public bool IsActive { get; set; }

        public Timer SetIsActive(bool value)
        {
            IsActive = value;
            return this;
        }

        /// <summary>
        ///     Gets or sets the timer's value (not the upper bound) in milliseconds.
        /// </summary>
        /// <value>
        ///     The time in milliseconds.
        /// </value>
        public float ValueInMillis
        {
            get => (float) (Value - MinValue);
            set => Value = MinValue + value;
        }

        public Timer SetValueInMilliseconds(float value)
        {
            ValueInMillis = value;
            return this;
        }

        /// <summary>
        ///     Resets the value of this timer to zero and the upper bound to <see cref="intervalInMilliseconds" />.
        /// </summary>
        /// <param name="intervalInMilliseconds"></param>
        /// <returns></returns>
        public Timer ResetTo(float intervalInMilliseconds)
        {
            MinValue = 0.0f;
            MaxValue = intervalInMilliseconds;
            Value = 0.0f;
            return this;
        }

        /// <summary>
        ///     Registers a timer-firing event-handler.
        /// </summary>
        /// <param name="eventHandler">The event handler.</param>
        /// <returns>This instance in order to support a fluent interface.</returns>
        public Timer Firing(EventHandler<TimerArgs> eventHandler)
        {
            TimerFiring += eventHandler;
            return this;
        }

        /// <summary>
        ///     Registers a timer-fired event-handler.
        /// </summary>
        /// <param name="eventHandler">The event handler.</param>
        /// <returns>This instance in order to support a fluent interface.</returns>
        public Timer Fired(EventHandler<TimerArgs> eventHandler)
        {
            TimerFired += eventHandler;
            return this;
        }

        /// <summary>
        ///     Registers a timer-updating event-handler, if it is enabled.
        /// </summary>
        /// <param name="eventHandler">The event handler.</param>
        /// <returns>This instance in order to support a fluent interface.</returns>
        public Timer Updating(EventHandler<TimerArgs> eventHandler)
        {
            TimerUpdating += eventHandler;
            return this;
        }

        /// <summary>
        ///     Registers a timer-updated event-handler, if it is enabled.
        /// </summary>
        /// <param name="eventHandler">The event handler.</param>
        /// <returns>This instance in order to support a fluent interface.</returns>
        public Timer Updated(EventHandler<TimerArgs> eventHandler)
        {
            TimerUpdated += eventHandler;
            return this;
        }

        /// <summary>
        ///     Invokes a timer-firing event.
        /// </summary>
        protected void InvokeTimerFiring()
        {
            TimerFiring?.Invoke(this, new TimerArgs(this));
        }

        /// <summary>
        ///     Invokes a timer-fired event.
        /// </summary>
        protected void InvokeTimerFired()
        {
            TimerFired?.Invoke(this, new TimerArgs(this));
        }

        /// <summary>
        ///     Invokes a timer-updating - if the timer is enabled - event.
        /// </summary>
        protected void InvokeTimerUpdating()
        {
            TimerUpdating?.Invoke(this, new TimerArgs(this));
        }

        /// <summary>
        ///     Invokes a timer-updated - if the timer is enabled - event.
        /// </summary>
        protected void InvokeTimerUpdated()
        {
            TimerUpdated?.Invoke(this, new TimerArgs(this));
        }

        /// <summary>
        ///     Gets the time left to go until reset.
        /// </summary>
        /// <value>The time left to go until reset.</value>
        public float TimeLeftToGoUntilReset => (float) (MaxValue - Value);

        /// <summary>
        ///     Resets the timer by resetting the time-value. Does NOT activate it if it should have been disabled before.
        /// </summary>
        public Timer Reset()
        {
            Value = MinValue;
            IsActive = true;
            return this;
        }

        /// <summary>
        ///     Sets the timer so that it fires when calling update the next time (sets the value to maxValue).
        /// </summary>
        public Timer Set()
        {
            Value = MaxValue;
            return this;
        }

        /// <summary>
        ///     Connects the specified other timer to this one. This one is the first in line. When firing, it disables itself and
        ///     starts the next timer. The other timer, in return, gets connected to this one since it has, in turn, to enable this
        ///     one when it fires itself. So essentially they form a ring-list.<br />We assume that at least the chain you're
        ///     calling this from is valid and has only one activated timer. All timers from the other chain (the to-be-added
        ///     chain) are deactivated.
        /// </summary>
        /// <param name="otherTimer">The other timer.</param>
        /// <returns>
        ///     The instance at the end of the added chain in order to support a fluent interface.
        /// </returns>
        public Timer Connect(Timer otherTimer)
        {
            // Ensure that the other timer-chain is turned off.
            otherTimer.DeactivateChain();

            if (Next == null)
            {
                Next = this;
            }

            if (otherTimer.Next == null)
            {
                otherTimer.Next = otherTimer;
            }

            var last = otherTimer.Previous();
            last.Next = Next;
            Next = otherTimer;
            return last;
        }

        /// <summary>
        ///     Returns the timer that resides before this instance in the timer-chain. This method is necessary since the timers
        ///     are a singly-linked ring-list and not a doubly-linked one.
        /// </summary>
        /// <returns>The timer that resides before this timer in the timer-chain or itself, if it links to itself.</returns>
        public Timer Previous()
        {
            var timer = this;
            while (timer.Next != this)
            {
                timer = timer.Next;
            }

            return timer;
        }

        /// <summary>
        ///     Visits the chained timers and performs the given action on them.
        ///     <b>Does not perform the action on this instance!</b>
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>This instance in order to support a fluent interface.</returns>
        public Timer Visit(Action<Timer> action)
        {
            var timer = this;
            while (timer.Next != this)
            {
                timer = timer.Next;
                action(timer);
            }

            return this;
        }

        /// <summary>
        ///     Deactivates all chained timers and this instance as well.
        /// </summary>
        /// <returns>This instance in order to support a fluent interface.</returns>
        public Timer DeactivateChain()
        {
            IsActive = false;
            Visit(DeactivationAction);
            return this;
        }

        private void DeactivationAction(Timer timer)
        {
            timer.IsActive = false;
        }

        /// <summary>
        ///     Updates the timer and returns <see langword="true" /> if it has
        ///     fired.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        /// <param name="isReassignOverlap">
        ///     If set to <c>true</c> it reassigns the overlap (the rest of the passed time) to the timer (modulo timer-length of
        ///     course),
        ///     in order to loose no time.
        /// </param>
        /// <returns>
        ///     <see langword="true" />, if it has fired; Otherwise
        ///     <see langword="false" />.
        /// </returns>
        public bool Update(GameTime gameTime, bool isReassignOverlap = false)
        {
            return Update((float) gameTime.ElapsedGameTime.TotalMilliseconds, isReassignOverlap);
        }

        /// <summary>
        ///     Updates the timer and returns <see langword="true" /> if it has
        ///     fired.
        /// </summary>
        /// <param name="elapsedTimeInMilliseconds">The elapsed time in milliseconds.</param>
        /// <param name="isReassignOverlap">
        ///     If set to <c>true</c> it reassigns the overlap (the rest of the passed time) to the timer (modulo timer-length of
        ///     course),
        ///     in order to loose no time.
        /// </param>
        /// <returns>
        ///     <see langword="true" />, if it has fired; Otherwise
        ///     <see langword="false" />.
        /// </returns>
        public bool Update(float elapsedTimeInMilliseconds = 0f, bool isReassignOverlap = false)
        {
            if (!IsActive)
                return false;

            InvokeTimerUpdating();
            // The new value has to be saved separately because the Value cannot be higher than MaxValue, since it's a Fader.
            var newValue = Value + elapsedTimeInMilliseconds;
            Value = newValue;

            if (newValue >= MaxValue)
            {
                InvokeTimerFiring();
                if (isReassignOverlap)
                {
                    ValueInMillis = (float) newValue % (float) MaxValue;
                }
                else
                {
                    Value = MinValue;
                }

                IsActive = false;
                Next.IsActive = true;
                InvokeTimerFired();
                InvokeTimerUpdated();
                return true;
            }

            InvokeTimerUpdated();

            return false;
        }
    }
}