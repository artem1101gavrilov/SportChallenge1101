using SportChallenge.DataBase;
using SportChallenge.Models.Sport;
using SportChallenge.Services;
using SportChallenge.Training;

namespace SportChallenge.Factories;

public class TrainingFactory
{
    private readonly Dictionary<SportType, SportsExerciseBase> _trainingDictionary;

    public TrainingFactory(UnitOfWork unitOfWork, NotificationService notificationController)
    {
        _trainingDictionary = new Dictionary<SportType, SportsExerciseBase>
        {
            { SportType.Squats, new Squats(unitOfWork, notificationController) },
            { SportType.PullUps, new PullUps(unitOfWork, notificationController) },
            { SportType.PushUps, new PushUps(unitOfWork, notificationController) },
            { SportType.Plank, new Plank(unitOfWork, notificationController) },
            { SportType.Crunch, new Crunch(unitOfWork, notificationController) },
            { SportType.BurpeeV1, new BurpeeV1(unitOfWork, notificationController) },
            { SportType.BurpeeV2, new BurpeeV2(unitOfWork, notificationController) },
            { SportType.Scissors, new Scissors(unitOfWork, notificationController) },
            { SportType.LegSwings, new LegSwings(unitOfWork, notificationController) },
        };
    }

    public SportsExerciseBase GetTraining(SportType sportType)
        => _trainingDictionary[sportType];
}