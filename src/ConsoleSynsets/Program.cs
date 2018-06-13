
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
        private static WordNetEngine _wordNetEngine = new WordNetEngine(@"C:\Users\Christopher\Documents\GitHub\WordNet\resources", false);
        static void Main(string[] args)
        {
            List<String> relaciones = new List<String>();

            if (File.Exists("words.txt"))
            {
                // Usernames are listed first in users.ul, and are followed by a period and then the password associated with that username.
                StreamReader reader = new StreamReader("words.txt");
                string line;

                Set<SynSet> synSetsToShow = null;
                List<String> rels = new List<string>();

                using (var file = File.CreateText("relaciones.csv"))
                {
                    while ((line = reader.ReadLine()) != null)
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
                                    //related SynSet
                                    SynSet s2 = s1.GetRelatedSynSets(synSetRelation, false).First();
                                    //fill the line
                                    String csv_line = s1.Words[0] + "," + s2.Words[0] + "," + synSetRelation;

                                    file.WriteLine(csv_line);
                                    //out of the foreach loop
                                    break;
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    }
                    reader.Close();
                }
            }
        }
    }
}
