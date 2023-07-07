using System.Data;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class GameSettings
{
    public GameSettings()
    {
        LoadSettings();
    }

    public static Settings Values => GameManager.Instance().Settings.values;

    private Settings values;

    private const string OPTIONKEY = "GameSettings";

    public void LoadSettings()
    {
        string settings = PlayerPrefs.GetString(OPTIONKEY, "");
        values = Settings.FromSaveString(settings);
        if (string.IsNullOrEmpty(settings)) { SaveSettings(); return; }
        ApplySettings();
    }

    public void SaveSettings()
    {
        string settings = values.ToSaveString();
        PlayerPrefs.SetString(OPTIONKEY, settings);
        ApplySettings();
    }

    public void ChangeSettings(Settings newSettings)
    {
        values = newSettings;
        SaveSettings();
    }

    private void ApplySettings()
    {
        var player = PlayerManager.Instance();
        var camera = player.GetComponentInChildren<UniversalAdditionalCameraData>();
        camera.antialiasing = values.antiAlias ? AntialiasingMode.FastApproximateAntialiasing : AntialiasingMode.None;
        var actions = player.GetComponentsInChildren<ActionBasedControllerManager>();
        foreach (var action in actions) action.smoothTurnEnabled = values.softTurn;
    }

    public readonly struct Settings
    {
        public Settings(bool doAutoSave, bool antiAlias, bool softTurn, bool rumble, int settingBGM, int settingSE)
        {
            this.doAutoSave = doAutoSave;
            this.antiAlias = antiAlias;
            this.softTurn = softTurn;
            this.rumble = rumble;
            this.settingBGM = settingBGM;
            this.settingSE = settingSE;
        }

        /// <summary>
        /// 자동저장 여부
        /// </summary>
        public readonly bool doAutoSave;

        public readonly bool antiAlias;

        public readonly bool softTurn;

        public readonly bool rumble;

        public float BGM => settingBGM * 0.1f;

        private readonly int settingBGM;

        public float SE => settingSE * 0.1f;

        private readonly int settingSE;

        public string ToSaveString()
        {
            StringBuilder SB = new();
            SB.Append($"{nameof(doAutoSave)},{doAutoSave}|");
            SB.Append($"{nameof(antiAlias)},{antiAlias}|");
            SB.Append($"{nameof(softTurn)},{softTurn}|");
            SB.Append($"{nameof(rumble)},{rumble}|");
            SB.Append($"{nameof(settingBGM)},{settingBGM}|");
            SB.Append($"{nameof(settingSE)},{settingSE}|");
            return SB.ToString();
        }

        public static Settings FromSaveString(string saveString)
        {
            bool d = true, t = false, s = true, r = true; int b = 10, e = 100;

            if (string.IsNullOrEmpty(saveString)) goto END;
            var array = saveString.Split('|');
            foreach (var item in array)
            {
                var a = item.Split(',');
                switch (a[0])
                {
                    case nameof(doAutoSave):
                        d = bool.Parse(a[1]);
                        break;
                    case nameof(antiAlias):
                        t = bool.Parse(a[1]);
                        break;
                    case nameof(softTurn):
                        s = bool.Parse(a[1]);
                        break;
                    case nameof(rumble):
                        r = bool.Parse(a[1]);
                        break;
                    case nameof(settingBGM):
                        b = int.Parse(a[1]);
                        break;
                    case nameof(settingSE):
                        e = int.Parse(a[1]);
                        break;
                }
            }
            END:
            return new Settings(d, t, s, r, b, e);
        }
    }

    


    

}