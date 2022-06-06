using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeStage.AntiCheat.Storage;
public class gunlukodul : MonoBehaviour
{
    public GameObject[] gunler;
    private int[] gun_oduller = new int[] {1, 1, 1, 1, 1, 1, 2};

    public GameObject gunluk_odul_panel;
    public void check0()
    {
        if (ObscuredPrefs.GetInt("gecicigunlukk14") == 0)
        {
            ObscuredPrefs.SetInt("gecicigunlukk14", 1);
            ObscuredPrefs.SetInt("gunluk_hangi_gun", 1);

            ObscuredPrefs.SetString("pre1_access", (ConvertToTimestamp(System.DateTime.Now)).ToString());
            ObscuredPrefs.SetString("pre2_access", (ConvertToTimestamp(System.DateTime.Now)).ToString());
            ObscuredPrefs.SetString("pre3_access", (ConvertToTimestamp(System.DateTime.Now)).ToString());
            ObscuredPrefs.SetString("pre4_access", (ConvertToTimestamp(System.DateTime.Now)).ToString());

            ObscuredPrefs.SetString("gunluk_odul", ConvertToTimestamp(System.DateTime.Now).ToString());
            check(true);
        }
        else
        {
            check(false);
        }
    }
    private void check(bool status)
    {
        if ((Convert.ToDouble(ObscuredPrefs.GetString("gunluk_odul")) + 24 * 60 * 60 < ConvertToTimestamp(System.DateTime.Now)) || status == true)
        {
            gunluk_odul_panel.SetActive(true);
            
            for (int i = 0; i < 7; i++)
            {
                //////////////////////
                if (ObscuredPrefs.GetInt("gunluk_hangi_gun") >= i + 1)
                {
                    if (ObscuredPrefs.GetInt("gunluk_hangi_gun") == i + 1)
                    {
                        gunler[i].SetActive(true);
                    }
                    //////////////////
                    gunler[i].SetActive(true);
                }
                else
                {
                    gunler[i].SetActive(false);
                }
            }
        }
    }
    private int coin_gecici = 0;
    public void odulu_al()
    {
        gunluk_odul_panel.SetActive(false);
        coin_gecici = GameObject.Find("ayarlar").GetComponent<iap>().get_coin() + gun_oduller[ObscuredPrefs.GetInt("gunluk_hangi_gun") - 1];

        GameObject.Find("firebase").GetComponent<databasee>().coin_updated_firebase(coin_gecici);
        GameObject.Find("ayarlar").GetComponent<iap>().coin_updated(coin_gecici);

        GameObject.Find("firebase").GetComponent<databasee>().bildirim(gun_oduller[ObscuredPrefs.GetInt("gunluk_hangi_gun") - 1] + " Jeton eklendi");

        if (ObscuredPrefs.GetInt("gunluk_hangi_gun") < 7)
        {
            ObscuredPrefs.SetInt("gunluk_hangi_gun", ObscuredPrefs.GetInt("gunluk_hangi_gun") + 1);
        }
        else if (ObscuredPrefs.GetInt("gunluk_hangi_gun") == 7)
        {
            ObscuredPrefs.SetInt("gunluk_hangi_gun", 1);
        }
        ObscuredPrefs.SetString("gunluk_odul", ConvertToTimestamp(System.DateTime.Now).ToString());
    }
    private double ConvertToTimestamp(DateTime value)
    {
        TimeSpan span = (value - new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc).ToLocalTime());

        return (double)span.TotalSeconds;
    }
}
