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


    public async Task SaveGameData(int[] playerHand, int dealerCard, string action, string outcome)
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

       await newRoundRef.SetAsync(roundData); // Use await here to ensure it's done before continuing

        Debug.Log("Round data saved successfully with ID: " + newRoundRef.Id);
    }

    public async Task SaveUserData(int amount, int games, int wins, int loses)
    {

        FirebaseUser user = FirebaseAuthManager.Instance.GetCurrentUser();

        if (user == null)
        {
            Debug.LogError("User not logged in!");
            return;
        }

        string userId = user.UserId;
        // Reference to the user's rounds subcollection
         DocumentReference newUserData = db.Collection("users").Document(userId).Collection("userData").Document("data");

        Dictionary<string, object> userData = new Dictionary<string, object>
        {
            { "amount", amount },
            { "games", games },
            { "wins", wins },
            { "loses", loses },
            
        };

        try
        {
            await newUserData.SetAsync(userData, SetOptions.MergeAll);
            Debug.Log("User data saved successfully with ID: " + newUserData.Id);
        }
        catch (Exception e)
        {
            Debug.LogError("Error saving User Data: " + e.Message);
        }

    }

    public async Task LoadUserData()
    {

        FirebaseUser user = FirebaseAuthManager.Instance.GetCurrentUser();

        if (user == null)
        {
            Debug.LogError("User not logged in!");
            return;
        }

        string userId = user.UserId;

        DocumentReference dataRef = db.Collection("users").Document(userId).Collection("userData").Document("data");

        DocumentSnapshot snapshot = await dataRef.GetSnapshotAsync(); // חכה שהנתונים ייטענו

        if (snapshot.Exists)
        {
            UserData.amount = snapshot.ContainsField("amount") ? snapshot.GetValue<int>("amount") : 10000;
            UserData.games = snapshot.ContainsField("games") ? snapshot.GetValue<int>("games") : 0;
            UserData.wins = snapshot.ContainsField("wins") ? snapshot.GetValue<int>("wins") : 0;
            UserData.loses = snapshot.ContainsField("loses") ? snapshot.GetValue<int>("loses") : 0;

            Debug.Log($"User Data Loaded: Amount={UserData.amount}, Games={UserData.games}, Wins={UserData.wins}, Loses={UserData.loses}");
        }
        else
        {
            Debug.LogWarning($"Document {snapshot.Id} does not exist! Creating default data...");
            await SaveUserData(10000, 0, 0, 0); // שמירת נתונים ראשונית אם אין מסמך
        }

    }
}