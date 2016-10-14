using System;

namespace admob
{
    // Interface for the methods to be invoked by the native plugin.
	internal interface IAdmobListener
    {
        void onAdmobEvent(string adtype,string eventName,string paramString);
    }
}
