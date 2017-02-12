using UnityEngine;


//---------------
// Mouse logic:
// - Left&Right:    if(tile is revealed)        Reveal Neighbors
// - Left click:    if(not after Left&Right)    Reveal Tile
// - Right click:   if(not after Left&Right)    Flag Tile

public class PlayerInput : MonoBehaviour {

	public AudioClip playOnceSoundBoom;
	private AudioSource _audioSource;

    //-------------------------------------
    // Variable Declarations

    // static variables
    private static bool _rightAndLeftPressed;
    private static bool _revealAreaIssued;
    private static bool _initialClickIssued;    // used to track first-click-death and start-timer
	private static bool _flagMode;

	public float TouchTime;


    // private variables
    private GridScript _grid;

    // handles
    public UIManager UI;
    public Canvas PauseMenu;

    //--------------------------------------------------------
    // Function Definitions

    // getters & setters
    public GridScript Grid
    {
        get { return _grid; }
        set { _grid = value; }
    }

    public static bool InitialClickIssued
    {
        get { return _initialClickIssued; }
        set
        {
            _initialClickIssued = value;
        }
    }

	public static bool FlagMode
	{
		get { return _flagMode; }
		set { 
			_flagMode = value;
			GameObject.Find ("Flag_Button").GetComponent<ButtonScript> ().TurnFlag ();
		}
	}

    // unity functions

	void Awake() {
		_audioSource = this.gameObject.AddComponent<AudioSource>();
		_audioSource.loop = false;
	}

	void Update () 
    {
		
    }

    public void TogglePauseMenu()
    {
        // Toggle pause menu and companion components
        PauseMenu.enabled = !PauseMenu.enabled;

        // update gamestate
        if(!GameManager.IsGameOver)
            GameManager.IsGamePaused = PauseMenu.enabled;

        Time.timeScale = System.Convert.ToSingle(!PauseMenu.enabled);
    }

    public void OnMouseOver(Tile tile)
    {
		if (Input.touchCount == 1) {
			TouchPhase phase = Input.GetTouch (0).phase;
			// HIGHLIGHT TILE
			if (phase == TouchPhase.Began || phase == TouchPhase.Moved || phase == TouchPhase.Stationary) {
				if (!tile.IsRevealed () && !tile.IsFlagged ()) {
					_grid.HighlightTile (tile.GridPosition);
				}
				if (phase == TouchPhase.Began) {
					// RECORD TIME
					TouchTime = Time.time;
				} else if (Time.time - TouchTime > .5) {
					// LONG PRESS: HIGHLIGHT NEIGHBOURS
					_grid.HighlightArea(tile.GridPosition);
					if (!tile.IsRevealed() && !tile.IsFlagged()) tile.Highlight();
					_rightAndLeftPressed = true;
				}
			} else if (phase == TouchPhase.Ended) {
				// REVEAL NEIGHBOURS
				if (_rightAndLeftPressed == true) {
					_rightAndLeftPressed = false;
					if (tile.IsRevealed () && tile.IsNeighborsFlagged ()) {
						_grid.RevealArea (tile);
					} else {
						_grid.RevertHighlightArea (tile.GridPosition);
						_grid.RevertHighlightTile (tile.GridPosition);
					}
				} else if (!tile.IsRevealed ()) {
					// REVEAL TILE
					if (PlayerInput.FlagMode) {
						tile.ToggleFlag ();
					} else if (!tile.IsFlagged()) {
						if (!_initialClickIssued) {
							if (tile.IsMine ()) {
								_grid.SwapTileWithMineFreeTile (tile.GridPosition);
							}
							_initialClickIssued = true;
							GetComponent<GameManager> ().StartTimer ();
							tile.Reveal ();
						} else {
							tile.Reveal ();
							// PLAY BOMB SOUND
							if (tile.IsMine ()) {
								AudioSource mainSource = GameObject.Find ("Main Camera").GetComponent<AudioSource> ();
								_audioSource.volume = mainSource.volume;
								if (!mainSource.mute) {
									_audioSource.PlayOneShot (playOnceSoundBoom);
								}
							}
						}
					}
				}

			}
		}
    }

    public void OnMouseExit(Tile tile)
    {
    
        // revert highlighted tiles
        if(!tile.IsRevealed() && !tile.IsFlagged()) 
            tile.RevertHighlight();

        foreach (Vector2 pos in tile.NeighborTilePositions)
        {
            Tile neighbor = _grid.Map[(int) pos.x][(int) pos.y];
            if (!neighbor.IsRevealed() && !neighbor.IsFlagged())
                neighbor.RevertHighlight();
        }
        
    }

}
