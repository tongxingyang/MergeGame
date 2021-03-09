using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RankUserData : MonoBehaviour {
    public RawImage flag;
    public TextMeshProUGUI userName;
    public TextMeshProUGUI userScore;

    public void SetRankData(object flag, object name, object score) {
        this.userName.text = name.ToString();
        this.userScore.text = score.ToString();
    }

}