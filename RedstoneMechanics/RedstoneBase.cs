using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class RedstoneBase : MonoBehaviour
{
    //par raport a la grille (grid)
    //public GridManager gridManager { get; protected set; }
    private GridManager _gridManager;
    public GridManager gridManager 
    { 
        get
        {
            if(_gridManager == null)
            {
                _gridManager = GetComponentInParent<GridManager>();
            }
            return _gridManager;
        }
        protected set => _gridManager = value; 
    }

    [HideInInspector]
    public Vector2Int posOfTile;


    [SerializeField]
    private bool _isSpecial = false;
    public bool isSpecial { get => _isSpecial; protected set => _isSpecial = value; }


    [SerializeField]
    private bool _isLikeASource = false;
    public bool isLikeASource { get => _isLikeASource; protected set => _isLikeASource = value; }

    protected bool haveLoadedParent = false;

    [SerializeField]
    private bool _canBeConfigurate = true;
    public bool canBeConfigurate { get => _canBeConfigurate; protected set => _canBeConfigurate = value; }

    [SerializeField]
    private bool _canBeTurn = true;
    public bool canBeTurn { get => _canBeTurn; protected set => _canBeTurn = value; }

    public bool canBeReplace = true;

    [SerializeField]
    private bool _forceConnectToIt = false;
    public bool forceConnectToIt { get => _forceConnectToIt; protected set => _forceConnectToIt = value; }


    //liste les précédentes tuile chargé lors du reload
    protected List<RedstoneBase> parentTiles_lst = new List<RedstoneBase>();

    //liste des tuiles dans le même ciruit 
    public List<RedstoneBase> sameCircuit_lst;

    //connecteur
    [SerializeField]
    private int _connectorCount = 0;
    public int connectorCount { get => _connectorCount; protected set => _connectorCount = value; }

    public Dictionary<InputOutputConnector, InputOutputType> inputOutputConnector_dict { get; protected set; } = new Dictionary<InputOutputConnector, InputOutputType>();

    //puissance signal
    public const int MIN_STRENGTH_SIGNAL = 0;
    public const int MAX_STRENGTH_SIGNAL = 15;

    [SerializeField]
    private int _strengthSignal = 0;
    public int strengthSignal
    {
        get 
        {
            /*if(_canUpdateOther)
            {
                _oldStrengthSignal = _strengthSignal;
            }*/
            return _strengthSignal;
        }

        set
        {
            _strengthSignal = Mathf.Clamp(value, MIN_STRENGTH_SIGNAL, MAX_STRENGTH_SIGNAL);
            isActive = _strengthSignal > 0;
            changeSignal.Invoke();
        }
    }

    private int _oldStrengthSignal;


    //le signal peut être forcEpar un répéteur ou un comparateur
    //public bool signalIsForced;

    //type de composant
    public RedstoneTileType redstoneTileType { get; protected set; }

    //activE- désactivE
    private bool _isActive;
    public bool isActive
    {
        get => _isActive;
        protected set
        {
            _isActive = value;
            ChangeSprite();
        }
    }

    //unity event
    public UnityEvent changeSignal { get; private set; } = new UnityEvent();
    public UnityEvent turnTileFinish { get; private set; } = new UnityEvent();
    public UnityEvent endStart { get; private set; } = new UnityEvent();

    //sprite
    [SerializeField]
    protected Sprite tileSpriteOn;
    [SerializeField]
    protected Sprite tileSpriteOff;

    [SerializeField]
    protected GameObject childSpriteObj;
    protected SpriteRenderer childObjSpriteRenderer;

    //méthode
    protected virtual void Start()
    {
        //haveLoadedParent = isLikeASource;

        if (childSpriteObj != null)
        {
            childObjSpriteRenderer = childSpriteObj.GetComponent<SpriteRenderer>();
        }

        gridManager = GetComponentInParent<GridManager>();

        strengthSignal = strengthSignal;

        posOfTile.x = (int)transform.localPosition.x;
        posOfTile.y = (int)transform.localPosition.y;

        ChangeConnectorCount();

        gameObject.name = "Redstone" + redstoneTileType + $"({posOfTile.x}, {posOfTile.y})";
        

        //CreateCircuit();
        //Debug.LogWarning("endStart.Invoke");
        endStart.Invoke();
    }

    
    public void TurnTile(bool turnRight)
    {
        if (canBeTurn)
        {
            float rota_z = childSpriteObj.transform.rotation.eulerAngles.z + (turnRight ? -90 : 90);
            childSpriteObj.transform.rotation = Quaternion.Euler(0, 0, rota_z);

            Dictionary<InputOutputConnector, InputOutputType> lastInputOutputConnector_dict = new Dictionary<InputOutputConnector, InputOutputType>(inputOutputConnector_dict);


            //East                              <=                   North                   South
            TurnTile(turnRight, InputOutputConnector.East, InputOutputConnector.North, InputOutputConnector.South, lastInputOutputConnector_dict);

            //South                             <=                   East                    West
            TurnTile(turnRight, InputOutputConnector.South, InputOutputConnector.East, InputOutputConnector.West, lastInputOutputConnector_dict);

            //West                              <=                   South                   North
            TurnTile(turnRight, InputOutputConnector.West, InputOutputConnector.South, InputOutputConnector.North, lastInputOutputConnector_dict);

            //North                             <=                   West                    East
            TurnTile(turnRight, InputOutputConnector.North, InputOutputConnector.West, InputOutputConnector.East, lastInputOutputConnector_dict);


            turnTileFinish.Invoke();
        }
    }

    private void TurnTile(bool turnRight, /*ref bool haveConnector, bool whenTurnRight, bool whenTurnLeft, */
        InputOutputConnector replaceBy, InputOutputConnector thatWhenTurnRight, InputOutputConnector thatWhenTurnLeft, Dictionary<InputOutputConnector, InputOutputType> dictionary)
    {
        //haveConnector = turnRight ? whenTurnRight : whenTurnLeft;
        if (turnRight && inputOutputConnector_dict[thatWhenTurnRight] != InputOutputType.None)
        {
            inputOutputConnector_dict[replaceBy] = dictionary[thatWhenTurnRight];
        }
        else if (inputOutputConnector_dict[thatWhenTurnLeft] != InputOutputType.None)
        {
            inputOutputConnector_dict[replaceBy] = dictionary[thatWhenTurnLeft];
        }
    }


    public virtual void ChangeConnectorCount()
    {
        connectorCount = 0;
        connectorCount += inputOutputConnector_dict[InputOutputConnector.North] != InputOutputType.None ? 1 : 0;
        connectorCount += inputOutputConnector_dict[InputOutputConnector.East] != InputOutputType.None ? 1 : 0;
        connectorCount += inputOutputConnector_dict[InputOutputConnector.South] != InputOutputType.None ? 1 : 0;
        connectorCount += inputOutputConnector_dict[InputOutputConnector.West] != InputOutputType.None ? 1 : 0;
    }


    public void ReloadHisRedstone()
    {
        parentTiles_lst = new List<RedstoneBase>();
        ReloadHisRedstone(this, parentTiles_lst);
    }


    public virtual void ReloadHisRedstone(RedstoneBase itemToNotReload, List<RedstoneBase> parentSource)
    {
        //this.parentTiles_lst = new List<RedstoneBase>(parentSource);
        this.parentTiles_lst = parentSource;
        this.parentTiles_lst.Add(itemToNotReload);
    }


    public void ReloadAroundRedstoneAfterStart()
    {
        endStart.AddListener(ReloadAroundRedstone);
    }


    /// <summary>
    /// Actualise la redstone autour de la tuile actuel
    /// </summary>
    public virtual void ReloadAroundRedstone()//non utilisé
    {
        //Debug.Log("RedstoneTileType => " + redstoneTileType);
        RedstoneBase otherTile;


        /*InputOutputConnector[] input_output_pos = GetAllInputOutput(this);
        
        for (int i = 0; i < input_output_pos.Length; i++)
        {
            if (gridManager.TileHaveInputOrIsWireAt(GetPosByDirection(posOfTile, input_output_pos[i]), GetInvertConnector(input_output_pos[i]), out otherTile))
            {
                otherTile.ReloadHisRedstone(this, parentTiles_lst);
            }
        }*/

        //North
        if (gridManager.TileExist(new Vector2Int(posOfTile.x, posOfTile.y + 1), out otherTile) && IsNotParent(otherTile))
        {
            if (otherTile.redstoneTileType == RedstoneTileType.Wire || IsInput(otherTile.inputOutputConnector_dict[InputOutputConnector.South]))
            {
                otherTile.ReloadHisRedstone(this, parentTiles_lst);
            }
        }

        //East
        if (gridManager.TileExist(new Vector2Int(posOfTile.x + 1, posOfTile.y), out otherTile) && IsNotParent(otherTile))
        {
            if (otherTile.redstoneTileType == RedstoneTileType.Wire || IsInput(otherTile.inputOutputConnector_dict[InputOutputConnector.West]))
            {
                otherTile.ReloadHisRedstone(this, parentTiles_lst);
            }
        }

        //South
        if (gridManager.TileExist(new Vector2Int(posOfTile.x, posOfTile.y - 1), out otherTile) && IsNotParent(otherTile))
        {
            if (otherTile.redstoneTileType == RedstoneTileType.Wire || IsInput(otherTile.inputOutputConnector_dict[InputOutputConnector.North]))
            {
                otherTile.ReloadHisRedstone(this, parentTiles_lst);
            }
        }

        //West
        if (gridManager.TileExist(new Vector2Int(posOfTile.x - 1, posOfTile.y), out otherTile) && IsNotParent(otherTile))
        {
            if (otherTile.redstoneTileType == RedstoneTileType.Wire || IsInput(otherTile.inputOutputConnector_dict[InputOutputConnector.East]))
            {
                otherTile.ReloadHisRedstone(this, parentTiles_lst);
            }
        }
    }

    /// <summary>
    /// Change les sprite de la tuile
    /// </summary>
    protected virtual void ChangeSprite()
    {
        if (childSpriteObj != null)
        {
            childObjSpriteRenderer = childSpriteObj.GetComponent<SpriteRenderer>();
        }

        if (childObjSpriteRenderer != null)
        {
            childObjSpriteRenderer.sprite = isActive ? tileSpriteOn : tileSpriteOff;
        }
    }

    public virtual void Configurate() { }


    /// <summary>
    /// Crée le circuit
    /// </summary>
    /// <returns>True si au moins une torche active est présente dans le ciruit</returns>
    public void CreateCircuit()
    {
        bool haveTorch;

        if (redstoneTileType == RedstoneTileType.Empty)
        {
            RedstoneBase otherTile;

            InputOutputConnector[] connectors = (InputOutputConnector[])Enum.GetValues(typeof(InputOutputConnector));
            //Debug.Log("connectors.Length = " + connectors.Length);

            List<List<RedstoneBase>> previousSameCircuit = new List<List<RedstoneBase>>();

            for (int i = 0; i < connectors.Length; i++)
            {
                if (gridManager.TileIsWireOrTorch(GetPosByDirection(posOfTile, connectors[i]), out otherTile) &&
                    !CPCS.ListOfListContains(previousSameCircuit, otherTile))
                {
                    sameCircuit_lst = new List<RedstoneBase>();
                    sameCircuit_lst.Add(this);
                    
                    otherTile.ReloadCircuit(sameCircuit_lst);
                    previousSameCircuit.Add(new List<RedstoneBase>(sameCircuit_lst));

                    haveTorch = HaveLikeSourceActiveInCircuit();

                    if (!haveTorch)
                    {
                        RemoveStrenghSignalOfAllCircuit(sameCircuit_lst);
                    }
                }
            }
        }
        else
        {  
            sameCircuit_lst = new List<RedstoneBase>();
            ReloadCircuit(sameCircuit_lst);

            haveTorch = HaveLikeSourceActiveInCircuit();
            if (!haveTorch)
            {
                RemoveStrenghSignalOfAllCircuit(sameCircuit_lst);
            }
        }
    }

    /// <summary>
    /// Recharge le cicuit avec cette élément et ceux aux alentours
    /// </summary>
    public void ReloadCircuit(List<RedstoneBase> circuit)
    {
        sameCircuit_lst = circuit;
        sameCircuit_lst.Add(this);

        RedstoneBase otherTile;

        InputOutputConnector[] connectors = redstoneTileType == RedstoneTileType.Wire ? (InputOutputConnector[])Enum.GetValues(typeof(InputOutputConnector)) : GetAllInputOutput(this);

        for(int i = 0; i < connectors.Length; i++)
        {
            //TileIsSpecialHaveInputOrIsWireAt
            //TileHaveInputOutputOrIsWireAt
            //if (gridManager.TileIsSpecialHaveInputOrIsWireAt(GetPosByDirection(posOfTile, connectors[i]), GetInvertConnector(connectors[i]), out otherTile) && 
            //on enregistre les tuiles étant dans le même circuit
            if (gridManager.TileIsWireOrTorch(GetPosByDirection(posOfTile, connectors[i]), out otherTile) && 
                !sameCircuit_lst.Contains(otherTile))
            {
                //si on a un fil (wire) ou une torche
                otherTile.ReloadCircuit(sameCircuit_lst);
            }
        }
    }

    public int GetConnectorConnected()
    {
        int result = 0;
        RedstoneBase otherRile;
        //North
        if (gridManager.TileExist(new Vector2Int(posOfTile.x, posOfTile.y + 1), out otherRile))
        {
            if (otherRile.inputOutputConnector_dict[InputOutputConnector.South] != InputOutputType.None && otherRile.inputOutputConnector_dict[InputOutputConnector.South] != InputOutputType.Special)
            {
                result++;
            }
        }

        //East
        if (gridManager.TileExist(new Vector2Int(posOfTile.x + 1, posOfTile.y), out otherRile))
        {
            if (otherRile.inputOutputConnector_dict[InputOutputConnector.West] != InputOutputType.None && otherRile.inputOutputConnector_dict[InputOutputConnector.West] != InputOutputType.Special)
            {
                result++;
            }
        }

        //South
        if (gridManager.TileExist(new Vector2Int(posOfTile.x, posOfTile.y - 1), out otherRile))
        {
            if (otherRile.inputOutputConnector_dict[InputOutputConnector.North] != InputOutputType.None && otherRile.inputOutputConnector_dict[InputOutputConnector.North] != InputOutputType.Special)
            {
                result++;
            }
        }

        //West
        if (gridManager.TileExist(new Vector2Int(posOfTile.x - 1, posOfTile.y), out otherRile))
        {
            if (otherRile.inputOutputConnector_dict[InputOutputConnector.East] != InputOutputType.None && otherRile.inputOutputConnector_dict[InputOutputConnector.East] != InputOutputType.Special)
            {
                result++;
            }
        }

        return result;
    }

    //comparaison avec la liste "parentTiles_lst"
    protected bool IsNotParent(RedstoneBase tile)
    {
        return !parentTiles_lst.Contains(tile);
    }

    protected bool CanBecomeDifferent(RedstoneBase tile)
    {
        //si la tuile est un parent
        if(!IsNotParent(tile))
        {
            //la tuile cible est un fil
            if(tile.redstoneTileType == RedstoneTileType.Wire)
            {
                //la tuile actuelle est un fil
                if(this.redstoneTileType == RedstoneTileType.Wire)
                {
                    //          10                  12              => true
                    //          10                  11              => false
                    if (tile.strengthSignal < this.strengthSignal - 1)
                    {
                        return true;
                    }
                }
                else //la tuile actuelle est une tuile en input (repeteur pour le moment)
                {
                    //la puissance de tuile cible est inférieur a celle de la tuile active
                    if(tile.strengthSignal < this.strengthSignal)
                    {
                        return true;
                    }
                }
            }

        }
        return false;
    }

    protected bool ParentHaveLoadedSource()
    {
        return isLikeASource && isActive || parentTiles_lst.Where(tile => tile.haveLoadedParent).Count() > 0;
    }


    //comparaison avec la liste "sameCircuit_lst"
    protected bool HaveLikeSourceActiveInCircuit()
    {
        //int a = GetRedstoneLikeSourceFromCircuit().Length;
        //Debug.Log(a);
        return GetRedstoneLikeSourceFromCircuit().Length > 0;   
    }

    /// <summary>
    /// Retourn un tableau contenant tous les "Redstonetorch" de sameCircuit_lst
    /// </summary>
    protected RedstoneBase[] GetRedstoneLikeSourceFromCircuit()
    {
        //return sameCircuit_lst.Where(tile => tile.isActive && tile is RedstoneTorch).Select(tile => tile as RedstoneTorch).ToArray();
        return sameCircuit_lst.Where(tile => tile.isActive && tile.isLikeASource).ToArray();
    }

    #region static

    /// <summary>
    /// Met à 0 tous les tuiles présent dans le circuit, seul les fils (wire) et les torches sont autorisé
    /// </summary>
    public static void RemoveStrenghSignalOfAllCircuit(List<RedstoneBase> circuit)
    {
        //circuit.ForEach(tile => tile.strengthSignal = MIN_STRENGTH_SIGNAL);
        for (int i = 0; i < circuit.Count; i++)
        {
            if(circuit[i].redstoneTileType == RedstoneTileType.Wire)
            {
                circuit[i].strengthSignal = MIN_STRENGTH_SIGNAL;
            }
            /*else //circuit[i].isSpecial
            {
                circuit[i].ReloadHisRedstone();
            }*/
        }
        
        //circuit.Where(tile => tile.redstoneTileType == RedstoneTileType.Wire).ToList().ForEach(tile => tile.strengthSignal = MIN_STRENGTH_SIGNAL);
        //circuit.Where(tile => tile.redstoneTileType == RedstoneTileType.Repeater).ToList().ForEach(tile => tile.ReloadHisRedstone());
    }

    //all InputOutputConnector
    public static InputOutputConnector[] GetAllInput(RedstoneBase tile)
    {
        return tile.inputOutputConnector_dict.Where(keyValue => IsInput(keyValue.Value)).Select(keyValue => keyValue.Key).ToArray();
    }

    public static InputOutputConnector[] GetAllInputOutput(RedstoneBase tile)
    {
        return tile.inputOutputConnector_dict.Where(keyValue => IsInputOrOutput(keyValue.Value)).Select(keyValue => keyValue.Key).ToArray();
    }
    

    public static InputOutputConnector[] GetAllOuput(RedstoneBase tile)
    {
        return tile.inputOutputConnector_dict.Where(keyValue => IsOutput(keyValue.Value)).Select(keyValue => keyValue.Key).ToArray();
    }

    public static InputOutputConnector[] GetAllSpecial(RedstoneBase tile)
    {
        return tile.inputOutputConnector_dict.Where(keyValue => keyValue.Value == InputOutputType.Special).Select(keyValue => keyValue.Key).ToArray();
    }

    /// <summary>
    /// Retourne la position de la tuile en fonction de la position actuel et du connecteur visé.
    /// </summary>
    public static Vector2Int GetPosByDirection(Vector2Int original, InputOutputConnector inputOutputConnector)
    {
        switch (inputOutputConnector)
        {
            case InputOutputConnector.North:
                original.y += 1;
                break;

            case InputOutputConnector.East:
                original.x += 1;
                break;

            case InputOutputConnector.South:
                original.y -= 1;
                break;

            default:// InputOutputConnector.West:
                original.x -= 1;
                break;
        }
        return original;
    }

    /// <summary>
    /// Récupère le connecteur opposé de celui donné.
    /// </summary>
    public static InputOutputConnector GetInvertConnector(InputOutputConnector inputOutputConnector)
    {
        InputOutputConnector result;

        switch (inputOutputConnector)
        {
            case InputOutputConnector.North:
                result = InputOutputConnector.South;
                break;

            case InputOutputConnector.East:
                result = InputOutputConnector.West;
                break;

            case InputOutputConnector.South:
                result = InputOutputConnector.North;
                break;

            default:// InputOutputConnector.West:
                result = InputOutputConnector.East;
                break;
        }

        return result;
    }

    /// <summary>
    /// Retourne True si <paramref name="inputOutputState"/> correspond à InputOutputType.Input ou InputOutputType.InputOuput.
    /// Sinon false.
    /// </summary>
    public static bool IsInput(InputOutputType inputOutputState)
    {
        return inputOutputState == InputOutputType.Input || inputOutputState == InputOutputType.InputOutput; //|| inputOutputState == InputOutputType.Special
    }

    /// <summary>
    /// Retourne True si <paramref name="inputOutputState"/> correspond à InputOutputType.Input ou InputOutputType.InputOuput ou InputOutputType.Output.
    /// Sinon false.
    /// </summary>
    public static bool IsInputOrOutput(InputOutputType inputOutputState)
    {
        return IsInput(inputOutputState) || IsOutput(inputOutputState); //|| inputOutputState == InputOutputType.Special
    }

    /// <summary>
    /// Retourne True si <paramref name="inputOutputState"/> correspond à InputOutputType.Output ou InputOutputType.InputOuput ou InputOutputType.Output.
    /// Sinon false.
    /// </summary>
    public static bool IsOutput(InputOutputType inputOutputState)
    {
        return inputOutputState == InputOutputType.Output || inputOutputState == InputOutputType.InputOutput;
    }
    #endregion
}


