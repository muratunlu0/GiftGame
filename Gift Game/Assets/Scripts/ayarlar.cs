using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using CodeStage.AntiCheat.Storage;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Text;

public class ayarlar : MonoBehaviour
{
    public GameObject uploading;

    private void Awake()
    {
        Application.targetFrameRate = 100;
    }

    public void PrivacyPolicy()
    {
        Application.OpenURL("https://www.freeprivacypolicy.com/live/9b1ef7c0-fb47-409c-9207-9847b75820b3");
    }
    string MyEscapeURL(string url)
    {
        return WWW.EscapeURL(url).Replace("+", "%20");
    }
    public void muraad()
    {
        Application.OpenURL("https://www.instagram.com/muratunlu0/");
    }
}
