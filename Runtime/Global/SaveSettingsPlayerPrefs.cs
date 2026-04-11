using UnityEngine;

namespace Sobia.Utils
{
    public static class SaveSettingsPlayerPrefs
    {
        //Audio
        public static void SaveMasterVolume(float volume)
        {
            PlayerPrefs.SetFloat("MasterVolume", volume);
        }

        public static float LoadMasterVolume()
        {
            return PlayerPrefs.GetFloat("MasterVolume", 0.2f);
        }

        public static void SaveSFXVolume(float volume)
        {
            PlayerPrefs.SetFloat("SFXVolume", volume);
        }

        public static float LoadSFXVolume()
        {
            return PlayerPrefs.GetFloat("SFXVolume", 0.35f);
        }

        public static void SaveMusicVolume(float volume)
        {
            PlayerPrefs.SetFloat("MusicVolume", volume);
        }

        public static float LoadMusicVolume()
        {
            return PlayerPrefs.GetFloat("MusicVolume", 0.25f);
        }

        public static void SaveUIVolume(float volume)
        {
            PlayerPrefs.SetFloat("UIVolume", volume);
        }

        public static float LoadUIVolume()
        {
            return PlayerPrefs.GetFloat("UIVolume", 0.3f);
        }

        //Graphics
        public static void SaveBrightness(float volume)
        {
            PlayerPrefs.SetFloat("Brightness", volume);
        }

        public static float LoadBrightness()
        {
            return PlayerPrefs.GetFloat("Brightness", 0.0f);
        }

        public static void SaveFOV(float volume)
        {
            PlayerPrefs.SetFloat("FOV", volume);
        }

        public static float LoadFOV()
        {
            return PlayerPrefs.GetFloat("FOV", 75f);
        }

        public static void SaveShowFps(bool enabled)
        {
            PlayerPrefs.SetInt("ShowFps", enabled ? 1 : 0);
        }

        public static bool LoadShowFps()
        {
            return PlayerPrefs.GetInt("ShowFps", 1) == 1;
        }

        public static void SaveTextureQuality(int index)
        {
            PlayerPrefs.SetInt("TextureQuality", index);
        }

        public static int LoadTextureQuality()
        {
            return PlayerPrefs.GetInt("TextureQuality", 0);
        }

        public static void SaveVSync(bool enabled)
        {
            PlayerPrefs.SetInt("VSync", enabled ? 1 : 0);
        }

        public static bool LoadVSync()
        {
            return PlayerPrefs.GetInt("VSync", 1) == 1;
        }

        public static void SaveResolution(int index)
        {
            PlayerPrefs.SetInt("Resolution", index);
        }

        public static int LoadResolution()
        {
            return PlayerPrefs.GetInt("Resolution", 0);
        }

        public static void SaveMaxFramerate(float volume)
        {
            PlayerPrefs.SetFloat("MaxFramerate", volume);
        }

        public static float LoadMaxFramerate()
        {
            return PlayerPrefs.GetFloat("MaxFramerate", 60f);
        }

        public static void SaveFullscreen(bool enabled)
        {
            PlayerPrefs.SetInt("Fullscreen", enabled ? 1 : 0);
        }

        public static bool LoadFullscreen()
        {
            return PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        }

        //General

        public static void SaveInvertMouseX(bool enabled)
        {
            PlayerPrefs.SetInt("InvertMouseX", enabled ? 1 : 0);
        }

        public static bool LoadInvertMouseX()
        {
            return PlayerPrefs.GetInt("InvertMouseX", 0) == 1;
        }

        public static void SaveInvertMouseY(bool enabled)
        {
            PlayerPrefs.SetInt("InvertMouseY", enabled ? 1 : 0);
        }

        public static bool LoadInvertMouseY()
        {
            return PlayerPrefs.GetInt("InvertMouseY", 0) == 1;
        }

        public static void SaveSensitivity(float volume)
        {
            PlayerPrefs.SetFloat("Sensitivity", volume);
        }

        public static float LoadSensitivity()
        {
            return PlayerPrefs.GetFloat("Sensitivity", 1f);
        }
    }
}