using System.Text;

namespace ImpossibleHangman_Blazor.Data
{
    public class ImpossibleHangmanGame
    {
        public ImpossibleHangmanGame()
        {
        }

        public static Dictionary<String, HashSet<String>> Partition(char guess, HashSet<String> currWordSet)
        {
            Dictionary<String, HashSet<String>> subsetDictionary = new();
            foreach (string word in currWordSet)
            {
                String subsetKey = GetSubsetKey(word, guess);
                if (subsetDictionary.Keys.Contains(subsetKey))
                {
                    HashSet<String> words = subsetDictionary[subsetKey];
                    words.Add(word);
                    subsetDictionary[subsetKey] = words;
                }
                else
                {
                    HashSet<String> subset = new();
                    subset.Add(word);
                    subsetDictionary[subsetKey] = subset;
                }
            }
            return subsetDictionary;
        }

        /// <summary>
        /// Get subset key for the already guessed letter. Anything in the same wordset returns the same key.
        /// </summary>
        /// <param name="word">Word from which the subset key is computed</param>
        /// <param name="guess">The character on which the key is based.</param>
        /// <returns>Subset key for the guessed letter</returns>
        private static string GetSubsetKey(string word, char guess)
        {
            StringBuilder subsetKey = new StringBuilder();
            foreach (char letter in word)
            {
                if (letter == guess)
                {
                    subsetKey.Append(guess);
                }
                else
                {
                    subsetKey.Append('_');
                }
            }
            return subsetKey.ToString();
        }

        /// <summary>
        /// Gets the largest subset in the partition
        /// </summary>
        /// <param name="partition">Partition of multiple potential subsets</param>
        /// <returns>Largest subset of the partition</returns>
        public static Dictionary<String, HashSet<String>> LargestSubset(Dictionary<String, HashSet<String>> partition)
        {
            // Get highest word count of the subsetKeys
            int highestCount = 0;
            foreach (HashSet<string> subset in partition.Values)
            {
                int count = subset.Count();
                if ((count > highestCount))
                {
                    highestCount = count;
                }
            }

            // Fill a new Dictionary with only those subsets that have the most entries
            Dictionary<String, HashSet<String>> largestSubsets = new();
            foreach (KeyValuePair<string, HashSet<string>> pair in partition)
            {
                if (pair.Value.Count == highestCount)
                {
                    largestSubsets[pair.Key] = pair.Value;
                }
            }
            return largestSubsets;
        }

        public static HashSet<String> BestSubset(int wordLength, Dictionary<String, HashSet<String>> subsetDictionary)
        {
            // 1. Letter doesn't appear at all? Return that.
            StringBuilder blankKey = new StringBuilder();
            for (int i = 0; (i < wordLength); i++)
            {
                blankKey.Append('_');
            }
            if (subsetDictionary.Keys.Contains(blankKey.ToString()))
            {
                return subsetDictionary[blankKey.ToString()];
            }

            // 2. Fewest letters? If only one (no tie), return that.
            Dictionary<String, HashSet<String>> fewestLetterSubsets = FewestLetterSubsets(subsetDictionary);
            if (fewestLetterSubsets.Count == 1)
            {
                return fewestLetterSubsets.First().Value;
            }

            // 3. Rightmost guessed letter (which is recursive and can only by definition return size 1). Return that.
            return RightmostLetterSubset(wordLength, fewestLetterSubsets);
        }

        /// <summary>
        /// Gets all subsets with the lowest numbrer of guessed letters
        /// </summary>
        /// <param name="partition"></param>
        /// <returns>All the subsets with the lowest number of guessed letters</returns>
        private static Dictionary<String, HashSet<String>> FewestLetterSubsets(Dictionary<String, HashSet<String>> partition)
        {
            // Get fewest letter count of the subsetKeys
            int lowestCount = int.MaxValue;
            foreach (string key in partition.Keys)
            {
                int count = NumLetters(key);
                if (count < lowestCount)
                {
                    lowestCount = count;
                }        
            }

            // Fill a new Map with only those subsets that have the fewest letters
            Dictionary<String, HashSet<String>> fewestLetterSubsets = new();
            foreach (KeyValuePair<string, HashSet<string>> pair in partition)
            {
                if (NumLetters(pair.Key) == lowestCount)
                {
                    fewestLetterSubsets[pair.Key] = pair.Value;
                }
            }

            return fewestLetterSubsets;
        }

        /// <summary>
        /// Counts number of letters, not chars, in the given string
        /// </summary>
        /// <returns>Number of alphabetical letters in the string</returns>
        private static int NumLetters(string word)
        {
            int count = 0;
            foreach (char letter in word)
            {
                if (char.IsLetter(letter))
                {
                    count++;
                }
            }
            return count;
        }
        
        /// <summary>
        /// Breaks tie by taking the subset with the rightmost guessed letters
        /// </summary>
        /// <returns>Subset with the rightmost guessed letters</returns>
        private static HashSet<String> RightmostLetterSubset(int wordLength, Dictionary<String, HashSet<String>> partition)
        {
            return RightmostLetterSubset_Helper(partition, (wordLength - 1));
        }

        private static HashSet<String> RightmostLetterSubset_Helper(Dictionary<String, HashSet<String>> partition, int index)
        {
            Dictionary<String, HashSet<String>> rightmostSubsets = new();
            foreach (KeyValuePair<string, HashSet<string>> pair in partition)
            {
                string subsetKey = pair.Key;
                if (char.IsLetter(subsetKey[index]))
                {
                    rightmostSubsets[pair.Key] = pair.Value;
                }
            }
            if ((rightmostSubsets.Count() == 1))
            {
                return rightmostSubsets.First().Value;
            }
            else if ((rightmostSubsets.Count() > 1))
            {
                return RightmostLetterSubset_Helper(rightmostSubsets, (index - 1));
            }
            else
            {
                return RightmostLetterSubset_Helper(partition, (index - 1));
            }
        }

        /// <summary>
        /// Gets the ambiguous, underscored string given the previously identified potential word and current guess
        /// </summary>
        /// <returns>String with underscores and only the letters guessed</returns>
        public static string GetNewCurrWord(string currWord, string potentialWord, char guess)
        {
            StringBuilder UpdatedWord = new StringBuilder(currWord);
            String subsetKey = DashedSubsetKey(potentialWord, guess);

            for (int i = 0; i < currWord.Length; ++i)
            {
                if (char.IsLetter(subsetKey[i]))
                {
                    UpdatedWord[i] = subsetKey[i];
                }
            }

            return UpdatedWord.ToString();
        }

        private static string DashedSubsetKey(string word, char guess)
        {
            StringBuilder subsetKey = new StringBuilder();
            foreach (char letter in word)
            {
                if (letter == guess)
                {
                    subsetKey.Append(guess);
                }
                else
                {
                    subsetKey.Append('-');
                }
            }
            return subsetKey.ToString();
        }
    }
}
