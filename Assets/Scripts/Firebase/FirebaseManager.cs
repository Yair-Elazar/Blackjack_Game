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
using static UnityEngine.Rendering.DebugUI;
using static UnityEngine.UIElements.UxmlAttributeDescription;
using System.Threading.Tasks;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;
using UnityEngine.UIElements;

public class FirebaseManager : MonoBehaviour
{

    // Firebase variable
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public static FirebaseManager Instance { get; private set; }
    public FirebaseAuth auth;
    public FirebaseUser user;
    public DatabaseReference DBreference;

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
    private InputField userName;
    private Text userAmount;
    private Text userGamesPlayed;
    private Text userWins;
    private Text userLoses;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); //Keeps FirebaseManager alive across scenes

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
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeFirebase()
    {
        //Set the default instance object
        auth = FirebaseAuth.DefaultInstance;
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;
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

    public void SignOutButton()
    {
        auth.SignOut();
        UIManager.Instance.OpenLoginPanel();
        ClearLoginFeilds();
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
            StartCoroutine(LoadUserData(user.UserId));

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

    public void ClearLoginFeilds()
    {
        emailLoginField.text = "";
        passwordLoginField.text = "";
    }

    public void ClearRegisterFeilds()
    {
        nameRegisterField.text = "";
        emailRegisterField.text = "";
        passwordRegisterField.text = "";
        confirmPasswordRegisterField.text = "";
    }

    private IEnumerator LoadUserData(string userId)
    {
        //Get the currently logged in user data
        Task<DataSnapshot> DBTask = DBreference.Child("users").Child(userId).GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else if (DBTask.Result.Value == null)
        {
            //No data exists yet
            newUserData();
            //userName.text = user.DisplayName;
        }
        else
        {
            //Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;

            //Tranport to method
            UserData.userId = user.UserId;
            UserData.userName = snapshot.Child("username").Value.ToString();
            UserData.amount = int.Parse(snapshot.Child("amount").Value.ToString());
            UserData.games = int.Parse(snapshot.Child("games").Value.ToString());
            UserData.wins = int.Parse(snapshot.Child("wins").Value.ToString());
            UserData.loses = int.Parse(snapshot.Child("loses").Value.ToString());
        }
    }

    private void newUserData()
    {
        StartCoroutine(UpdateUsernameAuth(user.DisplayName));
        StartCoroutine(UpdateUsernameDatabase(user.DisplayName));
        StartCoroutine(Amount(user.UserId, 1000));
        StartCoroutine(GamesPlayed(user.UserId, 0));
        StartCoroutine(Wins(user.UserId, 0));
        StartCoroutine(Loses(user.UserId, 0));
    }

    private IEnumerator UpdateUsernameAuth(string _username)
    {
        //Create a user profile and set the username
        UserProfile profile = new UserProfile { DisplayName = _username };

        //Call the Firebase auth update user profile function passing the profile with the username
        Task ProfileTask = user.UpdateUserProfileAsync(profile);
        //Wait until the task completes
        yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

        if (ProfileTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
        }
        else
        {
            //Auth username is now updated
        }
    }

    private IEnumerator UpdateUsernameDatabase(string _username)
    {
        //Set the currently logged in user username in the database
        Task DBTask = DBreference.Child("users").Child(user.UserId).Child("username").SetValueAsync(_username);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Database username is now updated
        }
    }

    private IEnumerator Amount(string userId, int amount)
    {
        //Set the currently logged in user xp
        Task DBTask = DBreference.Child("users").Child(userId).Child("amount").SetValueAsync(amount);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //userAmount.text = amount.ToString();
        }
    }

    private IEnumerator GamesPlayed(string userId, int games)
    {
        //Set the currently logged in user xp
        Task DBTask = DBreference.Child("users").Child(userId).Child("games").SetValueAsync(games);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //userGamesPlayed.text = games.ToString();
        }
    }

    private IEnumerator Wins(string userId, int wins)
    {
        //Set the currently logged in user kills
        Task DBTask = DBreference.Child("users").Child(userId).Child("wins").SetValueAsync(wins);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //userWins.text = wins.ToString();
        }
    }

    private IEnumerator Loses(string userId, int loses)
    {
        //Set the currently logged in user deaths
        Task DBTask = DBreference.Child("users").Child(userId).Child("loses").SetValueAsync(loses);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //userLoses.text = loses.ToString();
        }
    }


    public void SaveGameState(string userId, int amount, int gamesPlayed, int wins, int loses)
    {
        if (DBreference != null)
        {
            StartCoroutine(Amount(userId, amount));
            StartCoroutine(GamesPlayed(userId,gamesPlayed)); ;
            StartCoroutine(Wins(userId, wins));
            StartCoroutine(Loses(userId, loses));
        }
        else
        {
            Debug.LogError("Database reference is null!");
        }
        
    }
}
