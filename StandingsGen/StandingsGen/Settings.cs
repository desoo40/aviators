using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StandingsGen
{
    class Settings
    {
        public int GamesBetween = 0;
        public int PointsPerWin = 0;
        public int PointsPerDraw = 0;
        public int PointsPerWinOt = 0;
        public int PointsPerLoseOt = 0;
        public string NameLiga = "";
        public string NameTable= "";

        public bool PartLogo = false;
        public bool PartNames = false;
        public bool PartScores = false;
        public bool PartG = false;
        public bool PartW = false;
        public bool PartWO = false;
        public bool PartWP = false;
        public bool PartLP = false;
        public bool PartLO = false;
        public bool PartL = false;
        public bool PartDiff = false;
        public bool PartGF = false;
        public bool PartGA = false;
        public bool PartDiffPM = false;
        public bool PartP = false;

        public List<Indicator> ListIndicators = new List<Indicator>();

        public int truesCnt = 0;

        public int FillSettings(List<string> lines)
        {
            int i = 0;
            int widhtStandartIndic = 80;

            for (; lines[i].ToLower() != "games"; ++i)
            {
                if (lines[i].ToLower() == "gamesbetween")
                    GamesBetween = Convert.ToInt32(lines[++i]);

                if (lines[i].ToLower() == "draw")
                    PointsPerDraw = Convert.ToInt32(lines[++i]);

                if (lines[i].ToLower() == "win")
                    PointsPerWin = Convert.ToInt32(lines[++i]);

                if (lines[i].ToLower() == "winot")
                    PointsPerWinOt = Convert.ToInt32(lines[++i]);

                if (lines[i].ToLower() == "loseot")
                    PointsPerLoseOt = Convert.ToInt32(lines[++i]);

                if (lines[i].ToLower() == "liga")
                    NameLiga = lines[++i];

                if (lines[i].ToLower() == "tablename")
                    NameTable = lines[++i];

                if (lines[i].ToLower() == "logo" && lines[++i] == "+")
                    PartLogo = true;

                if (lines[i].ToLower() == "names" && lines[++i] == "+")
                    PartNames = true;

                if (lines[i].ToLower() == "scores" && lines[++i] == "+")
                    PartScores = true;

                if (lines[i].ToLower() == "g" && lines[++i] == "+")
                {
                    PartG = true;
                    ++truesCnt;
                    ListIndicators.Add(new Indicator("И", widhtStandartIndic));
                }

                if (lines[i].ToLower() == "w" && lines[++i] == "+")
                {
                    PartW = true;
                    ++truesCnt;
                    ListIndicators.Add(new Indicator("В", widhtStandartIndic));
                }

                if (lines[i].ToLower() == "wo" && lines[++i] == "+")
                {
                    PartWO = true;
                    ++truesCnt;
                    ListIndicators.Add(new Indicator("ВО", widhtStandartIndic));
                }

                if (lines[i].ToLower() == "wp" && lines[++i] == "+")
                {
                    PartWP = true;
                    ++truesCnt;
                    ListIndicators.Add(new Indicator("ВБ", widhtStandartIndic));
                }

                if (lines[i].ToLower() == "lp" && lines[++i] == "+")
                {
                    PartLP = true;
                    ++truesCnt;
                    ListIndicators.Add(new Indicator("ПБ", widhtStandartIndic));
                }

                if (lines[i].ToLower() == "lo" && lines[++i] == "+")
                {
                    PartLO = true;
                    ++truesCnt;
                    ListIndicators.Add(new Indicator("ПО", widhtStandartIndic));
                }

                if (lines[i].ToLower() == "l" && lines[++i] == "+")
                {
                    PartL = true;
                    ++truesCnt;
                    ListIndicators.Add(new Indicator("П", widhtStandartIndic));
                }

                if (lines[i].ToLower() == "diff" && lines[++i] == "+")
                {
                    PartDiff = true;
                    ++truesCnt;
                    ListIndicators.Add(new Indicator("Ш", 150));
                }

                if (lines[i].ToLower() == "gf" && lines[++i] == "+")
                {
                    PartGF = true;
                    ++truesCnt;
                    ListIndicators.Add(new Indicator("ГЗ", widhtStandartIndic));
                }

                if (lines[i].ToLower() == "ga" && lines[++i] == "+")
                {
                    PartGA = true;
                    ++truesCnt;
                    ListIndicators.Add(new Indicator("ГП", widhtStandartIndic));
                }

                if (lines[i].ToLower() == "diffpm" && lines[++i] == "+")
                {
                    PartDiffPM = true;
                    ++truesCnt;
                    ListIndicators.Add(new Indicator("Р", widhtStandartIndic));
                }

                if (lines[i].ToLower() == "p" && lines[++i] == "+")
                {
                    PartP = true;
                    ++truesCnt;
                    ListIndicators.Add(new Indicator("О", widhtStandartIndic));
                }

            }

            return i;
        }
    }
}
