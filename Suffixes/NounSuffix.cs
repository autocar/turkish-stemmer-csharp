
namespace TurkishStemmer.Suffixes
{
    public sealed class NounSuffix : Suffix
    {
        public const int
            S16 = 0, S7 = 1, S3 = 2, S5 = 3, S1 = 4, 
            S14 = 5, S15 = 6, S17 = 7, S10 = 8, S19 = 9,
            S4 = 10, S9 = 11, S12 = 12, S13 = 13, S18 = 14, 
            S2 = 15, S6 = 16, S8 = 17, S11 = 18;

        /// <remarks>
        /// The order of the enum definition determines the priority of the suffix.
        /// For example, -nU (S9 suffix) is  checked before -(s)U (S6 suffix).
        /// </remarks>
        public static readonly NounSuffix[] VALUES = new NounSuffix[] 
        {
            new NounSuffix("-nDAn",     "ndan|ntan|nden|nten",      null,       true,   S16),
            new NounSuffix("-lArI",     "ları|leri",                null,       true,   S7),
            new NounSuffix("-(U)mUz",   "mız|miz|muz|müz",          "ı|i|u|ü",  true,   S3),
            new NounSuffix("-(U)nUz",   "nız|niz|nuz|nüz",          "ı|i|u|ü",  true,   S5),
            new NounSuffix("-lAr",      "lar|ler",                  null,       true,   S1),
            new NounSuffix("-nDA",      "nta|nte|nda|nde",          null,       true,   S14),
            new NounSuffix("-DAn",      "dan|tan|den|ten",          null,       true,   S15),
            new NounSuffix("-(y)lA",    "la|le",                    "y",        true,   S17),
            new NounSuffix("-(n)Un",    "ın|in|un|ün",              "n",        true,   S10),
            new NounSuffix("-(n)cA",    "ca|ce",                    "n",        true,   S19),
            new NounSuffix("-Un",       "ın|in|un|ün",              null,       true,   S4),
            new NounSuffix("-nU",       "nı|ni|nu|nü",              null,       true,   S9),
            new NounSuffix("-nA",       "na|ne",                    null,       true,   S12),
            new NounSuffix("-DA",       "da|de|ta|te",              null,       true,   S13),
            new NounSuffix("-ki",       "ki",                       null,       false,  S18),
            new NounSuffix("-(U)m",     "m",                        "ı|i|u|ü",  true,   S2),
            new NounSuffix("-(s)U",     "ı|i|u|ü",                  "s",        true,   S6),
            new NounSuffix("-(y)U",     "ı|i|u|ü",                  "y",        true,   S8),
            new NounSuffix("-(y)A",     "a|e",                      "y",        true,   S11)
        };

        public NounSuffix(string name, string pattern, string optionalLetter, bool checkHarmony, int suffixType) :
            base(name, pattern, optionalLetter, checkHarmony, suffixType)
        {
        }
    }
}
