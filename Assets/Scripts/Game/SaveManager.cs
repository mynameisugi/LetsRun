using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 저장 관리
/// </summary>
public class SaveManager
{
    public SaveManager()
    {
    }

    /// <summary>
    /// 세이브를 파일에서 불러옴
    /// </summary>
    public void LoadFromPrefs(int slot = 0)
    {
        string json = PlayerPrefs.GetString(GetSlotPref(slot), string.Empty);
        if (string.IsNullOrEmpty(json)) { Reset(); SaveToPrefs(0); return; }
        saveData = (Dictionary<string, object>)JsonConvert.DeserializeObject(json, typeof(Dictionary<string, object>));

    }

    /// <summary>
    /// 세이브를 파일로 저장
    /// </summary>
    public void SaveToPrefs(int slot = 0)
    {
        OnSaveToPref?.Invoke(this);
        string json = JsonConvert.SerializeObject(saveData);
        PlayerPrefs.SetString(GetSlotPref(slot), json);
    }

    /// <summary>
    /// 세이브 리셋: 새로운 세이브를 생성
    /// </summary>
    public void Reset()
    {
        PlayerPrefs.DeleteAll();
        saveData = new Dictionary<string, object>();
        SaveValue(SAVESEED, (int)DateTime.Now.Ticks);
        SaveValue(TimeManager.SAVEKEY, 0);
        GameManager.Instance().StartIntro();
        SaveToPrefs(0);
    }

    public delegate void SaveEventHandler(SaveManager save);

    /// <summary>
    /// 파일을 세이브할 때 발생
    /// </summary>
    public SaveEventHandler OnSaveToPref = null;

    private Dictionary<string, object> saveData;

    private const string SAVEDATAONPREFS = "SaveData";

    private static string GetSlotPref(int slot) => SAVEDATAONPREFS + slot.ToString();

    private const string SAVESEED = "Seed";

    /// <summary>
    /// 세이브 파일의 시드값
    /// </summary>
    public int GetSeed() => LoadValue(SAVESEED, 0);

    /// <summary>
    /// 값 저장
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void SaveValue<T>(string key, T value)
    {
        saveData.Remove(key);
        saveData.Add(key, value);

    }

    /// <summary>
    /// 값 불러오기
    /// </summary>
    public T LoadValue<T>(string key, T defaultValue = default)
    {
        if (saveData.TryGetValue(key, out var value))
        {
            if (value is T tValue) return tValue;
            T convertedValue = (T)Convert.ChangeType(value, typeof(T));
            if (convertedValue != null) return convertedValue;
        }

        SaveValue(key, defaultValue);
        return defaultValue;
    }

}