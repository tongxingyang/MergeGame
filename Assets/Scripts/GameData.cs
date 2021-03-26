using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataInfo {
	[System.Serializable]
	public class GameData {
		public int bestScore = 0;
		public int coin = 0;
		public int adsCount;
		public int evalutionCount;
		public int rankupItemCount;
		public int destroyItemCount;

		public string key;
		public int rankFlag;
		public string rankName;
		public int rankScore;

		public bool[] styleProducts;
		public bool[] wallpaperProducts;
		public int styleNum;
		public int wallpaperNum;

		public bool isEffectVolum;
		public bool isBGMVolum;

		public bool isPremium = false;
		public bool isDoubleCoin = false;

		public string lastLanguageFileName;

		public DateTime initTimer;
	}
}
