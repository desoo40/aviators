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
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace StandingsGen
{
    class Program
    {
        static Settings sett = null;

        static List<Team> teams = null;

        static Dictionary<string, int> DictIndTeamByName = null;
        static Dictionary<int, string> DictNameTeamByInd = null;
        static Dictionary<string, FullName> DictFullName = null;

        static int currInd = 0;

        static List<List<List<Score>>> games = new List<List<List<Score>>>();

        static void Main(string[] args)
        {
            var file = "tournaments//РБилл2019";
            Presets(file);
            DrawingTable();

            //file = "tournaments//МСХЛ2018";
            //Presets(file);
            //DrawingTable();

            //file = "tournaments//МСХЛА2018";
            //Presets(file);
            //DrawingTable();

            //file = "tournaments//МСХЛБ2018";
            //Presets(file);
            //DrawingTable();

            //file = "tournaments//СХЛЗ2018";
            //Presets(file);
            //DrawingTable();

            //file = "tournaments//СХЛВ2018";
            //Presets(file);
            //DrawingTable();
        }

        private static void Presets(string file)
        {
            sett = new Settings();
            teams = new List<Team>();
            DictIndTeamByName = new Dictionary<string, int>();
            DictNameTeamByInd = new Dictionary<int, string>();
            DictFullName = new Dictionary<string, FullName>();


            ReadFile(file);

            SortStandings();
            PrintStandingsWithoutScores();
        }

        private static void DrawingTable()
        {
            var marginLower = 80;
            var marginUpper = 160;
            var marginSides = 20;

            var heightHat = 60;
            var heightTableRow = 100;

            var widthLogo = sett.PartLogo ? 100 : 0;
            var widthTeamName = sett.PartNames ? 230 : 0;

            var widthPartLogoNames = widthLogo + widthTeamName;

            var widthScore = CalcWidhtScore();
            //var widthPucksDiff = sett.PartNames ? 150 : 0;

            var teamsCnt = teams.Count;

            var widhtPartScore = widthScore * teamsCnt;
            var widthPartIndicators = CalcIndicWidth(); // + widthPucksDiff - widthIndicator

            var widthBitmap = 2 * marginSides + widthPartLogoNames + widhtPartScore + widthPartIndicators;
            var heightBitmap = marginUpper + marginLower + heightHat + heightTableRow * teamsCnt;

            Image bitmap = new Bitmap(widthBitmap, heightBitmap);

            Image imgLayer = Image.FromFile("images//layer.png");
            Image imgWinter = Image.FromFile("images//winter.png");
            Image imgBorn  = Image.FromFile("images//#borntofly.png");

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                #region EarlyBackground

                int occurLayerBckgrSize = 10;

                var rectBckgrLayer = new Rectangle(-occurLayerBckgrSize / 2, -occurLayerBckgrSize / 2,
                    widthBitmap + occurLayerBckgrSize,
                    heightBitmap + occurLayerBckgrSize);

                var sizeBckgrLayer = new Size(widthBitmap + occurLayerBckgrSize,
                    heightBitmap + occurLayerBckgrSize);

                var inscBckrng = GetInscribed(rectBckgrLayer, sizeBckgrLayer);


                var colorTableBckgr = ColorTranslator.FromHtml("#e0e6ea");
                var colorLines = ColorTranslator.FromHtml("#00337f");

                var brushTableBckrg = new SolidBrush(colorTableBckgr);
                var brushLines = new SolidBrush(colorLines);

                var occurTableBckg = 2;
                var weightTableBckg = widthBitmap + 2 * occurTableBckg;
                var heightTableBckg = heightBitmap - marginUpper - marginLower;
                var rectTableBckgr = new Rectangle(-occurTableBckg, marginUpper, weightTableBckg, heightTableBckg);

                g.DrawImage(imgLayer, inscBckrng);
                g.FillRectangle(brushTableBckrg, rectTableBckgr);
                g.DrawImage(imgWinter, inscBckrng);

                #endregion

                #region Drawing Hat + "UnderHat"

                #region Hat

                #region Liga + Logo

                var fileNameLiga = $"ligas//{sett.NameLiga}_logo.png";

                Image imgLiga = null;

                if (!File.Exists(fileNameLiga))
                    imgLiga = Image.FromFile("ligas//no_logo.png");
                else
                    imgLiga = Image.FromFile(fileNameLiga);

                var marginSideLigaLogo = marginSides + 45;
                var marginUpDownLigaLogo = 20;
                var sizeLigaLogo = 120;

                var rectLiga = new Rectangle(marginSideLigaLogo, marginUpDownLigaLogo, sizeLigaLogo, sizeLigaLogo);

                var inscLigaLogo = GetInscribed(rectLiga, imgLiga.Size);

                g.DrawImage(imgLiga, inscLigaLogo);

                //var imgAvi = Image.FromFile("teams//маи.png");
                var imgAvi = Image.FromFile(ForWho());
                var xLogoAvi = widthBitmap - marginSideLigaLogo - sizeLigaLogo;
                var yLogoAvi = marginUpDownLigaLogo;

                var rectLogoAvi = new Rectangle(xLogoAvi, yLogoAvi, sizeLigaLogo, sizeLigaLogo);

                var inscLogoAvi = GetInscribed(rectLogoAvi, imgAvi.Size);

                g.DrawImage(imgAvi, inscLogoAvi);

                #endregion

                #region TableName

                var marginFromLogo = 20;

                var xTableName = marginSideLigaLogo + sizeLigaLogo + marginFromLogo;
                var yTableName = marginUpDownLigaLogo * 2;
                var widthTableName = widthBitmap - 2 * xTableName;
                var heightTableName = sizeLigaLogo - marginUpDownLigaLogo;

                var rectTableName = new Rectangle(xTableName, yTableName, widthTableName, heightTableName);

                var brushWhite = new LinearGradientBrush(new Point(0, rectTableName.Y),
                    new Point(0, rectTableName.Y + rectTableName.Height / 2 - 5),
                    ColorTranslator.FromHtml("#dedede"),
                    ColorTranslator.FromHtml("#ffffff"));

                var fileFontTableName = "fonts//FiraSans-SemiBold.ttf";
                var sizeFontTableName = 80;
                var fontTableName = CreateFont(fileFontTableName, sizeFontTableName);

                var fmt = new Format();
                var centerFormat = fmt.centerFormat;

                g.DrawString(sett.NameTable, fontTableName, brushWhite, rectTableName, centerFormat);

                #endregion

                #endregion

                #region UnderHat

                var occurSocialPosition = 15;

                var rectInscSocial = new Rectangle(widthBitmap / 2 - imgBorn.Width / 2,
                    heightBitmap - marginLower + occurSocialPosition,
                    imgBorn.Width, imgBorn.Height);

                g.DrawImage(imgBorn, rectInscSocial);

                #endregion

                #endregion

                #region Table Hat

                var fileFontHat = "fonts//FiraSans-ExtraBold.ttf";
                var sizeFontHat = 33;

                var fontHat = CreateFont(fileFontHat, sizeFontHat);

                var brushBlue = new LinearGradientBrush(new Point(0, marginUpper),
                    new Point(0, marginUpper + heightHat / 2),
                    ColorTranslator.FromHtml("#00337f"),
                    ColorTranslator.FromHtml("#0041a1"));

                var marginHatTable = 5;

                var xHatTAbleIndic = marginSides + widthPartLogoNames + widhtPartScore;
                var yHatTable = marginUpper + marginHatTable;

                if (sett.PartScores)
                {
                    var xHatTableScores = marginSides + widthPartLogoNames;

                    for (int i = 0; i < teamsCnt; ++i)
                    {
                        var rectHatTableScores = new Rectangle(xHatTableScores + i * widthScore, yHatTable, widthScore,
                            heightHat);
                        g.DrawString((i + 1).ToString(), fontHat, brushBlue, rectHatTableScores, centerFormat);

                    }
                }

                for (int i = 0; i < sett.ListIndicators.Count; i++)
                {

                    var nameInd = sett.ListIndicators[i].Name;
                    var widthCurrInd = sett.ListIndicators[i].Widht;
                    var widthPredInd = i > 0 ? sett.ListIndicators[i - 1].Widht : 0;

                    xHatTAbleIndic += widthPredInd;


                    var rectHatTableIndic = new Rectangle(xHatTAbleIndic, yHatTable, widthCurrInd, heightHat);
                    //g.DrawRectangle(Pens.Red, rectHatTableIndic);
                    g.DrawString(nameInd, fontHat, brushBlue, rectHatTableIndic, centerFormat);

                }


                #endregion

                #region Logos

                if (sett.PartLogo)
                {
                    for (int i = 0; i < teams.Count; i++)
                    {
                        var occurX = 0;
                        var occurY = 7;

                        var xLogos = marginSides + occurX;
                        var yLogos = marginUpper + heightHat + occurY;

                        var sizeOfLogo = 90;

                        var name = teams[i].name;

                        var fileNameTeam = $"teams//{name}.png";

                        Image imgLogo = null;

                        if (!File.Exists(fileNameTeam))
                            imgLogo = Image.FromFile("teams//nologo.png");
                        else
                            imgLogo = Image.FromFile(fileNameTeam);

                        var rectLogo = new Rectangle(xLogos, yLogos + i * heightTableRow, sizeOfLogo,
                            sizeOfLogo);

                        var inscLogo = GetInscribed(rectLogo, imgLogo.Size);

                        g.DrawImage(imgLogo, inscLogo);
                    }
                }

                #endregion

                #region Names

                if (sett.PartNames)
                {
                    var fileFontNames = "fonts//FiraSans-Regular.ttf";
                    var fileFontNamesForAvi = "fonts//FiraSans-Medium.ttf";
                    var sizeFontNames = 25;
                    var sizeFontInst = sizeFontNames * 0.9;



                    var marginUpTeamName = 13;
                    var marginUpTeamAloneName = 30;
                    var xTeamNames = sett.PartLogo ? marginSides + widthLogo : 0;
                    var yTeamName = marginUpper + heightHat + marginUpTeamName;
                    var widthTN = sett.PartLogo ? widthTeamName : marginSides + widthTeamName;
                    var heightTN = 50;

                    var distTeamInst = 37;
                    var yTeamInst = yTeamName + distTeamInst;

                    var someShitThatMakesMeHappy = 45;

                    for (int i = 0; i < teamsCnt; i++)
                    {
                        var full = DictFullName[teams[i].name];

                        if (full == null)
                            full = new FullName(teams[i].name, "");

                        if (full.nameInst == "")
                            yTeamName = marginUpper + heightHat + marginUpTeamAloneName;



                        var rectTeamName = new Rectangle(xTeamNames, yTeamName + i * heightTableRow, widthTN, heightTN);
                        var rectTeamInst = new Rectangle(xTeamNames, yTeamInst + i * heightTableRow, widthTN, heightTN);

                        var ptBrName11 = new Point(0, rectTableName.Y);
                        var ptBrName12 = new Point(0, rectTableName.Y + heightTN / 2);

                        var ptBrName21 = new Point(0, rectTeamInst.Y);
                        var ptBrName22 = new Point(0, rectTeamInst.Y + heightTN / 2);


                        var brushName1 = new LinearGradientBrush(ptBrName11, ptBrName12,
                            ColorTranslator.FromHtml("#00337f"),
                            ColorTranslator.FromHtml("#0041a1"));

                        var brushName2 = new LinearGradientBrush(ptBrName21, ptBrName22,
                            ColorTranslator.FromHtml("#00337f"),
                            ColorTranslator.FromHtml("#0041a1"));

                        Font fontTeamName = null;
                        Font fontTeamInst = null;

                        if (full.nameTeam.Contains("виаторы"))
                            fontTeamName = CreateFont(fileFontNamesForAvi, CalcFontSize(full.nameTeam.Length, widthTN + someShitThatMakesMeHappy, sizeFontNames));
                        else
                            fontTeamName = CreateFont(fileFontNames, CalcFontSize(full.nameTeam.Length, widthTN + someShitThatMakesMeHappy, sizeFontNames));

                        g.DrawString(full.nameTeam, fontTeamName, brushName1, rectTeamName, centerFormat);

                        if (full.nameTeam.Contains("виаторы"))
                            fontTeamInst = CreateFont(fileFontNamesForAvi, CalcFontSize(full.nameInst.Length, widthTN + someShitThatMakesMeHappy, (int)sizeFontInst));
                        else
                            fontTeamInst = CreateFont(fileFontNames, CalcFontSize(full.nameInst.Length, widthTN + someShitThatMakesMeHappy, (int)sizeFontInst));

                        g.DrawString(full.nameInst, fontTeamInst, brushName2, rectTeamInst, centerFormat);

                        yTeamName = marginUpper + heightHat + marginUpTeamName;
                    }
                }

                

                #endregion

                #region Lines

                #region HorizontalMain

                int marginHorizontalLines = 4;
                int widthLines = 4;
                var widthInTableLine = 3;

                for (int i = 0; i < teamsCnt + 1; ++i)
                {
                    //g.DrawRectangle(new Pen(rectCol, 1), 4, 220 + i * 100, 1592, 4);
                    g.FillRectangle(brushLines, marginHorizontalLines,
                        marginUpper + heightHat + i * heightTableRow,
                        widthBitmap - 2 * marginHorizontalLines, widthLines);
                }

                #endregion

                #region BetweenLogoName

                

                #endregion

                #region VerticMain
                if (sett.PartNames || sett.PartLogo)
                {
                    var occurVerticLine = 1;

                    var xVerticLine = marginSides + widthPartLogoNames;
                    var yVerticLine = marginUpper + heightHat + occurVerticLine;
                    var heigtVerticLine = heightBitmap - marginUpper - marginLower - heightHat;

                    var rectVerticLine = new Rectangle(xVerticLine, yVerticLine, widthLines, heigtVerticLine);

                    g.FillRectangle(brushLines, rectVerticLine);
                }

                if (sett.PartScores)
                {
                    var occurVerticLine = 1;

                    var xVerticLine = marginSides + widthPartLogoNames;
                    var yVerticLine = marginUpper + heightHat + occurVerticLine;
                    var heigtVerticLine = heightBitmap - marginUpper - marginLower - heightHat;

                    var rectVerticLine = new Rectangle(xVerticLine + widhtPartScore, yVerticLine, widthLines, heigtVerticLine);

                    g.FillRectangle(brushLines, rectVerticLine);
                }

                

                #endregion

                #region InScore
                
                for (int i = 0; i < teamsCnt ; ++i)
                {
                    var marginVerticLineInTable = 10;
                    var yVerticsInTable = marginUpper + heightHat + marginVerticLineInTable;
                    var xVerticsInTable = marginSides + widthPartLogoNames + widhtPartScore;

                    for (int k = 0; k + 1 < sett.ListIndicators.Count; ++k)
                    {
                        var widthCurrInd = sett.ListIndicators[k].Widht;


                        xVerticsInTable += widthCurrInd;

                        var rectVerticsInTable = new Rectangle(xVerticsInTable, yVerticsInTable + i * heightTableRow,
                            widthInTableLine, heightTableRow - 2 * marginVerticLineInTable);

                        g.FillRectangle(brushLines, rectVerticsInTable);
                    }

                    if (sett.PartLogo && sett.PartNames)
                    {
                        if (sett.PartLogo && sett.PartNames)
                        {
                            var xTmp = marginSides + widthLogo;

                            var rectVerticsInTable = new Rectangle(xTmp, yVerticsInTable + i * heightTableRow,
                                widthInTableLine, heightTableRow - 2 * marginVerticLineInTable);

                            g.FillRectangle(brushLines, rectVerticsInTable);
                        }
                    }

                    if (sett.PartScores)
                    {

                        bool inSett = false;

                        for (int j = 0; j + 1 < teamsCnt; ++j)
                        {
                            if (!sett.PartNames && !inSett)
                            {
                                j = -1;
                                inSett = true;
                            }

                            xVerticsInTable = marginSides + widthPartLogoNames + widthScore;


                            var rectVerticsInTable = new Rectangle(xVerticsInTable + j * widthScore,
                                yVerticsInTable + i * heightTableRow,
                                widthInTableLine, heightTableRow - 2 * marginVerticLineInTable);


                            g.FillRectangle(brushLines, rectVerticsInTable);

                        }

                        for (int j = 0; j < teamsCnt; ++j)
                        {
                            if (i == j)
                                continue;

                            if (sett.GamesBetween > 1)
                            {
                                var marginSideInScoreBox = 27;
                                var marginUpInScoreBox = 50;
                                var occurScoreOverTwo = 1;

                                var xHorLineInBox =
                                    occurScoreOverTwo + marginSides + widthPartLogoNames + marginSideInScoreBox;
                                var yHorLineInBox = marginUpper + heightHat + marginUpInScoreBox;
                                var widthHorLineInBox = widthScore / 2;
                                var heightHorLineInBox = 2;


                                if (sett.GamesBetween == 2)
                                {
                                    var rectHorLineInBox = new Rectangle(xHorLineInBox + j * widthScore,
                                        yHorLineInBox + i * heightTableRow, widthHorLineInBox, heightHorLineInBox);

                                    g.FillRectangle(brushLines, rectHorLineInBox);
                                }


                                if (sett.GamesBetween == 4)
                                {
                                    widthHorLineInBox = widthScore / 3;

                                    var rectHorLineInBox = new Rectangle(xHorLineInBox + j * widthScore,
                                        yHorLineInBox + i * heightTableRow, widthHorLineInBox, heightHorLineInBox);

                                    g.FillRectangle(brushLines, rectHorLineInBox);

                                    xHorLineInBox = xHorLineInBox + widthScore - widthHorLineInBox -
                                                    2 * marginSideInScoreBox;

                                    rectHorLineInBox = new Rectangle(xHorLineInBox + j * widthScore,
                                        yHorLineInBox + i * heightTableRow, widthHorLineInBox, heightHorLineInBox);

                                    g.FillRectangle(brushLines, rectHorLineInBox);

                                    var marginUpVerInScoreBox = 10;
                                    var xVerLineInBox = marginSides + widthPartLogoNames + widthScore / 2;
                                    var yVerLineInBox =
                                        occurScoreOverTwo + marginUpper + heightHat + marginUpVerInScoreBox;
                                    var heightVerLineInBox = heightTableRow / 3;
                                    var widhtVerLineInBox = heightHorLineInBox;

                                    var rectVerLineInBox = new Rectangle(xVerLineInBox + j * widthScore,
                                        yVerLineInBox + i * heightTableRow, widhtVerLineInBox, heightVerLineInBox);

                                    g.FillRectangle(brushLines, rectVerLineInBox);

                                    yVerLineInBox = yVerLineInBox + heightTableRow - heightVerLineInBox -
                                                    2 * marginUpVerInScoreBox;

                                    rectVerLineInBox = new Rectangle(xVerLineInBox + j * widthScore,
                                        yVerLineInBox + i * heightTableRow, widhtVerLineInBox, heightVerLineInBox);

                                    g.FillRectangle(brushLines, rectVerLineInBox);

                                }
                            }
                        }
                    }

                }

                #endregion

                #endregion

                #region Logos

                if (sett.PartScores)
                {
                    for (int i = 0; i < teamsCnt; ++i)
                    {
                        var name = teams[i].name;

                        var fileNameTeam = $"teams//{name}.png";

                        Image imgLogo = null;

                        if (!File.Exists(fileNameTeam))
                            imgLogo = Image.FromFile("teams//nologo.png");
                        else
                            imgLogo = Image.FromFile(fileNameTeam);

                        var marginInBoxForLogo = 7;
                        var occurX = 5;
                        var occurY = 2;
                        var sizeOfLogo = 85;

                        var xLogo = marginSides + widthPartLogoNames + marginInBoxForLogo + widthScore / 2 - sizeOfLogo / 2 -
                                    occurX;
                        var yLogo = marginUpper + heightHat + marginInBoxForLogo + occurY;

                        var rectLogo = new Rectangle(xLogo + i * widthScore, yLogo + i * heightTableRow, sizeOfLogo,
                            sizeOfLogo);

                        var inscLogo = GetInscribed(rectLogo, imgLogo.Size);

                        g.DrawImage(imgLogo, inscLogo);
                        //g.DrawString(name, f, br, sidesMargin + teamNameWidht + inBoxMarginForLogo + i * 100, upperMagrin + hatHight + inBoxMarginForLogo + i * 100);

                    }
                }

                #endregion

                #region Scores

                if (sett.PartScores)
                {
                    var fontWin = "fonts//FiraSans-Medium.ttf";
                    var fontOt = "fonts//FiraSans-Medium.ttf";
                    var fontLose = "fonts//FiraSans-Light.ttf";
                    var sizeFontScore = 35;
                    var sizeFontOT = sizeFontScore / 2.5f;



                    for (int i = 0; i < teamsCnt; ++i)
                    {
                        for (int j = 0; j < teamsCnt; ++j)
                        {
                            if (i == j)
                                continue;

                            var indSide = DictIndTeamByName[teams[i].name];
                            var indUp = DictIndTeamByName[teams[j].name];

                            var workList = games[indSide][indUp];

                            if (workList.Count == 0)
                                continue;


                            if (sett.GamesBetween == 1)
                            {
                                var occurStr = 2;
                                Font fontScore = null;
                                var workGame = workList[0];

                                if (workGame.HomeTeamGoals > workGame.AwayTeamGoals)
                                    fontScore = CreateFont(fontWin, sizeFontScore);
                                else
                                    fontScore = CreateFont(fontLose, sizeFontScore);

                                var heightStrScore = 40;
                                var marginUpScore = heightTableRow / 2 - heightStrScore / 2;
                                var marginSidesScore = 5;


                                var xScore = marginSides + widthPartLogoNames + occurStr - marginSidesScore;
                                var yScore = marginUpper + heightHat + marginUpScore + occurStr;
                                var widthStrScore = widthScore + 2 * marginSidesScore;

                                var rectScore = new Rectangle(xScore + j * widthScore, yScore + i * heightTableRow,
                                    widthStrScore, heightStrScore);

                                //var rectScore1 = new Rectangle(xScore + j * widthScore, yScore + i * heightTableRow,
                                //    widthStrScore, heightStrScore);

                                //g.DrawRectangle(new Pen(Color.Red), rectScore);

                                var strScore = $"{workGame.HomeTeamGoals}:{workGame.AwayTeamGoals}";

                                g.DrawString(strScore, fontScore, brushBlue, rectScore, centerFormat);

                                if (workGame.IsOt > 0)
                                {
                                    var marginSideOt = widthScore / 4;
                                    var xOt = xScore + widthScore - marginSideOt;
                                    var yOt = yScore + heightStrScore * 3 / 4;
                                    var widthOt = widthScore / 4;
                                    var heightOt = heightStrScore / 2;

                                    var fontOtPen = CreateFont(fontOt, sizeFontOT);

                                    var rectOt = new Rectangle(xOt + j * widthScore, yOt + i * heightTableRow,
                                        widthOt, heightOt);

                                    //g.DrawRectangle(new Pen(Color.Red), rectOt);
                                    var strPorOt = workGame.IsOt == 1 ? "ОТ" : "Б";
                                    g.DrawString(strPorOt, fontOtPen, brushBlue, rectOt, centerFormat);
                                }
                            }

                            if (sett.GamesBetween == 2)
                            {
                                var occurStrTwo = 4;
                                var sizeFontScoreTwo = 30;
                                var sizeFontOTTwo = sizeFontScoreTwo / 2.5f;
                                Font fontScore = null;

                                for (int k = 0; k < workList.Count; ++k)
                                {
                                    var workGame = workList[k];

                                    if (workGame.HomeTeamGoals > workGame.AwayTeamGoals)
                                        fontScore = CreateFont(fontWin, sizeFontScoreTwo);
                                    else
                                        fontScore = CreateFont(fontLose, sizeFontScoreTwo);

                                    var heightStrScore = 30;
                                    var marginUpScore = heightTableRow / 4 - heightStrScore / 2;
                                    var marginSidesScore = 5;


                                    var xScore = marginSides + widthPartLogoNames + occurStrTwo / 2 - marginSidesScore;
                                    var yScore = marginUpper + heightHat + marginUpScore + occurStrTwo;
                                    var widthStrScore = widthScore + 2 * marginSidesScore;

                                    var addTwo = k * 3 * heightStrScore / 2;

                                    var rectScore = new Rectangle(xScore + j * widthScore,
                                        yScore + i * heightTableRow + addTwo,
                                        widthStrScore, heightStrScore + 2 * occurStrTwo);

                                    //var rectScore1 = new Rectangle(xScore + j * widthScore, yScore + i * heightTableRow,
                                    //    widthStrScore, heightStrScore);

                                    //g.DrawRectangle(new Pen(Color.Red), rectScore);

                                    var strScore = $"{workGame.HomeTeamGoals}:{workGame.AwayTeamGoals}";

                                    g.DrawString(strScore, fontScore, brushBlue, rectScore, centerFormat);

                                    if (workGame.IsOt > 0)
                                    {
                                        var marginSideOt = widthScore / 4;
                                        var xOt = xScore + widthScore - marginSideOt;
                                        var yOt = yScore + heightStrScore * 3 / 4;
                                        var widthOt = widthScore / 4;
                                        var heightOt = heightStrScore / 2;

                                        var fontOtPen = CreateFont(fontOt, sizeFontOTTwo);

                                        var rectOt = new Rectangle(xOt + j * widthScore,
                                            yOt + i * heightTableRow + addTwo,
                                            widthOt, heightOt);

                                        //g.DrawRectangle(new Pen(Color.Red), rectOt);
                                        var strPorOt = workGame.IsOt == 1 ? "ОТ" : "Б";
                                        g.DrawString(strPorOt, fontOtPen, brushBlue, rectOt, centerFormat);
                                    }
                                }

                            }


                            if (sett.GamesBetween == 4)
                            {
                                var occurStrTwo = 4;
                                var sizeFontScoreFour = 30;
                                var sizeFontOTFour = sizeFontScoreFour / 2.5f;
                                Font fontScore = null;

                                var sum = 0;

                                for (int k = 0; sum < workList.Count; ++k)
                                {
                                    for (int l = 0; l < 2 && sum < workList.Count; l++)
                                    {
                                        var workGame = workList[sum];

                                        if (workGame.HomeTeamGoals > workGame.AwayTeamGoals)
                                            fontScore = CreateFont(fontWin, sizeFontScoreFour);
                                        else
                                            fontScore = CreateFont(fontLose, sizeFontScoreFour);

                                        var heightStrScore = 30;
                                        var marginUpScore = heightTableRow / 4 - heightStrScore / 2;
                                        var marginSidesScore = 5;


                                        var xScore = marginSides + widthPartLogoNames + occurStrTwo / 2 - marginSidesScore;
                                        var yScore = marginUpper + heightHat + marginUpScore + occurStrTwo;
                                        var widthStrScore = widthScore / 2 + 2 * marginSidesScore;

                                        var xAddFour = l * (widthStrScore - 2 * marginSidesScore - occurStrTwo / 2);
                                        var yAddFour = k * 3 * heightStrScore / 2;

                                        var rectScore = new Rectangle(xScore + j * widthScore + xAddFour,
                                            yScore + i * heightTableRow + yAddFour,
                                            widthStrScore, heightStrScore + 2 * occurStrTwo);

                                        //var rectScore1 = new Rectangle(xScore + j * widthScore, yScore + i * heightTableRow,
                                        //    widthStrScore, heightStrScore);

                                        //g.DrawRectangle(new Pen(Color.Red), rectScore);

                                        var strScore = $"{workGame.HomeTeamGoals}:{workGame.AwayTeamGoals}";

                                        g.DrawString(strScore, fontScore, brushBlue, rectScore, centerFormat);

                                        if (workGame.IsOt > 0)
                                        {
                                            var marginSideOt = widthScore / 9;
                                            var xOt = xScore + widthScore / 2 - marginSideOt;
                                            var yOt = yScore + heightStrScore * 3 / 4 - 2;
                                            var widthOt = widthScore / 4;
                                            var heightOt = heightStrScore / 2;

                                            var fontOtPen = CreateFont(fontOt, sizeFontOTFour);

                                            var rectOt = new Rectangle(xOt + j * widthScore + xAddFour,
                                                yOt + i * heightTableRow + yAddFour,
                                                widthOt, heightOt);

                                            //g.DrawRectangle(new Pen(Color.Red), rectOt);
                                            var strPorOt = workGame.IsOt == 1 ? "ОТ" : "Б";
                                            g.DrawString(strPorOt, fontOtPen, brushBlue, rectOt, fmt.leftFormat);
                                        }

                                        ++sum;
                                    }
                                }
                            }


                        }
                    }
                }

                #endregion

                #region Indicators

                var fontIndic = "fonts//FiraSans-Bold.ttf";
                var sizeFontIndic = 35;

                for (int i = 0; i < teamsCnt; ++i)
                {
                    var occurStr = 2;

                    var heightStrScore = 40;
                    var marginUpScore = heightTableRow / 2 - heightStrScore / 2;
                    var marginSidesScore = 5;

                    var xIndic = marginSides + widthPartLogoNames + widhtPartScore + 3*occurStr - marginSidesScore;
                    var yIndic = marginUpper + heightHat + marginUpScore + occurStr;


                    for (int k = 0; k < sett.ListIndicators.Count; ++k)
                    {
                        var widthCurrInd = sett.ListIndicators[k].Widht;
                        var widthPredInd = k > 0 ? sett.ListIndicators[k - 1].Widht : 0;

                        xIndic += widthPredInd;

                        var rectIndic = new Rectangle(xIndic, yIndic + i * heightTableRow,
                            widthCurrInd, heightStrScore);

                        var fontInd = CreateFont(fontIndic, sizeFontIndic);
                        var strIndic = CreateIndicStr(sett.ListIndicators[k].Name, i);

                        g.DrawString(strIndic, fontInd, brushBlue, rectIndic, centerFormat);
                    }
                }

                #endregion

                }



            var file = $"{sett.NameTable.Replace("|", "")}.png";

            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 100L);
            myEncoderParameters.Param[0] = myEncoderParameter;

            ImageCodecInfo pngEncoder = GetEncoder(ImageFormat.Png);

            bitmap.Save(file, pngEncoder, myEncoderParameters);
            Console.WriteLine("Сохранил!\n");
        }

        private static string ForWho()
        {
            foreach (var t in teams)
            {
                if (t.name == "2010")
                    return "teams//2010.png";

                if (t.name == "маи")
                    return "teams//маи.png";

                if(sett.NameLiga == "схл")
                    return "teams//маи.png";
            }

            return "teams//2010.png";
        }

        private static string CreateIndicStr(string name, int i)
        {
            var team = teams[i];
           
            if (name == "И")
            {
                return team.games.ToString();
            }
            if (name == "В")
            {
                return team.wins.ToString();
            }
            if (name == "ВО")
            {
                return team.winsOT.ToString();
            }
            if (name == "ВБ")
            {
                return team.winsPen.ToString();
            }
            if (name == "ПБ")
            {
                return team.losePen.ToString();
            }
            if (name == "ПО")
            {
                return team.loseOT.ToString();
            }
            if (name == "П")
            {
                return team.loses.ToString();
            }
            if (name == "Ш")
            {
                return $"{team.goalsFor}-{team.goalsAgainst}";
            }
            if (name == "ГЗ")
            {
                return team.goalsFor.ToString();
            }
            if (name == "ГП")
            {
                return team.goalsAgainst.ToString();
            }
            if (name == "+/-")
            {
                return (team.goalsFor - team.goalsAgainst).ToString();
            }
            if (name == "О")
            {
                return team.points.ToString();
            }

            return "";
        }

        private static int CalcIndicWidth()
        {
            int width = 0;

            foreach (var el in sett.ListIndicators)
            {
                width += el.Widht;
            }

            return width;
        }

        //private static int CalcReglamentSett(int ind, int pd)
        //{
        //    int w = ind * sett.truesCnt;

        //    if (sett.PartDiff)
        //        w += pd - ind;

        //    return w;
        //}


        private static float CalcFontSize(int nameTeamLength, int widthTn, float sizeFontNames)
        {
            //var sf = (float) sizeFontNames;

            while (sizeFontNames * nameTeamLength > widthTn)
                sizeFontNames -= 0.1f;

            return sizeFontNames;
        }

        private static Font CreateFont(string f, float size)
        {
            PrivateFontCollection fontColl = new PrivateFontCollection();

            fontColl.AddFontFile(f);

            return new Font(fontColl.Families[0], size);
        }

        private static int CalcWidhtScore()
        {
            if (!sett.PartScores)
                return 0;

            return sett.GamesBetween > 2 ? 200 : 100;
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

            int i = sett.FillSettings(lines);

            for (++i; i < lines.Count; ++i)
                ParseGame(lines[i]);
        }

        private static void ParseGame(string v)
        {
            v = v.Replace(" ", "");
            var game = v.Split(';').ToList();

            int otOrPen = 0;

            string homeTeamName = game[0].ToLower();
            string awayTeamName = game[1].ToLower();

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
                team.points += sett.PointsPerLoseOt;
            }

            if (otOrPen == 2)
            {
                ++team.losePen ;
                team.points += sett.PointsPerLoseOt;
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
                team.points += sett.PointsPerWin;
            }

            if (otOrPen == 1)
            {
                ++team.winsOT;
                team.points += sett.PointsPerWinOt;
            }

            if (otOrPen == 2)
            {
                ++team.winsPen;
                team.points += sett.PointsPerWinOt;
            }

            team.goalsFor += homeTeamGoals;
            team.goalsAgainst += awayTeamGoals;

            teams.Add(team);
        }
        #endregion
    }
}
