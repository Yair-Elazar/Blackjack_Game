using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System.Collections;
using UnityEngine.UI;
using System;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditor.PackageManager;

public class FirebaseAuthManager : MonoBehaviour
{

    // Firebase variable
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser user;
    public DatabaseReference DB;

    // Login Variables
    [Space]
    [Header("Login")]
    public InputField emailLoginField;
    public InputField passwordLoginField;
    public Text loginText;

    // Registration Variables
    [Space]
    [Header("Registration")]
    public InputField nameRegisterField;
    public InputField emailRegisterField;
    public InputField passwordRegisterField;
    public InputField confirmPasswordRegisterField;

    [Space]
    [Header("UserData")]
    public InputField userName;
    public InputField userAmount;
    public InputField gamePlay;
    public InputField userWins;
    public InputField userLoses;


    private void Awake()
    {
        // Check that all of the necessary dependencies for firebase are present on the system
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;

            if (dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all firebase dependencies: " + dependencyStatus);
            }
        });
    }

    void InitializeFirebase()
    {
        //Set the default instance object
        auth = FirebaseAuth.DefaultInstance;

        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    // Track state changes of the auth object.
    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;

            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
            }

            user = auth.CurrentUser;

            if (signedIn)
            {
                Debug.Log("Signed in " + user.UserId);
            }
        }
    }

    public void Login()
    {
        //Call the login coroutine passing the email and password
        StartCoroutine(LoginAsync(emailLoginField.text, passwordLoginField.text));
    }

    public void Register()
    {
        //Call the register coroutine passing the email, password, and username
        StartCoroutine(RegisterAsync(nameRegisterField.text, emailRegisterField.text, passwordRegisterField.text, confirmPasswordRegisterField.text));
    }


    private IEnumerator LoginAsync(string email, string password)
    {
        var loginTask = auth.SignInWithEmailAndPasswordAsync(email, password);

        yield return new WaitUntil(() => loginTask.IsCompleted);

        if (loginTask.Exception != null)
        {
            //If there are errors handle them
            Debug.LogError(loginTask.Exception);
            FirebaseException firebaseException = loginTask.Exception.GetBaseException() as FirebaseException;
            AuthError authError = (AuthError)firebaseException.ErrorCode;


            string failedMessage = "Login Failed! Because ";

            switch (authError)
            {
                case AuthError.InvalidEmail:
                    failedMessage += "Email is invalid";
                    break;
                case AuthError.WrongPassword:
                    failedMessage += "Wrong Password";
                    break;
                case AuthError.MissingEmail:
                    failedMessage += "Email is missing";
                    break;
                case AuthError.MissingPassword:
                    failedMessage += "Password is missing";
                    break;
                default:
                    failedMessage = "Login Failed";
                    break;
            }

            Debug.Log(failedMessage);
        }
        else
        {
            user = loginTask.Result.User;

            Debug.LogFormat("{0} You Are Successfully Logged In", user.DisplayName);
            loginText.text = "Logged In";

            yield return new WaitForSeconds(2);

            //References.userName = user.DisplayName;
            UnityEngine.SceneManagement.SceneManager.LoadScene("MenuScene");
        }
    }


    private IEnumerator RegisterAsync(string name, string email, string password, string confirmPassword)
    {
        if (name == "")
        {
            //If the username field is blank show a warning
            Debug.LogError("User Name is empty");
        }
        else if (email == "")
        {
            //If the email field is blank show a warning
            //add UI warning text
            Debug.LogError("email field is empty");
        }
        else if (passwordRegisterField.text != confirmPasswordRegisterField.text)
        {
            //If the password does not match show a warning
            Debug.LogError("Password does not match");
        }
        else
        {
            //Call the Firebase auth signin function passing the email and password
            var registerTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);

            yield return new WaitUntil(() => registerTask.IsCompleted);

            if (registerTask.Exception != null)
            {
                // If there are errors handle them
                Debug.LogError(registerTask.Exception);

                FirebaseException firebaseException = registerTask.Exception.GetBaseException() as FirebaseException;
                AuthError authError = (AuthError)firebaseException.ErrorCode;

                string failedMessage = "Registration Failed! Becuase ";
                switch (authError)
                {
                    case AuthError.InvalidEmail:
                        failedMessage += "Email is invalid";
                        break;
                    case AuthError.WrongPassword:
                        failedMessage += "Wrong Password";
                        break;
                    case AuthError.MissingEmail:
                        failedMessage += "Email is missing";
                        break;
                    case AuthError.MissingPassword:
                        failedMessage += "Password is missing";
                        break;
                    default:
                        failedMessage = "Registration Failed";
                        break;
                }

                Debug.Log(failedMessage);
            }
            else
            {
                // Get The User After Registration Success
                //User has now been created
                //Now get the result
                user = registerTask.Result.User;

                if (user != null)
                {
                
                    UserProfile userProfile = new UserProfile { DisplayName = name };

                    var updateProfileTask = user.UpdateUserProfileAsync(userProfile);

                    yield return new WaitUntil(() => updateProfileTask.IsCompleted);

                    if (updateProfileTask.Exception != null)
                    {
                        // Delete the user if user update failed
                        user.DeleteAsync();

                        Debug.LogError(updateProfileTask.Exception);

                        FirebaseException firebaseException = updateProfileTask.Exception.GetBaseException() as FirebaseException;
                        AuthError authError = (AuthError)firebaseException.ErrorCode;

                        string failedMessage = "Profile update Failed! Becuase ";
                        switch (authError)
                        {
                            case AuthError.InvalidEmail:
                                failedMessage += "Email is invalid";
                                break;
                            case AuthError.WrongPassword:
                                failedMessage += "Wrong Password";
                                break;
                            case AuthError.MissingEmail:
                                failedMessage += "Email is missing";
                                break;
                            case AuthError.MissingPassword:
                                failedMessage += "Password is missing";
                                break;
                            default:
                                failedMessage = "Profile update Failed";
                                break;
                        }

                        Debug.Log(failedMessage);
                    }
                    else
                    {
                        Debug.Log("Registration Sucessful Welcome " + user.DisplayName);
                        //UIManagr.openLoginPanel();
                        UIManager.Instance.OpenLoginPanel();
                    }
                }
            }
        }
    }
}
