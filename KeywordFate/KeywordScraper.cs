using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Web;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;

namespace KeywordFate
{
    public partial class KeywordScraper : Form
    {
        Form1 firstformRef;
        public KeywordScraper(ref Form1 form1handle)
        {
            firstformRef = form1handle;
            InitializeComponent();
        }

        private void btnScrapeLSIs_Click(object sender, EventArgs e)
        {
            List<KeyValuePair<string, int>> lsis = scrapeLSIs(txtMainKeyword.Text);

            //display lsis in a box with checkboxes to specify if we should check competition
            dataGridView1.DataSource = lsis;
            dataGridView1.Columns[1].HeaderText = "Phrase";
            dataGridView1.Columns[2].HeaderText = "Count";
            dataGridView1.Columns[0].Visible = true;
        }

        public List<KeyValuePair<string, int>> scrapeLSIs(string mainkeyword)
        {
            Dictionary<string, int> phraseCounts = new Dictionary<string, int>();
            List<KeyValuePair<string, int>> sortedPhrases = new List<KeyValuePair<string, int>>();

            try
            {
                Form1 f1 = new Form1();
                string resultspage = f1.getSERP(mainkeyword);
                List<string> top10 = f1.getTop10(resultspage);

                //get source for each page, strip all tags, then sort and tally word counts
                foreach (string site in top10)
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(site);

                    request.Accept = "application/javascript, */*;q=0.8";
                    request.Headers.Set(HttpRequestHeader.AcceptLanguage, "en-US");
                    request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";
                    request.Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip, deflate");

                    //this is used to decompress the results page into plain text
                    request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                    //Let's get the returned results, including the page itself
                    HttpWebResponse getresponse = (HttpWebResponse)request.GetResponse();

                    //Used to read the page
                    StreamReader postreqreader = new StreamReader(getresponse.GetResponseStream());

                    //string to hold the page itself
                    string resultsPage = postreqreader.ReadToEnd();

                    //strip all javascript code
                    string strippedsite = Regex.Replace(resultspage, "<script.*?</script>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);

                    //strip html tags
                    var reg = new Regex("<[^>]+>", RegexOptions.IgnoreCase);
                    strippedsite =  reg.Replace(resultspage, "");

                    //strip punctuation with LINQ :)
                    strippedsite = new string(strippedsite.Where(c => !char.IsPunctuation(c)).ToArray());

                    //convert to lower case
                    strippedsite = strippedsite.ToLower();

                    string[] words = strippedsite.Split(' ');

                    //split site into 3, 4 and 5 word strings
                    List<string> phrases = new List<string>();
                    for (int i = 0; i < words.Length; ++i)
                    {
                        if (i+2 < words.Length) //3 words
                            phrases.Add( words[i] + ' ' + words [i+1] + ' ' + words[i+2] );

                        if (i + 3 < words.Length) //4 words
                            phrases.Add(words[i] + ' ' + words[i + 1] + ' ' + words[i + 2] + ' ' + words[i + 3] );

                        if (i + 4 < words.Length) //5 words
                            phrases.Add(words[i] + ' ' + words[i + 1] + ' ' + words[i + 2] + ' ' + words[i + 3] + ' ' + words[i + 4]);
                    }
                    
                    //store phrase/count in a dictionary (map)
                    foreach (string phrase in phrases)
                    {
                        if (!phraseCounts.ContainsKey(phrase))
                            phraseCounts.Add(phrase, 1);
                        //don't store phrases containing numbers                    or phrases which are just whitespace
                        else if ((phrase.All(c => Char.IsLetter(c) || c == ' ')) && (!phrase.All(c => c == ' ')))
                            phraseCounts[phrase]++;
                    }
                }
                //sort the list of phrases by count
                sortedPhrases = phraseCounts.ToList();
                sortedPhrases.Sort((firstPair, nextPair) =>
                {
                    return nextPair.Value.CompareTo(firstPair.Value);
                });

            }
            catch (Exception ex)
            {

            }
            return sortedPhrases;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            List<string> returnMe = new List<string>();
            for (int i = dataGridView1.Rows.Count - 1; i >= 0; i--)
            {
                if ((bool)dataGridView1.Rows[i].Cells[0].FormattedValue)
                {
                    returnMe.Add(dataGridView1.Rows[i].Cells[1].FormattedValue.ToString());
                }
            }
            Control[] controls = firstformRef.Controls.Find("txtKeywords", true);
            foreach (string phrase in returnMe)
                firstformRef.txtKeywords.AppendText(phrase + '\n');
        }
    }
}
