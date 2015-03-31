using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Acr.BarCodes;
using DigicuApp;

namespace DigicuApp.Droid
{
    [Activity(Label = "DigiCuApp", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
    {
        private static bool initDone = false;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            if (!initDone)
            {
                BarCodes.Init(this);
                initDone = true;
            }
            
            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());
        }
    }

}



