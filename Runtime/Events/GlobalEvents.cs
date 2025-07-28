using System;
using System.Collections.Generic;
using System.Linq;

namespace SBN.Events
{
    /// <summary>
    /// Very basic generic global events system.
    /// Can be used to publish, subscribe and unsubscribe to events 
    /// all over the application. 
    /// 
    /// This eventhandler also makes sure there's no accidental 
    /// 'double subscriptions' by using a HashSet. 
    /// 
    /// Events (TEvent) must be a struct.
    /// </summary>
    public static class GlobalEvents<TEvent> where TEvent : struct
    {
        private class Subscription : IComparable<Subscription>
        {
            public Action<TEvent> Callback;
            public int ExecutionOrder;

            public Subscription(Action<TEvent> callback, int executionOrder)
            {
                Callback = callback;
                ExecutionOrder = executionOrder;
            }

            public int CompareTo(Subscription other)
            {
                return ExecutionOrder.CompareTo(other.ExecutionOrder);
            }
        }

        private static readonly List<Subscription> subscriptions = new();
        private static List<Subscription> sortedSubscriptions = new();

        public static void Publish(TEvent args)
        {
            // Make a new list, to prevent subcriptions from being removed while an event is being published to all the subscribers.
            // Could perhaps just simply iterate the list backwards, but test it..
            sortedSubscriptions.Clear();
            sortedSubscriptions.AddRange(subscriptions);
            sortedSubscriptions.Sort();

            for (int i = 0; i < sortedSubscriptions.Count; i++)
            {
                sortedSubscriptions[i].Callback.Invoke(args);
            }
        }

        public static void Subscribe(Action<TEvent> callback, int executionOrder = 0)
        {
            if (subscriptions.Any(s => s.Callback == callback))
                return;

            subscriptions.Add(new Subscription(callback, executionOrder));
        }

        public static void Unsubscribe(Action<TEvent> callback)
        {
            subscriptions.RemoveAll(s => s.Callback == callback);
        }
    }
}