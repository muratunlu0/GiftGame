using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeStage.AntiCheat.Storage;

public class yildizla : MonoBehaviour
{
    public GameObject yildizla_bt;

    void Start()
    {
        if (ObscuredPrefs.GetInt("yildizla") == 0)
        {
            yildizla_bt.SetActive(true);
        }
        else
        {
            yildizla_bt.SetActive(false);
        }
    }
    private int coin_gecici = 0;
    public void puanla(int index)
    {
        if (index > 2)
        {
            Application.OpenURL("https://play.google.com/store/apps/details?id=com.appx.carkproject");

            do_it();
        }
        else if (index > -1)
        {
            do_it();
        }
        else if (index == -1)
        {
           // GameObject.Find("firebase").GetComponent<databasee>().bildirim("You did not rate us :)");
        }
    }
    private void do_it()
    {
        yildizla_bt.SetActive(false);
        ObscuredPrefs.SetInt("yildizla", 1);

        coin_gecici = GameObject.Find("ayarlar").GetComponent<iap>().get_coin() + 2;

        GameObject.Find("firebase").GetComponent<databasee>().coin_updated_firebase(coin_gecici);
        GameObject.Find("ayarlar").GetComponent<iap>().coin_updated(coin_gecici);

        GameObject.Find("firebase").GetComponent<databasee>().bildirim("2 Jeton eklendi");
    }
}
