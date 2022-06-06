using System.Collections.Generic;
using UnityEngine; 
using CodeStage.AntiCheat.Storage;
using UnityEngine.UI;
using System;
public class iap : MonoBehaviour
{
    void Start()
    {
        coin_market.text = "0";
    }

    public int get_coin()
    {
        return coin_;
    }

    public void coin_updated(int coinn)
    {
        coin_ = coinn;
        coin_market.text = coin_.ToString();
    }
    public Text coin_market;
    private int coin_;

    private void bildirimm(int count)
    {
        GameObject.Find("firebase").GetComponent<databasee>().bildirim(count + " Jeton eklendi");
    }
    private double ConvertToTimestamp(DateTime value)
    {
        TimeSpan span = (value - new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc).ToLocalTime());

        return (double)span.TotalSeconds;
    }

    public void coin_sfiirla()
    {
        GameObject.Find("firebase").GetComponent<databasee>().coin_updated_firebase(5);
    }
}
