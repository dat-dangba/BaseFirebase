using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Analytics;
using UnityEngine;

public class FirebaseTracking : MonoBehaviour
{
    private bool isInitialized;

    public void Init()
    {
        isInitialized = true;
    }

    public void LogEvent(string name, params Parameter[] parameters)
    {
        if (!isInitialized)
        {
            return;
        }
        FirebaseAnalytics.LogEvent(name, parameters);
    }

    public void LevelStart(int level, string mode = "")
    {
        if (!isInitialized)
        {
            return;
        }
        Parameter[] levelStartParameters;
        if (mode == "")
        {
            levelStartParameters = new Parameter[]
            {
                 new Parameter(FirebaseAnalytics.ParameterLevel, level)
            };
        }
        else
        {
            levelStartParameters = new Parameter[]
            {
                new Parameter(FirebaseAnalytics.ParameterLevel, level),
                new Parameter("level_mode", mode)
            };
        }

        FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLevelStart, levelStartParameters);
    }

    public void LevelEnd(int level, bool success, string mode = "")
    {
        if (!isInitialized)
        {
            return;
        }

        Parameter[] levelEndParameters;
        if (mode == "")
        {
            levelEndParameters = new Parameter[]
            {
                new Parameter(FirebaseAnalytics.ParameterLevel, level),
                new Parameter(FirebaseAnalytics.ParameterSuccess, success.ToString())
            };
        }
        else
        {
            levelEndParameters = new Parameter[]
            {
                new Parameter(FirebaseAnalytics.ParameterLevel, level),
                new Parameter("level_mode", mode),
                new Parameter(FirebaseAnalytics.ParameterSuccess, success.ToString())
            };
        }

        FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLevelEnd, levelEndParameters);
    }

    public void InAppPurchase(string productId, decimal iapValue, string iapCurrency, int level = 0, string mode = "")
    {
        if (!isInitialized)
        {
            return;
        }

        Parameter[] iapRevenueParameters;
        if (mode == "")
        {
            iapRevenueParameters = new Parameter[]
            {
                new Parameter(FirebaseAnalytics.ParameterLevel, level),
                new Parameter(FirebaseAnalytics.ParameterValue, (double)iapValue),
                new Parameter(FirebaseAnalytics.ParameterCurrency, iapCurrency),
                new Parameter("product_id", productId)
            };
        }
        else
        {
            iapRevenueParameters = new Parameter[]
            {
                new Parameter(FirebaseAnalytics.ParameterLevel, level),
                new Parameter("level_mode", mode),
                new Parameter(FirebaseAnalytics.ParameterValue, (double)iapValue),
                new Parameter(FirebaseAnalytics.ParameterCurrency, iapCurrency),
                new Parameter("product_id", productId)
            };
        }

        FirebaseAnalytics.LogEvent("iap_sdk", iapRevenueParameters);
    }

    /*
     *  adPlatform: admob, applovin
     *  adFormat: open, inter...
     *
     */
    public void AdRevenue(string adPlatform, string adFormat, double revenue, string adPos)
    {
        if (!isInitialized)
        {
            return;
        }

        var impressionParameters = new[] {
                new Parameter("ad_platform", adPlatform),   // admob, applovin
                new Parameter("ad_format", adFormat),       // open_app, interstitial...
                new Parameter("value", revenue),
                new Parameter("currency", "USD"),
                new Parameter("ad_position", adPos)      // quang cao tai vi tri nao
            };

        FirebaseAnalytics.LogEvent("ad_impression", impressionParameters);
    }


}
