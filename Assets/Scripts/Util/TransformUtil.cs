using UnityEngine;

public static class TransformUtil {

    public static void Reset(this Transform tran){
        tran.localPosition =  Vector3.zero;
        tran.localScale =  Vector3.one;
        tran.localRotation = Quaternion.identity;
    }

    public static void Reset(this Transform tran,Transform parent){
        tran.SetParent(parent,false);
        tran.Reset();
    }
}