using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject campaignModeCanvas;
    public GameObject mainMenuCanvas;
    public GameObject startGameCanvas;
    public GameObject optionsCanvas;
    public GameObject creditsCanvas;
    public GameObject quitCanvas;
    [SerializeField] Button ResumeGame;
    [SerializeField] Text vsync;
    [SerializeField] Text screenmode;
    [SerializeField] Text Resolution;

    [SerializeField] RectTransform Mastervolume;
    [SerializeField] RectTransform Musicvolume;

    [SerializeField] int MaxMastervolumerange;
    [SerializeField] int MinMastervolumerange;

    [SerializeField] int MaxMusicvolumerange;
    [SerializeField] int MinMusicvolumerange;
    List<Resolution> resolutions = new List<Resolution>();
    int currentresolution;
    int noofresolutions;
    private void Awake()
    {

        int prevreswidth = 0;
        int prevresheigth = 0;
        noofresolutions = 0;
        foreach (Resolution res in Screen.resolutions)
        {
            if (prevreswidth != res.width && prevresheigth != res.height)
            {
                resolutions.Add(res);
                noofresolutions++;
                prevreswidth = res.width;
                prevresheigth = res.height;
            }
        }
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {

            if (PlayerPrefs.GetInt("FirstTimeStarting") == 0)
            {
                PlayerPrefs.SetInt("vsync", QualitySettings.vSyncCount);
                currentresolution = noofresolutions - 1;
                Debug.Log(noofresolutions);
                PlayerPrefs.SetInt("Swidth", resolutions[currentresolution].width);
                PlayerPrefs.SetInt("Sheight", resolutions[currentresolution].height);
                PlayerPrefs.SetInt("CurrentResolution", currentresolution);
                PlayerPrefs.SetInt("ScreenMode", 1);
                PlayerPrefs.SetFloat("MusicVolumeBar", Musicvolume.anchoredPosition.x);
                PlayerPrefs.SetFloat("MasterVolumeBar", Mastervolume.anchoredPosition.x);
                PlayerPrefs.SetFloat("MusicVolume", 0.222f);
                PlayerPrefs.SetFloat("MasterVolume", 0.333f);
                PlayerPrefs.SetInt("CurrentLevel", 1);
                PlayerPrefs.SetInt("FirstTimeStarting", 1);


            }
            int vsynccount = PlayerPrefs.GetInt("vsync");
            QualitySettings.vSyncCount = vsynccount;
            if (vsynccount == 1)
            {
                vsync.text = "ON";
            }

            currentresolution = PlayerPrefs.GetInt("CurrentResolution");
            int width = resolutions[currentresolution].width;
            int height = resolutions[currentresolution].height;
            int refreshrate = resolutions[currentresolution].refreshRate;
            bool fullscreen = false;
            if (PlayerPrefs.GetInt("ScreenMode") == 1)
            {
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                screenmode.text = "ON";
                fullscreen = true;
            }
            else
            {
                Screen.fullScreenMode = FullScreenMode.Windowed;
                screenmode.text = "OFF";
                fullscreen = false;
            }

            Screen.SetResolution(width, height, fullscreen);
            Resolution.text = width.ToString() + " x " + height.ToString();
            Musicvolume.anchoredPosition = new Vector2(PlayerPrefs.GetFloat("MusicVolumeBar"), Musicvolume.anchoredPosition.y);
            Mastervolume.anchoredPosition = new Vector2(PlayerPrefs.GetFloat("MasterVolumeBar"), Musicvolume.anchoredPosition.y);
        }
    }

    private void Start()
    {
        if(SceneManager.GetActiveScene().buildIndex == 0)
        {
            int gamestarted = PlayerPrefs.GetInt("GameStarted");
            int level = PlayerPrefs.GetInt("CurrentLevel");
            if (gamestarted == 1)
            {
                ResumeGame.interactable = true;
            }
        }
    }

    // Update is called once per frame
    public void StartNewGame()
    {
        PlayerPrefs.SetInt("GameStarted", 1);
        PlayerPrefs.SetFloat("UpgradePoints", 0);
        PlayerPrefs.SetFloat("Health", 500);
        PlayerPrefs.SetFloat("Damage", 1f);
        PlayerPrefs.SetFloat("HealthLevel", 1);
        PlayerPrefs.SetFloat("DamageLevel", 1);
        GameManager.Instance.Loadlevel(1);
        mainMenuCanvas.SetActive(false);
        startGameCanvas.SetActive(false);
    }
    public void Resumegame()
    {
        int level = PlayerPrefs.GetInt("CurrentLevel", 0);
        GameManager.Instance.Loadlevel(level);
        mainMenuCanvas.SetActive(false);
        startGameCanvas.SetActive(false);
    }
    public void startBloodyOcean()
    {
        GameManager.Instance.Loadlevel(6);
        mainMenuCanvas.SetActive(false);
        startGameCanvas.SetActive(false);
    }

    public void enableCampaignModeCanvas()
    {

        mainMenuCanvas.SetActive(false);
        campaignModeCanvas.SetActive(true);
    }

    public void onQuitCampaignMode()
    {
        mainMenuCanvas.SetActive(true);
        campaignModeCanvas.SetActive(false);
    }

    public void enableOptionsCanvas()
    {
        mainMenuCanvas.SetActive(false);
        optionsCanvas.SetActive(true);
    }

    public void enableCreditsCanvas()
    {
        mainMenuCanvas.SetActive(false);
        creditsCanvas.SetActive(true);
    }

    public void enableQuitCanvas()
    {

        mainMenuCanvas.SetActive(false);
        quitCanvas.SetActive(true);
    }

    public void OnApplicationQuit()
    {
        Application.Quit();
    }

    public void onBackQuit()
    {

        mainMenuCanvas.SetActive(true);
        quitCanvas.SetActive(false);
    }
    public void TurnVsynOnorOff()
    {
        if (vsync.text == "OFF")
        {
            QualitySettings.vSyncCount = 1;
            vsync.text = "ON";
            PlayerPrefs.SetInt("vsync", 1);
        }
        else
        {
            QualitySettings.vSyncCount = 0;
            vsync.text = "OFF";
            PlayerPrefs.SetInt("vsync", 0);
        }

    }
    public void changeResolutionRight()
    {
        bool flag = false;
        currentresolution += 1;
        if (currentresolution >= resolutions.Count)
            currentresolution = 0;
        bool fullscreen = false;
        if (screenmode.text == "ON")
            fullscreen = true;
        int width = resolutions[currentresolution].width;
        int height = resolutions[currentresolution].height;
        int refreshrate = resolutions[currentresolution].refreshRate;
        Screen.SetResolution(width, height, fullscreen);
        Resolution.text = width.ToString() + " x " + height.ToString();
        PlayerPrefs.SetInt("Swidth", resolutions[currentresolution].width);
        PlayerPrefs.SetInt("Sheight", resolutions[currentresolution].height);
        PlayerPrefs.SetInt("CurrentResolution", currentresolution);

    }
    public void changeResolutionLeft()
    {
        currentresolution -= 1;
        if (currentresolution < 0)
            currentresolution = resolutions.Count - 1;
        bool fullscreen = false;
        if (PlayerPrefs.GetInt("ScreenMode") == 1)
            fullscreen = true;
        int width = resolutions[currentresolution].width;
        int height = resolutions[currentresolution].height;
        int refreshrate = resolutions[currentresolution].refreshRate;
        Screen.SetResolution(width, height, fullscreen);
        Resolution.text = width.ToString() + " x " + height.ToString();
        PlayerPrefs.SetInt("Swidth", resolutions[currentresolution].width);
        PlayerPrefs.SetInt("Sheight", resolutions[currentresolution].height);
        PlayerPrefs.SetInt("CurrentResolution", currentresolution);
    }
    public void ChangeScreenModer()
    {
        if (screenmode.text == "ON")
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
            screenmode.text = "OFF";
            PlayerPrefs.SetInt("ScreenMode", 0);
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            screenmode.text = "ON";
            PlayerPrefs.SetInt("ScreenMode", 1);
        }

    }

    public void IncreaseMasterVolume()
    {
        float volume = Mastervolume.anchoredPosition.x + 50f;
        if (volume > MaxMastervolumerange)
            volume = MaxMastervolumerange;
        SoundManager.Instance.changeSoundVolume(0.111f);
        Mastervolume.anchoredPosition = new Vector2(volume, Mastervolume.anchoredPosition.y);

        // PlayerPrefs.SetFloat("MasterVolume", mastersound.volume);
        PlayerPrefs.SetFloat("MasterVolumeBar", Mastervolume.anchoredPosition.x);
    }
    public void DecreaseMasterVolume()
    {

        float volume = Mastervolume.anchoredPosition.x - 50f;
        if (volume < MinMastervolumerange)
            volume = MinMastervolumerange;
        SoundManager.Instance.changeSoundVolume(-0.111f);
        Mastervolume.anchoredPosition = new Vector2(volume, Mastervolume.anchoredPosition.y);
        // PlayerPrefs.SetFloat("MasterVolume", mastersound.volume);
        PlayerPrefs.SetFloat("MasterVolumeBar", Mastervolume.anchoredPosition.x);
    }
    public void IncreaseMusicVolume()
    {
        float volume = Musicvolume.anchoredPosition.x + 50f;
        if (volume > MaxMusicvolumerange)
            volume = MaxMusicvolumerange;
        SoundManager.Instance.changeMusicVolume(0.111f);
        Musicvolume.anchoredPosition = new Vector2(volume, Musicvolume.anchoredPosition.y);
        // PlayerPrefs.SetFloat("MusicVolume", musicsound.volume);
        PlayerPrefs.SetFloat("MusicVolumeBar", Musicvolume.anchoredPosition.x);
    }
    public void DecreaseMusicVolume()
    {

        float volume = Musicvolume.anchoredPosition.x - 50f;
        if (volume < MinMusicvolumerange)
            volume = MinMusicvolumerange;
        SoundManager.Instance.changeMusicVolume(-0.111f);

        Musicvolume.anchoredPosition = new Vector2(volume, Musicvolume.anchoredPosition.y);
        //PlayerPrefs.SetFloat("MusicVolume", musicsound.volume);
        PlayerPrefs.SetFloat("MusicVolumeBar", Musicvolume.anchoredPosition.x);
    }
}
