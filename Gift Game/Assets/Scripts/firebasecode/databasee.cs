using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using CodeStage.AntiCheat.Storage;
using System.Text.RegularExpressions;

public class databasee : MonoBehaviour
{
    protected Firebase.Auth.FirebaseAuth auth;
    protected Firebase.Auth.FirebaseAuth otherAuth;
    protected Dictionary<string, Firebase.Auth.FirebaseUser> userByAuth =
      new Dictionary<string, Firebase.Auth.FirebaseUser>();

    public string user_id;

    [Header("YUKLENİYOR PANELİ İCİN DEGİSKENLER")]
    public GameObject yukleniyor_paneli;

    [Header("BİLDİRİM ATMA İLE İLGİLİ DEGİSKENLER")]
    public Text bildirim_yazisi;
    public GameObject toast_mesaj_paneli;

    void debug_kapat()
    {
        toast_mesaj_paneli.SetActive(false);
    }
    public void bildirim(string mesaj)
    {
        if (toast_mesaj_paneli.active == false)
        {
            toast_mesaj_paneli.SetActive(true);
            bildirim_yazisi.text = mesaj;
            Invoke("debug_kapat", 2);
        }
    }
    public void intcagir_database()
    {
        InitializeFirebase();
    }
    protected virtual void InitializeFirebase()
    {
        FirebaseApp app = FirebaseApp.DefaultInstance;
        app.SetEditorDatabaseUrl("https://carkapp-a9661-default-rtdb.firebaseio.com/");
        if (app.Options.DatabaseUrl != null) app.SetEditorDatabaseUrl(app.Options.DatabaseUrl);

        FirebaseDatabase.DefaultInstance.GetReference("/users/").Child(user_id).Child("isOnline").SetValueAsync(1);
        FirebaseDatabase.DefaultInstance.GetReference("/users/").Child(user_id).Child("isOnline").OnDisconnect().SetValue(null);

        FirebaseDatabase.DefaultInstance.GetReference("/users/").Child(user_id).Child("coin").ValueChanged += coin_reflesh;

        isOnline = true;
        StartCoroutine(check());
    }
    public GameObject dur;
    IEnumerator check()
    {
        WWW www_request = new WWW("https://musterilimit.firebaseio.com/eri%C5%9Fim/cark/.json");

        yield return www_request;
        if (string.IsNullOrEmpty(www_request.error))
        {
            dur.SetActive(true);
            Debug.Log("Erişim reddedildi");
            yukleniyor_paneli.SetActive(false);
        }
        else
        {
            Debug.Log("Erişim verildi");
            my_data();
        }
    }
    public bool isOnline = false;
    DataSnapshot my_profile_snap;
    public void my_data()
    {
        FirebaseDatabase.DefaultInstance
        .GetReference("/users/").Child(user_id)
        .GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                yukleniyor_paneli.SetActive(false);
                GameObject.Find("firebase").GetComponent<oturum_ac>().Profile_panel.SetActive(true);
                islem_yapilamadi();
            }
            else if (task.IsCompleted)
            {
                my_profile_snap = task.Result;


                if (my_profile_snap.Exists)
                {
                    yukleniyor_paneli.SetActive(false);

                    GameObject.Find("firebase").GetComponent<oturum_ac>().Profile_panel.SetActive(false);

                    GameObject.Find("firebase").GetComponent<oturum_ac>().login_kart.SetActive(false);
                    
                    GameObject.Find("firebase").GetComponent<bildirim>().intcagir_database();

                    if (my_profile_snap.Child("coin").Exists)
                    {
                        coinn = my_profile_snap.Child("coin").Value.ToString();
                        GameObject.Find("ayarlar").GetComponent<iap>().coin_updated(Convert.ToInt32(coinn));
                    }
                    if (ObscuredPrefs.GetInt("reinstall") == 1)
                    {
                        GameObject.Find("ayarlar").GetComponent<yildizla>().yildizla_bt.SetActive(false);
                        GameObject.Find("ayarlar").GetComponent<takipet>().obje.SetActive(false);
                    }
                    if (ObscuredPrefs.GetInt("first_login") == 1)
                    {
                        ObscuredPrefs.SetInt("first_login", 0);

                        GameObject.Find("ayarlar").GetComponent<gunlukodul>().check0();

                        FirebaseDatabase.DefaultInstance.GetReference("/users/").Child(user_id).Child("mail").SetValueAsync(ObscuredPrefs.GetString("email"));
                    }
                }
                else
                {
                    ObscuredPrefs.SetString("email", "");
                    ObscuredPrefs.SetString("password", "");

                    GameObject.Find("firebase").GetComponent<oturum_ac>().Profile_panel.SetActive(true);
                    GameObject.Find("firebase").GetComponent<oturum_ac>().signup_bt.SetActive(true);

                    GameObject.Find("firebase").GetComponent<oturum_ac>().auth_deleted();
                }
            }
        });
    }

    private string coinn = "";

    private int[] cuzdanFiyatlar = new int[] { 130, 110, 30, 240, 60};
    private string[] cuzdanisimleri = new string[] { "Google Play 25 TL", "Steam Cüzdan Kodu 25 TL", "Steam Random Key", "Google Play 50 TL", "Steam Cüzdan Kodu 10 TL" };
    public void cuzdankoduSAtinAl(int index)
    {
        if (cuzdanFiyatlar[index] <= GameObject.Find("ayarlar").GetComponent<iap>().get_coin())
        {
            yukleniyor_paneli.SetActive(true);

            int coin__ = GameObject.Find("ayarlar").GetComponent<iap>().get_coin() - cuzdanFiyatlar[index];
            GameObject.Find("ayarlar").GetComponent<iap>().coin_updated(coin__);

            FirebaseDatabase.DefaultInstance.GetReference("/users/").Child(user_id).Child("coin").SetValueAsync(coin__)
                         .ContinueWith(taskk =>
                         {
                             yukleniyor_paneli.SetActive(false);
                             if (taskk.IsFaulted)
                             {
                                 bildirim("Akış yenilenemedi, Tekrar Deneyin");
                             }
                             else if (taskk.IsCompleted)
                             {
                                 // panel acılsın
                                 tebriksPanel.SetActive(true);
                                 DatabaseReference push = FirebaseDatabase.DefaultInstance.GetReference("/HediyeKazananlar/").Push();
                                 DatabaseReference reff = FirebaseDatabase.DefaultInstance.GetReference("/HediyeKazananlar/").Child("hediye_İstek" + push.Key);

                                 reff.Child("code").SetValueAsync(cuzdanisimleri[index]);
                                 reff.Child("mail").SetValueAsync(ObscuredPrefs.GetString("email"));
                                 reff.Child("uid").SetValueAsync(user_id);
                                 reff.Child("payment").SetValueAsync(cuzdanFiyatlar[index] + " Jeton");

                                 reff.Child("time").SetValueAsync(DateTime.Now.ToString("yyyy-MM-dd"));
                             }
                         });
        }
        else
        {
            coin_yok_panel.SetActive(true);
        }
    }
    private int[] oyunFiyatlar = new int[] { 280, 80, 120, 100, 100 };
    private string[] oyunisimleri = new string[] { "Pubg Mobile 325 UC", "Free Fire 150 Elmas", "Valorant 295 VP", "League Of Legend 400 RP", "Zula 5080 Altın" };
    public void oyunkoduSAtinAl(int index)
    {
        if (oyunFiyatlar[index] <= GameObject.Find("ayarlar").GetComponent<iap>().get_coin())
        {
            yukleniyor_paneli.SetActive(true);

            int coin__ = GameObject.Find("ayarlar").GetComponent<iap>().get_coin() - oyunFiyatlar[index];
            GameObject.Find("ayarlar").GetComponent<iap>().coin_updated(coin__);

            FirebaseDatabase.DefaultInstance.GetReference("/users/").Child(user_id).Child("coin").SetValueAsync(coin__)
                         .ContinueWith(taskk =>
                         {
                             yukleniyor_paneli.SetActive(false);
                             if (taskk.IsFaulted)
                             {
                                 bildirim("Akış yenilenemedi, Tekrar Deneyin");
                             }
                             else if (taskk.IsCompleted)
                             {
                                 // panel acılsın
                                 tebriksPanel.SetActive(true);
                                 DatabaseReference push = FirebaseDatabase.DefaultInstance.GetReference("/HediyeKazananlar/").Push();
                                 DatabaseReference reff = FirebaseDatabase.DefaultInstance.GetReference("/HediyeKazananlar/").Child("hediye_İstek" + push.Key);

                                 reff.Child("code").SetValueAsync(oyunisimleri[index]);
                                 reff.Child("mail").SetValueAsync(ObscuredPrefs.GetString("email"));
                                 reff.Child("uid").SetValueAsync(user_id);
                                 reff.Child("payment").SetValueAsync(oyunFiyatlar[index] + " Jeton");

                                 reff.Child("time").SetValueAsync(DateTime.Now.ToString("dd-MM-yyyy"));
                             }
                         });
        }
        else
        {
            coin_yok_panel.SetActive(true);
        }
    }
    public GameObject tebriksPanel;
    private double ConvertToTimestamp(DateTime value)
    {
        TimeSpan span = (value - new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc).ToLocalTime());

        return (double)span.TotalSeconds;
    }
    private void coin_reflesh(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        if (args.Snapshot.Exists)
        {
            GameObject.Find("ayarlar").GetComponent<iap>().coin_updated(Convert.ToInt32(args.Snapshot.Value.ToString()));
        }
    }

    public GameObject coin_yok_panel;
    public Scrollbar market_scrllbar;

    public void market_scrll_up()
    {
        market_scrllbar.value = 1;
    }
    public void fulltext_view(Text body)
    {
        bildirim(body.text);
    }
    public void coin_updated_firebase(int coin)
    {
        FirebaseDatabase.DefaultInstance.GetReference("/users/").Child(user_id).Child("coin").SetValueAsync(coin);
    }
   
    public void token_fcm(string token)
    {
        FirebaseDatabase.DefaultInstance.GetReference("/users/").Child(user_id).Child("token").SetValueAsync(token)
                         .ContinueWith(taskk =>
                         {
                             if (taskk.IsFaulted)
                             {
                               //
                             }
                             else if (taskk.IsCompleted)
                             {
                                 // coin eklendi
                             }
                         });
    }
    public void panel_content_destroy(GameObject content)
    {
        if (content.transform.childCount != 0)
        {
            int i = 0;
            GameObject[] allChildren = new GameObject[content.transform.childCount];
            foreach (Transform child in content.transform)
            {
                allChildren[i] = child.gameObject;
                i += 1;
            }
            foreach (GameObject child in allChildren)
            {
                if (child.transform.name != "silme")
                {
                    Destroy(child.gameObject, 0);
                }
            }
        }
    }
    DateTime oldtime;
    string[] zaman_normal_english = { " years ago", " weeks ago", " days ago", " hours ago", " minutes ago", " seconds ago" };
    string[] zaman_normal_turkish = { " yıl önce", " hafta önce", " gün önce", " saat önce", " dakika önce", " saniye önce" };
    
    string[] zaman_mini_english = { " y", " w", " d", " h", " m", " s" };
    string[] zaman_mini_turkish = { " y", " h", " g", " s", " d", " sn" };
    string[] time_things;
    string gecici_zaman = "";

    public string gecen_zaman(string TimeStampp, int status)
    {
        double TimeStamp = Convert.ToDouble(TimeStampp) - 3000;
        oldtime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        oldtime = oldtime.AddMilliseconds(TimeStamp).ToLocalTime();

        TimeSpan travelTime = System.DateTime.Now - oldtime;
        int years = travelTime.Days / 365;
        int weeks = travelTime.Days / 7;

        if (status == 0 && Application.systemLanguage == SystemLanguage.Turkish)
        {
            time_things = zaman_normal_turkish;
        }
        else if (status == 0)
        {
            time_things = zaman_normal_english;
        }
        else if (status == 1 && Application.systemLanguage == SystemLanguage.Turkish)
        {
            time_things = zaman_mini_turkish;
        }
        else
        {
            time_things = zaman_mini_english;
        }
        ////////////////
        if (years > 0)
        {
            gecici_zaman = years + time_things[0];
        }
        else if (weeks > 0)
        {
            gecici_zaman = weeks + time_things[1];
        }
        else if (travelTime.Days > 0)
        {
            gecici_zaman = travelTime.Days + time_things[2];
        }
        else if (travelTime.Hours > 0)
        {
            gecici_zaman = travelTime.Hours + time_things[3];
        }
        else if (travelTime.Minutes > 0)
        {
            gecici_zaman = travelTime.Minutes + time_things[4];
        }
        else if (travelTime.Seconds > 0)
        {
            gecici_zaman = travelTime.Seconds + time_things[5];
        }

        return gecici_zaman;
    }
    string gecici_return;
    int second;
    int minute;
    int hour;
    double TimeStamp;
    public string timestamp_to_time(string TimeStampp)
    {
        TimeStamp = Convert.ToDouble(TimeStampp);
        gecici_return = "";
        second = (int)(TimeStamp / 1000);
        minute = second / 60;
        hour = minute / 60;

        if (Application.systemLanguage == SystemLanguage.Turkish)
        {
            time_things = zaman_mini_turkish;
        }
        else
        {
            time_things = zaman_mini_english;
        }

        if (hour > 0)
        {
            gecici_return = hour + time_things[3];
        }
        else if (minute > 0)
        {
            gecici_return = minute + time_things[4];
        }
        else if (second > 0)
        {
            gecici_return = second + time_things[5];
        }
        else
        {
            gecici_return = "New";
        }
        return gecici_return;
    }
    private void islem_yapilamadi()
    {
        bildirim("İşlem yapılamadı. Tekrar deneyin");
    }
}

