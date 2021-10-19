using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingToStartWindow : MonoBehaviour
{
    
    void Start()
    {
        //Bird.GetInstance().OnStart += WaitingToStart_OnStart;
    }

    private void WaitingToStartWindow_OnStartedPlaying(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
}
