using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class MenuManager : MonoBehaviour
{
    [SerializeField] Slider soundSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] AudioSource soundSource;
    [SerializeField] AudioSource musicSource;
    private float _t1ckSoundsV0lum3;
    private float _elmus1kaVolume;

    void Start()
    {
        _t1ckSoundsV0lum3 = PlayerPrefs.GetFloat("soundVolume", 1f);  
        _elmus1kaVolume = PlayerPrefs.GetFloat("musicVolume", 1f);  
        soundSlider.value = _t1ckSoundsV0lum3;
        musicSlider.value = _elmus1kaVolume;
    }
    public void play()
    {
        SceneManager.LoadScene("PlayScene");
    }

    public void exit()
    {
        Application.Quit();
    }
    public void changeSoundVolume()
    {
        PlayerPrefs.SetFloat("soundVolume", soundSlider.value);
        soundSource.volume = soundSlider.value;
    }
    public void changeMusicVolume()
    {
        PlayerPrefs.SetFloat("musicVolume", musicSlider.value);
        musicSource.volume = musicSlider.value;
    }
}
