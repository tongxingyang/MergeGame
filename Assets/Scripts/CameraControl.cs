using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {
    private static readonly float AUDIO_FADEOUT_VAL = 0.2f;


    public bool isGameOver = false;

    private AudioSource audioSource;
    private float _currAudioVolum;
    public float currAudioVolum {
        get { return _currAudioVolum; }
        set { _currAudioVolum = value; }
    }

    private void Start() {
        audioSource = GetComponent<AudioSource>();
        currAudioVolum = audioSource.volume;
    }

    void Update() {
        if (isGameOver) {
            audioSource.volume -= Time.deltaTime * AUDIO_FADEOUT_VAL;
        } else {
            audioSource.volume = currAudioVolum;
        }
    }
}
