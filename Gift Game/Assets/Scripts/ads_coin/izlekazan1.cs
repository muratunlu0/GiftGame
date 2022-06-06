using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine; using CodeStage.AntiCheat.Storage;
using UnityEngine.UI;
using GoogleMobileAds.Api;

public class izlekazan1 : MonoBehaviour
{
    private int coin_gecici = 0;

    ////////////////////// odullu coin kazan check
    DateTime oldtime;
    TimeSpan travelTime;
    public Text kredi_kazan;
    public Button video_izle_coin_bt;

    public void izle_kazan()
    {
        GameObject.Find("ayarlar").GetComponent<reklam>().reklam_goster_tam_ekran();

        coin_gecici = GameObject.Find("ayarlar").GetComponent<iap>().get_coin() + 1;
        GameObject.Find("firebase").GetComponent<databasee>().coin_updated_firebase(coin_gecici);
        GameObject.Find("ayarlar").GetComponent<iap>().coin_updated(coin_gecici);

        ObscuredPrefs.SetString("izlekazan1.,.,", ConvertToTimestamp(System.DateTime.Now).ToString());

        GameObject.Find("firebase").GetComponent<databasee>().bildirim(1 + " Jeton eklendi");
    }
    private double ConvertToTimestamp(DateTime value)
    {
        TimeSpan span = (value - new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc).ToLocalTime());

        return (double)span.TotalSeconds;
    }
    void Update()
    {
        kredi_kazan_check();
    }
    double gecici_sayac;
    public void kredi_kazan_check()
    {
        if (ObscuredPrefs.GetString("izlekazan1.,.,") != "")
        {
            oldtime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            oldtime = oldtime.AddSeconds(Convert.ToDouble(ObscuredPrefs.GetString("izlekazan1.,.,"))).ToLocalTime();
            travelTime = System.DateTime.Now - oldtime;

            if (travelTime.TotalSeconds > 60 * 60 * 24)
            {
                ObscuredPrefs.SetString("izlekazan1.,.,", "");
            }
            else
            {
                gecici_sayac = 60*60*24 - travelTime.TotalSeconds;

                kredi_kazan.text = ((int)((gecici_sayac / 60))/60).ToString() + ":" + ((int)((gecici_sayac / 60)) % 60).ToString() + ":" + ((int)(gecici_sayac % 60)).ToString();

                video_izle_coin_bt.interactable = false;
            }
        }
        else
        {
            kredi_kazan.text = "izle";
            video_izle_coin_bt.interactable = true;
        }
    }
}
