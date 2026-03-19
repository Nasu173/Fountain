using Fountain.Common;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Fountain.MiniGame.ControlFountain
{
    /// <summary>
    /// 计分板UI,显示分数,就在这里简单的相应事件即可
    /// </summary>
    public class ScoreUI : MonoBehaviour
    {
        private TextMeshProUGUI scoreText;
        private int score;
        private void Start()
        {
            scoreText = this.transform.FindChildByName(nameof(scoreText)).
                GetComponent<TextMeshProUGUI>();
            score = 0;
            scoreText.text = "0";
            GameEventBus.Subscribe<ControlFountainHit>((e) =>
            {
                score++;
                scoreText.text = score.ToString();
            });
        }
    }
}
