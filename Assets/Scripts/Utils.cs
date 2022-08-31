using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    // Static class with utility methods
    public static string FormatTime(float timer) {
        string minutes = Mathf.Floor(timer / 60).ToString("00");
        string seconds = (timer % 60).ToString("00");

        return string.Format("{0}:{1}", minutes, seconds);
    }
}
