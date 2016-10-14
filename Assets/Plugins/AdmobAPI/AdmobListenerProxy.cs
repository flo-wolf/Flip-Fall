#if UNITY_ANDROID
using UnityEngine;
using System.Collections;
namespace admob
{
	public class AdmobListenerProxy : AndroidJavaProxy {
		private IAdmobListener listener;
		internal AdmobListenerProxy(IAdmobListener listener)
            : base("com.admob.plugin.IAdmobListener")
		{
			this.listener = listener;
		}
         void onAdmobEvent(string adtype,string eventName,string paramString){
           //  Debug.Log("c# admoblisterproxy "+adtype+"   "+eventName+"   "+paramString);
             if(listener!=null)
         	 listener.onAdmobEvent(adtype,eventName,paramString);
         }
		string toString(){
			return "AdmobListenerProxy";
		}
	}
}
#endif