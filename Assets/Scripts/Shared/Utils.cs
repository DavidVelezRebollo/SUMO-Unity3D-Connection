using System.Globalization;
using UnityEngine;

public class Utils : MonoBehaviour
{
    public static void SetExecutableRoute(string route)
    {
        if (string.IsNullOrEmpty(route))
        {
            PlayerPrefs.SetString("Route", null);
            return;
        }
        
        PlayerPrefs.SetString("Route", route.Replace('\\', '/'));
        PlayerPrefs.Save();
    }

    public static string GetExecutableRoute() => PlayerPrefs.HasKey("Route") ? PlayerPrefs.GetString("Route") : null;
    
    public static float ParseFloat(string data)
    {
        CultureInfo info = CultureInfo.InvariantCulture;
        return float.Parse(data, info);
    }
}
