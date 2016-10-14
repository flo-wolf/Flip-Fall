using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
namespace admob
{
	public class Admob {
        public delegate void AdmobEventHandler(string eventName, string msg);

        public event AdmobEventHandler bannerEventHandler;
        public event AdmobEventHandler interstitialEventHandler;
        public event AdmobEventHandler rewardedVideoEventHandler;
        public event AdmobEventHandler nativeBannerEventHandler;
        private static Admob _instance;	
	
		public static Admob Instance()
	    {
	        if(_instance == null)
	        {
	            _instance = new Admob();
				_instance.preInitAdmob ();
	        }
	        return _instance;
	    }
        
		#if UNITY_EDITOR
		private void preInitAdmob()
		{

		}

		public void initAdmob(string bannerID, string fullID)
		{
		Debug.Log("calling initAdmob");
		}


		public void showBannerAbsolute(AdSize size, int x, int y,string instanceName="defaultBanner")
		{
		Debug.Log("calling showBannerAbsolute");
		}


		public void showBannerRelative(AdSize size, int position, int marginY,string instanceName="defaultBanner")
		{
		Debug.Log("calling showBannerRelative");
		}


		public void removeBanner(string instanceName="defaultBanner")
		{
		Debug.Log("calling removeBanner");
		}


		public void loadInterstitial()
		{
		Debug.Log("calling loadInterstitial");
		}


		public bool isInterstitialReady()
		{
		Debug.Log("calling isInterstitialReady");
		return false;
		}


		public void showInterstitial()
		{
		Debug.Log("calling showInterstitial");
		}

		public void loadRewardedVideo(string rewardedVideoID)
		{
		Debug.Log("calling loadRewardedVideo");
		}
		public bool isRewardedVideoReady()
		{
		Debug.Log("calling isRewardedVideoReady");
		return false;
		}
		public void showRewardedVideo()
		{
		Debug.Log("calling showRewardedVideo");
		}

		public void setTesting(bool v)
		{
		Debug.Log("calling setTesting");
		}


		public void setForChildren(bool v)
		{
		Debug.Log("calling setForChildren");
		}
		public void showNativeBannerRelative(AdSize size, int position, int marginY,string nativeBannerID, string instanceName = "defaultNativeBanner")
		{
		Debug.Log("calling showNativeBannerRelative");
		}
		public void showNativeBannerAbsolute(AdSize size, int x, int y, string nativeBannerID,string instanceName = "defaultNativeBanner")
		{
		Debug.Log("calling showNativeBannerAbsolute");
		}
		public void removeNativeBanner(string instanceName = "defaultNativeBanner")
		{
		Debug.Log("calling removeNativeBanner");
		}

        #elif UNITY_IOS
        internal delegate void AdmobAdCallBack(string adtype, string eventName, string msg);
        private void preInitAdmob()
        {

        }
        [DllImport("__Internal")]
        private static extern void _kminitAdmob(string bannerid, string fullid, AdmobAdCallBack callback);
        public void initAdmob(string bannerID, string fullID)
        {
            _kminitAdmob(bannerID, fullID, onAdmobEventCallBack);
        }

        [DllImport("__Internal")]
        private static extern void _kmshowNativeBannerAbsolute(int width, int height, int x, int y,string nativeID, string instanceName);
        public void showNativeBannerAbsolute(AdSize size, int x, int y,string nativeID, string instanceName = "defaultNativeBanner")
        {
            _kmshowNativeBannerAbsolute(size.Width, size.Height, x, y,nativeID, instanceName);
        }

        [DllImport("__Internal")]
        private static extern void _kmshowNativeBannerRelative(int width, int height, int position, int marginY, string nativeID, string instanceName);
        public void showNativeBannerRelative(AdSize size, int position, int marginY, string nativeID, string instanceName = "defaultNativeBanner")
        {
            _kmshowNativeBannerRelative(size.Width, size.Height, position, marginY,nativeID, instanceName);
        }

        [DllImport("__Internal")]
        private static extern void _kmremoveNativeBanner(string instanceName);
        public void removeNativeBanner(string instanceName = "defaultNativeBanner")
        {
            _kmremoveNativeBanner(instanceName);
        }

        [DllImport("__Internal")]
        private static extern void _kmshowBannerAbsolute(int width, int height, int x, int y,string instanceName);
        public void showBannerAbsolute(AdSize size, int x, int y,string instanceName="defaultBanner")
        {
            _kmshowBannerAbsolute(size.Width, size.Height, x, y,instanceName);
        }

        [DllImport("__Internal")]
        private static extern void _kmshowBannerRelative(int width, int height, int position, int marginY,string instanceName);
        public void showBannerRelative(AdSize size, int position, int marginY,string instanceName="defaultBanner")
        {
            _kmshowBannerRelative(size.Width, size.Height, position, marginY,instanceName);
        }

        [DllImport("__Internal")]
        private static extern void _kmremoveBanner(string instanceName);
        public void removeBanner(string instanceName="defaultBanner")
        {
            _kmremoveBanner(instanceName);
        }

        [DllImport("__Internal")]
        private static extern void _kmloadInterstitial();
        public void loadInterstitial()
        {
            _kmloadInterstitial();
        }

        [DllImport("__Internal")]
        private static extern bool _kmisInterstitialReady();
        public bool isInterstitialReady()
        {
            return _kmisInterstitialReady();
        }

        [DllImport("__Internal")]
        private static extern void _kmshowInterstitial();
        public void showInterstitial()
        {
            _kmshowInterstitial();
        }

         [DllImport("__Internal")]
        private static extern void _kmloadRewardedVideo(string rewardedVideoID);
        public void loadRewardedVideo(string rewardedVideoID)
        {
            _kmloadRewardedVideo(rewardedVideoID);
        }

        [DllImport("__Internal")]
        private static extern bool _kmisRewardedVideoReady();
        public bool isRewardedVideoReady()
        {
            return _kmisRewardedVideoReady();
        }

        [DllImport("__Internal")]
        private static extern void _kmshowRewardedVideo();
        public void showRewardedVideo()
        {
            _kmshowRewardedVideo();
        }

        [DllImport("__Internal")]
        private static extern void _kmsetTesting(bool v);
        public void setTesting(bool v)
        {
            _kmsetTesting(v);
        }

        [DllImport("__Internal")]
        private static extern void _kmsetForChildren(bool v);
        public void setForChildren(bool v)
        {
          //  Debug.Log("set for child in c#");
            _kmsetForChildren(v);
        }

        [MonoPInvokeCallback(typeof(AdmobAdCallBack))]
        public static void onAdmobEventCallBack(string adtype, string eventName, string msg)
        {
         //   Debug.Log("c# receive callback " + adtype + "  " + eventName + "  " + msg);
            if (adtype == "banner")
            {
                if (Admob.Instance().bannerEventHandler != null)
                Admob.Instance().bannerEventHandler(eventName, msg);
            }
            else if (adtype == "interstitial")
            {
                if (Admob.Instance().interstitialEventHandler != null)
                Admob.Instance().interstitialEventHandler(eventName, msg);
            }
            else if (adtype == "rewardedVideo")
            {
                if (Admob.Instance().rewardedVideoEventHandler != null)
                Admob.Instance().rewardedVideoEventHandler(eventName, msg);
            }
            else if (adtype == "nativeBanner")
            {
                if (Admob.Instance().nativeBannerEventHandler != null)
                    Admob.Instance().nativeBannerEventHandler(eventName, msg);
            }
        }
        
#elif UNITY_ANDROID
	private AndroidJavaObject jadmob;
         private void preInitAdmob(){
            if (jadmob == null) {
                AndroidJavaClass admobUnityPluginClass = new AndroidJavaClass("com.admob.plugin.AdmobUnityPlugin");
                jadmob = admobUnityPluginClass.CallStatic<AndroidJavaObject>("getInstance");
                InnerAdmobListener innerlistener = new InnerAdmobListener();
                innerlistener.admobInstance = this;
                AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject activy = jc.GetStatic<AndroidJavaObject>("currentActivity");

                jadmob.Call("setContext", new object[] { activy, new AdmobListenerProxy(innerlistener) });
			}
		}
		public void initAdmob(string bannerID,string fullID){
			jadmob.Call ("initAdmob", new object[]{bannerID,fullID});
		}
        public void showBannerRelative(AdSize size, int position,int marginY,string instanceName="defaultBanner")
        {
            jadmob.Call("showBannerRelative", new object[] { size.Width,size.Height,position,marginY,instanceName});
		}
        public void showBannerAbsolute(AdSize size, int x, int y, string instanceName = "defaultBanner")
        {
            jadmob.Call("showBannerAbsolute", new object[] { size.Width, size.Height,x,y ,instanceName});
        }
        public void removeBanner(string instanceName = "defaultBanner")
        {
            jadmob.Call("removeBanner",instanceName);
        }


        public void loadInterstitial()
        {
            jadmob.Call("loadInterstitial");
        }
        public bool isInterstitialReady()
        {
            bool isReady = jadmob.Call<bool>("isInterstitialReady");
            return isReady;
        }
        public void showInterstitial()
        {
            jadmob.Call("showInterstitial");
        }


        public void loadRewardedVideo(string rewardedVideoID)
        {
            jadmob.Call("loadRewardedVideo", new object[] { rewardedVideoID });
        }
        public bool isRewardedVideoReady()
        {
            bool isReady = jadmob.Call<bool>("isRewardedVideoReady");
            return isReady;
        }
        public void showRewardedVideo()
        {
            jadmob.Call("showRewardedVideo");
        }

        public void setTesting(bool value)
        {
            jadmob.Call("setTesting",value);
        }
        public void setForChildren(bool value)
        {
            jadmob.Call("setForChildren",value);
        }
        public void showNativeBannerRelative(AdSize size, int position, int marginY,string nativeBannerID, string instanceName = "defaultNativeBanner")
        {
            jadmob.Call("showNativeBannerRelative", new object[] { size.Width, size.Height, position, marginY,nativeBannerID, instanceName });
        }
        public void showNativeBannerAbsolute(AdSize size, int x, int y, string nativeBannerID,string instanceName = "defaultNativeBanner")
        {
            jadmob.Call("showNativeBannerAbsolute", new object[] { size.Width, size.Height, x, y,nativeBannerID, instanceName });
        }
        public void removeNativeBanner(string instanceName = "defaultNativeBanner")
        {
            jadmob.Call("removeNativeBanner", instanceName);
        }
        class InnerAdmobListener : IAdmobListener
        {
            internal Admob admobInstance;
            public void onAdmobEvent(string adtype, string eventName, string paramString)
            {
                if (adtype == "banner")
                {
                    if (admobInstance.bannerEventHandler != null)
                    admobInstance.bannerEventHandler(eventName, paramString);
                }
                else if (adtype == "interstitial")
                {
                    if (admobInstance.interstitialEventHandler != null)
                    admobInstance.interstitialEventHandler(eventName, paramString);
                }
                else if (adtype == "rewardedVideo")
                {
                    if (admobInstance.rewardedVideoEventHandler != null)
                    admobInstance.rewardedVideoEventHandler(eventName, paramString);
                }
                else if (adtype == "nativeBanner")
                {
                    if(admobInstance.nativeBannerEventHandler!=null)
                    admobInstance.nativeBannerEventHandler(eventName, paramString);
                }
            }
        }

#else
        private void preInitAdmob()
        {
           
        }
        
        public void initAdmob(string bannerID, string fullID)
        {
            Debug.Log("calling initAdmob");
        }

        
        public void showBannerAbsolute(AdSize size, int x, int y,string instanceName="defaultBanner")
        {
            Debug.Log("calling showBannerAbsolute");
        }

        
        public void showBannerRelative(AdSize size, int position, int marginY,string instanceName="defaultBanner")
        {
            Debug.Log("calling showBannerRelative");
        }

        
        public void removeBanner(string instanceName="defaultBanner")
        {
            Debug.Log("calling removeBanner");
        }

        
        public void loadInterstitial()
        {
            Debug.Log("calling loadInterstitial");
        }

        
        public bool isInterstitialReady()
        {
            Debug.Log("calling isInterstitialReady");
        return false;
        }

        
        public void showInterstitial()
        {
            Debug.Log("calling showInterstitial");
        }

        public void loadRewardedVideo(string rewardedVideoID)
        {
            Debug.Log("calling loadRewardedVideo");
        }
        public bool isRewardedVideoReady()
        {
            Debug.Log("calling isRewardedVideoReady");
            return false;
        }
        public void showRewardedVideo()
        {
            Debug.Log("calling showRewardedVideo");
        }
        
        public void setTesting(bool v)
        {
            Debug.Log("calling setTesting");
        }

        
        public void setForChildren(bool v)
        {
            Debug.Log("calling setForChildren");
        }
         public void showNativeBannerRelative(AdSize size, int position, int marginY,string nativeBannerID, string instanceName = "defaultNativeBanner")
        {
           Debug.Log("calling showNativeBannerRelative");
        }
        public void showNativeBannerAbsolute(AdSize size, int x, int y, string nativeBannerID,string instanceName = "defaultNativeBanner")
        {
           Debug.Log("calling showNativeBannerAbsolute");
        }
        public void removeNativeBanner(string instanceName = "defaultNativeBanner")
        {
           Debug.Log("calling removeNativeBanner");
        }
#endif

    }
}
