using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Firestore;
using Unity.VisualScripting;
using UnityEngine;


public class FirestoreManager : MonoBehaviour
{
    public static FirestoreManager Instance;
    private FirebaseFirestore db;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        db = FirebaseFirestore.DefaultInstance;
    }


    public void SaveGameData(int[] playerHand, int dealerCard, string action, string outcome)
    {
        FirebaseUser user = FirebaseAuthManager.Instance.GetCurrentUser();

        if (user == null)
        {
            Debug.LogError("User not logged in!");
            return;
        }

        string userId = user.UserId;
        // Reference to the user's rounds subcollection
        DocumentReference userDocRef = db.Collection("users").Document(userId);
        CollectionReference roundsRef = userDocRef.Collection("rounds");

        // Auto - generate a unique round ID
        DocumentReference newRoundRef = roundsRef.Document();

        // Save the round data
        Dictionary<string, object> roundData = new Dictionary<string, object>
        {
            { "playerHand", playerHand },
            { "dealerCard", dealerCard },
            { "action", action },
            { "outcome", outcome },
            { "timestamp", FieldValue.ServerTimestamp }
        };

        newRoundRef.SetAsync(roundData).ContinueWith(task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                Debug.Log("Round data saved successfully with ID: " + newRoundRef.Id);
            }
            else
            {
                Debug.LogError("Error saving round data: " + task.Exception);
            }
        });
    }

    public void SaveUserData(int amount, int games, int wins, int loses)
    {

        FirebaseUser user = FirebaseAuthManager.Instance.GetCurrentUser();

        if (user == null)
        {
            Debug.LogError("User not logged in!");
            return;
        }

        string userId = user.UserId;
        // Reference to the user's rounds subcollection
        DocumentReference userDocRef = db.Collection("users").Document(userId);
        CollectionReference userDataRef = userDocRef.Collection("userData");

        DocumentReference newUserData = userDataRef.Document("data");

        Dictionary<string, object> userData = new Dictionary<string, object>
        {
            { "amount", amount },
            { "games", games },
            { "wins", wins },
            { "loses", loses },
            
        };

        newUserData.SetAsync(userData, SetOptions.MergeAll).ContinueWith(task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                Debug.Log("User data saved successfully with ID: " + newUserData.Id);
            }
            else
            {
                Debug.LogError("Error saving User Data: " + task.Exception);
            }
        });

    }

    public void LoadUserData()
    {

        FirebaseUser user = FirebaseAuthManager.Instance.GetCurrentUser();

        if (user == null)
        {
            Debug.LogError("User not logged in!");
            return;
        }

        string userId = user.UserId;

        UserData.amount = 10000;
        UserData.games = 0;
        UserData.wins = 0;
        UserData.loses = 0;

        DocumentReference dataRef = db.Collection("users").Document(userId).Collection("userData").Document("data");
        dataRef.GetSnapshotAsync().ContinueWith((task) =>
        {
            var snapshot = task.Result;
            if (snapshot.Exists)
            {
                UserData.amount = snapshot.GetValue<int>("amount");
                UserData.games = snapshot.GetValue<int>("games");
                UserData.wins = snapshot.GetValue<int>("wins");
                UserData.loses = snapshot.GetValue<int>("loses");
            }

            else
            {
                Debug.Log(String.Format("Document {0} does not exist!", snapshot.Id));
            }
        });
        


    }
}