using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.Linq;
using UnityEngine;
using System;

public class RedstoneRepeater : RedstoneBase
{
    [SerializeField]
    private Sprite[] _spriteOn;

    [SerializeField]
    private Sprite[] _spriteOff;

    [SerializeField]
    private Transform[] _tickPosLocked;

    [SerializeField] 
    private GameObject _iconRepeaterLocked;

    public const int MIN_TICK_LEVEL = 1;
    public const int MAX_TICK_LEVEL = 4;

    private bool _isLock = false;
    public bool isLock
    {
        get
        {
            return _isLock;
        }

        set
        {
            _isLock = value;
            _iconRepeaterLocked.SetActive(_isLock);
        }
    }



    //[SerializeField]
    //[Range(MIN_TICK_LEVEL, MAX_TICK_LEVEL)]
    private int _tickLevel = 1;
    public int tickLevel //{ get ; protected set; } = RedstoneWireType.I;
    {
        get => _tickLevel;
        set
        {
            _tickLevel = Mathf.Clamp(value, MIN_TICK_LEVEL, MAX_TICK_LEVEL);
            ChangeSprite();
        }
    }



    private float _lastTimeForUpdate;
    private bool _changeActiveValue = false;

    void Awake()
    {
        redstoneTileType = RedstoneTileType.Repeater;

        inputOutputConnector_dict.Add(InputOutputConnector.North, InputOutputType.Output);
        inputOutputConnector_dict.Add(InputOutputConnector.East, InputOutputType.Special);
        inputOutputConnector_dict.Add(InputOutputConnector.South, InputOutputType.Input);
        inputOutputConnector_dict.Add(InputOutputConnector.West, InputOutputType.Special);

        ChangeSprite();
        changeSignal.AddListener(ChangeSprite);
        //endStart.AddListener(ReloadHisRedstone);
    }

    protected override void ChangeSprite()
    {
        tileSpriteOn = _spriteOn[tickLevel - 1];
        tileSpriteOff = _spriteOff[tickLevel - 1];
        _iconRepeaterLocked.transform.position = _tickPosLocked[tickLevel - 1].position;
        base.ChangeSprite();
    }

    public override void ReloadHisRedstone(RedstoneBase itemWhoReloadThis, List<RedstoneBase> parentSource)
    {
        //liste des précédant tuile ayant utilisé la méthode "ReloadHisRedstone"
        base.ReloadHisRedstone(itemWhoReloadThis, parentSource);

        //ReloadAroundRedstoneWire();

        //changement de la force de signal en fonction des info précédante
        ReloadStrengthSignal(itemWhoReloadThis);
    }

    private void ReloadAroundRedstoneWire()
    {
        //Debug.Log("ReloadAroundRedstoneWire");
        InputOutputConnector[] input_output_pos = GetAllInputOutput(this);

        RedstoneWire otherTile;

        for(int i = 0; i < input_output_pos.Length; i++)//input_output_pos.length = 2
        {
            if (gridManager.TileIsWire(GetPosByDirection(posOfTile, input_output_pos[i]), out otherTile))
            {
                //Debug.Log(otherTile);
                otherTile.ChangeConnectorCount();
            }
        }
    }

    public void ReloadStrengthSignal(RedstoneBase itemWhoReloadThis)
    {
        //is lock
        InputOutputConnector[] special_lock = GetAllSpecial(this);

        RedstoneBase otherTile;
        
        for(int i = 0; i < special_lock.Length; i++)//special_lock.Length = 2
        {
            if(gridManager.TileIsSpecialAndHaveOutputAt(GetPosByDirection(posOfTile, special_lock[i]), GetInvertConnector(special_lock[i]), out otherTile) && otherTile.isActive)
            {
                isLock = true;
                break;
            }
            else if(i == 1)
            {
                //on attend d'avoir tout tenté pour lock le repeater pour plus de performance
                isLock = false;
            }
        }


        //input
        if (!isLock)
        {
            InputOutputConnector input_pos = inputOutputConnector_dict.Where(keyValue => keyValue.Value == InputOutputType.Input).First().Key;
            InputOutputConnector output_pos = inputOutputConnector_dict.Where(keyValue => keyValue.Value == InputOutputType.Output).First().Key;

            if(gridManager.TileHaveOutputAt(GetPosByDirection(posOfTile, input_pos), GetInvertConnector(input_pos), out otherTile))
            {
                //on prévoit un changement d'état (actvié/désactvié)
                //SI on a pas déja prévue de changer l'état ET QUE
                //SOIT on possède bien une tuile éméttrice au niveau de l'input du répeteur ET QUE son état soit différent du repéteur actuel
                //SOIT on ne possède pas de tuile éméttrice au niveau de l'input du répeteur ET QUE le répeteur est activé
                if (!_changeActiveValue && ((otherTile != null && this.isActive != otherTile.isActive) || otherTile == null && isActive))
                {
                    _changeActiveValue = true;
                    //newActiveValue = otherTile != null ? this.isActive : false;
                    _lastTimeForUpdate = Time.time;// + Time.deltaTime;
                }
            }



            //on applique le changement d'état
            //SI on Doit changer l'état ET QUE
            //l'on a bien attendu le nombre de dixème de secondes (en fonction des ticks du répéteur) nécessaire
            if (_changeActiveValue && Time.time > _lastTimeForUpdate + ((float)tickLevel / 10))// + 0.009)
            {
                this.strengthSignal = otherTile != null && otherTile.isActive ? MAX_STRENGTH_SIGNAL : MIN_STRENGTH_SIGNAL;

                _changeActiveValue = false;
            }

            //on envoie le signal à l'output s'il y a une tuile et que ce n'et pas la même qui a lancé l'update
            /*if (gridManager.TileHaveInputAt(GetPosByDirection(posOfTile, output_pos), GetInvertConnector(output_pos), out otherTile))
            {
                //_tileToUpdate = otherTile;
                canUpdateOther = false;
                _lastTimeForUpdate = Time.time;
                //if (otherTile != itemWhoReloadThis){}
            }*/
        }
    }

    public override void Configurate()
    {
        //Debug.Log("RedstoneRepeater.Configurate()");
        tickLevel = tickLevel < 4 ? tickLevel + 1 : 1;
    }
}
