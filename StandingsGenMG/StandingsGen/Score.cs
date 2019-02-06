namespace StandingsGen
{
    internal class Score
    {
        public int HomeTeamGoals = 0;
        public int AwayTeamGoals = 0;
        public int IsOt = 0;

        public Score(int homeTeamGoals, int awayTeamGoals, int isOt)
        {
            HomeTeamGoals = homeTeamGoals;
            AwayTeamGoals = awayTeamGoals;
            IsOt = isOt;
        }
    }
}