using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;
using UnityEngine; using CodeStage.AntiCheat.Storage;
using UnityEngine.UI;
public class oturum_ac : MonoBehaviour {

    protected Firebase.Auth.FirebaseAuth auth;
    protected Firebase.Auth.FirebaseAuth otherAuth;
    protected Dictionary<string, Firebase.Auth.FirebaseUser> userByAuth =
      new Dictionary<string, Firebase.Auth.FirebaseUser>();

    private string logText = "";
    protected string displayName = "";
    private bool fetchingToken = false;
    public bool usePasswordInput = false;
    private Vector2 scrollViewVector = Vector2.zero;

    public GameObject yukleniyor;

    public InputField email_;
    public InputField password_;
    public GameObject Profile_panel;

    public GameObject login_bt;
    public GameObject signup_bt;

    public GameObject login_kart;

    Firebase.Auth.FirebaseUser user;

    public void email_check()
    {
        email_.text = email_.text.Replace(" ", string.Empty);
    }

    private Firebase.AppOptions otherAuthOptions = new Firebase.AppOptions
    {
        ApiKey = "",
        AppId = "",
        ProjectId = ""
    };

    const int kMaxLogSize = 16382;
    Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;

    public virtual void Start()
    {
        login_bt.SetActive(false);
        signup_bt.SetActive(true);

        if (ObscuredPrefs.GetString("email") != "" && ObscuredPrefs.GetString("password") != "")
        {
            email_.text = ObscuredPrefs.GetString("email");
            password_.text = ObscuredPrefs.GetString("password");
            giris_yap_email();
        }
        else
        {
            Profile_panel.SetActive(true);
        }
    }
    protected void InitializeFirebase()
    {
        DebugLog("Setting up Firebase Auth");
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        auth.IdTokenChanged += IdTokenChanged;
        
        // Specify valid options to construct a secondary authentication object.
        if (otherAuthOptions != null &&
            !(String.IsNullOrEmpty(otherAuthOptions.ApiKey) ||
              String.IsNullOrEmpty(otherAuthOptions.AppId) ||
              String.IsNullOrEmpty(otherAuthOptions.ProjectId)))
        {
            try
            {
                otherAuth = Firebase.Auth.FirebaseAuth.GetAuth(Firebase.FirebaseApp.Create(
                  otherAuthOptions, "Secondary"));
                otherAuth.StateChanged += AuthStateChanged;
                otherAuth.IdTokenChanged += IdTokenChanged;
            }
            catch (Exception)
            {
                DebugLog("ERROR: Failed to initialize secondary authentication object.");
            }
        }
        GetUserInfo();
        AuthStateChanged(this, null);
    }
    // Output text to the debug log text field, as well as the console.
    public void DebugLog(string s)
    {
        Debug.Log(s);
        logText += s + "\n";

        while (logText.Length > kMaxLogSize)
        {
            int index = logText.IndexOf("\n");
            logText = logText.Substring(index + 1);
        }
        scrollViewVector.y = int.MaxValue;
    }

    // Display user information.
    void DisplayUserInfo(Firebase.Auth.IUserInfo userInfo, int indentLevel)
    {
        string indent = new String(' ', indentLevel * 2);
        var userProperties = new Dictionary<string, string> {
      {"Display Name", userInfo.DisplayName},
      {"Email", userInfo.Email},
      {"Photo URL", userInfo.PhotoUrl != null ? userInfo.PhotoUrl.ToString() : null},
      {"Provider ID", userInfo.ProviderId},
      {"User ID", userInfo.UserId}
    };
        foreach (var property in userProperties)
        {
            if (!String.IsNullOrEmpty(property.Value))
            {
                DebugLog(String.Format("{0}{1}: {2}", indent, property.Key, property.Value));
            }
        }
    }

    // Display a more detailed view of a FirebaseUser.
    void DisplayDetailedUserInfo(Firebase.Auth.FirebaseUser user, int indentLevel)
    {
        DisplayUserInfo(user, indentLevel);
        DebugLog("  Anonymous: " + user.IsAnonymous);
        DebugLog("  Email Verified: " + user.IsEmailVerified);
        var providerDataList = new List<Firebase.Auth.IUserInfo>(user.ProviderData);
        if (providerDataList.Count > 0)
        {
            DebugLog("  Provider Data:");
            foreach (var providerData in user.ProviderData)
            {
                DisplayUserInfo(providerData, indentLevel + 1);
            }
        }
    }

    // Track state changes of the auth object.
    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        Firebase.Auth.FirebaseAuth senderAuth = sender as Firebase.Auth.FirebaseAuth;
        Firebase.Auth.FirebaseUser user = null;
        if (senderAuth != null) userByAuth.TryGetValue(senderAuth.App.Name, out user);
        if (senderAuth == auth && senderAuth.CurrentUser != user)
        {
            bool signedIn = user != senderAuth.CurrentUser && senderAuth.CurrentUser != null;
            if (!signedIn && user != null)
            {
                DebugLog("Signed out " + user.UserId);
            }
            user = senderAuth.CurrentUser;
            userByAuth[senderAuth.App.Name] = user;
            if (signedIn)
            {
                DebugLog("Signed in " + user.UserId);
                displayName = user.DisplayName ?? "";
                DisplayDetailedUserInfo(user, 1);
            }
        }
    }
    // Track ID token changes.
    void IdTokenChanged(object sender, System.EventArgs eventArgs)
    {
        Firebase.Auth.FirebaseAuth senderAuth = sender as Firebase.Auth.FirebaseAuth;
        if (senderAuth == auth && senderAuth.CurrentUser != null && !fetchingToken)
        {
            senderAuth.CurrentUser.TokenAsync(false).ContinueWith(
              task => DebugLog(String.Format("Token[0:8] = {0}", task.Result.Substring(0, 8))));
        }
    }

    // Log the result of the specified task, returning true if the task
    // completed successfully, false otherwise.
    bool LogTaskCompletion(Task task, string operation)
    {
        bool complete = false;
        if (task.IsCanceled)
        {
            DebugLog(operation + " canceled.");
        }
        else if (task.IsFaulted)
        {
            DebugLog(operation + " encounted an error.");
            foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
            {
                string authErrorCode = "";
                Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
                if (firebaseEx != null)
                {
                    authErrorCode = String.Format("AuthError.{0}: ",
                      ((Firebase.Auth.AuthError)firebaseEx.ErrorCode).ToString());
                }
                DebugLog(authErrorCode + exception.ToString());
            }
        }
        else if (task.IsCompleted)
        {
            DebugLog(operation + " completed");
            complete = true;
        }
        return complete;
    }

    public void kaydol_email()
    {
        yukleniyor.SetActive(true);
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.CreateUserWithEmailAndPasswordAsync(email_.text, password_.text).ContinueWith(task =>
        {
            yukleniyor.SetActive(false);
            if (task.IsCanceled)
            {
                islem_yapilamadi();
                return;
            }
            if (task.IsFaulted)
            {
                Debug.Log(task.Exception);
                if (task.Exception.ToString().Contains("The email address is already in use by another account"))
                {
                    bildirim("E-postaya ait bir hesap zaten mevcut");
                    login_bt.SetActive(true);
                    signup_bt.SetActive(false);
                }
                else if (task.Exception.ToString().Contains("The given password is invalid"))
                {
                    bildirim("Şifre en az 6 karakterden oluşmalıdır");
                }
                else
                {
                    bildirim("Üyelik başarısız oldu");
                }
                return;
            }
            login_bt.SetActive(true);
            signup_bt.SetActive(false);
            ObscuredPrefs.SetInt("first_login", 1);
            ObscuredPrefs.SetInt("reinstall", 0);
            giris_yap_email();
        });
    }

    [Header("BİLDİRİM ATMA İLE İLGİLİ DEGİSKENLER")]
    public Text bildirim_yazisi;
    public GameObject toast_mesaj_paneli;

    void debug_kapat()
    {
        toast_mesaj_paneli.SetActive(false);
    }
    public void bildirim(string mesaj)
    {
        toast_mesaj_paneli.SetActive(true);
        bildirim_yazisi.text = mesaj;
        Invoke("debug_kapat", 2);
    }
    public void butonla_giris()
    {
        ObscuredPrefs.SetInt("reinstall", 1);
        giris_yap_email();
    }
    public void giris_yap_email()
    {
        if (email_.text == "" || password_.text == "")
        {
            bildirim("Boşlukları doldurun");
        }
        else if (email_.text != "" && password_.text != "")
        {
            yukleniyor.SetActive(true);
            auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
            Firebase.Auth.Credential credential =
         Firebase.Auth.EmailAuthProvider.GetCredential(email_.text, password_.text);
            auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    yukleniyor.SetActive(false);
                    Profile_panel.SetActive(true);
                    islem_yapilamadi();
                    return;
                }
                if (task.IsFaulted)
                {
                    yukleniyor.SetActive(false);
                    Profile_panel.SetActive(true);
                    Debug.Log("SignInWithCredentialAsync encountered an error: " + task.Exception);
                    
                    if (task.Exception.ToString().Contains("The password is invalid or the user does not have a password"))
                    {
                        bildirim("Sanırım şifreni unuttun :/");
                    }
                    else if (task.Exception.ToString().Contains("There is no user record corresponding to this identifier. The user may have been deleted"))
                    {
                        bildirim("Böyle bir kullanıcı yok :(");
                    }
                    else if (task.Exception.ToString().Contains("The email address is badly formatted"))
                    {
                        bildirim("E-postanızı doğru girmelisiniz");
                    }
                    else
                    {
                        islem_yapilamadi();
                    }
                    return;
                }

                user = task.Result;
                InitializeFirebase();

                ObscuredPrefs.SetString("email", email_.text);
                ObscuredPrefs.SetString("password", password_.text);

                GameObject.Find("firebase").GetComponent<databasee>().user_id = user.UserId;
                GameObject.Find("firebase").GetComponent<databasee>().intcagir_database();
            });
        }
    }
    public void email_sifre_sifirla()
    {
        if (email_.text == "")
        {
            bildirim("Sıfırlamak istediğiniz e-postayı girmelisiniz");
        }
        else
        {
            yukleniyor.SetActive(true);
            auth.SendPasswordResetEmailAsync(email_.text).ContinueWith(task => {
                yukleniyor.SetActive(false);
                if (task.IsCanceled)
                {
                    islem_yapilamadi();
                    Debug.LogError("SendPasswordResetEmailAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    islem_yapilamadi();
                    Debug.LogError("SendPasswordResetEmailAsync encountered an error: " + task.Exception);
                    return;
                }
                bildirim("Parola sıfırlama e-postası başarıyla gönderildi");
            });
        }
    }
    public void auth_deleted()
    {
        Firebase.Auth.FirebaseUser user = auth.CurrentUser;
        if (user != null)
        {
            user.DeleteAsync().ContinueWith(task => {
                yukleniyor.SetActive(false);
                if (task.IsCanceled)
                {
                    Debug.LogError("DeleteAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("DeleteAsync encountered an error: " + task.Exception);
                    return;
                }
                bildirim("Kullanıcı silindi");
            });
        }
    }
    void HandleSigninResult(Task<Firebase.Auth.FirebaseUser> authTask)
    {
        LogTaskCompletion(authTask, "Sign-in");
    }

    void GetUserInfo()
    {
        if (auth.CurrentUser == null)
        {
            DebugLog("Not signed in, unable to get info.");
        }
        else
        {
            DebugLog("Current user info:");
            
            DisplayDetailedUserInfo(auth.CurrentUser, 1);
        }
    }
    public void SignOut()
    {
        DebugLog("Signing out.");
        auth.SignOut();
    }
    public Task DeleteUserAsync()
    {
        if (auth.CurrentUser != null)
        {
            DebugLog(String.Format("Attempting to delete user {0}...", auth.CurrentUser.UserId));
            return auth.CurrentUser.DeleteAsync().ContinueWith(HandleDeleteResult);
        }
        else
        {
            DebugLog("Sign-in before deleting user.");
            // Return a finished task.
            return Task.FromResult(0);
        }
    }

    void HandleDeleteResult(Task authTask)
    {
        LogTaskCompletion(authTask, "Delete user");
    }
    public void hesabısil()
    {
        DeleteUserAsync();
    }
    private void islem_yapilamadi()
    {
        bildirim("İşlem yapılamadı. Tekrar deneyin");
    }
}
