using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataInfo {
	[System.Serializable]
	public class GameData {
		public float bestScore;
		public int adsCount;

		public float bgmVolume;
		public float effectVolume;

		public bool isPremium = false;
	}
}
