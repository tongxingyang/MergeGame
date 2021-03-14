using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RankUserData : MonoBehaviour {
    public TextMeshProUGUI userRank;
    public RawImage flag;
    public TextMeshProUGUI userName;
    public TextMeshProUGUI userScore;

    public void SetRankData(int rank, Sprite flag, object name, object score) {
        this.userRank.text = rank.ToString();
        this.flag.texture = flag.texture;
        this.userName.text = name.ToString();
        this.userScore.text = score.ToString();
    }
}