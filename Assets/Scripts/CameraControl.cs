using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {
    private static readonly float AUDIO_FADEOUT_VAL = 0.2f;

    public int width = 9;
    public int height = 17;

    public bool isBGMFadeOut = false;

    public static CameraControl init = null;
    private void Awake() {
        if (init == null) {
            init = this;

            audioSource = GetComponent<AudioSource>();
        }
        else if (init != this) {
            Destroy(this.gameObject);
        }
/*
        Camera cam = GetComponent<Camera>();
        Rect rt = cam.rect;
        float scale_height = ((float)Screen.width / Screen.height) / ((float)width / height);

        float scale_width = 1f / scale_height;

        if (scale_height < 1) {
            rt.height = scale_height;
            rt.y = (1f - scale_height) / 2f;
        } else {
            rt.width = scale_width;
            rt.x = (1f - scale_width) / 2f;
        }

        rt.height = Screen.height;
        cam.rect = rt;*/
    }


    public AudioSource audioSource;
    private float _currAudioVolum;
    public float currAudioVolum {
        get { return _currAudioVolum; }
        set { _currAudioVolum = value; }
    }

    private void Start() {
        currAudioVolum = audioSource.volume;
    }

    private void Update() {
        if (isBGMFadeOut) {
            audioSource.volume -= Time.deltaTime * AUDIO_FADEOUT_VAL;
        } else {
            audioSource.volume = currAudioVolum;
        }
    }

    public void BGMFadeOut(bool value) {
        isBGMFadeOut = value;
    }
}
