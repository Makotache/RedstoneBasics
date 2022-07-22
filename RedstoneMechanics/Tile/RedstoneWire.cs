using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public enum RedstoneWireType
{
    I, L,
    T, X
}

public class RedstoneWire : RedstoneBase
{
    [SerializeField]
    private Sprite _spriteDustOn;
    [SerializeField]
    private Sprite _spriteDustOff;

    [SerializeField]
    private Sprite _spriteSplitOn;
    [SerializeField]
    private Sprite _spriteSplitOff;

    [SerializeField]
    private GameObject childSpriteObjDust;
    private SpriteRenderer childObjSpriteRendererDust;

    [SerializeField]
    private GameObject childSpriteObjNorth;
    private SpriteRenderer childObjSpriteRendererNorth;

    [SerializeField]
    private GameObject childSpriteObjEast;
    private SpriteRenderer childObjSpriteRendererEast;

    [SerializeField]
    private GameObject childSpriteObjSouth;
    private SpriteRenderer childObjSpriteRendererSouth;

    [SerializeField]
    private GameObject childSpriteObjWest;
    private SpriteRenderer childObjSpriteRendererWest;

    [SerializeField]
    private TextMeshProUGUI _showStrengthSignal;


    [SerializeField]
    private RedstoneWireType _currentRedstoneWireType = RedstoneWireType.X;
    public RedstoneWireType currentRedstoneWireType //{ get ; protected set; } = RedstoneWireType.I;
    {
        get => _currentRedstoneWireType;
        protected set
        {
            _currentRedstoneWireType = value;
            ChangeSpriteVisibility();
        }
    }

    void Awake()
    {
        //on initialise les SpriteRendrer des différentes parties du fil

        childObjSpriteRendererDust = childSpriteObjDust.GetComponent<SpriteRenderer>();

        childObjSpriteRendererNorth = childSpriteObjNorth.GetComponent<SpriteRenderer>();
        childObjSpriteRendererEast = childSpriteObjEast.GetComponent<SpriteRenderer>();

        childObjSpriteRendererSouth = childSpriteObjSouth.GetComponent<SpriteRenderer>();
        childObjSpriteRendererWest = childSpriteObjWest.GetComponent<SpriteRenderer>();


        //on définit le type de composant (ici un fil de redstone)
        redstoneTileType = RedstoneTileType.Wire;

        inputOutputConnector_dict.Add(InputOutputConnector.North, InputOutputType.None);
        inputOutputConnector_dict.Add(InputOutputConnector.East, InputOutputType.None);
        inputOutputConnector_dict.Add(InputOutputConnector.South, InputOutputType.None);
        inputOutputConnector_dict.Add(InputOutputConnector.West, InputOutputType.None);

        

        //on fais en sorte de changer le TEXTE de la valeur du signal en fonction
        //de la VALEUR du signal a chaque changement de la valeur du signal
        changeSignal.AddListener(ChangeTextStrenghtSignal);

        //on met les sprites en fonctionde l'état de la tuiles
        //endStart.AddListener(ReloadHisRedstone);
    }

    public override void ChangeConnectorCount()
    {
        RedstoneBase otherTile;

        //on actualise le fil de redstone en fonction de s'il y a des Input/Output aux alentours
        //North
        if (gridManager.TileExist(new Vector2Int(posOfTile.x, posOfTile.y + 1), out otherTile) && ((otherTile.redstoneTileType != RedstoneTileType.Wire && otherTile.forceConnectToIt) || otherTile.redstoneTileType == RedstoneTileType.Wire))
        {
            inputOutputConnector_dict[InputOutputConnector.North] = IsInputOrOutput(otherTile.inputOutputConnector_dict[InputOutputConnector.South]) || otherTile.redstoneTileType == RedstoneTileType.Wire 
                ? InputOutputType.InputOutput : InputOutputType.None;
            //inputOutputConnector_dict[InputOutputConnector.North] = tile.currentRedstoneTileType == RedstoneTileType.Wire ? InputOutputType.InputOuput : InputOutputType.None;
        }
        else
        {
            inputOutputConnector_dict[InputOutputConnector.North] = InputOutputType.None;
        }

        //East
        if (gridManager.TileExist(new Vector2Int(posOfTile.x + 1, posOfTile.y), out otherTile) && ((otherTile.redstoneTileType != RedstoneTileType.Wire && otherTile.forceConnectToIt) || otherTile.redstoneTileType == RedstoneTileType.Wire))
        {
            inputOutputConnector_dict[InputOutputConnector.East] = IsInputOrOutput(otherTile.inputOutputConnector_dict[InputOutputConnector.West]) || otherTile.redstoneTileType == RedstoneTileType.Wire 
                ? InputOutputType.InputOutput : InputOutputType.None;
        }
        else
        {
            inputOutputConnector_dict[InputOutputConnector.East] = InputOutputType.None;
        }

        //South
        if (gridManager.TileExist(new Vector2Int(posOfTile.x, posOfTile.y - 1), out otherTile) && ((otherTile.redstoneTileType != RedstoneTileType.Wire && otherTile.forceConnectToIt) || otherTile.redstoneTileType == RedstoneTileType.Wire))
        {
            inputOutputConnector_dict[InputOutputConnector.South] = IsInputOrOutput(otherTile.inputOutputConnector_dict[InputOutputConnector.North]) || otherTile.redstoneTileType == RedstoneTileType.Wire 
                ? InputOutputType.InputOutput : InputOutputType.None;
        }
        else
        {
            inputOutputConnector_dict[InputOutputConnector.South] = InputOutputType.None;
        }

        //West
        if (gridManager.TileExist(new Vector2Int(posOfTile.x - 1, posOfTile.y), out otherTile) && ((otherTile.redstoneTileType != RedstoneTileType.Wire && otherTile.forceConnectToIt) || otherTile.redstoneTileType == RedstoneTileType.Wire))
        {
            inputOutputConnector_dict[InputOutputConnector.West] = IsInputOrOutput(otherTile.inputOutputConnector_dict[InputOutputConnector.East]) || otherTile.redstoneTileType == RedstoneTileType.Wire 
                ? InputOutputType.InputOutput : InputOutputType.None;
        }
        else
        {
            inputOutputConnector_dict[InputOutputConnector.West] = InputOutputType.None;
        }


        //on change le type de fil en fonction du nombre de connecteur
        base.ChangeConnectorCount();
        if(connectorCount == 0)
        {
            connectorCount = 4;
            inputOutputConnector_dict[InputOutputConnector.North] = InputOutputType.InputOutput;
            inputOutputConnector_dict[InputOutputConnector.East] = InputOutputType.InputOutput;
            inputOutputConnector_dict[InputOutputConnector.South] = InputOutputType.InputOutput;
            inputOutputConnector_dict[InputOutputConnector.West] = InputOutputType.InputOutput;
        }
        ChangeTypeWire();
    }


    public override void ReloadHisRedstone(RedstoneBase itemWhoReloadThis, List<RedstoneBase> parentSource)
    {
        //liste des précédant tuile ayant utilisé la méthode "ReloadHisRedstone"
        base.ReloadHisRedstone(itemWhoReloadThis, parentSource);

        //changement du type de fil
        ChangeConnectorCount();

        //changement de la force de signal en fonction des info précédante
        ReloadStrengthSignal();
        
        //reload de la redstone autour de la tuile
        //ReloadAroundRedstone();

        parentTiles_lst.Clear();
    }


    public void ReloadStrengthSignal()//bool havePerviouslySourceLoaded)
    {
        RedstoneBase otherTile;

        int maxStrengthSignalValue = 0;
        
        
        InputOutputConnector[] inputs = GetAllInput(this);

        for (int i = 0; i < inputs.Length; i++)
        {
            if (gridManager.TileHaveOutputOrIsWireAt(GetPosByDirection(posOfTile, inputs[i]), GetInvertConnector(inputs[i]), out otherTile))
            {

                int otherTileStrength = otherTile.strengthSignal;

                if (otherTile.redstoneTileType == RedstoneTileType.Wire)
                {
                    //on enlève 1 que si le signal proviens d'un fil
                    otherTileStrength--;
                }

                if (maxStrengthSignalValue < otherTileStrength)
                {
                    maxStrengthSignalValue = otherTileStrength;
                }
            }
        }

        this.strengthSignal = maxStrengthSignalValue;

    }


    private void ChangeTypeWire()
    {
        switch (connectorCount)
        {
            case 1:
                //North
                if (inputOutputConnector_dict[InputOutputConnector.North] == InputOutputType.InputOutput)
                {
                    inputOutputConnector_dict[InputOutputConnector.South] = InputOutputType.InputOutput;
                }
                //East
                else if (inputOutputConnector_dict[InputOutputConnector.East] == InputOutputType.InputOutput)
                {
                    inputOutputConnector_dict[InputOutputConnector.West] = InputOutputType.InputOutput;
                }
                //South
                else if (inputOutputConnector_dict[InputOutputConnector.South] == InputOutputType.InputOutput)
                {
                    inputOutputConnector_dict[InputOutputConnector.North] = InputOutputType.InputOutput;
                }
                //West
                else
                {
                    inputOutputConnector_dict[InputOutputConnector.East] = InputOutputType.InputOutput;
                }

                connectorCount++;
                currentRedstoneWireType = RedstoneWireType.I;

                break;

            case 2:
                if (inputOutputConnector_dict[InputOutputConnector.North] == InputOutputType.InputOutput && inputOutputConnector_dict[InputOutputConnector.South] == InputOutputType.InputOutput ||
                    inputOutputConnector_dict[InputOutputConnector.East] == InputOutputType.InputOutput && inputOutputConnector_dict[InputOutputConnector.West] == InputOutputType.InputOutput)
                {
                    currentRedstoneWireType = RedstoneWireType.I;
                }
                else
                {
                    currentRedstoneWireType = RedstoneWireType.L;
                }
                break;

            case 3:
                currentRedstoneWireType = RedstoneWireType.T;
                break;

            case 4:
                currentRedstoneWireType = RedstoneWireType.X;
                break;
        }
    }

    private void ChangeSpriteVisibility()
    {
        //on change l'endroit on l'on vois les différents sprite "split"
        VisibilitySprite(InputOutputConnector.North, childObjSpriteRendererNorth, childSpriteObjNorth);
        VisibilitySprite(InputOutputConnector.East, childObjSpriteRendererEast, childSpriteObjEast);
        VisibilitySprite(InputOutputConnector.South, childObjSpriteRendererSouth, childSpriteObjSouth);
        VisibilitySprite(InputOutputConnector.West, childObjSpriteRendererWest, childSpriteObjWest);

        childObjSpriteRendererDust.sprite = isActive ? _spriteDustOn : _spriteDustOff;
        childSpriteObjDust.SetActive(currentRedstoneWireType != RedstoneWireType.I);
    }

    private void VisibilitySprite(InputOutputConnector inputOutputConnector, SpriteRenderer spriteRenderer, GameObject childObj)
    {
        if (inputOutputConnector_dict[inputOutputConnector] != InputOutputType.None)
        {
            spriteRenderer.sprite = isActive ? _spriteSplitOn : _spriteSplitOff;
            childObj.SetActive(true);
        }
        else
        {
            childObj.SetActive(false);
        }
    }

    private void ChangeTextStrenghtSignal()
    {
        ChangeSpriteVisibility();
        _showStrengthSignal.text = strengthSignal.ToString();
    }

}
