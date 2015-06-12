
namespace TurkishStemmer.Suffixes
{
    public sealed class DerivationalSuffix : Suffix
    {
        public const int S1 = 0;

        public static readonly DerivationalSuffix[] VALUES = new DerivationalSuffix[] 
        {
            new DerivationalSuffix("-lU", "lı|li|lu|lü", null, true, S1)
        };

        public DerivationalSuffix(string name, string pattern, string optionalLetter, bool checkHarmony, int suffixType) :
            base(name, pattern, optionalLetter, checkHarmony, suffixType)
        {
        }
    }
}