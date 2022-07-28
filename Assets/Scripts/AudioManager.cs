using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public enum AudioChannel
    {
        Master,
        Sound,
        Music,
    }

    [SerializeField] AudioMixer masterMixer;

    // ReSharper disable once NotAccessedField.Local
    [SerializeField] AudioMixerGroup masterGroup;

    // ReSharper disable once NotAccessedField.Local
    [SerializeField] AudioMixerGroup musicGroup;

    // ReSharper disable once NotAccessedField.Local
    [SerializeField] AudioMixerGroup soundGroup;

    protected void Start()
    {
        SetVolume(AudioChannel.Master, GetChannelValue(AudioChannel.Master));
        SetVolume(AudioChannel.Sound, GetChannelValue(AudioChannel.Sound));
        SetVolume(AudioChannel.Music, GetChannelValue(AudioChannel.Music));
    }

    public static float GetChannelValue(AudioChannel channel)
    {
        return PlayerPrefs.GetFloat(channel + "Volume", 0.8f);
    }

    /// <param name="channel"></param>
    /// <param name="value">From 0 to 1</param>
    public void SetVolume(AudioChannel channel, float value)
    {
        masterMixer.SetFloat(channel + "Volume", ConvertValueToVolume(value));
        PlayerPrefs.SetFloat(channel + "Volume", value);
    }

    /// <param name="value">From 0 to 1</param>
    static float ConvertValueToVolume(float value)
    {
        return value == 0 ? -100 : 30 * Mathf.Log10(Mathf.Max(value, 0.0001f));
    }
}
