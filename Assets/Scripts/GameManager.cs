using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Image[] difficultyImages;
    [SerializeField]
    private GameObject[] difficultyGroups;
    private GameObject _selectedGroup;
    [SerializeField]
    private GameObject gamePanel;
    [SerializeField]
    private GameObject difficultyPanel;
    [SerializeField]
    private Sprite[] itemsSprites;
    [SerializeField]
    private GameObject nextLevelButton;
    [SerializeField]
    private GameObject WinPanel;
    [SerializeField]
    private GameObject losePanel;
    [SerializeField]
    private AudioSource musicSource;
    [SerializeField]
    private AudioSource soundSource;
    [SerializeField] Slider soundSlider;
    [SerializeField] Slider musicSlider;
    private List<Sprite> correctSequence;
    private List<GameObject> items;
    private int _difficulty = 0;
    private int itemsSpawned;
    private GameObject _firstSelectedItem;
    private GameObject _secondSelectedItem;
    private GameObject remember_sequence_Text;
    [SerializeField]
    private AudioClip[] winLoseSounds;
    private bool _isPlaying = false;
    // Start is called before the first frame update
    void Start()
    {
        int restartSceneDifficulty = PlayerPrefs.GetInt("RestartDifficulty", 0);
        float _volumeSound = PlayerPrefs.GetFloat("soundVolume", 1);
        float _volumeMusic = PlayerPrefs.GetFloat("musicVolume", 1);
        musicSource.volume = _volumeMusic;
        soundSource.volume = _volumeSound;
        if ( restartSceneDifficulty > 0 )
        {
            _difficulty = restartSceneDifficulty;
            startGame();
            PlayerPrefs.SetInt("RestartDifficulty", 0);
        }
    }
    public void startGame()
    {
        soundSource.Play();
        //starting the game only when a difficulty level is selected
        if (_difficulty > 0)
        {
            //initialization
            correctSequence = new List<Sprite>();
            items = new List<GameObject>();
            remember_sequence_Text = gamePanel.transform.Find("repeat_text").gameObject;
            //calculating how many items will be spawned (to be added to a list)
            itemsSpawned = _difficulty * 3;
            //adjusting panels visibility accordingly
            difficultyPanel.SetActive(false);
            gamePanel.SetActive(true);
            //selecting the game object that holds the difficulty items (easy, middle, hard) depending on which difficulty the player selected
            //then making it visible
            _selectedGroup = difficultyGroups[_difficulty - 1];
            _selectedGroup.SetActive(true);

            //making a temp list of items sprites
            List<Sprite> tempSprites = new List<Sprite>(itemsSprites);
            //Selecting the background parent object of items in the current difficulty panel
            GameObject backgroundObject = _selectedGroup.transform.Find("background").gameObject;
            for (int i = 0; i < itemsSpawned; i++) //Looping through the items 
            {               
                //Selecting the i+1'th item to add a ranndom sprite to it
                GameObject item = backgroundObject.transform.Find($"item{i + 1}").gameObject;
                int randomIndex = Random.Range(0, tempSprites.Count - 1);
                item.GetComponent<Image>().sprite = tempSprites[randomIndex];
                //adding the item to the correct sequence list to use it later to compare
                correctSequence.Add(tempSprites[randomIndex]);
                Debug.Log($"correct item: {correctSequence[i]}");
                //removing the already added sprite from the list
                tempSprites.RemoveAt(randomIndex);
                items.Add(item);
            }
            tempSprites.Clear();
        }
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
    public void HowHard_U_WannaGo(int diff)
    {
        soundSource.Play();
        //setting difficulty in the class/scene
        _difficulty = diff;
        //set transparency of selected difficulty image to 1 
        Color color = difficultyImages[diff - 1].color;
        color.a = 1;
        difficultyImages[diff - 1].color = color;
        //set transparency of other difficulty images to 0.5
        for (int i = 0; i < 3; i++)
        {
            if (i == diff - 1) continue;
            else
            {
                color = difficultyImages[i].color;
                color.a = 0.5f;
                difficultyImages[i].color = color;
            }
        }
    }

    public void startShuffle()
    {
        soundSource.Play();
        StartCoroutine(shuffle());
    }
    private IEnumerator shuffle()
    {
        foreach (var item in items)
            item.SetActive(false);
        remember_sequence_Text.SetActive(false);
        for (int k = 0; k < itemsSpawned; k++)
        {
            for (int i = 0; i < itemsSpawned; i++)
            {
                //switching sprites of items
                Sprite temp = items[i].GetComponent<Image>().sprite;
                int rIndex = Random.Range(0, itemsSpawned - 1);
                items[i].GetComponent<Image>().sprite = items[rIndex].GetComponent<Image>().sprite;
                items[rIndex].GetComponent<Image>().sprite = temp;
                Debug.Log($"item II : {items[i]} // item rIndex : {items[rIndex]}");
            }
        }
        yield return new WaitForSeconds(1.0f);
        foreach (var item in items) item.SetActive(true);
        remember_sequence_Text.SetActive(true);
        _isPlaying = true;
    }
    public void itemClicked(Button button)
    {
        if (_isPlaying)
        {
            soundSource.Play();
            if (_firstSelectedItem == null)
            {
                _firstSelectedItem = button.gameObject;
                Debug.Log($"First selected item : {_firstSelectedItem}");
            }
            else if (_secondSelectedItem == null && _firstSelectedItem != button.gameObject)
            {
                _secondSelectedItem = button.gameObject;
                Debug.Log($"Second selected item : {_secondSelectedItem}");
                Sprite temp = _firstSelectedItem.GetComponent<Image>().sprite;
                _firstSelectedItem.GetComponent<Image>().sprite = _secondSelectedItem.GetComponent<Image>().sprite;
                _secondSelectedItem.GetComponent<Image>().sprite = temp;
                _firstSelectedItem = null;
                _secondSelectedItem = null;
            }
        }
    }
    
    public void readyClicked()
    {
        soundSource.Play();
        bool win = true;
        for (int i=0;i<items.Count;i++)
        {
            if (items[i].GetComponent<Image>().sprite != correctSequence[i])
            {
                win = false; break;
            }
        }
        if (win)
        {
            musicSource.clip = winLoseSounds[0];
            musicSource.loop = false;
            musicSource.Play();
            Debug.Log("WIN");
            if (_difficulty == 3) nextLevelButton.SetActive(false);
            gamePanel.SetActive(false);
            WinPanel.SetActive(true);
            
        }
        else
        {
            musicSource.clip = winLoseSounds[1];
            musicSource.loop = false;
            musicSource.Play();
            gamePanel.SetActive(false); 
            losePanel.SetActive(true);
        };
    }
    public void GetMeMenuBro()
    {
        soundSource.Play();
        SceneManager.LoadScene("MenuScene");
    }
    public void TravelNextHehe()
    {
        soundSource.Play();
        _difficulty++;
        PlayerPrefs.SetInt("RestartDifficulty", _difficulty);
        SceneManager.LoadScene("PlayScene");
    }
    public void repeatLevel()
    {
        soundSource.Play();
        PlayerPrefs.SetInt("RestartDifficulty", _difficulty);
        SceneManager.LoadScene("PlayScene");
    }
}
