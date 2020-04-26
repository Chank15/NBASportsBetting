using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NBASportsBetting
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
        }

        private void btnGetGames_Click(object sender, EventArgs e)
        {
            findTeams();
            refAverages();
            teamStats();
            bettingStats();
            combineMOV();
            combineAvgs();
            makePicks();
            backtestOU();
            backtestATS();
        }

        private void findTeams()
        {
            var date = dateTimePicker1.Value.Date.ToString("yyyyMMdd"); //Establish correct format

            var a = 3;
            var b = 3;
            int numGames = 0;
            var ovun = "";
            var ats = "";
            double stat1Val = 0;
            double stat2Val = 0;

            List<string> teamList = new List<string>();
            List<string> atsList = new List<string>();
            List<string> ouList = new List<string>();
            List<string> scoreList = new List<string>();

            TextBox[] teamtb = { textBox1, textBox2, textBox3, textBox4, textBox5, textBox6, textBox7, textBox8, textBox9, textBox10, textBox11, textBox12, textBox13,
                                    textBox14, textBox15, textBox16, textBox17, textBox18, textBox19, textBox20, textBox21, textBox22};

            TextBox[] bettb = { textBox23, textBox24, textBox25, textBox26, textBox27, textBox28, textBox29, textBox30, textBox31, textBox32, textBox33, textBox34, textBox35,
                                    textBox36, textBox37, textBox38, textBox39, textBox40, textBox41, textBox42, textBox43, textBox44};

            TextBox[] scoretb = { textBox45, textBox46, textBox47, textBox48, textBox49, textBox50, textBox51, textBox52, textBox53, textBox54, textBox55, textBox56, textBox57,
                                    textBox58, textBox59, textBox60, textBox61, textBox62, textBox63, textBox64, textBox65, textBox66};

            
            //Identify number of games
            HtmlAgilityPack.HtmlWeb web = new HtmlAgilityPack.HtmlWeb();
            HtmlAgilityPack.HtmlDocument doc = web.Load("http://www.donbest.com/nba/scores/" + date + ".html");

            try
            {
                while (b < 20)
                {
                    b++;
                    var team = doc.DocumentNode.SelectNodes("//*/tr[" + b + "]/td[2]/a/span[1]")[0].InnerText;
                }
            }

            catch
            {
                Console.WriteLine("error");
            }
            numGames = (Convert.ToInt32(b) - 3);
            label27.Text = "Games: ";
            label28.Text = numGames.ToString();


            //*****************************************************************************************
            //Getting donbest games for date selected
            var team1 = doc.DocumentNode.SelectNodes("//*/tr[" + a + "]/td[2]/a/span[1]")[0].InnerText;
            var team2 = doc.DocumentNode.SelectNodes("//*/tr[" + a + "]/td[2]/a/span[2]")[0].InnerText;

            var stat1 = doc.DocumentNode.SelectNodes("//*/tr[" + a + "]/td[10]/div[1]")[0].InnerText;
            var stat2 = doc.DocumentNode.SelectNodes("//*/tr[" + a + "]/td[10]/div[2]")[0].InnerText;

            var score1 = doc.DocumentNode.SelectNodes("//*/tr[" + a + "]/td[8]/div[1]")[0].InnerText;
            var score2 = doc.DocumentNode.SelectNodes("//*/tr[" + a + "]/td[8]/div[2]")[0].InnerText;

            while (a <= (numGames + 2))
            {
                team1 = doc.DocumentNode.SelectNodes("//*/tr[" + a + "]/td[2]/a/span[1]")[0].InnerText;
                team2 = doc.DocumentNode.SelectNodes("//*/tr[" + a + "]/td[2]/a/span[2]")[0].InnerText;

                stat1 = doc.DocumentNode.SelectNodes("//*/tr[" + a + "]/td[10]/div[1]")[0].InnerText;
                stat2 = doc.DocumentNode.SelectNodes("//*/tr[" + a + "]/td[10]/div[2]")[0].InnerText;

                score1 = doc.DocumentNode.SelectNodes("//*/tr[" + a + "]/td[8]/div[1]")[0].InnerText;
                score2 = doc.DocumentNode.SelectNodes("//*/tr[" + a + "]/td[8]/div[2]")[0].InnerText;

                stat1Val = Convert.ToDouble(stat1);
                stat2Val = Convert.ToDouble(stat2);

                if (stat1Val < 100) //Determine if stat is over under or spread line.  It changes based on who is favored (negative number)
                {
                    stat1Val = stat1Val * -1;
                    ats = stat1Val.ToString();
                    ovun = stat2Val.ToString();
                }
                else
                {
                    ats = stat2Val.ToString();
                    ovun = stat1Val.ToString();
                }


                a++;

                teamList.Add(team1);
                teamList.Add(team2);
                atsList.Add(ats);
                ouList.Add(ovun);
                scoreList.Add(score1);
                scoreList.Add(score2);

            }

            var c = 0;
            int d = 0;
            int e = 1;
            while (c < numGames)
            {
                c++;
                teamtb[d].Text = teamList[0];
                teamtb[e].Text = teamList[1];
                bettb[d].Text = ouList[0];
                bettb[e].Text = atsList[0];
                scoretb[d].Text = scoreList[0];
                scoretb[e].Text = scoreList[1];
                d += 2;
                e += 2;

                //Remove teams to maintain 1st and second position in list
                teamList.RemoveAt(0);
                teamList.RemoveAt(0);
                ouList.RemoveAt(0);
                atsList.RemoveAt(0);
                scoreList.RemoveAt(0);
                scoreList.RemoveAt(0);

            }
        }
        private void refAverages()
        {

            TextBox[] reftb = { textBox67, textBox68, textBox69, textBox70, textBox71, textBox72, textBox73, textBox74, textBox75, textBox76, textBox77};
            int a = 2;
            int b = 0;
            var games = Convert.ToInt32(label28.Text);

            List<string> numList = new List<string>();
            List<string> refList = new List<string>();
            List<double> ppgList = new List<double>();

            var date = dateTimePicker1.Value.Date.ToString("yyyyMMdd");

            HtmlAgilityPack.HtmlWeb web = new HtmlAgilityPack.HtmlWeb();
            HtmlAgilityPack.HtmlDocument doc = web.Load("http://www.donbest.com/nba/scores/" + date + ".html");
            var teamNum = doc.DocumentNode.SelectNodes("//*/tr[3]/td[1]/text()[1]")[0].InnerText;

            while (b < games)
            {
                a++;
                teamNum = doc.DocumentNode.SelectNodes("//*/tr[" + a + "]/td[1]/text()[1]")[0].InnerText;
                numList.Add(teamNum); //This is to identify the game number so you can input it into the correct link below for the referees
                b++;
            }

            var c = 0;
            while (c < games)
            {
                HtmlAgilityPack.HtmlWeb web1 = new HtmlAgilityPack.HtmlWeb();
                HtmlAgilityPack.HtmlDocument doc1 = web1.Load("http://www.donbest.com/nba/scores_summary/3-" + numList[0] + "-" + date + "-box/");
                var refs = doc1.DocumentNode.SelectNodes("//*/tr[2]/td/text()[1]")[0].InnerText;
                List<string> ref1List = refs.Split(',').ToList<string>(); //split string of three refs where the commas are at
                numList.RemoveAt(0);
                c++;

                refList.Add(ref1List[0].Trim()); //trim white spaces surrounding names after splitting from commas.
                refList.Add(ref1List[1].Trim());
                refList.Add(ref1List[2].Trim());
            }

            //Now find the identified refs in the covers.com statistics and pull/display their stats.
            var d = 0;
            var e = 1;
            var f = 1;
            var g = 1;
            while (d < games)
            {
                d++;
                e = 1;
                f = 1;
                g = 1;
                HtmlAgilityPack.HtmlWeb web2 = new HtmlAgilityPack.HtmlWeb();
                HtmlAgilityPack.HtmlDocument doc2 = web2.Load("https://www.covers.com/sport/basketball/nba/referees/statistics/2019-2020");
                var siteRef0 = doc2.DocumentNode.SelectNodes("//*/tr[" + e + "]/ td[2]/a")[0].InnerText;
                var siteRef1 = doc2.DocumentNode.SelectNodes("//*/tr[" + f + "]/td[2]/a")[0].InnerText;
                var siteRef2 = doc2.DocumentNode.SelectNodes("//*/tr[" + g + "]/td[2]/a")[0].InnerText;
                var ppg1 = doc2.DocumentNode.SelectNodes("//*/tr[1]/td[8]")[0].InnerText;
                var ppg2 = doc2.DocumentNode.SelectNodes("//*/tr[1]/td[8]")[0].InnerText;
                var ppg3 = doc2.DocumentNode.SelectNodes("//*/tr[1]/td[8]")[0].InnerText;

                while (refList[0] != siteRef0)
                {
                    e++;
                    siteRef0 = doc2.DocumentNode.SelectNodes("//*/tr[" + e + "]/td[2]/a")[0].InnerText;
                }

                if (refList[0] == siteRef0)
                {
                    ppg1 = doc2.DocumentNode.SelectNodes("//*/tr[" + e + "]/td[8]")[0].InnerText;
                }

                while (refList[1] != siteRef1)
                {
                    f++;
                    siteRef1 = doc2.DocumentNode.SelectNodes("//*/tr[" + f + "]/td[2]/a")[0].InnerText;
                }

                if (refList[1] == siteRef1)
                {
                    ppg2 = doc2.DocumentNode.SelectNodes("//*/tr[" + f + "]/td[8]")[0].InnerText;
                }

                while (refList[2] != siteRef2)
                {
                    g++;
                    siteRef2 = doc2.DocumentNode.SelectNodes("//*/tr[" + g + "]/td[2]/a")[0].InnerText;
                }

                if (refList[2] == siteRef2)
                {
                    ppg3 = doc2.DocumentNode.SelectNodes("//*/tr[" + g + "]/td[8]")[0].InnerText;
                }

                double ref1Val = Convert.ToDouble(ppg1);
                double ref2Val = Convert.ToDouble(ppg2);
                double ref3Val = Convert.ToDouble(ppg3);
                double refAvg = (ref1Val + ref2Val + ref3Val) / 3;
                ppgList.Add(refAvg);

                refList.RemoveAt(0);
                refList.RemoveAt(0);
                refList.RemoveAt(0);

            }

            var h = 0;
            int i = 0;
            while (h < games)
            {
                h++;
                reftb[i].Text = ppgList[0].ToString("#.00");
                i++;

                ppgList.RemoveAt(0);

            }
        }

        private void teamStats()
        {
            TextBox[] teamtb = { textBox78, textBox79, textBox80, textBox81, textBox82, textBox83, textBox84, textBox85, textBox86, textBox87, textBox88,
                                textBox89, textBox90, textBox91, textBox92, textBox93, textBox94, textBox95, textBox96, textBox97, textBox98, textBox99};

            TextBox[] teamnametb = { textBox1, textBox2, textBox3, textBox4, textBox5, textBox6, textBox7, textBox8, textBox9, textBox10, textBox11, textBox12, textBox13,
                                    textBox14, textBox15, textBox16, textBox17, textBox18, textBox19, textBox20, textBox21, textBox22};

            TextBox[] movppgtb = { textBox125, textBox126, textBox127, textBox128, textBox129, textBox130, textBox131, textBox132, textBox133, textBox134, textBox135,
                                    textBox136, textBox137, textBox138, textBox139, textBox140, textBox141, textBox142, textBox143, textBox144, textBox145, textBox146 };

            var games = Convert.ToInt32(label28.Text);
            List<string> teamList = new List<string>();
            List<double> teamPPG = new List<double>();
            List<double> movPPG = new List<double>();

            var e = 0;
            var i = 0;
            while (e < (games * 2))
            {
                var team = teamnametb[i].Text;
                teamList.Add(team);
                e++;
                i++;
            }


            var f = 0;
            while (f < games)
            {
                f++;

                var a = 7;
                var b = 7;
                var c = 7;
                var d = 7;

                var teamat = teamMatch(teamList[0]);
                var teamht = teamMatch(teamList[1]);

                HtmlAgilityPack.HtmlWeb web = new HtmlAgilityPack.HtmlWeb();
                HtmlAgilityPack.HtmlDocument doc = web.Load("http://www.hoopsstats.com/basketball/fantasy/nba/teamstats/20/1/diffeff/3-1");//away team stats
                HtmlAgilityPack.HtmlDocument doc1 = web.Load("http://www.hoopsstats.com/basketball/fantasy/nba/opponentstats/20/1/diffeff/3-1"); //away team opp stats
                HtmlAgilityPack.HtmlDocument doc2 = web.Load("http://www.hoopsstats.com/basketball/fantasy/nba/teamstats/20/1/diffeff/2-1"); //home team stats
                HtmlAgilityPack.HtmlDocument doc3 = web.Load("http://www.hoopsstats.com/basketball/fantasy/nba/opponentstats/20/1/diffeff/2-1"); //home team opp stats

                //Have to create siteTeam to pull data from different link listed above as the rankings of teams would change and potentially display incorrect data
                var siteTeam = doc.DocumentNode.SelectNodes("//*/table[" + a + "]/tr/td[2]")[0].InnerText;
                var siteTeam1 = doc1.DocumentNode.SelectNodes("//*/table[" + b + "]/tr/td[2]")[0].InnerText;
                var siteTeam2 = doc2.DocumentNode.SelectNodes("//*/table[" + c + "]/tr/td[2]")[0].InnerText;
                var siteTeam3 = doc3.DocumentNode.SelectNodes("//*/table[" + d + "]/tr/td[2]")[0].InnerText;

                //Also have to run PPG and aPPG from for same reasons noted above
                var atPPG = doc.DocumentNode.SelectNodes("//*/table[" + a + "]/tr/td[5]")[0].InnerText;
                var ataPPG = doc1.DocumentNode.SelectNodes("//*/table[" + b + "]/tr/td[5]")[0].InnerText;
                var htPPG = doc2.DocumentNode.SelectNodes("//*/table[" + c + "]/tr/td[5]")[0].InnerText;
                var htaPPG = doc3.DocumentNode.SelectNodes("//*/table[" + d + "]/tr/td[5]")[0].InnerText;

                //Get away team PPG as away team
                while (teamat != siteTeam)
                {
                    a++;
                    siteTeam = doc.DocumentNode.SelectNodes("//*/table[" + a + "]/tr/td[2]")[0].InnerText;
                }

                if (teamat == siteTeam)
                {
                    atPPG = doc.DocumentNode.SelectNodes("//*/table[" + a + "]/tr/td[5]")[0].InnerText;
                }

                //Get away team aPPG as away team
                while (teamat != siteTeam1)
                {
                    b++;
                    siteTeam1 = doc1.DocumentNode.SelectNodes("//*/table[" + b + "]/tr/td[2]")[0].InnerText;
                }

                if (teamat == siteTeam1)
                {
                    ataPPG = doc1.DocumentNode.SelectNodes("//*/table[" + b + "]/tr/td[5]")[0].InnerText;
                }

                //Get home team PPG as home team
                while (teamht != siteTeam2)
                {
                    c++;
                    siteTeam2 = doc2.DocumentNode.SelectNodes("//*/table[" + c + "]/tr/td[2]")[0].InnerText;
                }

                if (teamht == siteTeam2)
                {
                    htPPG = doc2.DocumentNode.SelectNodes("//*/table[" + c + "]/tr/td[5]")[0].InnerText;
                }

                //Get home team aPPG as home team
                while (teamht != siteTeam3)
                {
                    d++;
                    siteTeam3 = doc3.DocumentNode.SelectNodes("//*/table[" + d + "]/tr/td[2]")[0].InnerText;
                }

                if (teamht == siteTeam3)
                {
                    htaPPG = doc3.DocumentNode.SelectNodes("//*/table[" + d + "]/tr/td[5]")[0].InnerText;
                }

                double atPPGValue = Convert.ToDouble(atPPG);
                double ataPPGValue = Convert.ToDouble(ataPPG);
                double htPPGValue = Convert.ToDouble(htPPG);
                double htaPPGValue = Convert.ToDouble(htaPPG);

                //Average points between each's teams PPG and aPPG.
                //Ex. HTPPG averaged with ATaPPG.
                double atPoints = (atPPGValue + htaPPGValue) / 2;
                double htPoints = (htPPGValue + ataPPGValue) / 2;
                teamPPG.Add(atPoints);
                teamPPG.Add(htPoints);

                //Calculate MOV for AT and HT using the averages calculated above
                double atMOV = atPoints - htPoints;
                double htMOV = htPoints - atPoints;

                movPPG.Add(atMOV);
                movPPG.Add(htMOV);

                //Remove first team in list (twice as when you remove #0, #1 would be come #0) in order to loop with new teams above and go through process.
                teamList.RemoveAt(0);
                teamList.RemoveAt(0);
                               
            }

            //Display in textboxes
            var g = 0;
            var h = 0;
            while (g < (games * 2))
            {
                g++;
                teamtb[h].Text = teamPPG[0].ToString("#.00");
                movppgtb[h].Text = movPPG[0].ToString("#.00");
                h++;
                teamPPG.RemoveAt(0);
                movPPG.RemoveAt(0);
            }
        }

        private void bettingStats()
        {
            TextBox[] teamtb = { textBox1, textBox2, textBox3, textBox4, textBox5, textBox6, textBox7, textBox8, textBox9, textBox10, textBox11, textBox12, textBox13,
                                    textBox14, textBox15, textBox16, textBox17, textBox18, textBox19, textBox20, textBox21, textBox22};
            TextBox[] ovrmovtb = { textBox147, textBox148, textBox149, textBox150, textBox151, textBox152, textBox153, textBox154, textBox155, textBox156, textBox157,
                                    textBox158, textBox159, textBox160, textBox161, textBox162, textBox163, textBox164, textBox165, textBox166, textBox167, textBox168 };
            TextBox[] bettb = { textBox23, textBox24, textBox25, textBox26, textBox27, textBox28, textBox29, textBox30, textBox31, textBox32, textBox33, textBox34, textBox35,
                                    textBox36, textBox37, textBox38, textBox39, textBox40, textBox41, textBox42, textBox43, textBox44};
            TextBox[] athtmovtb = { textBox169, textBox170, textBox171, textBox172, textBox173, textBox174, textBox175, textBox176, textBox177, textBox178, textBox179,
                                    textBox180, textBox181, textBox182, textBox183, textBox184, textBox185, textBox186, textBox187, textBox188, textBox189, textBox190 };
            TextBox[] fadogmovtb = { textBox191, textBox192, textBox193, textBox194, textBox195, textBox196, textBox197, textBox198, textBox199, textBox200, textBox201,
                                    textBox202, textBox203, textBox204, textBox205, textBox206, textBox207, textBox208, textBox209, textBox210, textBox211, textBox212 };

            var games = Convert.ToInt32(label28.Text);

            //Create lists to store gathered team names and MOV
            List<string> teams = new List<string>();
            List<double> spread = new List<double>();

            //Pull teams from textboxes
            var a = 0;
            while (a < (games * 2))
            {
                teams.Add(teamtb[a].Text);
                a++;
            }

            //Get overall MOV stats
            var b = 0;
            while (b < (games * 2))
            {
                
                //adjust naming convention of teams as the two sites list them differently
                var team = teamMatchTR(teams[b]);


                HtmlAgilityPack.HtmlWeb web = new HtmlAgilityPack.HtmlWeb();
                HtmlAgilityPack.HtmlDocument doc = web.Load("https://www.teamrankings.com/nba/trends/ats_trends/?sc=all_games");
                var siteTeam = doc.DocumentNode.SelectNodes("//*/tr[1]/td[1]/a")[0].InnerText;
                var mov = doc.DocumentNode.SelectNodes("//*/tr[1]/td[4]")[0].InnerText;

                var c = 1;
                while (team != siteTeam)
                {
                    c++;
                    siteTeam = doc.DocumentNode.SelectNodes("//*/tr[" + c + "]/td[1]/a")[0].InnerText;
                }

                if (team == siteTeam)
                {
                    mov = doc.DocumentNode.SelectNodes("//*/tr[" + c + "]/td[4]")[0].InnerText;
                    ovrmovtb[b].Text = mov;
                }

                b++;
            }


            //Get MOV as home-team and away-team
            var d = 0;
            var e = 1;
            var h = 0;
            while (h < games)
            {
                var teamAT = teamMatchTR(teams[d]);
                var teamHT = teamMatchTR(teams[e]);

                HtmlAgilityPack.HtmlWeb web = new HtmlAgilityPack.HtmlWeb();
                HtmlAgilityPack.HtmlDocument docAT = web.Load("https://www.teamrankings.com/nba/trends/ats_trends/?sc=is_away");
                HtmlAgilityPack.HtmlDocument docHT = web.Load("https://www.teamrankings.com/nba/trends/ats_trends/?sc=is_home");
                var siteAT = docAT.DocumentNode.SelectNodes("//*/tr[1]/td[1]/a")[0].InnerText;
                var movAT = docAT.DocumentNode.SelectNodes("//*/tr[1]/td[4]")[0].InnerText;
                var siteHT = docHT.DocumentNode.SelectNodes("//*/tr[1]/td[1]/a")[0].InnerText;
                var movHT = docHT.DocumentNode.SelectNodes("//*/tr[1]/td[4]")[0].InnerText;

                var f = 1;
                while (teamAT != siteAT)
                {
                    f++;
                    siteAT = docAT.DocumentNode.SelectNodes("//*/tr[" + f + "]/td[1]/a")[0].InnerText;
                }
                if (teamAT == siteAT)
                {
                    movAT = docAT.DocumentNode.SelectNodes("//*/tr[" + f + "]/td[4]")[0].InnerText;
                    athtmovtb[d].Text = movAT;
                }

                var g = 1;
                while (teamHT != siteHT)
                {
                    g++;
                    siteHT = docHT.DocumentNode.SelectNodes("//*/tr[" + g + "]/td[1]/a")[0].InnerText;
                }
                if (teamAT == siteAT)
                {
                    movHT = docHT.DocumentNode.SelectNodes("//*/tr[" + g + "]/td[4]")[0].InnerText;
                    athtmovtb[e].Text = movHT;
                }

                h++;
                d += 2;
                e += 2;
            }

            //Get MOV as 'as away favorite' or 'as home favorite'
            //Get spreads for games
            var i = 1;
            var x = 0;
            while (x < games)
            {
                spread.Add(Convert.ToDouble(bettb[i].Text));
                i += 2;
                x++;
            }

            var j = 0;
            var k = 0;
            var l = 1;
            while (j < games)
            {
                var teamAT = teamMatchTR(teams[k]);
                var teamHT = teamMatchTR(teams[l]);

                double ats = spread[j];

                var linkAT = "";
                var linkHT = "";

                if (ats < 0)
                {
                    linkAT = "is_away_dog";
                    linkHT = "is_home_fav";
                }

                if (ats > 0)
                {
                    linkAT = "is_away_fav";
                    linkHT = "is_home_dog";
                }

                HtmlAgilityPack.HtmlWeb web = new HtmlAgilityPack.HtmlWeb();
                HtmlAgilityPack.HtmlDocument docAsAT = web.Load("https://www.teamrankings.com/nba/trends/ats_trends/?sc=" + linkAT);
                HtmlAgilityPack.HtmlDocument docAsHT = web.Load("https://www.teamrankings.com/nba/trends/ats_trends/?sc=" + linkHT);
                var siteAsAT = docAsAT.DocumentNode.SelectNodes("//*/tr[1]/td[1]/a")[0].InnerText;
                var movAsAT = docAsAT.DocumentNode.SelectNodes("//*/tr[1]/td[4]")[0].InnerText;
                var siteAsHT = docAsHT.DocumentNode.SelectNodes("//*/tr[1]/td[1]/a")[0].InnerText;
                var movAsHT = docAsHT.DocumentNode.SelectNodes("//*/tr[1]/td[4]")[0].InnerText;

                var m = 1;
                while (teamAT != siteAsAT)
                {
                    m++;
                    siteAsAT = docAsAT.DocumentNode.SelectNodes("//*/tr[" + m + "]/td[1]/a")[0].InnerText;
                }
                if (teamAT == siteAsAT)
                {
                    movAsAT = docAsAT.DocumentNode.SelectNodes("//*/tr[" + m + "]/td[4]")[0].InnerText;
                    fadogmovtb[k].Text = movAsAT;
                }

                var n = 1;
                while (teamHT != siteAsHT)
                {
                    n++;
                    siteAsHT = docAsHT.DocumentNode.SelectNodes("//*/tr[" + n + "]/td[1]/a")[0].InnerText;
                }
                if (teamAT == siteAsAT)
                {
                    movAsHT = docAsHT.DocumentNode.SelectNodes("//*/tr[" + n + "]/td[4]")[0].InnerText;
                    fadogmovtb[l].Text = movAsHT;
                }

                j++;
                k += 2;
                l += 2;

            }
        }

        //Combine margin of victory for all sources pulled
        private void combineMOV()
        {
            var games = Convert.ToInt32(label28.Text);

            TextBox[] bettb = { textBox23, textBox24, textBox25, textBox26, textBox27, textBox28, textBox29, textBox30, textBox31, textBox32, textBox33, textBox34, textBox35,
                                    textBox36, textBox37, textBox38, textBox39, textBox40, textBox41, textBox42, textBox43, textBox44};
            TextBox[] movppgtb = { textBox125, textBox126, textBox127, textBox128, textBox129, textBox130, textBox131, textBox132, textBox133, textBox134, textBox135,
                                    textBox136, textBox137, textBox138, textBox139, textBox140, textBox141, textBox142, textBox143, textBox144, textBox145, textBox146 };
            TextBox[] ovrmovtb = { textBox147, textBox148, textBox149, textBox150, textBox151, textBox152, textBox153, textBox154, textBox155, textBox156, textBox157,
                                    textBox158, textBox159, textBox160, textBox161, textBox162, textBox163, textBox164, textBox165, textBox166, textBox167, textBox168 };
            TextBox[] athtmovtb = { textBox169, textBox170, textBox171, textBox172, textBox173, textBox174, textBox175, textBox176, textBox177, textBox178, textBox179,
                                    textBox180, textBox181, textBox182, textBox183, textBox184, textBox185, textBox186, textBox187, textBox188, textBox189, textBox190 };
            TextBox[] fadogmovtb = { textBox191, textBox192, textBox193, textBox194, textBox195, textBox196, textBox197, textBox198, textBox199, textBox200, textBox201,
                                    textBox202, textBox203, textBox204, textBox205, textBox206, textBox207, textBox208, textBox209, textBox210, textBox211, textBox212 };
            TextBox[] cmbmovtb = { textBox224, textBox225, textBox226, textBox227, textBox228, textBox229, textBox230, textBox231, textBox232, textBox233, textBox234,
                                    textBox235, textBox236, textBox237, textBox238, textBox239, textBox240, textBox241, textBox242, textBox243, textBox244, textBox245 };

            List<double> spread = new List<double>();

            //Get spread
            var x = 0;
            var i = 1;
            while (x < games)
            {
                spread.Add(Convert.ToDouble(bettb[i].Text) * -1); //Add the the first in the list as the away team
                spread.Add(Convert.ToDouble(bettb[i].Text)); //Add the second in the list as the home team
                i += 2;
                x++;
            }

            //Added spread to determine final outcome after consider +/- spread
            var a = 0;
            while (a < (games*2))
            {
                double movppg = Convert.ToDouble(movppgtb[a].Text);
                double ovrmov = Convert.ToDouble(ovrmovtb[a].Text);
                double athtmov = Convert.ToDouble(athtmovtb[a].Text);
                double fadogmov = Convert.ToDouble(fadogmovtb[a].Text);

                double cmbMOV = (movppg + spread[a]) + (ovrmov + spread[a]) + (athtmov + spread[a]) + (fadogmov + spread[a]);

                cmbmovtb[a].Text = cmbMOV.ToString();

                a++;
            }

        }
        private void combineAvgs()
        {
            //Pull number of games
            var games = Convert.ToInt32(label28.Text);

            TextBox[] teamscoretb = { textBox78, textBox79, textBox80, textBox81, textBox82, textBox83, textBox84, textBox85, textBox86, textBox87, textBox88,
                                textBox89, textBox90, textBox91, textBox92, textBox93, textBox94, textBox95, textBox96, textBox97, textBox98, textBox99};

            TextBox[] reftb = { textBox67, textBox68, textBox69, textBox70, textBox71, textBox72, textBox73, textBox74, textBox75, textBox76, textBox77 };

            TextBox[] combineavgtb = { textBox100, textBox101, textBox102, textBox103, textBox104, textBox105, textBox106, textBox107, textBox108, textBox109, textBox110 };

            //Create list to house total calculated average of both teams + refs
            List<double> avgScore = new List<double>();

            //Run down list of games/textboxes to gather correct info
            var a = 0;
            int b = 0;
            int c = 1;
            var d = 0;
            var e = 0;
            while (a < games)
            {
                //Gather scores from each textbox
                double atscore = Convert.ToDouble(teamscoretb[b].Text);
                double htscore = Convert.ToDouble(teamscoretb[c].Text);
                double refavg = Convert.ToDouble(reftb[d].Text);

                //Calculate averages and add to avgScore list
                double average = ((atscore + htscore) + refavg) / 2;
                avgScore.Add(average);

                //Display and remove number from list to continue
                combineavgtb[e].Text = avgScore[0].ToString("#.00");
                avgScore.RemoveAt(0);

                //Add appropriate numbers to variables to skip to the correct game/textboxes
                b += 2;
                c += 2;
                d++;
                e++;
                a++;
            }
        }

        private void makePicks()
        {
            var games = Convert.ToInt32(label28.Text);

            // Make OU Picks
            TextBox[] teamtb = { textBox1, textBox2, textBox3, textBox4, textBox5, textBox6, textBox7, textBox8, textBox9, textBox10, textBox11, textBox12, textBox13,
                                    textBox14, textBox15, textBox16, textBox17, textBox18, textBox19, textBox20, textBox21, textBox22};
            TextBox[] outb = { textBox23, textBox24, textBox25, textBox26, textBox27, textBox28, textBox29, textBox30, textBox31, textBox32, textBox33, textBox34, textBox35,
                                    textBox36, textBox37, textBox38, textBox39, textBox40, textBox41, textBox42, textBox43, textBox44};
            TextBox[] combineavgtb = { textBox100, textBox101, textBox102, textBox103, textBox104, textBox105, textBox106, textBox107, textBox108, textBox109, textBox110 };
            TextBox[] cmbmovtb = { textBox224, textBox225, textBox226, textBox227, textBox228, textBox229, textBox230, textBox231, textBox232, textBox233, textBox234,
                                    textBox235, textBox236, textBox237, textBox238, textBox239, textBox240, textBox241, textBox242, textBox243, textBox244, textBox245 };

            TextBox[] oupickstb = { textBox111, textBox112, textBox113, textBox114, textBox115, textBox116, textBox117, textBox118, textBox119, textBox120, textBox121 };
            TextBox[] atspickstb = { textBox213, textBox214, textBox215, textBox216, textBox217, textBox218, textBox219, textBox220, textBox221, textBox222, textBox223 };

            var a = 0;
            var b = 0;
            while (a < games)
            {
                double ouLine = Convert.ToDouble(outb[b].Text);
                double teamAvg = Convert.ToDouble(combineavgtb[a].Text);

                if (teamAvg > ouLine)
                    oupickstb[a].Text = "Over";
                if (teamAvg < ouLine)
                    oupickstb[a].Text = "Under";
                else if (teamAvg == ouLine)
                    oupickstb[a].Text = "No Bet";

                a++;
                b += 2;
            }

            // Make ATS Picks
            var c = 0;
            var d = 0;
            var e = 1;
            while (c < games)
            {
                double atMOV = Convert.ToDouble(cmbmovtb[d].Text);
                double htMOV = Convert.ToDouble(cmbmovtb[e].Text);

                if (atMOV > htMOV)
                {
                    atspickstb[c].Text = teamtb[d].Text;
                }

                if (htMOV > atMOV)
                {
                    atspickstb[c].Text = teamtb[e].Text;
                }

                if (atMOV == htMOV)
                {
                    atspickstb[c].Text = "No Bet";
                }

                c++;
                d += 2;
                e += 2;
            }


        }

        private void backtestOU()
        {
            var games = Convert.ToInt32(label28.Text);

            int win;
            int loss;
            int push;

            //Determine if there is already a value in the record boxes.  If there is, the value will be assign so can continue running record.
            if (textBox122.TextLength == 0)
                win = 0;
            else
                win = Convert.ToInt32(textBox122.Text);

            if (textBox123.TextLength == 0)
                loss = 0;
            else
                loss = Convert.ToInt32(textBox123.Text);

            if (textBox124.TextLength == 0)
                push = 0;
            else
                push = Convert.ToInt32(textBox124.Text);

            TextBox[] oupickstb = { textBox111, textBox112, textBox113, textBox114, textBox115, textBox116, textBox117, textBox118, textBox119, textBox120, textBox121 };

            TextBox[] scoretb = { textBox45, textBox46, textBox47, textBox48, textBox49, textBox50, textBox51, textBox52, textBox53, textBox54, textBox55, textBox56, textBox57,
                                    textBox58, textBox59, textBox60, textBox61, textBox62, textBox63, textBox64, textBox65, textBox66};

            TextBox[] bettb = { textBox23, textBox24, textBox25, textBox26, textBox27, textBox28, textBox29, textBox30, textBox31, textBox32, textBox33, textBox34, textBox35,
                                    textBox36, textBox37, textBox38, textBox39, textBox40, textBox41, textBox42, textBox43, textBox44};



            double totalScore;
            double ouLine;

            var a = 0;
            var b = 1;
            var c = 0;
            while (c < games)
            {
                double atScore = Convert.ToDouble(scoretb[a].Text);
                double htScore = Convert.ToDouble(scoretb[b].Text);
                totalScore = atScore + htScore;

                ouLine = Convert.ToDouble(bettb[a].Text);

                var ouPick = oupickstb[c].Text;

                if (ouPick == "Over")
                {
                    if (totalScore > ouLine)
                        win++;
                    else if (totalScore < ouLine)
                        loss++;
                    else if (totalScore == ouLine)
                        push++;
                }

                if (ouPick == "Under")
                {
                    if (totalScore > ouLine)
                        loss++;
                    else if (totalScore < ouLine)
                        win++;
                    else if (totalScore == ouLine)
                        push++;
                }

                if (ouPick == "No Bet")
                {
                    push = push + 1 - 1;
                }

                a += 2;
                b += 2;
                c++;
            }

            textBox122.Text = win.ToString();
            textBox123.Text = loss.ToString();
            textBox124.Text = push.ToString();
        }

        private void backtestATS()
        {
            var games = Convert.ToInt32(label28.Text);

            int win;
            int loss;
            int push;

            //Determine if there is already a value in the record boxes.  If there is, the value will be assign so can continue running record.
            if (textBox246.TextLength == 0)
                win = 0;
            else
                win = Convert.ToInt32(textBox246.Text);

            if (textBox247.TextLength == 0)
                loss = 0;
            else
                loss = Convert.ToInt32(textBox247.Text);

            if (textBox248.TextLength == 0)
                push = 0;
            else
                push = Convert.ToInt32(textBox248.Text);

            TextBox[] teamtb = { textBox1, textBox2, textBox3, textBox4, textBox5, textBox6, textBox7, textBox8, textBox9, textBox10, textBox11, textBox12, textBox13,
                                    textBox14, textBox15, textBox16, textBox17, textBox18, textBox19, textBox20, textBox21, textBox22};
            TextBox[] atspickstb = { textBox213, textBox214, textBox215, textBox216, textBox217, textBox218, textBox219, textBox220, textBox221, textBox222, textBox223 };
            TextBox[] bettb = { textBox23, textBox24, textBox25, textBox26, textBox27, textBox28, textBox29, textBox30, textBox31, textBox32, textBox33, textBox34, textBox35,
                                    textBox36, textBox37, textBox38, textBox39, textBox40, textBox41, textBox42, textBox43, textBox44};
            TextBox[] scoretb = { textBox45, textBox46, textBox47, textBox48, textBox49, textBox50, textBox51, textBox52, textBox53, textBox54, textBox55, textBox56, textBox57,
                                    textBox58, textBox59, textBox60, textBox61, textBox62, textBox63, textBox64, textBox65, textBox66};

            //Determine who covers
            var a = 0;
            var b = 0;
            var c = 1;
            while (a < games)
            {
                double atScore = Convert.ToDouble(scoretb[b].Text);
                double htScore = Convert.ToDouble(scoretb[c].Text);
                double spread = Convert.ToDouble(bettb[c].Text);

                double netScore = (spread + htScore) - atScore;

                var winner = "";

                if (netScore > 0)
                {

                    winner = teamtb[c].Text;
                }

                if (netScore < 0)
                {
                    winner = teamtb[b].Text;
                }

                if (netScore == 0)
                    winner = "Push";

                if (winner == atspickstb[a].Text)
                    win++;

                if (winner != atspickstb[a].Text)
                {
                    if (winner == "Push")
                        push++;
                    else
                        loss++;
                }

                a++;
                b += 2;
                c += 2;
            }

            textBox246.Text = win.ToString();
            textBox247.Text = loss.ToString();
            textBox248.Text = push.ToString();

        }

        static string teamMatch(string x)
        {
            var team = x;

            if (x == "Los Angeles Lakers")
                team = "L.A.Lakers";

            else if (x == "Los Angeles Clippers")
                team = "L.A.Clippers";

            else if (x == "Portland Trail Blazers")
                team = "Portland";

            else
            {
                var final = team.Split(' ');
                int count = final.Count();
                if (count > 2)
                {
                    string word1 = team.Split(' ')[0];
                    string word2 = team.Split(' ')[1];
                    team = word1 + " " + word2;
                }

                if (count < 3)
                {
                    team = team.Split(' ')[0];
                }
            }

            return team;
        }

        static string teamMatchTR(string x)
        {
            var team = x;

            if (x == "Los Angeles Lakers")
                team = "LA Lakers";

            else if (x == "Los Angeles Clippers")
                team = "LA Clippers";

            else if (x == "Oklahoma City Thunder")
                team = "Okla City";

            else if (x == "Portland Trail Blazers")
                team = "Portland";

            else
            {
                var final = team.Split(' ');
                int count = final.Count();
                if (count > 2)
                {
                    string word1 = team.Split(' ')[0];
                    string word2 = team.Split(' ')[1];
                    team = word1 + " " + word2;
                }

                if (count < 3)
                {
                    team = team.Split(' ')[0];
                }
            }

            return team;
        }
    }
}
