using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public Button login;
    public Button signUp;
    public Button userLogin;
    public Button userRegister;
    public GameObject mainTilte;
    public GameObject loginUI;
    public GameObject registerUI;

    void Start()
    {
        login.onClick.AddListener(() => LoginClicked());
        signUp.onClick.AddListener(() => SignUpClicked());
        userLogin.onClick.AddListener(() => UserLoginClicked());
       
    }

    private void UserLoginClicked()
    {
        SceneManager.LoadSceneAsync(1);
    }

    private void SignUpClicked()
    {
        mainTilte.SetActive(false);
        loginUI.SetActive(false);
        registerUI.SetActive(true);
    }

    private void LoginClicked()
    {
        mainTilte.SetActive(false);
        loginUI.SetActive(true);
        registerUI.SetActive(false);
    }

    public void backToMenu()
    {
        loginUI.SetActive(false);
        registerUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
