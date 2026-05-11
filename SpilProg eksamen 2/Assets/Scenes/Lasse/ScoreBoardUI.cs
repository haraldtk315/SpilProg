using TMPro;
using UnityEngine;

public class ScoreBoardUI : MonoBehaviour
{
    [SerializeField] private TMP_Text[] scoreTexts;

    private void Update()
    {
        if (ScoreBoardManager.instance == null)
            return;

        for (int i = 0; i < scoreTexts.Length; i++)
        {
            if (scoreTexts[i] == null)
                continue;

            if (i < ScoreBoardManager.instance.playerScores.Count)
            {
                scoreTexts[i].text = ScoreBoardManager.instance.playerScores[i].ToString();
            }
            else
            {
                scoreTexts[i].text = "0";
            }
        }
    }
}