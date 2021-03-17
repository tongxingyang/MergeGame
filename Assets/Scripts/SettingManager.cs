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

			effectText = effect.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
			effectImg = effect.transform.GetChild(1).GetComponent<RawImage>();
			bgmText = BGM.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
			bgmImg = BGM.transform.GetChild(1).GetComponent<RawImage>();
		}
		else if (init != this) {
			Destroy(this.gameObject);
		}
		DontDestroyOnLoad(this.gameObject);
	}

	private void Start() {

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
		UIManager.init.audioSource.mute = !isEffectOn;
		isEffectOn = !isEffectOn;

		SetText();
	}

	public void BGMOn() {
		CameraControl.init.audioSource.mute = !isBGMOn;
		isBGMOn = !isBGMOn;

		SetText();
	}

	public void SetText() {
		if (UIManager.init.settingPanel.activeSelf) {
			if (isBGMOn) {
				bgmText.text = LocalizationManager.init.GetLocalizedValue("backgroundon");
				bgmImg.texture = Resources.Load<Texture>("setting/iconBGMMute");
			} else {
				bgmText.text = LocalizationManager.init.GetLocalizedValue("backgroundoff");
				bgmImg.texture = Resources.Load<Texture>("setting/iconBGM");
			}

			if (isEffectOn) {
				effectText.text = LocalizationManager.init.GetLocalizedValue("effectson");
				effectImg.texture = Resources.Load<Texture>("setting/iconMute");
			} else {
				effectText.text = LocalizationManager.init.GetLocalizedValue("effectsoff");
				effectImg.texture = Resources.Load<Texture>("setting/iconEffect");
			}
		}
	}
}