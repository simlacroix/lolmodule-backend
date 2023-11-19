namespace LoLModule.Models.Response;

public class TeamResponse
{
  public TeamResponse(List<ParticipantResponse> teammates)
  {
    this.teammates = teammates;
  }
  public List<ParticipantResponse> teammates { get; set; }

  public int totalDragonKills => teammates.Sum(participant => participant.dragonKills);
  public int totalTurretKills =>teammates.Sum(participant => participant.turretKills);
  public int totalBaronKills => teammates.Sum(participant => participant.baronKills);
  public int totalKills => teammates.Sum(participant => participant.kills);
  public int totalGoldEarned => teammates.Sum(participant => participant.goldEarned);

}