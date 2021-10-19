using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour //created a class 
{
    private static GameAssets instance; //static means it acn be accessed from anywhere in our code

    public static GameAssets GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        instance = this;
    }

    public Sprite pipeHeadSprite;
    public Transform pfPipeBody;
    public Transform pfPipeHead;
    public Transform pfGround;
    public Transform pfCloud1;
    public Transform pfCloud2;
    public Transform pfCloud3;


    public SoundAudioClip[] soundAudioClipArray;

    [Serializable]
    public class SoundAudioClip
    {
        public SoundManager.Sound sound;
        public AudioClip audioClip;
    }
}
