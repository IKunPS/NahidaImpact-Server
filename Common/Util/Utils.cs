namespace NahidaImpact.Util;

public static class Utils
{
    public static uint AbilityHash(string str)
    {
        if (str == null) return 0;
        int v7 = 0;
        int v8 = 0;
        while (v8 < str.Length)
        {
            v7 = str[v8++] + 131 * v7;
        }
        return (uint)v7;
    }

}

