using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public AudioSource mainTheme;
    public AudioSource minotaurTheme;

    bool chasePlaying;
    bool themePlaying;

    Minotaur minotaur;

    void Start()
    {
        chasePlaying = false;
        themePlaying = false;
        minotaur = GameObject.FindObjectOfType<Minotaur>();
    }

    void Update()
    {
        if (!chasePlaying && minotaur.chaseState == ChaseState.Chase)
        {
            mainTheme.Stop();
            minotaurTheme.Play();
            chasePlaying = true;
            themePlaying = false;
        }
        
        if (!themePlaying && minotaur.chaseState != ChaseState.Chase)
        {
            minotaurTheme.Stop();
            mainTheme.Play();
            themePlaying = true;
            chasePlaying = false;
        }
    }
}
