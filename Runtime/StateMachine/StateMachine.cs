using System;
using System.Collections.Generic;
using UnityEngine;

namespace SBN.StateMachines
{
    /// <summary>
    /// A very simple Finite State Machine
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StateMachine<T> where T : class
    {
        private State<T> currentState;
        private Dictionary<Type, State<T>> states = new Dictionary<Type, State<T>>();

        /// <summary>
        /// Context object, which is the shared object between all the states.
        /// </summary>
        public T Context
        {
            get;
        }

        public State<T> CurrentState
        {
            get => currentState;
            private set => currentState = value;
        }
        
        public Dictionary<Type, State<T>> States
        {
            get => states;
            private set => states = value;
        }

        public StateMachine(T context, State<T> initialState)
        {
            Context = context;

            AddState(initialState);
            InternalChangeState(initialState);
        }

        /// <summary>
        /// Adds a new state to the statemachine
        /// </summary>
        /// <param name="state"></param>
        public void AddState(State<T> state)
        {
            var type = state.GetType();

            if (!States.ContainsKey(type))
            {
                state.StateMachine = this;
                state.Context = Context;

                States.Add(type, state);
            }
        }

        /// <summary>
        /// Removes a state from the statemachine
        /// </summary>
        /// <param name="state"></param>
        public void RemoveState(State<T> state)
        {
            var type = state.GetType();

            if (States.ContainsKey(type))
                States.Remove(type);
        }

        /// <summary>
        /// Changes state to the given state, if it exists in the state dictionary
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        public void ChangeState<TState>() where TState : State<T>
        {
            if (States.TryGetValue(typeof(TState), out State<T> state))
            {
                InternalChangeState(state);
            }
            else
            {
                Debug.LogError($"ERROR: State of type {typeof(TState)} was not present in the statemachine.");
            }
        }

        /// <summary>
        /// Returns a state of the given type, if it exists in the state dictionary
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <returns></returns>
        public State<T> GetState<TState>()
        {
            return States.TryGetValue(typeof(TState), out State<T> state) ? state : null;
        }

        /// <summary>
        /// Updates the current state of the machine. Must be called manually from outside
        /// </summary>
        /// <param name="deltaTime"></param>
        public void Update(float deltaTime)
        {
            var state = CurrentState;
            CurrentState.CheckTransitions();

            // Only update the current state, if it hasn't changed from a transition update
            if (state == CurrentState)
                CurrentState.UpdateState(deltaTime);
        }

        /// <summary>
        /// Used internally to change the current state to a given instance state
        /// </summary>
        /// <param name="state"></param>
        private void InternalChangeState(State<T> state)
        {
            CurrentState?.ExitState();
            CurrentState = state;
            CurrentState.EnterState();
        }
    }
}
