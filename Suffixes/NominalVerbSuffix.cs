
namespace TurkishStemmer.Suffixes
{
    public sealed class NominalVerbSuffix : Suffix
    {
        public NominalVerbSuffix(string name, string pattern, string optionalLetter, bool checkHarmony, int suffixType) :
            base(name, pattern, optionalLetter, checkHarmony, suffixType)
        {
        }

        public const int 
            S11 = 0, S4 = 1, S14 = 2, S15 = 3, S2 = 4, 
            S5 = 5, S9 = 6, S10 = 7, S3 = 8, S1 = 9,
            S12 = 10, S13 = 11, S6 = 12, S7 = 13, S8 = 14;

        /// <remarks>
        /// The order of the enum definition determines the priority of the suffix.
        /// For example, -(y)ken (S15 suffix) is  checked before -n (S7 suffix).
        /// </remarks>
        public static readonly NominalVerbSuffix[] VALUES = new NominalVerbSuffix[]
        {
            new NominalVerbSuffix("-cAsInA",    "casına|çasına|cesine|çesine",      null, true,     S11),
            new NominalVerbSuffix("-sUnUz",     "sınız|siniz|sunuz|sünüz",          null, true,     S4),
            new NominalVerbSuffix("-(y)mUş",    "muş|miş|müş|mış",                  "y", true,      S14),
            new NominalVerbSuffix("-(y)ken",    "ken",                              "y", true,      S15),
            new NominalVerbSuffix("-sUn",       "sın|sin|sun|sün",                  null, true,     S2),
            new NominalVerbSuffix("-lAr",       "lar|ler",                          null, true,     S5),
            new NominalVerbSuffix("-nUz",       "nız|niz|nuz|nüz",                  null, true,     S9),
            new NominalVerbSuffix("-DUr",       "tır|tir|tur|tür|dır|dir|dur|dür",  null, true,     S10),
            new NominalVerbSuffix("-(y)Uz",     "ız|iz|uz|üz",                      "y", true,      S3),
            new NominalVerbSuffix("-(y)Um",     "ım|im|um|üm",                      "y", true,      S1),
            new NominalVerbSuffix("-(y)DU",     "dı|di|du|dü|tı|ti|tu|tü",          "y", true,      S12),
            new NominalVerbSuffix("-(y)sA",     "sa|se",                            "y", true,      S13),
            new NominalVerbSuffix("-m",         "m",                                null, true,     S6),
            new NominalVerbSuffix("-n",         "n",                                null, true,     S7),
            new NominalVerbSuffix("-k",         "k",                                null, true,     S8)
        };
    }
}
