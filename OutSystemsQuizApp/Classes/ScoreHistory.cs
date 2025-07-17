using System.Collections.Generic;

public class ScoreHistory
{
    public Dictionary<string, (int Correct, int Total)> SectionScores { get; set; } = new();
}