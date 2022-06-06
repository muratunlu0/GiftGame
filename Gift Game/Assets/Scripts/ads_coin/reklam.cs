using UnityEngine; using CodeStage.AntiCheat.Storage;
using System;
using GoogleMobileAds.Api;
public class reklam : MonoBehaviour
{
    private InterstitialAd reklamObjesi;

    private BannerView reklamObjesi_banner;
    private BannerView reklamObjesi_banner_alt;
    void OnDestroy()
    {
        if (reklamObjesi_banner != null)
            reklamObjesi_banner.Destroy();
    }

//    public void banner_kapat()
//    {
//#if !UNITY_EDITOR && UNITY_ANDROID
//        reklamObjesi_banner.Hide();
//#endif
//    }

//    public void banner_ac()
//    {
//#if !UNITY_EDITOR && UNITY_ANDROID
//            if (reklamObjesi_banner != null)
//            {
//                reklamObjesi_banner.Show();
//            }
//            else
//            {
//                reklamObjesi_banner = new BannerView("ca-app-pub-6104591608224842/8318902203", new AdSize(AdSize.FullWidth, 650), AdPosition.Center);  //new BannerView("ca-app-pub-6104591608224842/8318902203", new AdSize(AdSize.FullWidth, 60), AdPosition.Bottom);
//                AdRequest reklamIstegi = new AdRequest.Builder().Build();
//                reklamObjesi_banner.LoadAd(reklamIstegi);
//            }
//#endif
//    }
//    public void banner_kapat_alt()
//    {
//#if !UNITY_EDITOR && UNITY_ANDROID
//       if (ObscuredPrefs.GetInt("haftalikad") == 0)
//        {
//        reklamObjesi_banner_alt.Hide();
//        }
//#endif
//    }
//    public void banner_ac_alt()
//    {
//#if !UNITY_EDITOR && UNITY_ANDROID
//        if (ObscuredPrefs.GetInt("haftalikad") == 0)
//        {
//            if (reklamObjesi_banner_alt != null)
//            {
//                reklamObjesi_banner_alt.Show();
//            }
//            else
//            {
//                reklamObjesi_banner_alt = new BannerView("ca-app-pub-6104591608224842/8318902203", new AdSize(AdSize.FullWidth, 60), AdPosition.Bottom);
//                AdRequest reklamIstegi = new AdRequest.Builder().Build();
//                reklamObjesi_banner_alt.LoadAd(reklamIstegi);
//            }
//        }
//#endif
//    }
    /// ////////////////////////////////////////////////
    void Start()
    {
        MobileAds.Initialize(reklamDurumu => { });
        YeniReklamAl(null, null);
    }
    /// gecis reklami göstermek için bu fonksiyonu çagirin
    public void reklam_goster_tam_ekran()
    {
        if (reklamObjesi.IsLoaded())
        {
            reklamObjesi.Show();
        }
    }
    public void YeniReklamAl(object sender, EventArgs args)
    {
        if (reklamObjesi != null)
            reklamObjesi.Destroy();

        reklamObjesi = new InterstitialAd("ca-app-pub-6647374994520041/5608274078");
        reklamObjesi.OnAdClosed += YeniReklamAl; // Kullanıcı reklamı kapattıktan sonra çağrılır

        AdRequest reklamIstegi = new AdRequest.Builder().Build();
        reklamObjesi.LoadAd(reklamIstegi);
    }
}
