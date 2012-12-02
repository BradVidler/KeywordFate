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
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Threading.Tasks;
using System.Globalization;
using System.Threading;

using HtmlAgilityPack; //awesome html parser

//TO DO:
/*
    Backlink count for each site in top 10
    Fix tag scraping (HTMLAgilityPack?)
    Strip whitespace on words when scraping LSIs
    Replace periods with spaces instead of just removing them when scraping LSIs
 * 
    Proxy support
    Multi-thread
*/

namespace KeywordFate
{
    public partial class Form1 : Form
    {
        private static ManualResetEvent[] resetEvents; //used to keep track of threads

        public Form1()
        {
            InitializeComponent();
        }

        // Wrapper method for use with thread pool.
        public void ThreadPoolCallback(Object threadContext)
        {
            int threadIndex = (int)threadContext;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            float competition = -1;

            
            try
            {
                //DEBUGGING/TESTING
                //fetchDA("http://www.google.ca");
                //fetchSEOC(resultsPage);
                //List<topSite> debugSites = new List<topSite>();
                //topSite spd = new topSite();
                //spd.setURL("http://www.splitpersonalitydisorder.net/split-away-from-split-personality-disorder/");
                //debugSites.Add(spd);
                //debugSites = analyzeKeywordCount(debugSites, "split personality disorder");

                //=================================

                /*
                //fetch results page for desired keyword ONE time to prevent bans
                //Note: SEOTC is a completely separate search (intitle: keyword)
                string resultsPage = getSERP(txtMainKeyword.Text);

                //get top 10 urls
                List<string> top10 = getTop10(resultsPage);

                //this will store the urls plus the extra information we need
                List<topSite> top10Details = new List<topSite>();

                foreach (string site in top10)
                {
                    topSite newSite = new topSite();
                    newSite.setPR(fetchPR(site));
                    newSite.setAge(fetchDA(site));
                    top10Details.Add(newSite);
                }

                keyword keyword = new keyword();
                keyword.setName(txtMainKeyword.Text);
                keyword.setSeoc(fetchSEOC(keyword.getName()));
                keyword.setSeotc(fetchSEOTC(keyword.getName()));

                competition = analyzeComp(top10Details, keyword);
                 */
                List<allResults> gatheredData = new List<allResults>();
                List<string> keywords = new List<string>();
                keywords.AddRange(txtKeywords.Lines);

                //gather all necessary data for judging competition - THREADING THIS
                //for (int i = 0; i < keywords.Count; ++i) //threaded
                foreach (string inputkeyword in keywords)
                {
                    gatheredData.Add( gatherAllData(inputkeyword) ); //non threaded

                    //threaded
                    //resetEvents[i] = new ManualResetEvent(false);
                    //ThreadPool.QueueUserWorkItem(new WaitCallback(gatherAllData), keywords[i]);
                }

                //check competition for each keyword
                foreach(allResults data in gatheredData)
                    data.setCompetition( analyzeComp(data.getTop10(), data.getKeyword()) );

                //display results
                //Recall:
                //The ranks will be as follows:
                //1    - Incredibly easy - Probably instant rankings
                //2    - Easy            - Little work necessary to rank
                //3-5  - Medium          - You should have some SEO knowledge to rank for these keywords
                //6-8  - Hard            - Probably a long time endeavour to rank for this keyword. Major SEO/time needed
                //9-10 - Impossible      - Probably not worth your time
                foreach (allResults data in gatheredData)
                {
                    string result = "";
                    string details = "";

                    Color colour = new Color();

                    if (data.getCompetition() >= 9)
                    {
                        result = "Impossible";
                        details = "Probably not worth your time!";
                        colour = Color.Red;
                    }
                    else if (data.getCompetition() >= 6)
                    {
                        result = "Hard";
                        details = "Probably a long endeavour to rank for this keyword. Major SEO/time needed!";
                        colour = Color.Red;
                    }
                    else if (data.getCompetition() >= 3)
                    {
                        result = "Medium";
                        details = "You should have some SEO knowledge to rank for these keywords!";
                        colour = Color.Orange;
                    }
                    else if (data.getCompetition() >= 2)
                    {
                        result = "Easy";
                        details = "Little work necessary to rank!";
                        colour = Color.Yellow;
                    }
                    else if (data.getCompetition() < 0)   //there was a problem
                    {
                        result = "ERROR";
                        details = "There was a problem fetching the competition!";
                        colour = Color.Black;
                    }
                    else
                    {
                        result = "Incredibly Easy";
                        details = "Probably instant rankings!";
                        colour = Color.Green;
                    }

                    //lblResult.Text = result;
                    //lblResult.ForeColor = colour;

                    //lblDetails.Text = details;

                    //add a row to the results
                    DataGridViewRow row = (DataGridViewRow)resultsView.Rows[0].Clone();
                    //DataGridViewCellStyle style = new DataGridViewCellStyle();
                    //style.ForeColor = colour;
                    row.DefaultCellStyle.BackColor = colour;
                    if (colour == Color.Black)
                        row.DefaultCellStyle.ForeColor = Color.White;
                    row.Cells[0].Value = data.getKeyword().getName();
                    row.Cells[1].Value = data.getCompetition().ToString();
                    resultsView.Rows.Add(row);
                }
                
            }
            catch (Exception ex)
            {
                //competition = -1;
            }
               
            
                 
        }

        //this method with fetch the search engine results page for the desired keyword
        //we will then send the results to the other methods to prevent multiple
        //calls to Google as this will result in a temp or perm ban from the server
        //NOTE: SEOTC requires it's own call/search (intitle: keyword)
        public string getSERP(string keyword) //sprint 2
        {
            try
            {
                //These are the headers that Fiddler picked up while querying Big G (Google).
                //We will mirror what our browser sent to appear as if we are just another G user.

                //GET http://www.google.ca/search?q=fiddler+google+traffic HTTP/1.1
                //Host: www.google.ca
                //User-Agent: Mozilla/5.0 (Windows NT 6.1; WOW64; rv:15.0) Gecko/20100101 Firefox/15.0.1
                //Accept: text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8
                //Accept-Language: en-us,en;q=0.5
                //Accept-Encoding: gzip, deflate
                //Connection: keep-alive
                //Referer: http://www.google.ca/
                //Cookie: PREF=ID=8f0f38439c2b3442:U=33736156ecc5ecfb:FF=0:NW=1:TM=1349062851:LM=1349063637:SG=2:S=9bDcT3SyzmL2jX6s; NID=64=SRNpsYuTnuG7qYqc7uim2K-YguKjTP_p2Yr20Zv48kZJgr0bopV-AZDhrW-CwIE-AoPStrkBi0nLlG5tO1y9rYyaTq39LvJlR5o97toIE9rd13lxh0v-1JEbCaJ11hJx

                //Here's where we keep all of our cookies
                CookieContainer tempCookies = new CookieContainer();

                //replace spaces for + in keyphrase
                keyword.Replace(' ', '+');

                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://www.google.ca/search?q=" + keyword);

                request.Method = "GET";
                request.Host = "www.google.ca";
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:15.0) Gecko/20100101 Firefox/15.0.1";
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                request.Headers.Add("Accept-Language: en-us,en;q=0.5");
                request.Headers.Add("Accept-Encoding: gzip, deflate");
                request.KeepAlive = true;
                request.Referer = "http://www.google.ca/";
                request.CookieContainer = tempCookies;

                //this is used to decompress the results page into plain text
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                //Let's get the returned results, including the page itself
                HttpWebResponse getresponse = (HttpWebResponse)request.GetResponse();

                //add the returned cookies. This isn't really needed for this type of 
                //software since we are not traversing multiple pages, 
                //but let's keep them anyway
                tempCookies.Add(getresponse.Cookies);

                //Used to read the page
                StreamReader postreqreader = new StreamReader(getresponse.GetResponseStream());

                //string to hold the page itself
                string resultsPage = postreqreader.ReadToEnd();

                //lets get rid of all of the escaped quotes to make things easier for our regex search
                string cleanPage = (Regex.Replace(resultsPage, @"^""|""$|\\n?", ""));

                //Search string - used to locate the top 10 websites for the given keyword
                //<h3 class="r"><a href="

                return cleanPage;
            }
            catch (Exception ex)
            {

            }

            return "";
        }

        public List<String> getTop10(string thePage)
        {
            //List to store the returned sites. We'll use regex to find and store the matches
            List<string> top10 = new List<string>();

            try
            {
                // Here we call Regex.Match.
                //Actual regex match we're looking for = (?<=<h3 class="r"><a href=")([^""]*)
                //Basically we match anything between <h3 class="r"><a href= and a quote
                //The best part is, it only took 2 hours to get this regex working! YES!
                string regex = "(?<=<h3 class=\"r\"><a href=\")" + @"([^""]*)";
                Match match = Regex.Match(thePage, regex, RegexOptions.IgnoreCase);
                MatchCollection matches = Regex.Matches(thePage, regex);

                // Here we check the Match instance.
                if (match.Success)
                {
                    foreach (Match topSite in matches)
                        top10.Add(topSite.ToString());
                }
            }
            catch (Exception ex)
            {
            }

            return top10;
        }//end getTop10


        //***analyzeComp() details***
        //===========================

        //Here we will determine an estimation of how difficult it will be to rank for a certain keyword.

        //For now we will keep it simple and only analyze a few metrics:
        //Domain Age         - How long the domain been registered for
        //Page Rank (PR)     - Google's page rank for the top 10 site
        //SEO Comp (SEOC)    - The total number of webpages GLOBALLY that mention the same keyword term
        //                     in the same (phrase) word order, in Bing's index. [AS DESCRIBED IN MARKET SAMURAI]
        //Title Comp (SEOTC) - The total number of webpages GLOBALLY that mention all of the words in a keyword
        //                     term in the title of a page. [AS DESCRIBED IN MARKET SAMURAI]

        //Possible other metrics to determine rank to be implemented:
        //Search for sites such as blogger, yahoo answers or similar sites that are easy to beat
        //Search for certain top level domains (TLDs) which are difficult to beat such as .gov and .edu

        //With any extra time, we will add more metrics to take a look at, but for now, this should be enough to get
        //a general overview of how difficult it might be to rank for the desired keyword.

        //We will be using a simple scale between 1 and 10 to determine difficulty. Higher domain age/competition will add points,
        //meaning a higher difficulty.

        //We will get a score between 1 to 10 for each metric and then get an average between them.

        //The ranks will be as follows:
        //1    - Incredibly easy - Probably instant rankings
        //2    - Easy            - Little work necessary to rank
        //3-5  - Medium          - You should have some SEO knowledge to rank for these keywords
        //6-8  - Hard            - Probably a long time endeavour to rank for this keyword. Major SEO/time needed
        //9-10 - Impossible      - Probably not worth your time

        //===================================================================================
        //          ***Update notes as of Monday, Oct 29th 2012(Sprint 2):***
        //===================================================================================
        //A new ranking system will be needed as the current one does not work as intended.
        //In this new system, each metric will have an associated "weight".
        //This is because some metrics are more important than others.
        public float analyzeComp(List<topSite> top10, keyword keyword)
        {
            keywordMetrics newKeywordMetrics = new keywordMetrics();
            float compScore = -1;

            try
            {
                //get average pr/domain age from top10
                int avgPR = 0;
                int avgDA = 0;

                foreach (topSite site in top10)
                {
                    if (site.getPR() > 0) //dont add negative prs cause by errors
                        avgPR += site.getPR();

                    avgDA += site.getAge();

                    //SPRINT 3====================================
                    //more important metrics are tallied differently
                    if (site.getDmozListed() == true)
                        newKeywordMetrics.setDmozListedScore(newKeywordMetrics.getDmozListedScore() + 1);

                    if (site.getIsEMD() == true)
                        newKeywordMetrics.setIsEMDScoreScore(newKeywordMetrics.getIsEMDScoreScore() + 3); //3 for emd

                    if (site.getKeywordInMeta() == true)
                        newKeywordMetrics.setKeywordInMetaScore(newKeywordMetrics.getKeywordInMetaScore() + 1);

                    if (site.getKeywordInTitle() == true)
                        newKeywordMetrics.setKeywordInTitleScore(newKeywordMetrics.getKeywordInTitleScore() + 3); //3 for title

                    if (site.getKeywordInURL() == true)
                        newKeywordMetrics.setKeywordInURLScore(newKeywordMetrics.getKeywordInURLScore() + 1);
                    //END SPRINT 3================================
                }

                avgPR = avgPR / top10.Count;
                avgDA = avgDA / top10.Count;

                //pr comp
                newKeywordMetrics.setPrScore(avgPR);

                //da comp
                if (avgDA >= 10)
                    newKeywordMetrics.setDaScore(10);
                else
                    newKeywordMetrics.setDaScore(avgDA);

                //seoc comp
                if (keyword.getSeoc() >= 1000000)
                    newKeywordMetrics.setSeocScore(10);
                else if (keyword.getSeoc() >= 500000)
                    newKeywordMetrics.setSeocScore(8);
                else if (keyword.getSeoc() >= 250000)
                    newKeywordMetrics.setSeocScore(6);
                else if (keyword.getSeoc() >= 100000)
                    newKeywordMetrics.setSeocScore(4);
                else if (keyword.getSeoc() >= 60000)
                    newKeywordMetrics.setSeocScore(2);
                else if (keyword.getSeoc() >= 30000)
                    newKeywordMetrics.setSeocScore(1);
                else
                    newKeywordMetrics.setSeocScore(0);

                //seotc comp
                if (keyword.getSeotc() >= 100000)
                    newKeywordMetrics.setSeotcScore(10);
                else if (keyword.getSeotc() >= 50000)
                    newKeywordMetrics.setSeotcScore(8);
                else if (keyword.getSeotc() >= 25000)
                    newKeywordMetrics.setSeotcScore(6);
                else if (keyword.getSeotc() >= 10000)
                    newKeywordMetrics.setSeotcScore(4);
                else if (keyword.getSeotc() >= 6000)
                    newKeywordMetrics.setSeotcScore(2);
                else if (keyword.getSeotc() >= 3000)
                    newKeywordMetrics.setSeotcScore(1);
                else
                    newKeywordMetrics.setSeotcScore(0);

                //max percentage for each metric variables
                
                //SPRINT 3 NOTE
                //Because of the way competition can be judged, the total percentages will exceed 100
                //This is because if a certain metrics FAR outweighs all the rest, the site could be ranking 
                //based on that metric alone.

                float maxPrScore = 5;
                float maxDaScore = 15;
                float maxSeocScore = 10;
                float maxSeotcScore = 20;

                //sprint 3 percentages
                float maxDmozScore = 10;
                float maxEMDScore = 10;
                float maxMetaScore = 5;
                float maxTitleScore = 20;
                float maxURLScore = 5;

                //get competition
                compScore = newKeywordMetrics.getPrScore() * (maxPrScore / 100) +
                                newKeywordMetrics.getDaScore() * (maxDaScore / 100) +
                                newKeywordMetrics.getSeocScore() * (maxSeocScore / 100) +
                                newKeywordMetrics.getSeotcScore() * (maxSeotcScore / 100) +
                                newKeywordMetrics.getDmozListedScore() * (maxDmozScore / 100) +
                                newKeywordMetrics.getIsEMDScoreScore() * (maxEMDScore / 100) +
                                newKeywordMetrics.getKeywordInMetaScore() * (maxMetaScore / 100) +
                                newKeywordMetrics.getKeywordInTitleScore() * (maxTitleScore / 100) +
                                newKeywordMetrics.getKeywordInURLScore() * (maxURLScore / 100);
            }
            catch (Exception ex)
            {
                compScore = -1;
            }

            return compScore;
        }

        //Retrieves the given site's page rank
        //For now, we'll just use this simple API to check PR
        //http://josh-fowler.com/prapi/?url=http://google.com
        public int fetchPR(string site) //sprint 1
        {
            int pr = -1;

            //GET /prapi/?url=google.com HTTP/1.1
            //Host: josh-fowler.com
            //User-Agent: Mozilla/5.0 (Windows NT 6.1; WOW64; rv:15.0) Gecko/20100101 Firefox/15.0.1
            //Accept: text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8
            //Accept-Language: en-us,en;q=0.5
            //Accept-Encoding: gzip, deflate
            //Connection: keep-alive

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://josh-fowler.com/prapi/?url=" + site);

            request.Method = "GET";
            request.Host = "josh-fowler.com";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:15.0) Gecko/20100101 Firefox/15.0.1";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            request.Headers.Add("Accept-Language: en-us,en;q=0.5");
            request.Headers.Add("Accept-Encoding: gzip, deflate");
            request.KeepAlive = true;
            //request.CookieContainer = tempCookies;

            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            //Let's get the returned results, including the page itself
            HttpWebResponse getresponse = (HttpWebResponse)request.GetResponse();

            //Used to read the page
            StreamReader postreqreader = new StreamReader(getresponse.GetResponseStream());

            //string to hold the page itself
            string prPage = postreqreader.ReadToEnd();

            pr = Convert.ToInt32(prPage);

            return pr;
        }

        //Retrieves the given site's domain age
        //this site is simple because it will cleanly return what we need and we can bulk search domains

        //Notes: Pretty useless to store cookies for this so we won't
        //       site used: http://www.bulkseotools.com/bulk-check-domain-age.php
        public int fetchDA(string site) //sprint 2
        {
            //GET http://www.bulkseotools.com/phpwhois/domain-age.php?query=google.ca HTTP/1.1
            //Referer: http://www.bulkseotools.com/bulk-check-domain-age.php
            //Content-Type: application/x-www-form-urlencoded
            //X-Requested-With: XMLHttpRequest
            //Accept: */*
            //Accept-Language: en-us
            //Accept-Encoding: gzip, deflate
            //User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)
            //Host: www.bulkseotools.com
            //Connection: Keep-Alive
             
            int da = 0;

            //trim http(s):// ,www. and anything after slashes (we only want the root domain) 
            if (site.Contains("http://"))
                site = site.Remove(site.IndexOf("http://"), 7);
            else if (site.Contains("https://"))
                site = site.Remove(site.IndexOf("https://"), 8);

            if (site.Contains("www."))
                site = site.Remove(site.IndexOf("www."), 4);

            //trim trailing anything after a '/' including the '/' iteself
            if (site.Contains('/'))
            {
                var urlParts = site.Split('/');
                site = urlParts[0];
            }

            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://www.bulkseotools.com/phpwhois/domain-age.php?query=" + site);

                request.Referer = "http://www.bulkseotools.com/bulk-check-domain-age.php";
                request.ContentType = "application/x-www-form-urlencoded";
                request.Headers.Add("X-Requested-With", @"XMLHttpRequest");
                request.Accept = "*/*";
                request.Headers.Set(HttpRequestHeader.AcceptLanguage, "en-us");
                request.Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
                request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";

                //this is used to decompress the results page into plain text
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                //Let's get the returned results, including the page itself
                HttpWebResponse getresponse = (HttpWebResponse)request.GetResponse();

                //Used to read the page
                StreamReader postreqreader = new StreamReader(getresponse.GetResponseStream());

                //string to hold the page itself
                string resultsPage = postreqreader.ReadToEnd();

                string strRegDate = resultsPage.Substring(site.Length+1, 10); //date is 10 chars long
                strRegDate = strRegDate.Replace("-", ""); //remove dashes from date

                var regDate = DateTime.ParseExact(strRegDate, "yyyyMMdd", CultureInfo.InvariantCulture);

                da = DateTime.Now.Year - regDate.Year;
            }
            catch (Exception ex)
            {
                return 0;
            }

            return da;
        }

        //String to search for - <div id=resultStats>About 21,600,000 results<nobr>
        public double fetchSEOC(string thePage) //sprint 2
        {
            double seoc = 0;

            try
            {
                string regex = @"(?<=<div id=resultStats>About) ([^ results<nobr>]*)";
                Match match = Regex.Match(thePage, regex, RegexOptions.IgnoreCase);
                MatchCollection matches = Regex.Matches(thePage, regex);

                // Here we check the Match instance.
                if (match.Success)
                {
                    foreach (Match seocMatch in matches)
                        seoc = Convert.ToDouble(seocMatch.Value.Replace(",", ""));
                }
            }
            catch (Exception ex)
            {
                return -1;
            }

            return seoc;
        }

        public double fetchSEOTC(string keyword) //sprint 2
        {
            double seotc = 0;

            try
            {
                string thePage = getSERP("intitle:\"" + keyword + '"'); //search intitle
                seotc = fetchSEOC(thePage); //code to fetch seoc is the same, only now we search intitle comp
            }
            catch(Exception ex)
            {
                return -1;
            }

            return seotc;
        }

        public bool checkDMOZ(string url)
        {
            //GET http://www.dmoz.org/search/search?q=splitpersonalitydisorder.net HTTP/1.1
            //Accept: text/html, application/xhtml+xml, */*
            //Accept-Language: en-US
            //User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)
            //Accept-Encoding: gzip, deflate
            //Connection: Keep-Alive
            //Host: www.dmoz.org

            bool dmozListed = false;

            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://www.dmoz.org/search/search?q=" + url);

                request.Accept = "text/html, application/xhtml+xml, */*";
                request.Headers.Add("Accept-Language: en-US");
                request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";
                request.Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
                request.KeepAlive = true;
                request.Host = "www.dmoz.org";

                //this is used to decompress the results page into plain text
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                //Let's get the returned results, including the page itself
                HttpWebResponse getresponse = (HttpWebResponse)request.GetResponse();

                //Used to read the page
                StreamReader postreqreader = new StreamReader(getresponse.GetResponseStream());

                //string to hold the page itself
                string resultsPage = postreqreader.ReadToEnd();

                //Search string: "Open Directory Categories"
                if (resultsPage.Contains("Open Directoy Categories"))
                    dmozListed = true;
                else
                    dmozListed = false;
            }
            catch (Exception ex)
            {
                dmozListed = false;
            }

            return dmozListed;
        }

        public List<topSite> analyzeKeywordCount(List<topSite> top10, string keyword)
        {
            string host = "";
            string keywordurl1 = keyword.Replace(" ", ""); //keyword with no spaces
            string keywordurl2 = keyword.Replace(" ", "-"); //keyword with dashes instead of spaces

            //GET http://choices.truste.com/ HTTP/1.1
            //Accept: application/javascript, */*;q=0.8
            //Referer: http://google.ca
            //Accept-Language: en-US
            //User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)
            //Accept-Encoding: gzip, deflate
            //Host: choices.truste.com

            foreach (topSite site in top10)
            {
                try
                {
                    host = site.getURL();
                    //trim domain for host header
                    if (host.Contains("http://"))
                        host = host.Remove(host.IndexOf("http://"), 7);
                    else if (host.Contains("https://"))
                        host = host.Remove(host.IndexOf("https://"), 8);

                    //trim trailing anything after a '/' including the '/' iteself
                    string pageurl = "";
                    if (host.Contains('/'))
                    {
                        var urlParts = host.Split('/');
                        host = urlParts[0];
                        pageurl = urlParts[1];
                    }

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(site.getURL());

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

                    //store the returned html in an htmlagilitypack document for easy parsing
                    var websiteHTML = new HtmlAgilityPack.HtmlDocument();
                    websiteHTML.LoadHtml(resultsPage.ToLower()); //lower case

                    //keyword in title - <title>Keyword</title> 
                    var title = websiteHTML.DocumentNode.SelectNodes("//title");
                    foreach (var t in title)
                    {
                        if (t.InnerText.Contains(keyword))
                            site.setKeywordInTitle(true);
                    }

                    //keyword in meta tags
                    var metatags = websiteHTML.DocumentNode.SelectNodes("//meta");
                    foreach (var m in metatags)
                    {
                        string content = m.GetAttributeValue("content", "");
                        if (content.Contains(keyword))
                            site.setKeywordInMeta(true);
                    }

                    //keyword in the url (EMD?) - keyword || key-word
                    if (host.Contains(keywordurl1) || host.Contains(keywordurl2))
                        site.setIsEMD(true);

                    if (pageurl.Contains(keywordurl1) || pageurl.Contains(keywordurl2))
                        site.setKeywordInURL(true);

                    //keyword bolded/italicized/h1/h2 etc. in the landing page <b> <i> <em> <h1-6>
                    //h tags
                    //for(int i = 1; i < 6; ++i)

                }
                catch (Exception ex)
                {
                    //couldnt access the site because it is probably flash or something else
                    //remove the site from our list
                    //top10.Remove(site);
                    //site.
                }
            }
            return top10;
        }

        public allResults gatherAllData(string inputkeyword)
        {
            allResults allData = new allResults();
            float competition = -1;

            try
            {
                //fetch results page for desired keyword ONE time to prevent bans
                //Note: SEOTC is a completely separate search (intitle: keyword)
                string resultsPage = getSERP(inputkeyword);

                //get top 10 urls
                List<string> top10 = getTop10(resultsPage);

                //this will store the urls plus the extra information we need
                List<topSite> top10Details = new List<topSite>();

                foreach (string site in top10)
                {
                    topSite newSite = new topSite();
                    newSite.setPR(fetchPR(site));
                    newSite.setAge(fetchDA(site));
                    newSite.setURL(site);
                    //newSite.setBacklinks =
                    newSite.setDmozListed(checkDMOZ(site));
                    
                    top10Details.Add(newSite);
                }

                //analyze keyword counts for every site in top 10
                top10Details = analyzeKeywordCount(top10Details, inputkeyword);

                keyword keyword = new keyword();
                keyword.setName(inputkeyword);
                keyword.setSeoc(fetchSEOC(keyword.getName()));
                keyword.setSeotc(fetchSEOTC(keyword.getName()));

                allData.setKeyword(keyword);
                allData.setTop10(top10Details);

                //competition = analyzeComp(top10Details, inputkeyword);
            }
            catch (Exception ex)
            {

            }

            return allData;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void btnOpenScraper_Click(object sender, EventArgs e)
        {
            Form1 oldform = new Form1();
            oldform = this;

            KeywordScraper ks = new KeywordScraper(ref oldform);

            ks.Show();
        }

        //remove all duplicate keywords in the list
        private void btnRemoveDupes_Click(object sender, EventArgs e) //might implement this later
        {
            //List<string> keywords = new List<string>();
            //keywords.AddRange(txtKeywords.Lines);

        }
    }

    //this class will store all the other classes for easy access later while managing multiple keywords
    public class allResults //sprint 3
    {
        private List<topSite> top10_ = new List<topSite>();
        private keyword keyword_;
        private keywordMetrics keywordMetrics_;
        private float competition_;

        public float getCompetition()
        {
            return competition_;
        }

        public void setCompetition(float competition)
        {
            competition_ = competition;
        }

        public keywordMetrics getKeywordMetrics()
        {
            return keywordMetrics_;
        }

        public void setKeywordMetrics(keywordMetrics keywordmetrics)
        {
            keywordMetrics_ = keywordmetrics;
        }

        public keyword getKeyword()
        {
            return keyword_;
        }

        public void setKeyword(keyword keyword)
        {
            keyword_ = keyword;
        }

        public List<topSite> getTop10()
        {
            return top10_;
        }

        public void setTop10(List<topSite> top10)
        {
            top10_ = top10;
        }
    }

    //this class will store all of metric scores for a given keyword and 
    //will make it easier to store data and display it later when we 
    //mass search keywords
    public class keywordMetrics //sprint 2
    {
        private int seocScore_ = 0;
        private int seotcScore_ = 0;
        private int daScore_ = 0;
        private int prScore_ = 0;

        //SPRINT 3========================
        private int backlinkScore_ = 0;
        private int keywordInTitleScore_ = 0;
        private int keywordInMetaScore_ = 0;
        private int isEMDScore_ = 0;
        private int keywordInURLScore_ = 0;
        private int dmozListedScore_ = 0;     
        //END SPRINT 3====================

        private int compScore_ = 0; //end result

        public int getDmozListedScore()
        {
            return dmozListedScore_;
        }

        public void setDmozListedScore(int dmozListedScore)
        {
            dmozListedScore_ = dmozListedScore;
        }

        public int getKeywordInURLScore()
        {
            return keywordInURLScore_;
        }

        public void setKeywordInURLScore(int keywordInURLScore)
        {
            keywordInURLScore_ = keywordInURLScore;
        }

        public int getIsEMDScoreScore()
        {
            return isEMDScore_;
        }

        public void setIsEMDScoreScore(int isEMDScore)
        {
            isEMDScore_ = isEMDScore;
        }

        public int getKeywordInMetaScore()
        {
            return keywordInMetaScore_;
        }

        public void setKeywordInMetaScore(int keywordInMetaScore)
        {
            keywordInMetaScore_ = keywordInMetaScore;
        }

        public int getKeywordInTitleScore()
        {
            return keywordInTitleScore_;
        }

        public void setKeywordInTitleScore(int keywordInTitleScore)
        {
            keywordInTitleScore_ = keywordInTitleScore;
        }

        public int getBacklinkScore()
        {
            return backlinkScore_;
        }

        public void setBacklinkScore(int backlinkScore)
        {
            backlinkScore_ = backlinkScore;
        }

        public int getSeocScore()
        {
            return seocScore_;
        }

        public void setSeocScore(int uSeocScore)
        {
            seocScore_ = uSeocScore;
        }

        public int getSeotcScore()
        {
            return seotcScore_;
        }

        public void setSeotcScore(int uSeotcScore)
        {
            seotcScore_ = uSeotcScore;
        }

        public int getDaScore()
        {
            return daScore_;
        }

        public void setDaScore(int uDaScore)
        {
            daScore_ = uDaScore;
        }

        public int getPrScore()
        {
            return prScore_;
        }

        public void setPrScore(int uPrScore)
        {
            prScore_ = uPrScore;
        }

        public int getCompScore()
        {
            return compScore_;
        }

        public void setCompScore(int uCompScore)
        {
            compScore_ = uCompScore;
        }
    }

    //this class holds information for a site on the first page of Google
    public class topSite //sprint 1
    {
        private string url_;     //url in google top 10 - sprint 3
        private int pr_;         //page rank
        private int age_;        //domain age
        private int backlinks_;  //number of sites pointing to this url - sprint 3
        private bool keywordInTitle_; //is the keyword in title tags
        private bool keywordInMeta_;  //is the keyword in the meta tags
        private bool isEMD_;          //is the site an exact match domain
        private bool keywordInURL_;   //is the keyword in the url
        private bool dmozListed_;     //is the site listen on dmoz

        public bool getKeywordInMeta()
        {
            return keywordInMeta_;
        }

        public void setKeywordInMeta(bool keywordInMeta)
        {
            keywordInMeta_ = keywordInMeta;
        }

        public bool getKeywordInTitle()
        {
            return keywordInTitle_;
        }

        public void setKeywordInTitle(bool keywordInTitle)
        {
            keywordInTitle_ = keywordInTitle;
        }

        public bool getKeywordInURL()
        {
            return keywordInURL_;
        }

        public void setKeywordInURL(bool keywordInURL)
        {
            keywordInURL_ = keywordInURL;
        }

        public bool getIsEMD()
        {
            return isEMD_;
        }

        public void setIsEMD(bool emd)
        {
            isEMD_ = emd;
        }

        public bool getDmozListed()
        {
            return dmozListed_;
        }

        public void setDmozListed(bool dmozListed)
        {
            dmozListed_ = dmozListed;
        }

        public string getURL()
        {
            return url_;
        }

        public void setURL(string url)
        {
            url_ = url;
        }

        public int getBacklinks()
        {
            return backlinks_;
        }

        public void setBacklinks(int backlinks)
        {
            backlinks_ = backlinks;
        }

        public int getPR()
        {
            return pr_;
        }

        public void setPR(int uPR)
        {
            pr_ = uPR;
        }

        public int getAge()
        {
            return age_;
        }

        public void setAge(int uAge)
        {
            age_ = uAge;
        }
    }

    //this class holds information on the desired keyword
    public class keyword //sprint 1
    {
        private string name_;    //the keyword itself
        private double seoc_;    //phrase competition
        private double seotc_;   //title comp (not phrase)

        public string getName()
        {
            return name_;
        }

        public void setName(string uName)
        {
            name_ = uName;
        }

        public double getSeoc()
        {
            return seoc_;
        }

        public void setSeoc(double uSeoc)
        {
            seoc_ = uSeoc;
        }

        public double getSeotc()
        {
            return seotc_;
        }

        public void setSeotc(double uSeotc)
        {
            seotc_ = uSeotc;
        }
    }
}
