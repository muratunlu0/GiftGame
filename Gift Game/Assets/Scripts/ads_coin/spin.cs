using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine; using CodeStage.AntiCheat.Storage;
using UnityEngine.UI;
using GoogleMobileAds.Api;

public class spin : MonoBehaviour
{
    public int hangi_odul;

    private RewardBasedVideoAd reklamObjesi_odullu_spin;
    private int coin_gecici = 0;

    ////////////////////// odullu coin kazan check
    DateTime oldtime;
    TimeSpan travelTime;
    public Text sayac;
    public Button video_izle_coin_bt;

    public GameObject spin_panel;
    public Text spin_body;

    void Start()
    {
        MobileAds.Initialize(reklamDurumu => { });
        reklamObjesi_odullu_spin = RewardBasedVideoAd.Instance;
        reklamObjesi_odullu_spin.OnAdClosed += YeniReklamAl_odullu_spin; // Kullanıcı reklamı kapattıktan sonra çağrılır
        reklamObjesi_odullu_spin.OnAdRewarded += Rewarded_odullu_spin; // Kullanıcı reklamı tamamen izledikten sonra çağrılır
        YeniReklamAl_odullu_spin(null, null);
    }
    public void reklam_goster_odullu()
    {
        hangi_odul = 1;
        reklamObjesi_odullu_spin.Show();
    }
    public void YeniReklamAl_odullu_spin(object sender, EventArgs args)
    {
        AdRequest reklamIstegi = new AdRequest.Builder().Build();
        reklamObjesi_odullu_spin.LoadAd(reklamIstegi, "ca-app-pub-6647374994520041/6320033593");
    }

    private void Rewarded_odullu_spin(object sender, Reward odul)
    {
        if (hangi_odul == 1)
        {
            int random = 1;

            coin_gecici = GameObject.Find("ayarlar").GetComponent<iap>().get_coin() + random;
            GameObject.Find("firebase").GetComponent<databasee>().coin_updated_firebase(coin_gecici);
            GameObject.Find("ayarlar").GetComponent<iap>().coin_updated(coin_gecici);

            ObscuredPrefs.SetString("spin", ConvertToTimestamp(System.DateTime.Now).ToString());

            spin_body.text = "<color=#C3C23C>" + random + "</color> Jeton kazandın";
            spin_panel.SetActive(true);
        }
    }
    private double ConvertToTimestamp(DateTime value)
    {
        TimeSpan span = (value - new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc).ToLocalTime());

        return (double)span.TotalSeconds;
    }
    void Update()
    {
        sayac_check();
    }
    double gecici_sayac;
    public void sayac_check()
    {
        if (ObscuredPrefs.GetString("spin") != "")
        {
            oldtime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            oldtime = oldtime.AddSeconds(Convert.ToDouble(ObscuredPrefs.GetString("spin"))).ToLocalTime();
            travelTime = System.DateTime.Now - oldtime;

            if (travelTime.TotalSeconds > 60 * 60 * 24)
            {
                ObscuredPrefs.SetString("spin", "");
            }
            else
            {
                gecici_sayac = 60 * 60 * 24 - travelTime.TotalSeconds;

                sayac.text = ((int)((gecici_sayac / 60)) / 60).ToString() + ":" + ((int)((gecici_sayac / 60)) % 60).ToString() + ":" + ((int)(gecici_sayac % 60)).ToString();

                video_izle_coin_bt.interactable = false;
            }
        }
        else
        {
            sayac.text = "Çevir";
            video_izle_coin_bt.interactable = true;
        }
    }
}
