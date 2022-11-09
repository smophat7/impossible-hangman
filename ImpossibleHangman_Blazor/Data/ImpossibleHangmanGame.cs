using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;

namespace ImpossibleHangman_Blazor.Data
{
    public class ImpossibleHangmanGame
    {

        //private SortedSet<char> GuessedLetters = new();

        //private HashSet<String>  CurrWordSet = new();

        //private int WordLength = 0;

        //public ImpossibleHangmanGame()
        //{

        //}

        //public void StartGame(int wordLength)
        //{
        //    CurrWordSet = new();
        //    HashSet<String> inputDictionary = new(File.ReadLines("dictionary.txt").Where(x => x.Length == WordLength));
        //    WordLength = wordLength;

        //    if (CurrWordSet.Count == 0)
        //    {
        //        throw new Exception("There were no words of the proper length (or no words at all) in the dictionary file");
        //    }

        //}

        //public HashSet<String> MakeGuess(char guess)
        //{
        //    StringBuilder sbGuess = new StringBuilder();
        //    sbGuess.Append(guess);
        //    String stringGuess = sbGuess.ToString().ToLower();
        //    guess = stringGuess[0];
        //    if (GuessedLetters.Contains(guess))
        //    {
        //        throw new Exception("You\'ve already guessed the letter " + guess);
        //    }
        //    else
        //    {
        //        Dictionary<String, HashSet<String>> subsetDictionary = Partition(guess);

        //        // Determine the new currWordSet based on the partition results
        //        Dictionary<String, HashSet<String>> largestSubsets = LargestSubset(subsetDictionary);
        //        if ((largestSubsets.Count == 1))
        //        {
        //            CurrWordSet = largestSubsets.First().Value;
        //        }
        //        else
        //        {
        //            CurrWordSet = BestSubset(largestSubsets);
        //        }

        //        GuessedLetters.Add(guess);
        //        return CurrWordSet;
        //    }

        //}

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
