using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class LocalizationManager : MonoBehaviour {


    public static LocalizationManager init;

    private string _lastLanguageFileName;
    public string lastLanguageFileName {
		get { return _lastLanguageFileName; }
		set {
            if (value.Equals("")) {
                InitLanguage();
            } else {
                _lastLanguageFileName = value;
                LoadLocalizedText(value);
            }
        }
	}

    private Dictionary<string, string> localizedText;
    private string missingTextString = "Localized text not found";


    // Start is called before the first frame update
    void Awake() {

        //싱글톤 패턴
        if (init == null) {
            init = this;
        } else if (init != this) {
            Destroy(gameObject);
        }
    }

	private void Start() {

    }

	public void LoadLocalizedText(string fileName) {
        _lastLanguageFileName = fileName;

        localizedText = new Dictionary<string, string>();
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
        filePath += ".json";
        if (File.Exists(filePath)) {
            string dataAsJson = File.ReadAllText(filePath); //json파일을 읽어서 string으로 뽑음
            LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(dataAsJson);    //deserialization

            //전체 아이템들에 대해서
            for (int i = 0; i < loadedData.items.Length; i++) {
                localizedText.Add(loadedData.items[i].key, loadedData.items[i].value);
            }

            //localizationText 데이터 불러오기 완료
            Debug.Log("Data loaded. Dictionary containts :" + localizedText.Count + " entries");

        } else {
            //파일이 존재하지않음
            Debug.LogError("Cannot find file");
        }
    }

    public string GetLocalizedValue(string key) {
        string result = missingTextString;
        if (localizedText.ContainsKey(key)) {
            result = localizedText[key];
        }

        return result;
    }

    private void InitLanguage() {
        string languageName = "en";

		switch (Application.systemLanguage) {
            case SystemLanguage.Korean:
                languageName = "ko";
                break;
            case SystemLanguage.French:
                languageName = "fr";
                break;
            case SystemLanguage.Spanish:
                languageName = "es";
                break;
            case SystemLanguage.German:
                languageName = "de";
                break;
            case SystemLanguage.Italian:
                languageName = "it";
                break;
            case SystemLanguage.ChineseSimplified:
                languageName = "zh_chs";
                break;
            case SystemLanguage.ChineseTraditional:
                languageName = "zh_cht";
                break;
            case SystemLanguage.Japanese:
                languageName = "ja";
                break;
            case SystemLanguage.Russian:
                languageName = "ru";
                break;
            case SystemLanguage.Danish:
                languageName = "da";
                break;
            case SystemLanguage.Norwegian:
                languageName = "no";
                break;
            case SystemLanguage.Portuguese:
                languageName = "pt";
                break;
            case SystemLanguage.Swedish:
                languageName = "sv";
                break;
            case SystemLanguage.Polish:
                languageName = "pl";
                break;
            case SystemLanguage.Turkish:
                languageName = "tr";
                break;
            case SystemLanguage.Vietnamese:
                languageName = "vt";
                break;
            case SystemLanguage.Indonesian:
                languageName = "in";
                break;
            default:
                languageName = "en";
                break;

        }

        languageName = "localizedText_" + languageName;
        LoadLocalizedText(languageName);
    }
}