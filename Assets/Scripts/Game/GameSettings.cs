using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class GameSettings
{
    public GameSettings()
    {

    }

    public static GameSettings Settings => GameManager.Instance().Settings;

    /// <summary>
    /// 자동저장 여부
    /// </summary>
    public bool DoAutoSave { get; set; } = true;

    public bool DepthOfField { get; set; } = true;

    public bool MotionBlur { get; set; } = true;

    public bool SoftTurn { get; set; } = true;

    public bool Rumble { get; set; } = true;

    public float BGM => settingBGM * 0.1f;

    private int settingBGM = 10;

    public float SE => settingSE * 0.1f;

    private int settingSE = 10;


    public string ToSaveString()
    {
        StringBuilder SB = new();
        SB.Append($"{nameof(DoAutoSave)},{DoAutoSave}|");
        return SB.ToString();
    }

    public void FromSaveString(string saveString)
    {
        var array = saveString.Split('|');
        foreach(var item in array)
        {
            var a = item.Split(',');
            switch (a[0])
            {
                case nameof(DoAutoSave):
                    DoAutoSave = bool.Parse(a[1]);
                    break;
            }
        }
    }

}