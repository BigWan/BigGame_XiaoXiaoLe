using UnityEngine;
using System.Collections;

public static class V2intUtil {

    public static int ManhattanDistance(this Vector2Int v) {
        return Mathf.Abs(v.x) + Mathf.Abs( v.y);
    }

    public static int ManhattanDistance(this Vector2Int s,Vector2Int e) {
        return ManhattanDistance(s - e);
    }

}
