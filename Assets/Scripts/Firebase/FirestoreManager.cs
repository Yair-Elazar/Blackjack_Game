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
        FirebaseUser user = FirebaseManager.Instance.GetCurrentUser();

        if (user == null)
        {
            Debug.LogError("User not logged in!");
            return;
        }

        string userId = user.UserId;
        // Reference to the user's rounds subcollection
        DocumentReference userDocRef = db.Collection("games").Document(userId);
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
}