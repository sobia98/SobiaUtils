using UnityEngine;

namespace Sobia.Utils
{
    public static class SaveSettingsPlayerPrefs
    {
        public static void SaveVSync(bool enabled)
        {
            PlayerPrefs.SetInt("VSync", enabled ? 1 : 0);
        }

        public static void SaveFullscreen(bool enabled)
        {
            PlayerPrefs.SetInt("Fullscreen", enabled ? 1 : 0);
        }

        public static void SaveShowFps(bool enabled)
        {
            PlayerPrefs.SetInt("ShowFps", enabled ? 1 : 0);
        }

        public static void SaveMasterVolume(float volume)
        {
            PlayerPrefs.SetFloat("MasterVolume", volume);
        }

        public static float LoadMasterVolume()
        {
            return PlayerPrefs.GetFloat("MasterVolume", 1.0f);
        }

        public static void SaveMusicVolume(float volume)
        {
            PlayerPrefs.SetFloat("MusicVolume", volume);
        }

        public static float LoadMusicVolume()
        {
            return PlayerPrefs.GetFloat("MusicVolume", 1.0f);
        }

        public static void SaveSfxVolume(float volume)
        {
            PlayerPrefs.SetFloat("SfxVolume", volume);
        }

        public static float LoadSfxVolume()
        {
            return PlayerPrefs.GetFloat("SfxVolume", 1.0f);
        }

        public static void SaveBrightness(float volume)
        {
            PlayerPrefs.SetFloat("Brightness", volume);
        }

        public static float LoadBrightness()
        {
            return PlayerPrefs.GetFloat("Brightness", 1.0f);
        }

        public static bool LoadVSync()
        {
            return PlayerPrefs.GetInt("VSync", 1) == 1;
        }

        public static bool LoadFullscreen()
        {
            return PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        }

        public static bool LoadShowFps()
        {
            return PlayerPrefs.GetInt("ShowFps", 1) == 1;
        }
    }
}