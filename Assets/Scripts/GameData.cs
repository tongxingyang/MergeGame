using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataInfo {
	[System.Serializable]
	public class GameData {
		public float bestScore;
		public int coin;
		public int adsCount;

		public string key = "";
		public int userRank = 0;
		public int flagIndex = 0;
		public string userName = "";

		public bool[] styleProducts;
		public bool[] wallpaperProducts;
		public int styleNum;
		public int wallpaperNum;

		public bool isEffectVolum;
		public bool isBGMVolum;

		public bool isPremium = false;
	}
}
