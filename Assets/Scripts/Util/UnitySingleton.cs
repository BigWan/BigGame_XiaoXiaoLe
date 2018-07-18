using UnityEngine;

/// <summary>
/// MonoBehaviour
/// </summary>
/// <typeparam name="T">MonoBehaviour</typeparam>
public class UnitySingleton<T> :MonoBehaviour where T :Component {

    private static T _instance;
    public static T Instance{
        get{
            if(_instance == null){
                CreateInstance();
            }
            return _instance;
        }
    }
    public static void CreateInstance(){
        _instance = FindObjectOfType(typeof(T)) as T;
        if(_instance == null){
            GameObject o = new GameObject {
                hideFlags = HideFlags.HideAndDontSave
            };
            _instance = o.AddComponent<T>();
        }
    }
}