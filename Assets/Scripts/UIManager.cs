using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


//TODO finish login and register pages + datebase
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public Button login;
    public Button register;
    //public Button userLogin;
    //public Button userRegister;
    public GameObject mainTilte;
    public GameObject loginUI;
    public GameObject registerUI;

    void Start()
    {
        login.onClick.AddListener(() => LoginClicked());
        register.onClick.AddListener(() => RegisterClicked());
        //userLogin.onClick.AddListener(() => UserLoginClicked());
    }

    private void Awake()
    {
        CreateInstance();
    }

    private void CreateInstance()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void RegisterClicked()
    {
        OpenRegisterPanel();
    }

    private void LoginClicked()
    {
        mainTilte.SetActive(false);
        loginUI.SetActive(true);
        registerUI.SetActive(false);
    }

    public void OpenLoginPanel()
    {
        LoginClicked();
    }

    public void OpenRegisterPanel()
    {
        mainTilte.SetActive(false);
        loginUI.SetActive(false);
        registerUI.SetActive(true);

    }

    public void BackToMenu()
    {
        loginUI.SetActive(false);
        registerUI.SetActive(false);
    }

}
