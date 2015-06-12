using System;
using System.Collections.Generic;

namespace TurkishStemmer.States
{
    using Suffixes;
    using Transitions;
    public abstract class State
    {
        protected readonly bool initialState;
        protected readonly bool finalState;
        protected readonly Suffix[] suffixes;

        public State(Boolean initialState, Boolean finalState, Suffix[] suffixes)
        {
            this.initialState = initialState;
            this.finalState = finalState;
            this.suffixes = suffixes;
        }

        /// <summary>
        /// Checks if the state is an initial state.
        /// </summary>
        public Boolean IsInitialState
        {
            get
            {
                return initialState;
            }
        }

        /// <summary>
        /// Checks if the state is final.
        /// </summary>
        public Boolean IsFinalState
        {
            get
            {
                return finalState;
            }
        }
           
        /// <summary>
        /// Adds possible transitions from the current state to other states
        /// about a word to a given list.
        /// </summary>
        /// <param name="word">a word to search transitions for</param>
        /// <param name="transitions">transitions the initial list to add transitions</param>
        /// <param name="marked">marked whether to mark the transitions as marked</param>
        public void AddTransitions(String word, LinkedList<Transition> transitions, Boolean marked)
        {
            foreach (Suffix suffix in suffixes)
            {
                if (suffix.Match(word))
                {
                    transitions.AddLast(new Transition(this, NextState(suffix), word, suffix, marked));
                }
            }
        }

        protected abstract State NextState(Suffix suffix);
    }
}