using UnityEngine;

public class ColorUtility
{
    //RGBカラーからHSVを取得
    public static ColorHSV GetHSVByRGB (Color rgb)
    {
        ColorHSV hsv = new ColorHSV ();
        Color.RGBToHSV (rgb, out hsv.h, out hsv.s, out hsv.v);
        return hsv;
    }
    //HSVでの色の比較(範囲内にいるか)
    public static bool WithinRange (ColorHSV a, float range, ColorHSV b)
    {
        return a.h - range < b.h && b.h < a.h + range &&
            a.s - range < b.s && b.s < a.s + range &&
            a.v - range < b.v && b.v < a.v + range;
    }
    //RGBでの色の比較(範囲内にいるか)
    public static bool WithinRange (Color a, float range, Color b)
    {
        return a.r - range < b.r && b.r < a.r + range &&
            a.g - range < b.g && b.g < a.g + range &&
            a.b - range < b.b && b.b < a.b + range;
    }
}

//HSVの構造体
public class ColorHSV
{
    public float h, s, v;
}


