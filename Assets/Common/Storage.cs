using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class Storage
{
    public static dynamic get (string key){
        string json = PlayerPrefs.GetString(key);
        Debug.Log(json);
        if(json == null || json == ""){
            return null;
        }else{
            return JsonMapper.ToObject(json);
        }

    }

    public static void set (string key, object value){
        // Debug.Log(JsonMapper.ToJson(value));
        PlayerPrefs.SetString(key, JsonMapper.ToJson(value));
        return;
    }

}
