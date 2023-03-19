#pragma warning disable 0618
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using UnityEngine.Networking;
using UnityEngine.EventSystems;
//using UnityEngine.iOS;

public static class CUtils
{
    public static void RateGame()
    {
#if UNITY_ANDROID
        Application.OpenURL("market://details?id=" + Application.identifier);
#elif UNITY_IOS
        Device.RequestStoreReview();
        Application.OpenURL("itms-apps://itunes.apple.com/app/id" + OnlineController.setting.iOSAppId);
#endif
    }

    public static void OpenStore(string id)
    {
#if UNITY_ANDROID
        Application.OpenURL("market://details?id=" + Application.identifier);
#elif UNITY_IOS
        Application.OpenURL("itms-apps://itunes.apple.com/app/id" + OnlineController.setting.iOSAppId);
#endif
    }

    public static void SetLikeFbPage(string id)
    {
        SetBool("like_page_" + id, true);
    }

    public static bool IsLikedFbPage(string id)
    {
        return GetBool("like_page_" + id, false);
    }

    public static bool IsGameRated
    {
        get { return GetBool("IsGameRated", false); }
        set { SetBool("IsGameRated", value); }
    }

    public static bool IsAdsRemoved
    {
        get { return GetBool("IsAdsRemoved", false); }
        set { SetBool("IsAdsRemoved", value); }
    }

    #region Double
    public static void SetDouble(string key, double value)
    {
        PlayerPrefs.SetString(key, DoubleToString(value));
    }

    public static double GetDouble(string key, double defaultValue)
    {
        string defaultVal = DoubleToString(defaultValue);
        return StringToDouble(PlayerPrefs.GetString(key, defaultVal));
    }

    public static double GetDouble(string key)
    {
        return GetDouble(key, 0d);
    }

    private static string DoubleToString(double target)
    {
        return target.ToString("R");
    }

    private static double StringToDouble(string target)
    {
        if (string.IsNullOrEmpty(target))
            return 0d;

        return double.Parse(target);
    }
    #endregion

    #region Bool
    public static void SetBool(string key, bool value)
    {
        PlayerPrefs.SetInt(key, value ? 1 : 0);
    }

    public static bool GetBool(string key, bool defaultValue = false)
    {
        int defaultVal = defaultValue ? 1 : 0;
        return PlayerPrefs.GetInt(key, defaultVal) == 1;
    }
    #endregion


    public static double GetCurrentTime()
    {
        TimeSpan span = DateTime.Now.Subtract(new DateTime(1970, 1, 1, 0, 0, 0));
        return span.TotalSeconds;
    }

    public static double GetCurrentTimeInDays()
    {
        TimeSpan span = DateTime.Now.Subtract(new DateTime(1970, 1, 1, 0, 0, 0));
        return span.TotalDays;
    }

    public static double GetCurrentTimeInMills()
    {
        TimeSpan span = DateTime.Now.Subtract(new DateTime(1970, 1, 1, 0, 0, 0));
        return span.TotalMilliseconds;
    }

    public static T GetRandom<T>(params T[] arr)
    {
        return arr[UnityEngine.Random.Range(0, arr.Length)];
    }

    public static bool IsActionAvailable(string action, int time, bool availableFirstTime = true)
    {
        if (!PlayerPrefs.HasKey(action + "_time")) // First time.
        {
            if (availableFirstTime == false)
            {
                SetActionTime(action);
            }
            return availableFirstTime;
        }

        int delta = (int)(GetCurrentTime() - GetActionTime(action));
        return delta >= time;
    }

    public static double GetActionDeltaTime(string action)
    {
        if (GetActionTime(action) == 0)
            return 0;
        return GetCurrentTime() - GetActionTime(action);
    }

    public static void SetActionTime(string action)
    {
        SetDouble(action + "_time", GetCurrentTime());
    }

    public static void SetActionTime(string action, double time)
    {
        SetDouble(action + "_time", time);
    }

    public static double GetActionTime(string action)
    {
        return GetDouble(action + "_time");
    }

    public static bool ShowInterstitialAd()
    {
        if (IsAdsRemoved) return false;

        if (IsActionAvailable("show_ads", 120))
        {
#if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
            
#else

#endif
        }
        return false;
    }

    public static void ShowBannerAd()
    {
        if (IsAdsRemoved) return;

#if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
        
#endif
    }

    public static void CloseBannerAd()
    {
#if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
        
#endif
    }

    public static string BuildStringFromCollection(ICollection values, char split = '|')
    {
        string results = "";
        int i = 0;
        foreach (var value in values)
        {
            results += value;
            if (i != values.Count - 1)
            {
                results += split;
            }
            i++;
        }
        return results;
    }

    public static List<T> BuildListFromString<T>(string values, char split = '|')
    {
        List<T> list = new List<T>();
        if (string.IsNullOrEmpty(values))
            return list;

        string[] arr = values.Split(split);
        foreach (var value in arr)
        {
            if (string.IsNullOrEmpty(value)) continue;
            T val = (T)Convert.ChangeType(value, typeof(T));
            list.Add(val);
        }
        return list;
    }

    public static IEnumerator LoadPicture(string url, string fileName, Action<Texture2D> callback)
    {
        string localPath = GetLocalPath(fileName);
        bool loaded = LoadFromLocal(callback, localPath);

        if (!loaded)
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(url, false);
            yield return www.SendWebRequest();
            if (!www.isNetworkError && !www.isHttpError)
            {
                Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                callback(texture);
                File.WriteAllBytes(localPath, texture.EncodeToPNG());
            }
            else
            {
                LoadFromLocal(callback, localPath);
            }
        }
    }

    public static string GetLocalPath(string fileName)
    {
        return Path.Combine(Application.persistentDataPath, fileName);
    }

    public static IEnumerator CachePicture(string url, string fileName, Action<bool> result)
    {
        string localPath = GetLocalPath(fileName);
        WWW www = new WWW(url);
        yield return www;
        if (www.isDone && string.IsNullOrEmpty(www.error))
        {
            File.WriteAllBytes(localPath, www.bytes);
            result?.Invoke(true);
        }
        else
        {
            result?.Invoke(false);
        }
    }

    public static bool LoadFromLocal(Action<Texture2D> callback, string localPath)
    {
        if (File.Exists(localPath))
        {
            var bytes = File.ReadAllBytes(localPath);
            var tex = new Texture2D(0, 0, TextureFormat.RGBA32, false);
            tex.LoadImage(bytes);
            if (tex != null)
            {
                callback(tex);
                return true;
            }
        }
        return false;
    }

    public static Sprite CreateSprite(Texture2D texture, int width, int height)
    {
        return Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 100.0f);
    }

    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Exists(x => x.gameObject.layer == 5);
    }

    public static bool IsPointerOverUIObject(GameObject uiObject)
    {
        if (uiObject == null) return false;
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var go in results)
        {
            if (go.gameObject == uiObject) return true;
        }
        return false;
    }

    public static void Shuffle<T>(this IList<T> array)
    {
        for (int i = 0; i < array.Count; i++)
        {
            var temp = array[i];
            var randomIndex = UnityEngine.Random.Range(0, array.Count);
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }

    public static Vector3 GetMiddlePoint(Vector3 begin, Vector3 end, float lerpT, float delta = 0)
    {
        Vector3 center = Vector3.Lerp(begin, end, lerpT);
        Vector3 beginEnd = end - begin;
        Vector3 perpendicular = new Vector3(-beginEnd.y, beginEnd.x, 0).normalized;
        Vector3 middle = center + perpendicular * delta;
        return middle;
    }

    public static string RichColoredText(string s, Color color)
    {
        return "<color=#" + ColorUtility.ToHtmlStringRGBA(color) + ">" + s + "</color>";
    }

    public static string RichColoredText(string s, string color)
    {
        return "<color=#" + color + ">" + s + "</color>";
    }
}
