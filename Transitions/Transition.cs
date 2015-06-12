using System;
using System.Collections.Generic;

namespace TurkishStemmer.Transitions
{
    using States;
    using Suffixes;
    public class Transition
    {
        private readonly State startState;
        private readonly State nextState;
        private readonly string word;
        private readonly Suffix suffix;
        private bool marked;

        public Transition(State startState, State nextState, string word, Suffix suffix, bool marked)
        {
            this.startState = startState;
            this.nextState = nextState;
            this.word = word;
            this.suffix = suffix;
            this.marked = false;
        }

        public State StartState
        {
            get { return startState; }
        }

        public State NextState
        {
            get { return nextState; }
        }

        public String Word
        {
            get { return word; }
        }

        public Suffix Suffix
        {
            get { return suffix; }
        }

        public Boolean Marked
        {
            get { return marked; }
            set { marked = value; }
        }

        public IEnumerable<Transition> SimilarTransitions(IEnumerable<Transition> transitions)
        {
            foreach (Transition transition in transitions)
            {
                if (this.startState == transition.startState &&
                    this.nextState == transition.nextState)
                {
                    yield return transition;
                }
            }
        }
    }
}
