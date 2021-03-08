using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingManager : MonoBehaviour {
	public bool isEffectOn = false;
	public bool isBGMOn = false;

	public Button effect;
	public Button BGM;

	private TextMeshProUGUI effectText;
	private RawImage effectImg;

	private TextMeshProUGUI bgmText;
	private RawImage bgmImg;

	public static SettingManager init = null;
	private void Awake() {
		if (init == null) {
			init = this;
		}
		else if (init != this) {
			Destroy(this.gameObject);
		}
		DontDestroyOnLoad(this.gameObject);
	}

	private void Start() {
		effectText = effect.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
		effectImg = effect.transform.GetChild(1).GetComponent<RawImage>();

		bgmText = BGM.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
		bgmImg = BGM.transform.GetChild(1).GetComponent<RawImage>();

		effect.onClick.AddListener(EffectOn);
		BGM.onClick.AddListener(BGMOn);
	}

	public void EffectOn(bool isMute) {
		isEffectOn = !isMute;
		EffectOn();
	}

	public void BGMOn(bool isMute) {
		isBGMOn = !isMute;
		BGMOn();
	}

	public void EffectOn() {
		if (isEffectOn) {
			effectText.text = "?????? ????";
			effectImg.texture = Resources.Load<Texture>("setting/iconEffect");
		} else {
			effectText.text = "?????? ????";
			effectImg.texture = Resources.Load<Texture>("setting/iconMute");
		}
		UIManager.init.audioSource.mute = !isEffectOn;
		isEffectOn = !isEffectOn;
	}

	public void BGMOn() {
		if (isBGMOn) {
			bgmText.text = "???????? ????";
			bgmImg.texture = Resources.Load<Texture>("setting/iconBGM");
		}
		else {
			bgmText.text = "???????? ????";
			bgmImg.texture = Resources.Load<Texture>("setting/iconBGMMute");
		}
		CameraControl.init.audioSource.mute = !isBGMOn;
		isBGMOn = !isBGMOn;
	}
}