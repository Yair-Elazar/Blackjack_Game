using System;
using Microsoft.Win32;
using UnityEngine;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    public Button play, settings, signOut;

    void Start()
    {
        play.onClick.AddListener(() => playClicked());
        settings.onClick.AddListener(settingsClicked);
        signOut.onClick.AddListener(signOutClicked);
    }

    private void signOutClicked()
    {
        throw new NotImplementedException();
    }

    private void settingsClicked()
    {
        throw new NotImplementedException();
    }

    private void playClicked()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }
}
