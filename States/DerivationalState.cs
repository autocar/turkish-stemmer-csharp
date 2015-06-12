namespace TurkishStemmer.States
{
    using Suffixes;

    public class DerivationalState : State
    {
        public const int A = 0, B = 1;

        public static readonly DerivationalState[] VALUES = new DerivationalState[]
        {
            new DerivationalState(true, false, DerivationalSuffix.S1),
            new DerivationalState(false, true)
        };

        public static DerivationalState InitialState
        {
            get { return VALUES[A]; }
        }

        private DerivationalState(bool initialState, bool finalState, params int[] suffixes)
            : this(initialState, finalState, Suffix.Get<DerivationalSuffix>(suffixes))
        {
        }

        private DerivationalState(bool initialState, bool finalState, Suffix[] suffixes)
            : base(initialState, finalState, suffixes)
        {
        }

        protected override State NextState(Suffix suffix)
        {
            return suffix.SuffixType == DerivationalSuffix.S1 ? VALUES[B] : null;
        }
    }
}