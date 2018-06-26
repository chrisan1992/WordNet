
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LAIR.ResourceAPIs.WordNet;
using LAIR.Collections.Generic;
using static LAIR.ResourceAPIs.WordNet.WordNetEngine;

namespace ConsoleSynsets
{
    class Program
    {
        private static String currentPath = Directory.GetCurrentDirectory();
        private static WordNetEngine _wordNetEngine = new WordNetEngine(Path.Combine(currentPath, @"..\..\..\..\resources"), false);
        static Random rnd = new Random();

        static void Main(string[] args)
        {
            List<String> relaciones = new List<String>();

            if (File.Exists("words.txt"))
            {
                var file = File.CreateText("relaciones.csv");
                List<String> allLinesText = File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), "words.txt")).ToList();
                Set<SynSet> synSetsToShow = null;              

                foreach (String line in allLinesText)
                {
                    try
                    {
                        synSetsToShow = _wordNetEngine.GetSynSets(line, LAIR.ResourceAPIs.WordNet.WordNetEngine.POS.Noun);
                        //main SynSet
                        SynSet s1 = synSetsToShow.Last();

                        foreach (WordNetEngine.SynSetRelation synSetRelation in s1.SemanticRelations)
                        {
                            if (synSetRelation.ToString().Equals("Hypernym") || synSetRelation.ToString().Equals("Hyponym"))
                            {
                                String oppositeSynSetRelation = synSetRelation.ToString() == "Hypernym" ? "Hyponym" : "Hypernym";
                                List<String> synSetsAdded = new List<String>();

                                SynSet s2 = s1.GetRelatedSynSets(synSetRelation, false).First();

                                if (!s1.Words[0].Contains("_") && !s2.Words[0].Contains("_"))
                                {
                                    String csv_line = s1.Words[0] + "," + s2.Words[0] + "," + synSetRelation;
                                    String csv_line2 = s2.Words[0] + "," + s1.Words[0] + "," + oppositeSynSetRelation;

                                    if (!synSetsAdded.Contains(csv_line) && !synSetsAdded.Contains(csv_line2))
                                    {
                                        synSetsAdded.Add(csv_line);
                                        synSetsAdded.Add(csv_line2);
                                        file.WriteLine(csv_line);
                                        file.WriteLine(csv_line2);
                                    }

                                    int randomPosition = rnd.Next(allLinesText.Count());
                                    String randomWord = allLinesText[randomPosition];                                    

                                    while (randomWord != "")
                                    {
                                        if (!s1.Words.Contains(randomWord) && !s2.Words.Contains(randomWord))
                                        {
                                            String csv_line3 = s1.Words[0] + "," + randomWord + ",None";
                                            file.WriteLine(csv_line3);

                                            randomWord = "";
                                        }                   
                                        else
                                        {
                                            randomPosition = rnd.Next(allLinesText.Count());
                                            randomWord = allLinesText[randomPosition];
                                        }
                                    }
                                }                                
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }

                file.Close();
            }
        }
    }
}
