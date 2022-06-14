using System;
using System.Collections.Generic;
using System.Linq;

namespace Scrabble
{
    class Program
    {
        static void Main(string[] args)
        {
            //Fetch data
            string[] Shakespeare = GetDataFromFiles(@"C:\Users\Robert\source\repos\Scrabble\words.shakespeare.txt");
            string[] Scrabble = GetDataFromFiles(@"C:\Users\Robert\source\repos\Scrabble\ospd.txt");

            List<string> shakespeareList = GetFilteredShakespeareList(Shakespeare, Scrabble);

            //Scores of every tile (a-z)
            int[] scores = { 1, 3, 3, 2, 1, 4, 2, 4, 1, 8, 5, 1, 3, 1, 1, 3, 10, 1, 1, 1, 1, 4, 4, 8, 4, 10 };

            //Distribution of every tile (a-z)
            int[] distributions = { 9, 2, 2, 1, 12, 2, 3, 2, 9, 1, 1, 4, 2, 6, 8, 2, 1, 6, 4, 6, 4, 2, 2, 1, 2, 1 };

            List<LetterNumber> letterScores = GetLetterAndNumberList(scores);
            List<LetterNumber> letterDistribution = GetLetterAndNumberList(distributions);

            List<string> validShakespeareList = RemoveImpossibleShakespearWords(letterDistribution, shakespeareList);

            List <ShakespeareWordScore> shakespeareWordScoresByOrder = BestScore(validShakespeareList, letterScores);

            PrintScore(shakespeareWordScoresByOrder, 3);
        }
        /// <summary>
        /// Fetch data from file and returns a string array
        /// </summary>
        /// <param name="filepath">The path of the file</param>
        /// <returns></returns>
        static string[] GetDataFromFiles(string filepath)
        {
            string[] stringArray = System.IO.File.ReadAllLines(filepath);
            return stringArray;
        }

        /// <summary>
        /// Filters out all words that are not in the Scrabble word list
        /// </summary>
        /// <param name="Shakespeare">Array of words from Shakespeare file</param>
        /// <param name="Scrabble">Array of valid words in Scrabble</param>
        /// <returns></returns>
        static List<string> GetFilteredShakespeareList(string[] Shakespeare, string[] Scrabble)
        {
            //The index of the current string
            int ShakespeareNumber = -1;

            List<string> shakespeareList = Shakespeare.ToList();

            //iterates through every word in the Shakespeare list, checks if it exist in Scrabble list and removes the word if it doesn't.
            foreach (string ShakespeareString in Shakespeare)
            {
                bool ShakespeareStringExist = false;
                ShakespeareNumber++;

                foreach (string ScrabbleString in Scrabble)
                {                    
                    if (ScrabbleString == ShakespeareString)
                    {
                        ShakespeareStringExist = true;
                        break;
                    }
                }
                if (ShakespeareStringExist == false)
                {
                    shakespeareList.RemoveAt(ShakespeareNumber);
                    ShakespeareNumber--;
                }                
            }
            return shakespeareList;
        }

        /// <summary>
        /// Matches the letter with its corresponding number
        /// </summary>
        /// <param name="numbers">int array of numbers</param>
        /// <returns></returns>
        static List<LetterNumber> GetLetterAndNumberList(int[] numbers)
        {
            char[] alphabet = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
            List<LetterNumber> letterNumbers = new List<LetterNumber>();

            for (int i = 0; i < 26; i++)
            {
                LetterNumber letterNumber = new LetterNumber(alphabet[i], numbers[i]);
                letterNumbers.Add(letterNumber);
            }
            return letterNumbers;
        }

        /// <summary>
        /// Removes every word that has more of a letter than there are tiles of that letter in the game
        /// </summary>
        /// <param name="letterDistribution">List of objects with the property Letter and Number(How many copies of the tile that exist)</param>
        /// <param name="shakespeareList">List of the words from the Shakespeare after filtration of invalid words</param>
        /// <returns></returns>
        static List<string> RemoveImpossibleShakespearWords(List<LetterNumber> letterDistribution, List<string> shakespeareList)
        {
            //The index of the current string
            int ShakespeareNumber = -1;

            //A copy in order to edit the list while iterating
            List<string> shakespeareListCopy = new List<string>(shakespeareList);

            //Iterates through every word in the Shakespeare list
            foreach (string ShakespeareListString in shakespeareList)
            {
                bool ShakespeareWordNotValid = false;
                ShakespeareNumber++;

                //Clones a temporary list to use for checking if the word has more letters than there are tiles in the game
                //After a word is checked the list gets cloned again.
                List<LetterNumber> letterDistributionCopy = letterDistribution.Select(x => x.Clone()).ToList();

                char[] ShakespeareWord = ShakespeareListString.ToCharArray();

                //Iterates through every letter in a word
                foreach (char c in ShakespeareWord)
                {
                    foreach (LetterNumber letterNumber in letterDistributionCopy)
                    {
                        if (c == letterNumber.Letter)
                        {
                            if (letterNumber.Number <= 0)
                            {
                                //If the word is not valid and needs to be removed from the list
                                ShakespeareWordNotValid = true;
                            }
                            else
                            {
                                //-1 on the property of the cloned list
                                letterNumber.Number -= 1;
                            }
                            if (ShakespeareWordNotValid == true)
                            {
                                break;
                            }
                        }
                        if (ShakespeareWordNotValid == true)
                        {
                            break;
                        }
                    }
                    if (ShakespeareWordNotValid == true)
                    {
                        break;
                    }
                }
                if (ShakespeareWordNotValid == true)
                {
                    shakespeareListCopy.RemoveAt(ShakespeareNumber);
                    ShakespeareNumber--;
                }
            }
            return shakespeareListCopy;
        }

        /// <summary>
        /// Iterates through the letters of every valid word and adds the correct points for every letter in the word
        /// Adds every word and its corresponding score to a list
        /// Sorts the list by the highest score
        /// </summary>
        /// <param name="validShakespeareList">List of the words from the Shakespeare after filtration of invalid words and words with too many letters</param>
        /// <param name="letterScores">List of objects with the property Letter and Number(How many points the letter gives)</param>
        /// <returns></returns>
        static List<ShakespeareWordScore> BestScore(List<string> validShakespeareList, List<LetterNumber> letterScores)
        {
            //List of the words with their corresponding score
            List<ShakespeareWordScore> shakespeareWordScores = new List<ShakespeareWordScore>();

            //Iterates through every word in the Shakespeare list
            foreach (string validShakespeareListString in validShakespeareList)
            {
                ShakespeareWordScore shakespeareWordScore = new ShakespeareWordScore();
                shakespeareWordScore.Word = validShakespeareListString;

                char[] ShakespeareWord = validShakespeareListString.ToCharArray();

                //Iterates through every letter in a word and adds the score of the letter to the words score count
                foreach (char c in ShakespeareWord)
                {
                    foreach (LetterNumber letterNumber in letterScores)
                    {
                        if (c == letterNumber.Letter)
                        {
                            shakespeareWordScore.Score += letterNumber.Number;
                            break;
                        }
                    }
                }
                //Adds the word and its score to the list
                shakespeareWordScores.Add(shakespeareWordScore);
            }

            //Orders the list by highest score
            List<ShakespeareWordScore> shakespeareWordScoresByOrder = shakespeareWordScores.OrderByDescending(o => o.Score).ToList();
            return shakespeareWordScoresByOrder;
        }

        /// <summary>
        /// Prints out the score
        /// </summary>
        /// <param name="shakespeareWordScoresByOrder">List of every valid word in order by most points</param>
        /// <param name="i">The number of items the user wishes to see</param>
        static void PrintScore(List<ShakespeareWordScore> shakespeareWordScoresByOrder, int i)
        {
            foreach (ShakespeareWordScore shakespeareWordScore in shakespeareWordScoresByOrder)
            {
                if (i <= 0)
                {
                    break;
                }
                Console.WriteLine("Word:" + shakespeareWordScore.Word + "       Score:" + shakespeareWordScore.Score);
                i--;                
            }               
        }
    }
}
