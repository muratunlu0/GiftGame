using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class takipet1 : MonoBehaviour
{
    public GameObject obje;
    private void Start()
    {
        if (PlayerPrefs.GetInt("youtube") == 0)
        {
            obje.SetActive(true);
        }
        else
        {
            obje.SetActive(false);
        }
    }
    public void func()
    {
        Application.OpenURL("https://www.youtube.com/c/TEKNOBEY159");
        PlayerPrefs.SetInt("youtube", 1);
        obje.SetActive(false);
        Invoke("do_it",2);
        do_it();
    }
    private int coin_gecici = 0;
    int coin_miktar = 1;

    private void do_it()
    {
        coin_gecici = GameObject.Find("ayarlar").GetComponent<iap>().get_coin() + coin_miktar;

        GameObject.Find("firebase").GetComponent<databasee>().coin_updated_firebase(coin_gecici);
        GameObject.Find("ayarlar").GetComponent<iap>().coin_updated(coin_gecici);

        GameObject.Find("firebase").GetComponent<databasee>().bildirim(coin_miktar + " Coin eklendi");
    }
}
