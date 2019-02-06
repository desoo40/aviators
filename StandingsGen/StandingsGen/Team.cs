namespace StandingsGen
{
    internal class Team
    {
        public string name = "";

        public int games = 0;
        public int wins = 0;
        public int winsOT = 0;
        public int winsPen = 0;
        public int loses = 0;
        public int loseOT = 0;
        public int losePen = 0;
        public int points = 0;
        public int goalsFor = 0;
        public int goalsAgainst = 0;
        public int diff => goalsFor - goalsAgainst;

        //fucking comment

        public Team(string s)
        {
            name = s;
        }

    }
}