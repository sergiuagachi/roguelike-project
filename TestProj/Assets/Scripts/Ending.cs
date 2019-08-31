using UnityEngine;
using UnityEngine.UI;

public class Ending : MonoBehaviour {

    public Text statisticsVisual;
    
    private void Start() {
        var statistics = Player.Instance.Statistics;
        Player.Instance.gameObject.SetActive(false);

        var minutes = Mathf.FloorToInt(GameManager.Instance.timeElapsed / 60);
        var seconds = Mathf.FloorToInt(GameManager.Instance.timeElapsed % 60);

        var niceTime = $"{minutes:0}:{seconds:00}";
        
        statisticsVisual.text = "Statistics" + "\n\n" +
            //"time elapsed: " + minutes + ":" + seconds + "\n" +
                                "time elapsed: " + niceTime + "\n" +
                                "death count: " + statistics.DeathCounter + "\n" +
                                "enemies killed: " + statistics.EnemiesKilled + "\n" +
                                "steps taken: " + statistics.StepsTaken + "\n" +
                                "damage dealt: " + statistics.DamageDealt + "\n" +
                                "damage received: " + statistics.DamageReceived + "\n" +
                                "floors changed: " + statistics.FloorsChanged + "\n" +
                                "total heal:" +statistics.TotalHeal;
    }
}
