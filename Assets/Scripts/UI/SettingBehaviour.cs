using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;

public class SettingBehaviour : MonoBehaviour
{
    [System.Serializable]
    public class Settings
    {
        public float masterVolume = 1;
        public float musicVolume = 1;
        public float sfxVolume = 1;
        public float lookSensitivity = 1;
        public bool motionBlur = true;
        public bool depthOfField = true;
        public bool ambientOcclusion = true;
    }

    public static Settings settings;
    private const string _SETTING_STRING_NAME_ = "_SETTINGS_";
    private void Awake()
    {
        if (settings == null)
        {
            if (PlayerPrefs.HasKey(_SETTING_STRING_NAME_))
            {
                var json = PlayerPrefs.GetString(_SETTING_STRING_NAME_);
                settings = JsonUtility.FromJson<Settings>(json);
            }
            else settings = new Settings();
        }
    }

    public Slider masterVolume;
    public Slider musicVolume;
    public Slider sfxVolume;
    public Slider lookSensitivity;
    public Volume ppVolume;
    public Toggle motionBlur;
    public Toggle depthOfField;
    public Toggle ambientOcclusion;
    public MotionBlur pp_mb;
    public DepthOfField pp_dof;
    public ScreenSpaceAmbientOcclusion pp_spao;

    private void Start()
    {
        masterVolume.SetValueWithoutNotify(settings.masterVolume);
        musicVolume.SetValueWithoutNotify(settings.musicVolume);
        sfxVolume.SetValueWithoutNotify(settings.sfxVolume);
        lookSensitivity.SetValueWithoutNotify(settings.lookSensitivity);

        ppVolume.profile.TryGet<MotionBlur>(out pp_mb);
        ppVolume.profile.TryGet<DepthOfField>(out pp_dof);
        ppVolume.profile.TryGet<ScreenSpaceAmbientOcclusion>(out pp_spao);
        SetPostProccesing();
        motionBlur.SetIsOnWithoutNotify(settings.motionBlur);
        depthOfField.SetIsOnWithoutNotify(settings.depthOfField);
        ambientOcclusion.SetIsOnWithoutNotify(settings.ambientOcclusion);
    }

    private void SetPostProccesing()
    {
        if (pp_mb)
            pp_mb.active = settings.motionBlur;
        if (pp_dof)
            pp_dof.active = settings.depthOfField;
        if (pp_spao)
            pp_spao.active = settings.ambientOcclusion;
    }

    private void SaveSettings()
    {
        var json = JsonUtility.ToJson(settings);
        PlayerPrefs.SetString(_SETTING_STRING_NAME_, json);
    }

    public void OnMasterChange()
    {
        settings.masterVolume = masterVolume.value;
        SoundManager.Instance.RecalculateMusicVolume();
        SaveSettings();
    }

    public void OnMusicChange()
    {
        settings.musicVolume = musicVolume.value;
        SoundManager.Instance.RecalculateMusicVolume();
        SaveSettings();
    }

    public void OnSfxChange()
    {
        settings.sfxVolume = sfxVolume.value;
        SaveSettings();
    }

    public void OnSensitivityChange()
    {
        settings.lookSensitivity = lookSensitivity.value;
        SaveSettings();
    }

    public void OnMotionBlurChange()
    {
        settings.motionBlur = motionBlur.isOn;
        SetPostProccesing();
        SaveSettings();
    }

    public void OnDepthOfFieldChange()
    {
        settings.depthOfField = depthOfField.isOn;
        SetPostProccesing();
        SaveSettings();
    }

    public void OnAmbientOclusionChange()
    {
        settings.ambientOcclusion = ambientOcclusion.isOn;
        SetPostProccesing();
        SaveSettings();
    }
}
