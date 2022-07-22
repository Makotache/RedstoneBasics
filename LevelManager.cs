using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    /*  [SerializeField]
        private TextMeshProUGUI _buttonTurnRight;
        [SerializeField]
        private TextMeshProUGUI _buttonTurnLeft;*/

    public InteractTileManager _interactTileManager { get; private set; }
    public GridManager _gridManager { get; private set; }
    public UiManager _uiManager { get; private set; }

    [SerializeField] private int _level = 0;

    public bool gameIsWin { get; private set; } = false;
    private bool lvl2_CheckLampWaitBool = false;
    private float lvl2_CheckLampWaitTime;

    void Awake()
    {
        //récupération du manager d'interaction des tuiles
        _interactTileManager = GameObject.FindObjectOfType<InteractTileManager>();
        _gridManager = GameObject.FindObjectOfType<GridManager>();
        _uiManager = GameObject.FindObjectOfType<UiManager>();
        
        switch(_level)
        {
            case -1:
                _uiManager.FreeMode();
                break;

            case 1:
                _uiManager.SetVictoryText("Enable Redstone Lamp, use Redstone Torch and Redstone Wire.");
                break;
            
            case 2:
                _uiManager.SetVictoryText("Enable Redstone Lamp on Top-Right, wait at least 2 seconds, and enable Redstone Lamp on Bottom-Right. Redstone Repeater at 1 notch, wait 0.1 second, use configure button on Top-Right to change notch.");
                break;
        }
        _uiManager.Init(_level);
    }

    private void FixedUpdate()
    {
        if(_level == 1)
        {
            Vector2Int pos_lamp_0 = _gridManager.finalLamp[0];
            if(_gridManager.grid[pos_lamp_0.x, pos_lamp_0.y].GetComponent<RedstoneLamp>().isActive)
            {
                gameIsWin = true;
            }
        }
        else if (_level == 2)
        {
            Vector2Int pos_lamp_0 = _gridManager.finalLamp[0];
            Vector2Int pos_lamp_1 = _gridManager.finalLamp[1];

            if (_gridManager.grid[pos_lamp_0.x, pos_lamp_0.y].GetComponent<RedstoneLamp>().isActive)
            {
                if(!lvl2_CheckLampWaitBool)
                {
                    lvl2_CheckLampWaitTime = Time.time - Time.deltaTime;
                    lvl2_CheckLampWaitBool = true;
                }
                else if(_gridManager.grid[pos_lamp_1.x, pos_lamp_1.y].GetComponent<RedstoneLamp>().isActive)
                {
                    if (Time.time < lvl2_CheckLampWaitTime + 2 + Time.deltaTime)
                    {
                        lvl2_CheckLampWaitBool = false;
                    }
                    else
                    //if (Time.time >= lvl2_CheckLampWaitTime + 2 + Time.deltaTime)
                    {
                        lvl2_CheckLampWaitBool = false;
                        gameIsWin = true;
                    }
                }
            }
        }


        if(gameIsWin)
        {
            _uiManager.Win();
        }
    }

    public void TurnTileMode(bool turnRight)
    {
        //si on a le mode pour tourner les tuiles activer
        if(_interactTileManager.turnTileDirection != TurnTileDirection.None)
        {
            //alors on le désactive
            _interactTileManager.turnTileDirection = TurnTileDirection.None;
        }
        //sinon si on décide de tourner a droite
        else if(turnRight)
        {
            //alors on tourne a droite
            _interactTileManager.turnTileDirection = TurnTileDirection.Right;
        }
        else
        {
            //alors on tourne a gauche
            _interactTileManager.turnTileDirection = TurnTileDirection.Left;
        }
    }

    public void RemoveTile()
    {
        _interactTileManager.isRemoveItem = !_interactTileManager.isRemoveItem;
    }

    public void ConfigTile()
    {
        _interactTileManager.configTile = !_interactTileManager.configTile;
    }

}
