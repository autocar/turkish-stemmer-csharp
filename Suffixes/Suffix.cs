using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TurkishStemmer.Suffixes
{
    public abstract class Suffix
    {
        private readonly string name;
        private readonly Regex pattern;
        private readonly bool checkHarmony;
        private readonly Regex optionalLetterPattern;
        private readonly bool optionalLetterCheck;
        private readonly int suffixType;

        public Suffix(string name, string pattern, string optionalLetter, bool checkHarmony, int suffixType)
        {
            this.name = name;
            this.suffixType = suffixType;
            this.pattern = new Regex("(" + pattern + ")$", RegexOptions.Compiled);
            if (optionalLetter == null)
            {
                this.optionalLetterCheck = false;
                this.optionalLetterPattern = null;
            }
            else
            {
                this.optionalLetterCheck = true;
                this.optionalLetterPattern = new Regex("(" + optionalLetter + ")$", RegexOptions.Compiled);
            }
            this.checkHarmony = checkHarmony;
        }

        /// <summary>
        /// Checks if a word has the certain suffix.
        /// </summary>
        /// <param name="word">the word to check about the suffix match</param>
        /// <returns>whether the word has the certain suffix or not</returns>
        public Boolean Match(String word)
        {
            return this.pattern.IsMatch(word);
        }

        /// <summary>
        /// Gets the optional last letter of the word if exists after removing the suffix.
        /// </summary>
        /// <param name="word">the word to get the optional letter for</param>
        /// <returns>the optional letter if exists</returns>
        public Char OptionalLetter(String word)
        {
            if (this.optionalLetterCheck)
            {
                Match match = this.optionalLetterPattern.Match(word);
                if (match.Success)
                {
                    return match.Groups[0].Value[0];
                }
            }
            return '\0';
        }

        /// <summary>
        /// Removes the suffix from a given word.
        /// </summary>
        /// <param name="word">the word to remove the suffix from</param>
        /// <returns>suffix stripped word</returns>
        public String RemoveSuffix(String word)
        {
            return this.pattern.Replace(word, "");
        }

        /// <summary>
        /// Checks if the suffix requires the word to be checked for vowel harmony
        /// </summary>
        public Boolean CheckHarmony
        {
            get
            {
                return this.checkHarmony;
            }
        }
        
        /// <summary>
        /// Gets the suffix type code
        /// </summary>
        public Int32 SuffixType
        {
            get
            {
                return this.suffixType;
            }
        }

        public static T[] Get<T>(params int[] enumValues) where T: Suffix
        {
            Type type = typeof(T);
            List<T> suffixes = new List<T>();
            if (enumValues.Length == 0)
            {
                return suffixes.ToArray();
            }

            Suffix[] VALUES;
            if(type == typeof(NounSuffix))
            {
                VALUES = NounSuffix.VALUES;
            }
            else if (type == typeof(NominalVerbSuffix))
            {
                VALUES = NominalVerbSuffix.VALUES;
            }
            else
            {
                VALUES = DerivationalSuffix.VALUES;
            }

            foreach (int @enum in enumValues)
            {
                suffixes.Add((T)VALUES[@enum]);
            }
            return suffixes.ToArray();
        }
    }
}
