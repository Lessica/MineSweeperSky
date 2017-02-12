﻿using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    // handles
    public GameManager GM;

    // private variables
    private bool _isCustom;
    private GameSettings _UI_GameSettings;

    [SerializeField] private MenuElements _menu;
    [SerializeField] private HUDElements _hud;


    //===========================================================
    // Function Definitions

    // getters & setters
    public MenuElements Menu
    {
        get { return _menu; }
    }
        
    public HUDElements HUD
    {
        get { return _hud; }
        set { _hud = value; }
    }   // TODO: CHECK ACCESS ??

    // unity functions
    void Awake()
    {
		int difficulty = PlayerPrefs.GetInt ("difficulty");
		GameObject.Find ("Difficulty_Slider").GetComponent<Slider> ().value = difficulty;
		int background = PlayerPrefs.GetInt ("background");
		GameObject.Find ("Background_Slider").GetComponent<Slider> ().value = background;
		int sound = PlayerPrefs.GetInt ("sound");
		if (sound == 0) {
			GameObject.Find ("Sound_Toggle").GetComponent<Toggle> ().isOn = false;
		} else {
			GameObject.Find ("Sound_Toggle").GetComponent<Toggle> ().isOn = true;
		}
		float volume = PlayerPrefs.GetFloat ("volume");
		GameObject.Find ("Sound_Slider").GetComponent<Slider> ().value = volume;
    }

	void Start ()
    {
        // initial update
	    _UI_GameSettings = GM.Settings;
	    WriteSettingsToInputText(_UI_GameSettings);  
	}

    void Update()
    {
		
    }

	public void SoundToggleUpdate(Boolean enabled) {
		GameObject.Find ("Main Camera").GetComponent<AudioSource> ().mute = !enabled;
		if (enabled) {
			PlayerPrefs.SetInt ("sound", 1);
		} else {
			PlayerPrefs.SetInt ("sound", 0);
		}
	}

	public void VolumeSliderUpdate(float val) {
		PlayerPrefs.SetFloat("volume", val);
	}

    //-----------------------------------------------------------
    // Game Options Function Definitions
    public void OptionSliderUpdate(float val)
    {
        int sliderValue = (int) val;    // slider has whole numbers --> safe to cast int

        switch (sliderValue)
        {
            case 0:     // beginner settings
                _menu.SliderDifficulty.text = "Beginner";
                _UI_GameSettings = GameSettings.Beginner;
                break;

            case 1:     // intermediate settings
                _menu.SliderDifficulty.text = "Intermediate";
                _UI_GameSettings = GameSettings.Intermediate;
                break;

            case 2:     // expert settings
                _menu.SliderDifficulty.text = "Expert";
                _UI_GameSettings = GameSettings.Expert;
                break;
            case 3:
                _menu.SliderDifficulty.text = "Custom";
                break;
        }

        WriteSettingsToInputText(_UI_GameSettings);

        // Set Custom Settings according to slider value. Read funct description
        SetCustomSettings(sliderValue == 3);

        //Debug.Log("UIMANAGER::SLIDERUPDATE val=" + sliderValue);

		PlayerPrefs.SetInt ("difficulty", sliderValue);
    }

    // Sets the interactability of input fields: if(3) true, else false;
    void SetCustomSettings(bool isCustom)
    {
        // set interactability
        _menu.WidthInput.interactable = isCustom;
        _menu.HeightInput.interactable = isCustom;
        _menu.MinesInput.interactable = isCustom;
        _isCustom = isCustom;

        //Debug.Log("MinesInput: " + _minesInput.IsInteractable());
    }

    public GameSettings ReadSettings()
    {
        if (_isCustom)
        {
            int w = Int32.Parse(_menu.WidthInput.text);
            int h = Int32.Parse(_menu.HeightInput.text);
            int m = Int32.Parse(_menu.MinesInput.text);
			PlayerPrefs.SetInt ("width", w);
			PlayerPrefs.SetInt ("height", h);
			PlayerPrefs.SetInt ("mines", m);
            _UI_GameSettings = new GameSettings(w, h, m, "custom");
        }

        return _UI_GameSettings;
    }

    void WriteSettingsToInputText(GameSettings settings)
    {
        if (settings == null)
        {
            Debug.Log("Settings NULL");
            return;
        }
        _menu.WidthInput.text   = settings.Width.ToString();
        _menu.HeightInput.text  = settings.Height.ToString();
        _menu.MinesInput.text   = settings.Mines.ToString();
    } // called from ReadSettings()

    //-----------------------------------------------------------
    // Game Options Error dialogue options

    public void EnableInputErrorDialogue()
    {
        _menu.InputErrorCanvas.enabled = true;
    }

    public void DisableInputErrorDialogue()
    {
        _menu.InputErrorCanvas.enabled = false;
    }

    public void ToggleInputErrorDialogue()
    {
        _menu.InputErrorCanvas.enabled = !_menu.InputErrorCanvas.enabled;
    }

    //-----------------------------------------------------------
    // "Other Settings" Function Definitions
    public void BackgroundSliderUpdate(float val)
    {
		PlayerPrefs.SetInt ("background", (int) val);
        GameObject.Find("Skybox Camera").GetComponent<Skybox>().material = Menu.Skyboxes[(int) val];
    }

    //-----------------------------------------------------------
    // Scores functions
//    public void GameOverButton()
//    {
//        GM.GameOver(true);
//    }

    //-------------------------------------
    // HUD Function definitons

    public void UpdateTimeText(int time)
    {
        _hud.TimerText.text = "Time ";
		if (time < 10) {
			_hud.TimerText.text += "000" + time;
		} else if (time < 100) {
			_hud.TimerText.text += "00" + time;
		} else if (time < 1000) {
			_hud.TimerText.text += "0" + time;
		} else if (time < 10000) {
			_hud.TimerText.text += time.ToString();
		}
    }

    public void UpdateFlagText(int flagCount)
    {
        _hud.FlagText.text = "Flags ";

        // handle the sign of the counter
        string flagCountText = "";
        if (flagCount < 0)
        {
            flagCountText += "-";
        }

        // set the counter value according to digit number of flag count
        flagCount = Mathf.Abs(flagCount);   // ignore sign
        if (Mathf.Abs(flagCount) < 10)
        {
            flagCountText += "00";
        }
        else if (Mathf.Abs(flagCount) < 100)
        {
            flagCountText += "0";
        }
        flagCountText += flagCount;
        

        // add the constructed flag count text to the UI Text
        _hud.FlagText.text += flagCountText;

    }

    public void RestartButton()
    {
        GM.StartNewGame(GM.Settings);
    }

	public void FlagButton()
	{
		PlayerInput.FlagMode = !PlayerInput.FlagMode;
	}

    public void NewGameButton()
    {
        GameSettings settings = ReadSettings();

        if (settings.isValid())
        {
            DisableInputErrorDialogue();
            GM.StartNewGame(settings);
            GM.GetComponent<PlayerInput>().TogglePauseMenu(); // TODO: close dialogue box if open in toggle funct
        }
        else
        {
            EnableInputErrorDialogue();
            Debug.Log("INVALID SETTINGS!");
        }
    }

    public void ResetHUD(int flagCount)
    {
        UpdateFlagText(flagCount);
        UpdateTimeText(0);   
    }
}

[Serializable]
public class MenuElements
{

    // GameOptions Elements
    [SerializeField] private InputField _heightInput;
    [SerializeField] private InputField _widthInput;
    [SerializeField] private InputField _minesInput;
    [SerializeField] private Text _sliderDifficulty;
    [SerializeField] private Material[] _skyboxes;
    [SerializeField] private Canvas _inputErrorCanvas;

    // Debug UI variables
    [SerializeField] private Text _timeScaleText;

    // getters & setters
    public Text TimeScaleText
    {
        get { return _timeScaleText; }
        set { _timeScaleText = value; }
    }

    public InputField HeightInput
    {
        get { return _heightInput; }
        set { _heightInput = value; }
    }

    public InputField MinesInput
    {
        get { return _minesInput; }
        set { _minesInput = value; }
    }

    public InputField WidthInput
    {
        get { return _widthInput; }
        set { _widthInput = value; }
    }

    public Text SliderDifficulty
    {
        get { return _sliderDifficulty; }
        set { _sliderDifficulty = value; }
    }

    public Material[] Skyboxes
    {
        get { return _skyboxes; }
    }

    public Canvas InputErrorCanvas
    {
        get { return _inputErrorCanvas; }
    }
}

[Serializable]
public class HUDElements
{
    [SerializeField] private Text _timerText;
    [SerializeField] private Text _flagText;


    // getters & setters
    public Text TimerText
    {
        get { return _timerText; }
        set { _timerText = value; }
    }

    public Text FlagText
    {
        get { return _flagText; }
        set { _flagText = value; }
    }
}
