using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataInfo {
	[System.Serializable]
	public class GameData {
		public float bestScore;
		public int adsCount;

		public bool isEffectVolum;
		public bool isBGMVolum;

		public bool isPremium = false;
	}
}
