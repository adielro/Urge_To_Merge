using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsMenu : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject deleteConfirmationPanel;
    [SerializeField] private Toggle musicToggle;
    [SerializeField] private Toggle sfxToggle;

    [Header("Volume Settings")]
    [SerializeField] private float duckingVolume = 0.2f;

    private void Start()
    {
        var settings = SaveSystem.Instance.LoadSettings();
        musicToggle.isOn = settings.musicEnabled;
        sfxToggle.isOn = settings.sfxEnabled;

        ApplyVolume(musicToggle.isOn, true);
        ApplyVolume(sfxToggle.isOn, false);

        musicToggle.onValueChanged.AddListener(ToggleMusic);
        sfxToggle.onValueChanged.AddListener(ToggleSFX);

        // Ensure panels are closed
        settingsPanel?.SetActive(false);
        deleteConfirmationPanel?.SetActive(false);
    }

    public void OpenSettings()
    {
        settingsPanel?.SetActive(true);

        if (musicToggle.isOn) SoundManager.Instance?.SetMusicVolume(duckingVolume);
        if (sfxToggle.isOn) SoundManager.Instance?.SetSFXVolume(duckingVolume);

        Time.timeScale = 0f;
    }

    public void CloseSettings()
    {
        settingsPanel?.SetActive(false);
        deleteConfirmationPanel?.SetActive(false);

        ApplyVolume(musicToggle.isOn, true);
        ApplyVolume(sfxToggle.isOn, false);

        Time.timeScale = 1f;
    }

    public void ToggleMusic(bool isOn)
    {
        SaveSystem.Instance.SaveSettings(isOn, sfxToggle.isOn);

        float volume = isOn ? (settingsPanel.activeSelf ? duckingVolume : 1f) : 0f;
        SoundManager.Instance?.SetMusicVolume(volume);
    }

    public void ToggleSFX(bool isOn)
    {
        SaveSystem.Instance.SaveSettings(musicToggle.isOn, isOn);

        float volume = isOn ? (settingsPanel.activeSelf ? duckingVolume : 1f) : 0f;
        SoundManager.Instance?.SetSFXVolume(volume);
    }

    private void ApplyVolume(bool isOn, bool isMusic)
    {
        float volume = isOn ? 1f : 0f;
        if (isMusic)
            SoundManager.Instance?.SetMusicVolume(volume);
        else
            SoundManager.Instance?.SetSFXVolume(volume);
    }

    public void ShowDeleteConfirmation()
    {
        deleteConfirmationPanel.SetActive(true);
    }

    public void HideDeleteConfirmation()
    {
        deleteConfirmationPanel.SetActive(false);
    }

    public void DeleteSave()
    {
        SaveSystem.Instance?.DeleteSave();
        HideDeleteConfirmation();
        CloseSettings();
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
