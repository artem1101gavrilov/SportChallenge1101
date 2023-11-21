namespace SportChallenge.Models.Sport;

public class ActiveTrainingDto
{
    public ActiveTrainingDto(long id, SportType sportType, int level, int count)
    {
        Id = id;
        SportType = sportType;
        Level = level;
        Count = count;
    }

    public long Id { get; set; }
    public SportType SportType { get; set; }
    public int Level { get; set; }
    public int Count { get; set; }
}