using System;
using UnityEngine;

public class ProjectSpellGameLobby : MonoBehaviour
{
    public static ProjectSpellGameLobby Singleton { get; private set; }

    private void Awake()
    {
        if (Singleton && Singleton != this)
        {
            Destroy(gameObject);
            return;
        }
        Singleton = this;

        DontDestroyOnLoad(this);
    }
}
