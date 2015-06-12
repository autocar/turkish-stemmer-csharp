namespace TurkishStemmer.States
{
    using Suffixes;
    public sealed class NounState : State
    {
        public const int A = 0, B = 1, C = 2, D = 3, E = 4, F = 5, G = 6, H = 7, K = 8, L = 9, M = 10;

        public static readonly NounState[] VALUES = new NounState[] {
            //A
            new NounState(true, true, NounSuffix.VALUES),
            //B
            new NounState(false, true, 
                NounSuffix.S1, NounSuffix.S2, NounSuffix.S3, NounSuffix.S4, NounSuffix.S5),
            //C
            new NounState(false, false, 
                NounSuffix.S6, NounSuffix.S7),
            //D
            new NounState(false, false, 
                NounSuffix.S10, NounSuffix.S13, NounSuffix.S14),
            //E
            new NounState(false, true, 
                NounSuffix.S1, NounSuffix.S2, NounSuffix.S3, NounSuffix.S4, NounSuffix.S5,
                NounSuffix.S6, NounSuffix.S7, NounSuffix.S18),
            //F
            new NounState(false, false, 
                NounSuffix.S6, NounSuffix.S7, NounSuffix.S18),
            //G
            new NounState(false, true, 
                NounSuffix.S1, NounSuffix.S2, NounSuffix.S3, NounSuffix.S4, NounSuffix.S5,
                NounSuffix.S18),
            //H
            new NounState(false, true, 
                NounSuffix.S1),
            //K
            new NounState(false, true),
            //L
            new NounState(false, true, 
                NounSuffix.S18),
            //M
            new NounState(false, true, 
                NounSuffix.S1, NounSuffix.S2, NounSuffix.S3, NounSuffix.S4, NounSuffix.S5,
                NounSuffix.S6, NounSuffix.S6, NounSuffix.S7)
        };

        public static NounState InitialState
        {
            get { return VALUES[A]; }
        }

        private NounState(bool initialState, bool finalState, params int[] suffixes)
            : this(initialState, finalState, Suffix.Get<NounSuffix>(suffixes))
        {
        }

        private NounState(bool initialState, bool finalState, Suffix[] suffixes)
            : base(initialState, finalState, suffixes)
        {
        }

        protected override State NextState(Suffix suffix)
        {
            object result = null;
            if (initialState && finalState)
            {
                switch (suffix.SuffixType)
                {
                    case NounSuffix.S8:
                    case NounSuffix.S11:
                    case NounSuffix.S13:
                        result = VALUES[B];
                        break;
                    case NounSuffix.S9:
                    case NounSuffix.S16:
                        result = VALUES[C];
                        break;
                    case NounSuffix.S18:
                        result = VALUES[D];
                        break;
                    case NounSuffix.S10:
                    case NounSuffix.S17:
                        result = VALUES[E];
                        break;
                    case NounSuffix.S12:
                    case NounSuffix.S14:
                        result = VALUES[F];
                        break;
                    case NounSuffix.S15:
                        result = VALUES[G];
                        break;
                    case NounSuffix.S2:
                    case NounSuffix.S3:
                    case NounSuffix.S4:
                    case NounSuffix.S5:
                    case NounSuffix.S6:
                        result = VALUES[H];
                        break;
                    case NounSuffix.S7:
                        result = VALUES[K];
                        break;
                    case NounSuffix.S1:
                        result = VALUES[L];
                        break;
                    case NounSuffix.S19:
                        result = VALUES[M];
                        break;
                }
            }
            else if (!initialState && finalState)
            {
                switch (suffix.SuffixType)
                {
                    case NounSuffix.S18:
                        result = VALUES[D];
                        break;
                    case NounSuffix.S2:
                    case NounSuffix.S3:
                    case NounSuffix.S4:
                    case NounSuffix.S5:
                    case NounSuffix.S6:
                        result = VALUES[H];
                        break;
                    case NounSuffix.S7:
                        result = VALUES[K];
                        break;
                    case NounSuffix.S1:
                        result = VALUES[L];
                        break;
                }
            }
            else if (!initialState && !finalState)
            {
                switch (suffix.SuffixType)
                {
                    case NounSuffix.S13:
                        result = VALUES[B];
                        break;
                    case NounSuffix.S18:
                        result = VALUES[D];
                        break;
                    case NounSuffix.S10:
                        result = VALUES[E];
                        break;
                    case NounSuffix.S14:
                        result = VALUES[F];
                        break;
                    case NounSuffix.S6:
                        result = VALUES[H];
                        break;
                    case NounSuffix.S7:
                        result = VALUES[K];
                        break;
                }
            }
            return result as NounState;
        }
    }
}
