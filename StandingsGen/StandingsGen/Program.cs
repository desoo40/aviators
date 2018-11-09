using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Drawing.Imaging;

namespace StandingsGen
{
    class Program
    {
        static int pointPerWinInMainTime = 0;
        static int pointPerWinAfterMainTime = 0;
        static int pointPerLoseAfterMainTime = 0;
        static int pointPerDraw = 0;
        static int gamesBetween = 0;

        static List<Team> teams = null;
        static Dictionary<string, int> DictIndTeamByName = null;
        static Dictionary<int, string> DictNameTeamByInd = null;
        static Dictionary<string, FullName> DictFullName = null;
        static int currInd = 0;

        static List<List<List<Score>>> games = new List<List<List<Score>>>();

        static void Main(string[] args)
        {
            teams = new List<Team>();
            DictIndTeamByName = new Dictionary<string, int>();
            DictNameTeamByInd = new Dictionary<int, string>();
            DictFullName = new Dictionary<string, FullName>();


            var file = "tournaments//МСХЛ2018";
            ReadFile(file);

            SortStandings();
            PrintStandingsWithoutScores();
            PrintScores();

            DrawingTable();
        }

        private static void DrawingTable()
        {
            int marginLower = 80;
            int marginUpper = 160;
            int marginSides = 20;
            int marginHorizontalLines = 4;

            int heightHat = 60;
            int heightTableRow = 100;

            int widhtTeamName = 230;
            int widhtScore = 100;
            int widthIndicator = 80;
            int widthPucksDiff = 150;

            int reglamentSett = 6;
            int teamsCnt = teams.Count;

            int widhtBitmap = 2 * marginSides + widhtTeamName + widhtScore * teamsCnt + widthIndicator * reglamentSett + widthPucksDiff;
            int heightBitmap = marginUpper + marginLower + heightHat + heightTableRow * teamsCnt;
            
            Image bitmap = new Bitmap(widhtBitmap, heightBitmap);

            Image imgLayer = Image.FromFile("images//layer.png");
            Image imgWinter = Image.FromFile("images//winter.png");
            Image imgBorn  = Image.FromFile("images//#borntofly.png");

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                var imgCut= GetInscribed(new Rectangle(0, 0, widhtBitmap, heightBitmap), new Size(widhtBitmap, heightBitmap));

                var colorTableBckgr = ColorTranslator.FromHtml("#e0e6ea");
                var colorLines = ColorTranslator.FromHtml("#00337f");

                var brushTableBckrg = new SolidBrush(colorTableBckgr);
                var brushLines = new SolidBrush(colorLines);

                var rectBckgr = new Rectangle(0, marginUpper, widhtBitmap, heightBitmap - marginUpper - marginLower);

                g.DrawImage(imgLayer, imgCut);
                g.FillRectangle(brushTableBckrg, rectBckgr);
                g.DrawImage(imgWinter, imgCut);

                var nameTableRect = new Rectangle(250, 25, 1100, 110);

                var brushWhite = new LinearGradientBrush(new Point(0, nameTableRect.Y),
                                                         new Point(0, nameTableRect.Y + nameTableRect.Height / 2),
                                                         ColorTranslator.FromHtml("#f1f1f1"),
                                                         ColorTranslator.FromHtml("#ffffff"));

                var brushBlue = new LinearGradientBrush(new Point(0, marginUpper),
                                                        new Point(0, marginUpper + heightHat / 2),
                                                        ColorTranslator.FromHtml("#00337f"),
                                                        ColorTranslator.FromHtml("#0041a1"));

                PrivateFontCollection fontColl = new PrivateFontCollection();

                fontColl.AddFontFile("fonts//FiraSans-SemiBold.ttf");
                var sizeFontTableName = 100;
                var fontTableName = new Font(fontColl.Families[0], sizeFontTableName);

                var fmt = new Format();
                var centerFormat = fmt.centerFormat;

                g.DrawString("МАГИСТР", fontTableName, brushWhite, nameTableRect, centerFormat);

               

                for (int i = 0; i < teamsCnt + 1; ++i)
                {
                    //g.DrawRectangle(new Pen(rectCol, 1), 4, 220 + i * 100, 1592, 4);
                    g.FillRectangle(brushLines, marginHorizontalLines,
                                    marginUpper + heightHat + i * heightTableRow,
                                    widhtBitmap - 2 * marginHorizontalLines, 4);
                }

                g.FillRectangle(brushLines, 250, 221, 4, heightBitmap - marginUpper - marginLower - heightHat);

                for (int i = 0; i < teamsCnt; ++i)
                {
                    for (int j = 0; j < teamsCnt; ++j)
                    {
                        g.FillRectangle(brushLines, 350 + i * 100, 230 + j * 100, 3, 80);
                    }
                }


                for (int i = 0; i < teamsCnt; ++i)
                {
                    var name = teams[i].name;
                    Image teamLogo = Image.FromFile($"teams//{name}.png");

                    int inBoxMarginForLogo = 13;
                    int sizeOfLogo = 80;

                    var pos = new Rectangle(marginSides + widhtTeamName + inBoxMarginForLogo + i * 100,
                                            marginUpper + heightHat + inBoxMarginForLogo + i * 100,
                                            sizeOfLogo, sizeOfLogo);

                    var kkk = GetInscribed(pos, teamLogo.Size);

                    g.DrawImage(teamLogo, kkk);
                    //g.DrawString(name, f, br, sidesMargin + teamNameWidht + inBoxMarginForLogo + i * 100, upperMagrin + hatHight + inBoxMarginForLogo + i * 100);

                }

                fontColl.AddFontFile("fonts//FiraSans-ExtraBold.ttf");
                var hatF = new Font(fontColl.Families[0], 35);

                for (int i = 0; i < teamsCnt; ++i)
                {
                    int startPos = marginSides + widhtTeamName;
                    
                    var rect = new Rectangle(startPos + i * widhtScore, marginUpper + 5, widhtScore, heightHat);

                    g.DrawString((i + 1).ToString(), hatF, brushBlue, rect, centerFormat);
                }
            }



            var file = "kek.png";

            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 100L);
            myEncoderParameters.Param[0] = myEncoderParameter;

            ImageCodecInfo pngEncoder = GetEncoder(ImageFormat.Png);

            bitmap.Save(file, pngEncoder, myEncoderParameters);
            Console.WriteLine("Сохранил!\n");
        }

        #region DrawingHelp
        private static Rectangle GetInscribed(Rectangle baseRect, Size inputsize)
        {
            Rectangle resRect = baseRect;

            //соотношение сторон
            float ratio = inputsize.Width / (float)inputsize.Height;

            int height = baseRect.Height;
            int width = (int)(height * ratio);

            if (width > baseRect.Width)
            {
                width = baseRect.Width;
                height = (int)(width / ratio);
            }

            var x = baseRect.X + baseRect.Width / 2 - width / 2;
            var y = baseRect.Y + baseRect.Height / 2 - height / 2;

            resRect = new Rectangle(x, y, width, height);

            return resRect;
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
        #endregion

        #region ConsolePrinting
        private static void PrintScores()
        {
            for (int i = 0; i < games.Count; ++i)
            {
                Console.WriteLine($"******************************");
                Console.WriteLine($"{DictNameTeamByInd[i]} против");

                for (int j = 0; j < games.Count; ++j)
                {
                    Console.WriteLine($"-----------------------------");
                    Console.WriteLine($"{DictNameTeamByInd[j]}");

                    var gamesBetweenTeams = games[i][j].Count;

                    if (gamesBetweenTeams == 0)
                    {
                        Console.WriteLine($"еще не играли");
                        continue;
                    }

                    for (int k = 0; k < gamesBetweenTeams; ++k)
                    {
                        var ot = "";

                        if (games[i][j][k].IsOt > 0)
                        {
                            ot = games[i][j][k].IsOt == 1 ? "OT" : "Pen";
                        }

                        Console.WriteLine($"{games[i][j][k].HomeTeamGoals} - {games[i][j][k].AwayTeamGoals} {ot}");
                    }
                }
            }
        }

        private static void PrintStandingsWithoutScores()
        {
            Console.WriteLine(String.Format("|{0,5}|{1,15}|{2,4}|{3,4}|{4,4}|{5,4}|{6,4}|{7,4}|{8,4}|{9,4}|{10,4}|",
                                            "PLACE", "TEAM", "GP", "W", "OTW", "L", "OTL", "GF", "GA", "Diff", "Pts"));
            int i = 0;
            foreach (var el in teams)
            {
                ++i;

                Console.WriteLine(String.Format("|{0,5}|{1,15}|{2,4}|{3,4}|{4,4}|{5,4}|{6,4}|{7,4}|{8,4}|{9,4}|{10,4}|",
                                           i, el.name, el.games, el.wins, el.winsOT + el.winsPen,
                                           el.loses, el.loseOT + el.losePen,
                                           el.goalsFor, el.goalsAgainst, el.diff,
                                           el.points));
            }
        }
        #endregion

        private static void SortStandings()
        {
            var tmpList = (from m in teams
                       orderby -m.points, -m.diff, m.games
                       select m).ToList();
            teams = tmpList;
        }

        #region Parsing
        private static void ReadFile(string v)
        {
            var lines = File.ReadAllLines(v).ToList();

            gamesBetween = Convert.ToInt32(lines[0]);
            pointPerDraw = Convert.ToInt32(lines[1]);
            pointPerWinInMainTime = Convert.ToInt32(lines[2]);
            pointPerWinAfterMainTime = Convert.ToInt32(lines[3]);
            pointPerLoseAfterMainTime = Convert.ToInt32(lines[4]);

            for(int i = 5; i < lines.Count; ++i)
            {
                ParseGame(lines[i]);
            }
        }

        private static void ParseGame(string v)
        {
            v = v.Replace(" ", "");
            var game = v.Split(';').ToList();

            int otOrPen = 0;

            string homeTeamName = game[0];
            string awayTeamName = game[1];

            int homeTeamGoals = Convert.ToInt32(game[2]);
            int awayTeamGoals = Convert.ToInt32(game[3]);

            if (game.Count == 5)
            {
                if (game[4].ToLower() == "от")
                    otOrPen = 1;
                
                if (game[4].ToLower() == "б")
                    otOrPen = 2;
            }

            if (homeTeamGoals > awayTeamGoals)
            {
                WinsUpd(homeTeamName, homeTeamGoals, awayTeamGoals, otOrPen);
                LoseUpd(awayTeamName, awayTeamGoals, homeTeamGoals, otOrPen);
            }

            else
            {
                LoseUpd(homeTeamName, homeTeamGoals, awayTeamGoals, otOrPen);
                WinsUpd(awayTeamName, awayTeamGoals, homeTeamGoals, otOrPen);
            }

            AddGameToMatrix(homeTeamName, awayTeamName, homeTeamGoals, awayTeamGoals, otOrPen);

        }

        #endregion

        #region UpdatingData
        private static void AddGameToMatrix(string homeTeamName, string awayTeamName, int homeTeamGoals, int awayTeamGoals, int otOrPen)
        {
            var scoreForHome = new Score(homeTeamGoals, awayTeamGoals, otOrPen);
            var scoreForAway = new Score(awayTeamGoals, homeTeamGoals, otOrPen);

            int indOfHome = DictIndTeamByName[homeTeamName];
            int indOfAway = DictIndTeamByName[awayTeamName];

            int maxInd = indOfHome > indOfAway ? indOfHome : indOfAway;

            if (games.Count < maxInd + 1)
                UpdGamesMesuare(maxInd);

            games[indOfHome][indOfAway].Add(scoreForHome);
            games[indOfAway][indOfHome].Add(scoreForAway);
        }

        private static void UpdGamesMesuare(int maxInd)
        {
            while (games.Count < maxInd + 1)
            {
                games.Add(new List<List<Score>>());

                for (int i = 0; i < games.Count; ++i)
                {
                    while (games[i].Count < maxInd + 1)
                        games[i].Add(new List<Score>());
                }
            }
        }

        private static void DictUpdate(Team team)
        {
            var full = TextHelper.FullNameFinder(team.name);

            DictFullName.Add(team.name, full);
            DictIndTeamByName.Add(team.name, currInd);
            DictNameTeamByInd.Add(currInd, team.name);

            ++currInd;
        }

        private static void LoseUpd(string awayTeamName, int awayTeamGoals, int homeTeamGoals, int otOrPen)
        {
            var team = teams.Find(x => x.name.Contains(awayTeamName));

            if (team == null)
            {
                team = new Team(awayTeamName);
                DictUpdate(team);
            } 
            else
                teams.Remove(team);

            ++team.games;

            if (otOrPen == 0)
                ++team.loses;

            if (otOrPen == 1)
            {
                ++team.loseOT;
                team.points += pointPerLoseAfterMainTime;
            }

            if (otOrPen == 2)
            {
                ++team.losePen ;
                team.points += pointPerLoseAfterMainTime;
            }

            team.goalsFor += awayTeamGoals;
            team.goalsAgainst += homeTeamGoals;

            teams.Add(team);
        }

        private static void WinsUpd(string homeTeamName, int homeTeamGoals, int awayTeamGoals, int otOrPen)
        {
            var team = teams.Find(x => x.name.Contains(homeTeamName));

            if (team == null)
            {
                team = new Team(homeTeamName);
                DictUpdate(team);
            }
            else
                teams.Remove(team);

            ++team.games;

            if (otOrPen == 0)
            {
                ++team.wins;
                team.points += pointPerWinInMainTime;
            }

            if (otOrPen == 1)
            {
                ++team.winsOT;
                team.points += pointPerWinAfterMainTime;
            }

            if (otOrPen == 2)
            {
                ++team.winsPen;
                team.points += pointPerWinAfterMainTime;
            }

            team.goalsFor += homeTeamGoals;
            team.goalsAgainst += awayTeamGoals;

            teams.Add(team);
        }
        #endregion
    }
}
