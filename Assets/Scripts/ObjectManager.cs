using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour {
	private static readonly int GARBAGE_COUNT = 10;
	private static readonly int MAX_CREATE_OBJECT_NUMBER = 5;
	private static readonly float TIME_TO_NEXT_OBJECT = 0.7f;
	private static readonly float INIT_Y_POSITION = 4;

	public enum MergeLevel {
		min, one, two, three, four, five, six, seven, eight, nine, max
	}

	public static ObjectManager init = null;
	private void Awake() {
		if (init == null) {
			init = this;
		} else if (init != this) {
			Destroy(this.gameObject);
		}
		DontDestroyOnLoad(this.gameObject);
	}

	public GameObject[] objects;
	public GameObject background;

	public float backgroundLeft {
        get {
			return -(background.GetComponent<SpriteRenderer>().sprite.rect.width
				/ (background.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit * 0.01f) * 0.005f);
		}
    }
	public float backgroundRight {
		get {
			return background.GetComponent<SpriteRenderer>().sprite.rect.width
				/ (background.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit * 0.01f) * 0.005f;
		}
	}
	public int currBackgroundNum;
	public int currStyleNum;

	[HideInInspector]
	public GameObject objParent;

	private static Queue<GameObject> garbageObjectContainer;
	private GameObject currObject;
	private GameObject currBackground;

	private void Start() {
		objParent = new GameObject("objParent");
		garbageObjectContainer = new Queue<GameObject>();

		if (currBackground == null)
			currBackground = Instantiate(background);

		CreateObject(0);
	}

	public void CreateObject(int rand) {
		if (!GameManager.init.isGameOver) {
			currObject = Instantiate(objects[rand], new Vector3(0, INIT_Y_POSITION, 0), Quaternion.identity);
			currObject.transform.parent = objParent.transform;
			MouseControl.init.SetCurrObject(currObject);
		}
	}

	public void CreateMergeObject(MainObject target) {
		GameObject tempObj = Instantiate(objects[(int)target.mergeLevel], target.transform.position, Quaternion.identity);
		tempObj.GetComponent<MainObject>().Setting();
		tempObj.transform.parent = objParent.transform;

		if (tempObj.GetComponent<MainObject>().mergeLevel == MergeLevel.max) {
			UIManager.init.OnMaxLevelPanel(tempObj);
		}

		StartMergeAudio();
	}

	public void RespawnCurrObject() {
		currObject.GetComponent<MainObject>().Setting();
		StartCoroutine(nameof(TimeToNextObject));
	}

	public void MergeObject(MainObject target, MainObject curr) {
		if ((target.mergeLevel == MergeLevel.max) || GameManager.init.isGameOver) 
			return;

		target.mergeLevel += 1;
		CreateMergeObject(target);

		AddMergedObjectToGarbage(new GameObject[] { target.gameObject, curr.gameObject });
		UIManager.init.AddScore(ScoreManager.MARGE_SCORE, target.mergeLevel);
	}

	public void AddMergedObjectToGarbage(GameObject[] gameObjects) {
		if (garbageObjectContainer.Count > GARBAGE_COUNT) {
			for (int i = 0; i < GARBAGE_COUNT; ++i) {
				Destroy(garbageObjectContainer.Dequeue());
			}
		}

		foreach (GameObject gameObject in gameObjects) {

			garbageObjectContainer.Enqueue(gameObject);
			gameObject.SetActive(false);
		}
	}

	IEnumerator TimeToNextObject() {
		yield return new WaitForSeconds(TIME_TO_NEXT_OBJECT);
		CreateObject(Random.Range(0, MAX_CREATE_OBJECT_NUMBER));
		UIManager.init.AddScore(ScoreManager.DROP_SCORE);
	}

	public void StopObj() {
		foreach (Transform gameObject in objParent.GetComponentsInChildren<Transform>()) {
			if (gameObject.gameObject.GetComponent<Rigidbody2D>() != null) {
				gameObject.gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
				gameObject.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			}
		}
	}

	public void InitObj() {
		garbageObjectContainer.Clear();
		foreach (Transform gameObject in objParent.GetComponentsInChildren<Transform>()) {
			Destroy(gameObject.gameObject);
		}
		Start();
	}

	private void StartMergeAudio() {
		UIManager.init.PlayEffectSouned();

	}

	public void ChangeObjectSpriteImage(int objNum) {
		currStyleNum = objNum;
		Sprite[] sprites = Resources.LoadAll<Sprite>("obj/objects" + objNum);

		currObject.GetComponent<SpriteRenderer>().sprite = sprites[0];
		for(int i = 0; i < objects.Length;  ++i) {
			objects[i].GetComponent<SpriteRenderer>().sprite = sprites[i];
		}
	}

	public void ChangeBackgroundImage(int wallNum) {
		currBackgroundNum = wallNum;
		Sprite sprite = Resources.Load<Sprite>("obj/background" + wallNum);
		currBackground.GetComponent<SpriteRenderer>().sprite = sprite;
	}

	public void RankUpItem(bool isAdsRewarded = false) {
		if (!isAdsRewarded) {
			if (ScoreManager.init.rankItemCount == 0) {
				UIManager.init.OpenRankUpItemPanel();
				return;
			}
			else if (ScoreManager.init.rankItemCount < 0) {
				return;
			}
		}

		List < Transform > objectRange = TargetOfRankUpItem();

		int count = objectRange.Count < 1 ? 0 : 1;
		if (count <= 0) {
			UIManager.init.itemUnavailableMessage.SetActive(true);
			return;
		}
		if(count > 0) {
			ScoreManager.init.rankItemCount--;
			while (count > 0) {
				int range = Random.Range(0, objectRange.Count);

				objectRange[range].GetComponent<MainObject>().OnRankUpItem();
				count--;
			}
		} else if (isAdsRewarded) {
			ScoreManager.init.rankItemCount--;
		}
	}
	
	public void DestroyItem(bool isAdsRewarded = false) {
		if (!isAdsRewarded) {
			if (ScoreManager.init.destroyItemCount == 0) {
				UIManager.init.OpenDestroyItemPanel();
				return;
			}
			else if (ScoreManager.init.destroyItemCount < 0) {
				return;
			}
		}

		List<Transform> objectRange = TargetOfDestroyItem();

		int count = 2;
		if(objectRange.Count < 2) {
			count = objectRange.Count;
		}

		if (count <= 0) {
			UIManager.init.itemUnavailableMessage.SetActive(true);
			return;
		}

		if (count > 0) {
			List<int> randDeduplication = new List<int>();

			ScoreManager.init.destroyItemCount--;
			while (count > 0) {
				int range = Random.Range(0, objectRange.Count);
				if (randDeduplication.Contains(range))
					continue;
				else
					randDeduplication.Add(range);

				objectRange[range].GetComponent<MainObject>().OnDestroyItem();
				count--;
			}
		} else if (isAdsRewarded) {
			ScoreManager.init.destroyItemCount--;
		}
	}

	public List<Transform> TargetOfRankUpItem() {
		return TargetOfItem(MergeLevel.three, MergeLevel.seven);
	}
	public List<Transform> TargetOfDestroyItem() {
		return TargetOfItem(MergeLevel.one, MergeLevel.seven);
	}

	public List<Transform> TargetOfItem(MergeLevel minLevel, MergeLevel maxLevel) {
		List<Transform> objectRange = new List<Transform>();

		foreach (Transform child in objParent.GetComponentsInChildren<Transform>()) {
			if (child.name == objParent.name)
				continue;

			if (child.gameObject.activeSelf && child.GetComponent<Rigidbody2D>().gravityScale > 0) {
				if (child.GetComponent<MainObject>().mergeLevel >= minLevel &&
					child.GetComponent<MainObject>().mergeLevel <= maxLevel)
					objectRange.Add(child);
			}
		}
		TargetOfItemFadeIn();
		return objectRange;
	}


	public List<Transform> NotTargetOfItem(MergeLevel minLevel, MergeLevel maxLevel) {
		List<Transform> objectRange = new List<Transform>();

		foreach (Transform child in objParent.GetComponentsInChildren<Transform>()) {
			if (child.name == objParent.name)
				continue;

			if (child.gameObject.activeSelf && child.GetComponent<Rigidbody2D>().gravityScale > 0) {
				if (child.GetComponent<MainObject>().mergeLevel > minLevel ||
					child.GetComponent<MainObject>().mergeLevel < maxLevel)
					objectRange.Add(child);
			}
		}

		return objectRange;
	}

	public void TargetOfItemFadeOutOnRankUp() {
		List<Transform> objectRange = NotTargetOfItem(MergeLevel.seven, MergeLevel.three);

		foreach (Transform obj in objectRange) {
			obj.GetComponent<MainObject>().IsFadeOut(true);
		}
	}

	public void TargetOfItemFadeOutOnDestroy() {
		List<Transform> objectRange = NotTargetOfItem(MergeLevel.seven, MergeLevel.one);

		foreach (Transform obj in objectRange) {
			obj.GetComponent<MainObject>().IsFadeOut(true);
		}
	}

	public void TargetOfItemFadeIn() {

		foreach (Transform child in objParent.GetComponentsInChildren<Transform>()) {
			if (child.name == objParent.name)
				continue;

			child.GetComponent<MainObject>().IsFadeOut(false);
		}
	}

}