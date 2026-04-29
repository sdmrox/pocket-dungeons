namespace PocketDungeons.Core.Services
{
    public interface IAudioService
    {
        void PlaySFX(string id, float volume = 1f);
        void PlayMusic(string id, float fadeTime = 1f);
        void StopMusic(float fadeTime = 0.5f);
        void SetMusicVolume(float volume);
        void SetSFXVolume(float volume);
    }
}
