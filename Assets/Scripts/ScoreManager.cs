using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

class ScoreManager {
	public static readonly float DROP_SCORE = 1f;
	public static readonly float MARGE_SCORE = 2f;
	private static readonly float BASE_SCORE = 13f;

	private float coin, currScore, bestScore;
	private Text coinText, currScoreText, bestScoreText;

	public ScoreManager(Text ct, Text cst, Text bst) {
		coinText = ct;
		currScoreText = cst;
		bestScoreText = bst;

		initScore();
	}

	private void initScore() {
		coinText.text = "0";
		currScoreText.text = "0";
		bestScoreText.text = "0";
	}

	public void AddScore(float type, ObjectManager.MergeLevel mergeLevel = ObjectManager.MergeLevel.one) {

		currScore += (type * (int)mergeLevel * BASE_SCORE);
		if (bestScore < currScore)
			bestScore = currScore;

		setScoreText();
	}

	private void setScoreText() {
		currScoreText.text = string.Format("{0}", currScore);
		bestScoreText.text = string.Format("{0}", bestScore);
	}

	private void setGoldText() {
		coinText.text = String.Format("{0}", coin);
	}
}