using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace TurkishStemmer
{
    using States;
    using Suffixes;
    using Transitions;

    public class TurkishStemmer
    {
        /// <summary>
        /// The turkish characters. They are used for skipping not turkish words.
        /// </summary>
        public static readonly HashSet<Char> ALPHABET = new HashSet<Char>("abcçdefgğhıijklmnoöprsştuüvyz");
        /// <summary>
        /// The turkish vowels.
        /// </summary>
        public static readonly HashSet<Char> VOWELS = new HashSet<Char>("üiıueöao");
        /// <summary>
        /// The turkish consonants.
        /// </summary>
        public static readonly HashSet<Char> CONSONANTS = new HashSet<Char>("bcçdfgğhjklmnprsştvyz");
        /// <summary>
        /// Rounded vowels which are used for checking roundness harmony.
        /// </summary>
        public static readonly HashSet<Char> ROUNDED_VOWELS = new HashSet<Char>("oöuü");
        /// <summary>
        /// Vowels that follow rounded vowels. They are combined with ROUNDED_VOWELS to check roundness harmony.
        /// </summary>
        public static readonly HashSet<Char> FOLLOWING_ROUNDED_VOWELS = new HashSet<Char>("aeuü");
        /// <summary>
        /// The unrounded vowels which are used for checking roundness harmony.
        /// </summary>
        public static readonly HashSet<Char> UNROUNDED_VOWELS = new HashSet<Char>("iıea");
        /// <summary>
        /// Front vowels which are used for checking frontness harmony.
        /// </summary>
        public static readonly HashSet<Char> FRONT_VOWELS = new HashSet<Char>("eiöü");
        /// <summary>
        /// Front vowels which are used for checking frontness harmony.
        /// </summary>
        public static readonly HashSet<Char> BACK_VOWELS = new HashSet<Char>("ıuao");
        /// <summary>
        /// The path of the file that contains the default set of protected words.
        /// </summary>
        public const String DEFAULT_PROTECTED_WORDS_FILE = "protected_words.txt";
        /// <summary>
        /// The path of the file that contains the default set of vowel harmony exceptions.
        /// </summary>
        public const String DEFAULT_VOWEL_HARMONY_EXCEPTIONS_FILE = "vowel_harmony_exceptions.txt";
        /// <summary>
        /// The path of the file that contains the default set of last consonant exceptions.
        /// </summary>
        public const String DEFAULT_LAST_CONSONANT_EXCEPTIONS_FILE = "last_consonant_exceptions.txt";
        /// <summary>
        /// The path of the file that contains the default set of average stem size exceptions.
        /// </summary>
        public const String DEFAULT_AVERAGE_STEM_SIZE_EXCEPTIONS_FILE = "average_stem_size_exceptions.txt";
        /// <summary>
        /// The set of nominal verb states that a word may pass during the stemming phase.
        /// </summary>
        private static readonly HashSet<NominalVerbState> nominalVerbStates = new HashSet<NominalVerbState>(NominalVerbState.VALUES);
        /// <summary>
        /// The set of noun states that a word may pass during the stemming phase.
        /// </summary>
        private static readonly HashSet<NounState> nounStates = new HashSet<NounState>(NounState.VALUES);
        /// <summary>
        /// The set of derivational states that a word may pass during the stemming phase.
        /// </summary>
        private static readonly HashSet<DerivationalState> derivationalStates = new HashSet<DerivationalState>(DerivationalState.VALUES);
        /// <summary>
        /// The set of nominal verb suffixes that the stemmer recognizes.
        /// </summary>
        private static readonly HashSet<NominalVerbSuffix> nominalVerbSuffixes = new HashSet<NominalVerbSuffix>(NominalVerbSuffix.VALUES);
        /// <summary>
        /// The set of noun suffixes that the stemmer recognizes.
        /// </summary>
        private static readonly HashSet<NounSuffix> nounSuffixes = new HashSet<NounSuffix>(NounSuffix.VALUES);
        /// <summary>
        /// The set of derivational suffixes that the stemmer recognizes.
        /// </summary>
        private static readonly HashSet<DerivationalSuffix> derivationalSuffixes = new HashSet<DerivationalSuffix>(DerivationalSuffix.VALUES);

        /// <summary>
        /// The average size of turkish stems based on which the selection of the final stem is performed.
        /// The idea behind the selection process is based on the paper
        /// F.Can, S.Kocberber, E.Balcik, C.Kaynak, H.Cagdas, O.Calan, O.Vursavas
        /// "Information Retrieval on Turkish Texts"
        /// </summary>
        private static readonly int AVERAGE_STEMMED_SIZE = 4;

        private readonly ISet<string> protectedWords;
        private readonly ISet<string> vowelHarmonyExceptions;
        private readonly ISet<string> lastConsonantExceptions;
        private readonly ISet<string> averageStemSizeExceptions;

        public TurkishStemmer(
            ISet<string> protectedWords,
            ISet<string> vowelHarmonyExceptions,
            ISet<string> lastConsonantExceptions,
            ISet<string> averageStemSizeExceptions)
        {
            this.protectedWords = protectedWords;
            this.vowelHarmonyExceptions = vowelHarmonyExceptions;
            this.lastConsonantExceptions = lastConsonantExceptions;
            this.averageStemSizeExceptions = averageStemSizeExceptions;
        }

        public TurkishStemmer()
            : this(DefaultSetHolder.DEFAULT_PROTECTED_WORDS,
            DefaultSetHolder.DEFAULT_VOWEL_HARMONY_EXCEPTIONS,
            DefaultSetHolder.DEFAULT_LAST_CONSONANT_EXCEPTIONS,
            DefaultSetHolder.DEFAULT_AVERAGE_STEM_SIZE_EXCEPTIONS)
        {
        }

        /// <summary>
        /// Finds the stem of a given word.
        /// </summary>
        /// <param name="s">an array with the characters of the word</param>
        /// <param name="len">the length of the word</param>
        /// <returns>the stemmed word</returns>
        public string Stem(IEnumerable<Char> s, int len)
        {
            return Stem(new string(s.ToArray(), 0, len));
        }

        /// <summary>
        /// Finds the stem of a given word.
        /// </summary>
        /// <param name="word">the word should be lowercased</param>
        /// <returns>the stemmed word</returns>
        public string Stem(string word)
        {
            string originalWord = word;
            if (!ProceedToStem(originalWord))
                return originalWord;

            ISet<string> stems;
            ISet<string> wordsToStem;
            stems = new HashSet<string>();

            // Process the word with the nominal verb suffix state machine.
            NominalVerbSuffixStripper(originalWord, stems);

            wordsToStem = new HashSet<string>(stems);
            wordsToStem.Add(originalWord);

            foreach (string w in wordsToStem)
            {
                // Process each possible stem with the noun suffix state machine.
                NounSuffixStripper(w, stems);
            }

            wordsToStem = new HashSet<string>(stems);
            wordsToStem.Add(originalWord);

            foreach (string w in wordsToStem)
            {
                // Process each possible stem with the derivational suffix state machine.
                DerivationalSuffixStripper(w, stems);
            }
            return PostProcess(stems, originalWord);
        }

        /// <summary>
        /// This method implements the state machine about nominal verb suffixes.
        /// It finds the possible stems of a word after applying the nominal verb
        /// suffix removal.
        /// </summary>
        /// <param name="word">the word that will get stemmed</param>
        /// <param name="stems">a set of stems to populate</param>
        public void NominalVerbSuffixStripper(string word, ISet<string> stems)
        {
            NominalVerbState initialState;
            if (nominalVerbStates.Count == 0 || nominalVerbSuffixes.Count == 0)
                return;

            initialState = NominalVerbState.InitialState;
            GenericSuffixStripper(initialState, word, stems, "NominalVerb");
        }

        /// <summary>
        /// This method implements the state machine about noun suffixes.
        /// It finds the possible stems of a word after applying the noun suffix removal.
        /// </summary>
        /// <param name="word">the word that will get stemmed</param>
        /// <param name="stems">a set of stems to populate</param>
        public void NounSuffixStripper(string word, ISet<string> stems)
        {
            NounState initialState;
            if (nounStates.Count == 0 || nounSuffixes.Count == 0)
                return;
            initialState = NounState.InitialState;
            GenericSuffixStripper(initialState, word, stems, "Noun");
        }

        /// <summary>
        /// This method implements the state machine about derivational suffixes.
        /// It finds the possible stems of a word after applying the derivational
        /// suffix removal.
        /// </summary>
        /// <param name="word">the word that will get stemmed</param>
        /// <param name="stems">a set of stems to populate</param>
        public void DerivationalSuffixStripper(string word, ISet<string> stems)
        {
            DerivationalState initialState;
            if (derivationalStates.Count == 0 || derivationalSuffixes.Count == 0)
                return;
            initialState = DerivationalState.InitialState;
            GenericSuffixStripper(initialState, word, stems, "Derivational");
        }

        /// <summary>
        /// Given the initial state of a state machine, it adds possible stems to a set of stems.
        /// </summary>
        /// <param name="initialState">an initial state</param>
        /// <param name="word">the word to stem</param>
        /// <param name="stems">the set to populate</param>
        /// <param name="machine">a string representing the name of the state machine. It is used for debugging reasons only.</param>
        private void GenericSuffixStripper(
            State initialState,
            string word,
            ISet<string> stems,
            string machine)
        {
            string stem, wordToStem;
            Transition transition;

            LinkedList<Transition> transitions = new LinkedList<Transition>();
            wordToStem = word;

            initialState.AddTransitions(wordToStem, transitions, false);

            while (transitions.Count > 0)
            {
                transition = transitions.First.Value;
                transitions.RemoveFirst();
                wordToStem = transition.Word;

                stem = StemWord(wordToStem, transition.Suffix);

                if (!stem.Equals(wordToStem, StringComparison.Ordinal))
                {
                    if (transition.NextState.IsFinalState)
                    {
                        foreach (Transition transitionToRemove in transitions.ToArray())
                        {
                            if ((transitionToRemove.StartState == transition.StartState &&
                                transitionToRemove.NextState == transition.NextState) ||
                                transitionToRemove.Marked)
                            {
                                transitions.Remove(transitionToRemove);
                            }
                        }
                        stems.Add(stem);
                        transition.NextState.AddTransitions(stem, transitions, false);
                    }
                    else
                    {
                        foreach (Transition similarTransition in transition.SimilarTransitions(transitions))
                        {
                            similarTransition.Marked = true;
                        }
                        transition.NextState.AddTransitions(stem, transitions, true);
                    }
                }
            }
        }

        /// <summary>
        /// Removes a certain suffix from the given word.
        /// </summary>
        /// <param name="word">the word to remove the suffix from</param>
        /// <param name="suffix">the suffix to be removed from the word</param>
        /// <returns>the stemmed word</returns>
        public string StemWord(string word, Suffix suffix)
        {
            string stemmedWord = word;
            if (ShouldBeMarked(word, suffix) && suffix.Match(word))
                stemmedWord = suffix.RemoveSuffix(stemmedWord);
            char optionalLetter = suffix.OptionalLetter(stemmedWord);
            if (optionalLetter != '\0')
            {
                if (ValidOptionalLetter(stemmedWord, optionalLetter))
                {
                    stemmedWord = stemmedWord.Substring(0, stemmedWord.Length - 1);
                }
                else
                {
                    stemmedWord = word;
                }
            }
            return stemmedWord;
        }

        /// <summary>
        /// It performs a post stemming process and returns the final stem.
        /// </summary>
        /// <param name="stems">a set of possible stems</param>
        /// <param name="originalWord">the original word that was stemmed</param>
        /// <returns>final stem</returns>
        public string PostProcess(ISet<string> stems, string originalWord)
        {
            ISet<string> finalStems = new HashSet<string>();
            stems.Remove(originalWord);

            foreach (string word in stems)
            {
                if (CountSyllables(word) > 0)
                {
                    finalStems.Add(LastConsonant(word));
                }
            }
            List<string> sortedStems = new List<string>(finalStems);
            sortedStems.Sort((s1, s2) =>
            {
                if (averageStemSizeExceptions.Contains(s1))
                    return -1;
                if (averageStemSizeExceptions.Contains(s2))
                    return 1;

                int average_distance = Math.Abs(s1.Length - AVERAGE_STEMMED_SIZE) - Math.Abs(s2.Length - AVERAGE_STEMMED_SIZE);
                return average_distance == 0 ? s1.Length - s2.Length : average_distance;
            });
            return sortedStems.Count == 0 ? originalWord : sortedStems[0];
        }

        /// <summary>
        /// Gets the vowels of a word.
        /// </summary>
        /// <param name="word">the word to get its vowels</param>
        /// <returns>the vowels</returns>
        public static string Vowels(string word)
        {
            return new string(word.Where(VOWELS.Contains).ToArray());
        }

        /// <summary>
        /// Gets the number of syllables of a word.
        /// </summary>
        /// <param name="word">the word to count its syllables</param>
        /// <returns>the number of syllables</returns>
        public static int CountSyllables(string word)
        {
            return Vowels(word).Length;
        }

        /// <summary>
        /// Checks the frontness harmony of two characters.
        /// </summary>
        /// <param name="vowel">the first character</param>
        /// <param name="candidate">candidate the second character</param>
        /// <returns>whether the two characters have frontness harmony or not.</returns>
        public static bool HasFrontness(char vowel, char candidate)
        {
            return ((FRONT_VOWELS.Contains(vowel) && FRONT_VOWELS.Contains(candidate)) ||
                    (BACK_VOWELS.Contains(vowel) && BACK_VOWELS.Contains(candidate)));
        }

        /// <summary>
        /// Checks the roundness harmony of two characters.
        /// </summary>
        /// <param name="vowel">the first character</param>
        /// <param name="candidate">the second character</param>
        /// <returns>whether the two characters have roundness harmony or not.</returns>
        public static bool HasRoundness(char vowel, char candidate)
        {
            return ((UNROUNDED_VOWELS.Contains(vowel) && UNROUNDED_VOWELS.Contains(candidate)) ||
                    (ROUNDED_VOWELS.Contains(vowel) && FOLLOWING_ROUNDED_VOWELS.Contains(candidate)));
        }

        /// <summary>
        /// Checks the vowel harmony of two characters.
        /// </summary>
        /// <param name="vowel">the first character</param>
        /// <param name="candidate">the second character</param>
        /// <returns>whether the two characters have vowel harmony or not.</returns>
        public static bool VowelHarmony(char vowel, char candidate)
        {
            return HasRoundness(vowel, candidate) && HasFrontness(vowel, candidate);
        }

        /// <summary>
        /// Checks the vowel harmony of a word.
        /// </summary>
        /// <param name="word">word  the word to check its vowel harmony</param>
        /// <returns>whether the word has vowel harmony or not.</returns>
        public static bool HasVowelHarmony(string word)
        {
            string vowelsOfWord = Vowels(word);
            int wordLength = vowelsOfWord.Length;
            char vowel, candidate;
            try
            {
                vowel = vowelsOfWord[wordLength - 2];
            }
            catch (IndexOutOfRangeException)
            {
                return true;
            }
            try
            {
                candidate = vowelsOfWord[wordLength - 1];
            }
            catch (IndexOutOfRangeException)
            {
                return true;
            }
            return VowelHarmony(vowel, candidate);
        }

        /// <summary>
        /// Checks the last consonant rule of a word.
        /// </summary>
        /// <param name="word">the word to check its last consonant</param>
        /// <returns>the new word affected by the last consonant rule</returns>
        public string LastConsonant(string word)
        {
            if (lastConsonantExceptions.Contains(word))
                return word;

            int wordLength = word.Length;
            char lastChar = word[wordLength - 1];
            bool replaced = true;
            switch (lastChar)
            {
                case 'b':
                    lastChar = 'p';
                    break;
                case 'c':
                    lastChar = 'ç';
                    break;
                case 'd':
                    lastChar = 't';
                    break;
                case 'ğ':
                    lastChar = 'k';
                    break;
                default:
                    replaced = false;
                    break;
            }
            return replaced ? string.Concat(word.Remove(wordLength - 1), lastChar) : word;
        }

        /// <summary>
        /// Checks whether an optional letter is valid or not.
        /// </summary>
        /// <param name="word">the word to check its last letter</param>
        /// <param name="candidate">the last character candidate</param>
        /// <returns>whether is valid or not</returns>
        /// <remarks>One should check if candidate character exists or not.</remarks>
        public static bool ValidOptionalLetter(string word, char candidate)
        {
            int wordLength = word.Length;
            char previousChar;
            try
            {
                previousChar = word[wordLength - 2];
            }
            catch (IndexOutOfRangeException)
            {
                return false;
            }

            if (VOWELS.Contains(candidate))
            {
                return CONSONANTS.Contains(previousChar);
            }
            else
            {
                return VOWELS.Contains(previousChar);
            }
        }

        /// <summary>
        /// Checks whether a word is written in Turkish alphabet or not.
        /// </summary>
        /// <param name="word">the word to check its letters</param>
        /// <returns>whether contains only Turkish letters or not.</returns>
        public static bool IsTurkish(string word)
        {
            return word.All(ALPHABET.Contains);
        }

        /// <summary>
        /// Checks whether a stem process should proceed or not.
        /// </summary>
        /// <param name="word">the word to check for stem</param>
        /// <returns>whether to proceed or not</returns>
        public bool ProceedToStem(string word)
        {
            if (string.IsNullOrEmpty(word))
                return false;

            if (!IsTurkish(word))
                return false;

            if (this.protectedWords.Contains(word))
                return false;

            if (CountSyllables(word) < 2)
                return false;

            return true;
        }

        /// <summary>
        /// Checks if a word should be stemmed or not.
        /// </summary>
        /// <param name="word">the word to be checked</param>
        /// <param name="suffix">the suffix that will be removed from the word</param>
        /// <returns>whether the word should be stemmed or not</returns>
        public bool ShouldBeMarked(string word, Suffix suffix)
        {
            return (!this.protectedWords.Contains(word) &&
                    (suffix.CheckHarmony && (HasVowelHarmony(word) || this.vowelHarmonyExceptions.Contains(word))) ||
                    !suffix.CheckHarmony);
        }

        private static ISet<string> LoadWordSet(string resourceName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            resourceName = string.Join(".", assembly.GetName().Name, "Resources", resourceName);
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (TextReader reader = new StreamReader(stream))
            {
                var result = new HashSet<string>();
                string word;
                while ((word = reader.ReadLine()) != null)
                {
                    result.Add(word.Trim());
                }
                return result;
            }
        }

        private static class DefaultSetHolder
        {
            internal static ISet<string> DEFAULT_PROTECTED_WORDS { get; private set; }
            internal static ISet<string> DEFAULT_VOWEL_HARMONY_EXCEPTIONS { get; private set; }
            internal static ISet<string> DEFAULT_LAST_CONSONANT_EXCEPTIONS { get; private set; }
            internal static ISet<string> DEFAULT_AVERAGE_STEM_SIZE_EXCEPTIONS { get; private set; }

            static DefaultSetHolder()
            {
                try
                {
                    DEFAULT_PROTECTED_WORDS = LoadWordSet(DEFAULT_PROTECTED_WORDS_FILE);
                }
                catch (IOException)
                {
                    Console.WriteLine("Unable to load default protected words");
                }

                try
                {
                    DEFAULT_VOWEL_HARMONY_EXCEPTIONS = LoadWordSet(DEFAULT_VOWEL_HARMONY_EXCEPTIONS_FILE);
                }
                catch (IOException)
                {
                    Console.WriteLine("Unable to load default vowel harmony exceptions");
                }

                try
                {
                    DEFAULT_LAST_CONSONANT_EXCEPTIONS = LoadWordSet(DEFAULT_LAST_CONSONANT_EXCEPTIONS_FILE);
                }
                catch (IOException)
                {
                    Console.WriteLine("Unable to load default vowel harmony exceptions");
                }

                try
                {
                    DEFAULT_AVERAGE_STEM_SIZE_EXCEPTIONS = LoadWordSet(DEFAULT_AVERAGE_STEM_SIZE_EXCEPTIONS_FILE);
                }
                catch (IOException)
                {
                    Console.WriteLine("Unable to load default average stem size exceptions");
                }
            }
        }
    }
}