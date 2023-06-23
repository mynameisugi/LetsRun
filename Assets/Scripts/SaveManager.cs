using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ���� ����
/// </summary>
public class SaveManager
{
    public SaveManager()
    {
    }

    /// <summary>
    /// ���̺긦 ���Ͽ��� �ҷ���
    /// </summary>
    public void LoadFromPrefs(int slot)
    {
        string json = PlayerPrefs.GetString(GetSlotPref(slot), string.Empty);
        if (string.IsNullOrEmpty(json)) { Reset(); return; }
        saveData = (Dictionary<string, object>)JsonConvert.DeserializeObject(json, typeof(Dictionary<string, object>));

    }

    /// <summary>
    /// ���̺긦 ���Ϸ� ����
    /// </summary>
    public void SaveToPrefs(int slot)
    {
        OnSaveToPref?.Invoke(this);
        string json = JsonConvert.SerializeObject(saveData);
        PlayerPrefs.SetString(GetSlotPref(slot), json);
    }

    /// <summary>
    /// ���̺� ����: ���ο� ���̺긦 ����
    /// </summary>
    public void Reset()
    {
        PlayerPrefs.DeleteAll();
        saveData = new Dictionary<string, object>();
        SaveValue(SAVESEED, (int)DateTime.Now.Ticks);
        OnSaveReset?.Invoke(this);
        //SaveToPrefs();
    }

    public delegate void SaveEventHandler(SaveManager save);

    /// <summary>
    /// ���ο� ���̺긦 ���� �� �߻�
    /// </summary>
    public SaveEventHandler OnSaveReset = null;

    /// <summary>
    /// ������ ���̺��� �� �߻�
    /// </summary>
    public SaveEventHandler OnSaveToPref = null;

    private Dictionary<string, object> saveData;

    private const string SAVEDATAONPREFS = "SaveData";

    private static string GetSlotPref(int slot) => SAVEDATAONPREFS + slot.ToString();

    private const string SAVESEED = "Seed";

    /// <summary>
    /// ���̺� ������ �õ尪
    /// </summary>
    public int GetSeed() => LoadValue(SAVESEED, 0);

    /// <summary>
    /// �� ����
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void SaveValue<T>(string key, T value)
    {
        saveData.Remove(key);
        saveData.Add(key, value);

    }

    /// <summary>
    /// �� �ҷ�����
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