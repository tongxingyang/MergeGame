using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

class ScoreManager {
	private static readonly int SCORE = 70;

	private int coin, currScore, bestScore;
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

	public void changingScore(int type) {
		currScore += (type * SCORE);
		if (bestScore < currScore)
			bestScore = currScore;

		setScoreText();
	}

	private void setScoreText() {
		coinText.text = coin.ToString();
		currScoreText.text = currScore.ToString();
		bestScoreText.text = bestScore.ToString();
	}
}
