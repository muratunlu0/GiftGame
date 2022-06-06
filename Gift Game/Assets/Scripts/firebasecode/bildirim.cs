using UnityEngine;
using UnityEngine.UI;
public class bildirim : MonoBehaviour
{
    public void intcagir_database()
    {
        InitializeFirebase();
    }
    void InitializeFirebase()
    {
        Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
        Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
        Debug.Log("Firebase Messaging Initialized");
    }
    public void Subscribe(string userid)
    {
        Firebase.Messaging.FirebaseMessaging.Subscribe(userid);
    }
    public void UnSubscribe(string userid)
    {
        Firebase.Messaging.FirebaseMessaging.Unsubscribe(userid);
    }
    public virtual void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
    {
       Debug.Log("Received a new message");
    }
    public virtual void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
    {
        Debug.Log("Received Registration Token: " + token.Token);
        GameObject.Find("firebase").GetComponent<databasee>().token_fcm(token.Token);
    }

    public void OnDestroy()
    {
        Firebase.Messaging.FirebaseMessaging.MessageReceived -= OnMessageReceived;
        Firebase.Messaging.FirebaseMessaging.TokenReceived -= OnTokenReceived;
    }
}

