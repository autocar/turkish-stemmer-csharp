namespace TurkishStemmer.States
{
    using Suffixes;
    public sealed class NominalVerbState : State
    {
        public const int A = 0, B = 1, C = 2, D = 3, E = 4, F = 5, G = 6, H = 7;

        public static readonly NominalVerbState[] VALUES = new NominalVerbState[] 
        {
            //A
            new NominalVerbState(true, false, NominalVerbSuffix.VALUES),
            //B
            new NominalVerbState(false, true, NominalVerbSuffix.S14),
            //C
            new NominalVerbState(false, true, NominalVerbSuffix.S10, NominalVerbSuffix.S12, NominalVerbSuffix.S13, NominalVerbSuffix.S14),
            //D
            new NominalVerbState(false, false, NominalVerbSuffix.S12, NominalVerbSuffix.S13),
            //E
            new NominalVerbState(false, true, NominalVerbSuffix.S1, NominalVerbSuffix.S2, NominalVerbSuffix.S3, 
                NominalVerbSuffix.S4, NominalVerbSuffix.S5, NominalVerbSuffix.S14),
            //F
            new NominalVerbState(false, true),
            //G
            new NominalVerbState(false, false, NominalVerbSuffix.S14),
            //H
            new NominalVerbState(false, false, NominalVerbSuffix.S1, NominalVerbSuffix.S2, NominalVerbSuffix.S3, 
                NominalVerbSuffix.S4, NominalVerbSuffix.S5, NominalVerbSuffix.S14)
        };

        public static NominalVerbState InitialState
        {
            get { return VALUES[A]; }
        }

        private NominalVerbState(bool initialState, bool finalState, params int[] suffixes)
            : this(initialState, finalState, Suffix.Get<NominalVerbSuffix>(suffixes))
        {
        }

        private NominalVerbState(bool initialState, bool finalState, Suffix[] suffixes)
            : base(initialState, finalState, suffixes)
        {
        }

        protected override State NextState(Suffix suffix)
        {
            object result = null;
            if (initialState && !finalState)
            {
                switch (suffix.SuffixType)
                {
                    case NominalVerbSuffix.S1:
                    case NominalVerbSuffix.S2:
                    case NominalVerbSuffix.S3:
                    case NominalVerbSuffix.S4:
                        result = VALUES[B];
                        break;
                    case NominalVerbSuffix.S5:
                        result = VALUES[C];
                        break;
                    case NominalVerbSuffix.S6:
                    case NominalVerbSuffix.S7:
                    case NominalVerbSuffix.S8:
                    case NominalVerbSuffix.S9:
                        result = VALUES[D];
                        break;
                    case NominalVerbSuffix.S10:
                        result = VALUES[E];
                        break;
                    case NominalVerbSuffix.S12:
                    case NominalVerbSuffix.S13:
                    case NominalVerbSuffix.S14:
                    case NominalVerbSuffix.S15:
                        result = VALUES[F];
                        break;
                    case NominalVerbSuffix.S11:
                        result = VALUES[H];
                        break;
                }
            }
            else if (!initialState && finalState)
            {
                switch (suffix.SuffixType)
                {
                    case NominalVerbSuffix.S1:
                    case NominalVerbSuffix.S2:
                    case NominalVerbSuffix.S3:
                    case NominalVerbSuffix.S4:
                    case NominalVerbSuffix.S5:
                        result = VALUES[G];
                        break;
                    case NominalVerbSuffix.S10:
                    case NominalVerbSuffix.S12:
                    case NominalVerbSuffix.S13:
                    case NominalVerbSuffix.S14:
                        result = VALUES[F];
                        break;

                }
            }
            else if (!initialState && !finalState)
            {
                switch (suffix.SuffixType)
                {
                    case NominalVerbSuffix.S1:
                    case NominalVerbSuffix.S2:
                    case NominalVerbSuffix.S3:
                    case NominalVerbSuffix.S4:
                    case NominalVerbSuffix.S5:
                        result = VALUES[G];
                        break;
                    case NominalVerbSuffix.S12:
                    case NominalVerbSuffix.S13:
                    case NominalVerbSuffix.S14:
                        result = VALUES[F];
                        break;
                }
            }
            return result as NominalVerbState;
        }
    }
}
