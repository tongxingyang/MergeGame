using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewCanvasLobby : MonoBehaviour {

	[SerializeField] private GameObject _score;

	private void Start() {
        GameManager.init.OnGameStart += () => InLobby(false);
        GameManager.init.OnLobby += () => InLobby(true);
    }

    private void InLobby(bool flag) {
		_score.SetActive(flag);
    }
}