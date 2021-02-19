using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircleProgressBar : MonoBehaviour {
    public float progressBarSpeed;
    public Image loadingBarImg;

    private float currProgress;
    private bool isEnd = false;

    public void StartProgress() {
        currProgress = 0;
        isEnd = false;

        StopCoroutine(nameof(Progress));
        StartCoroutine(nameof(Progress));
    }

    IEnumerator Progress() {
        while (!isEnd) {
            currProgress += progressBarSpeed * Time.deltaTime;
            loadingBarImg.fillAmount = currProgress;

            if (currProgress >= 1) isEnd = true;

            yield return null;

        }
    }
}
