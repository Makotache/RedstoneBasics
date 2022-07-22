using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private GameObject _redstoneEmpty;
    [SerializeField] private GameObject _redstoneNotReplace;
    [SerializeField] private GameObject _redstoneWire;
    [SerializeField] private GameObject _redstoneRepeater;
    [SerializeField] private GameObject _redstoneTorch;
    [SerializeField] private GameObject _redstoneLamp;

    public Vector2Int sizeGrid;
    public GameObject[,] grid;
    public Vector2Int[] originalRedstoneSource;
    public Vector2Int[] notRemovableTile;
    public Vector2Int[] finalLamp;
    public GameObject _emptyRedstoneBase;

    [SerializeField] private GameObject gridSpawn;

    void Awake()
    {
        grid = new GameObject[sizeGrid.x, sizeGrid.y];
        for (int x = 0; x < sizeGrid.x; x++)
        {
            for (int y = 0; y < sizeGrid.y; y++)
            {
                //si les coordonnées corresponde a celle de l'une source original
                if(IsCoordOfOriginalSource(x, y))
                {
                    //alors on instancie la source
                    grid[x, y] = Instantiate(_redstoneTorch, gridSpawn.transform);
                    grid[x, y].GetComponent<RedstoneBase>().canBeReplace = false;
                }
                else if(IsCoordOfNonRemovableTile(x,y))
                {
                    //alors on instancie la tuile non surppimable
                    grid[x, y] = Instantiate(_redstoneNotReplace, gridSpawn.transform);
                }
                else if (IsCoordOfFinalLamp(x, y))
                {
                    //alors on instancie la lampe
                    grid[x, y] = Instantiate(_redstoneLamp, gridSpawn.transform);
                    grid[x, y].GetComponent<RedstoneBase>().canBeReplace = false;
                }
                else
                {
                    //sinon on instancie une tuile "empty"
                    grid[x, y] = Instantiate(_emptyRedstoneBase, gridSpawn.transform);
                }
                grid[x, y].transform.localPosition = new Vector2(x, y);
                
                //sert a corrigé un bug lors du reload
                grid[x, y].GetComponent<Collider2D>().enabled = false;
                grid[x, y].GetComponent<Collider2D>().enabled = true;
            }
        }
    }

    private void FixedUpdate()
    {
        for (int x = 0; x < sizeGrid.x; x++)
        {
            for (int y = 0; y < sizeGrid.y; y++)
            {
                grid[x, y].GetComponent<RedstoneBase>().ReloadHisRedstone();
            }
        }
    }

    public bool IsCoordOfOriginalSource(int x, int y)
    {
        for (int i = 0; i < originalRedstoneSource.Length; i++)
        {
            if (x == originalRedstoneSource[i].x && y == originalRedstoneSource[i].y)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsCoordOfNonRemovableTile(int x, int y)
    {
        for (int i = 0; i < notRemovableTile.Length; i++)
        {
            if (x == notRemovableTile[i].x && y == notRemovableTile[i].y)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsCoordOfFinalLamp(int x, int y)
    {
        for (int i = 0; i < finalLamp.Length; i++)
        {
            if (x == finalLamp[i].x && y == finalLamp[i].y)
            {
                return true;
            }
        }
        return false;
    }

    #region TileInputOutput

    /// <summary>
    /// Retourne True si la tuile a la position "posOfTile" possède un Input au connecteur "inputPos", renvoie la "tile" via le out.
    /// Sinon false.
    /// </summary>
    public bool TileHaveInputAt(Vector2Int posOfTile, InputOutputConnector inputPos, out RedstoneBase tile)
    {
        if(TileExist(posOfTile, out tile))
        {
            return RedstoneBase.IsInput(tile.inputOutputConnector_dict[inputPos]);
        }
        return false;
    }

    /// <summary>
    /// Retourne True si la tuile a la position "posOfTile" possède un Output au connecteur "outputPos", renvoie la "tile" via le out.
    /// Sinon false.
    /// </summary>
    public bool TileHaveOutputAt(Vector2Int posOfTile, InputOutputConnector outputPos, out RedstoneBase tile)
    {
        if (TileExist(posOfTile, out tile))
        {
            return RedstoneBase.IsOutput(tile.inputOutputConnector_dict[outputPos]);
        }
        tile = null;
        return false;
    }

    /// <summary>
    /// Retourne True si la tuile a la position "posOfTile" possède un Input ou Output au connecteur "inputOutputPos", renvoie la "tile" via le out.
    /// Sinon false.
    /// </summary>
    public bool TileHaveInputOutputAt(Vector2Int posOfTile, InputOutputConnector inputOutputPos, out RedstoneBase tile)
    {
        if (TileExist(posOfTile, out tile))
        {
            return RedstoneBase.IsInputOrOutput(tile.inputOutputConnector_dict[inputOutputPos]);
        }
        tile = null;
        return false;
    }

    /// <summary>
    /// Retourne True si la tuile a la position "posOfTile" possède un Input au connecteur "inputPos" ou si c'est un fil, renvoie la "tile" via le out.
    /// Sinon false.
    /// </summary>
    public bool TileHaveInputOrIsWireAt(Vector2Int posOfTile, InputOutputConnector inputPos, out RedstoneBase tile)
    {
        if (TileExist(posOfTile, out tile))
        {
            Debug.Log(tile.name + " = " + tile.inputOutputConnector_dict[inputPos]);
            return tile.redstoneTileType == RedstoneTileType.Wire || RedstoneBase.IsInputOrOutput(tile.inputOutputConnector_dict[inputPos]);
        }
        tile = null;
        return false;
    }

    /// <summary>
    /// Retourne True si la tuile a la position "posOfTile" possède un Input ou Output au connecteur "inputOutputPos" ou si c'est un fil, renvoie la "tile" via le out.
    /// Sinon false.
    /// </summary>
    public bool TileHaveInputOutputOrIsWireAt(Vector2Int posOfTile, InputOutputConnector inputOutputPos, out RedstoneBase tile)
    {
        if (TileExist(posOfTile, out tile))
        {
            return tile.redstoneTileType == RedstoneTileType.Wire || RedstoneBase.IsInputOrOutput(tile.inputOutputConnector_dict[inputOutputPos]);
        }
        tile = null;
        return false;
    }

    /// <summary>
    /// Retourne True si la tuile a la position "posOfTile" possède un Output au connecteur "inputOutputPos" ou si c'est un fil, renvoie la "tile" via le out.
    /// Sinon false.
    /// </summary>
    public bool TileHaveOutputOrIsWireAt(Vector2Int posOfTile, InputOutputConnector inputOutputPos, out RedstoneBase tile)
    {
        if (TileExist(posOfTile, out tile))
        {
            return tile.redstoneTileType == RedstoneTileType.Wire || RedstoneBase.IsOutput(tile.inputOutputConnector_dict[inputOutputPos]);
        }
        tile = null;
        return false;
    }

    #endregion

    #region TileIsSpecial

    /// <summary>
    /// Retourne True si la tuile a la position "posOfTile" possède un Input au connecteur "inputOutputPos" et est spécial ou si c'est un fil, renvoie la "tile" via le out.
    /// Sinon false.
    /// </summary>
    public bool TileIsSpecialHaveInputOrIsWireAt(Vector2Int posOfTile, InputOutputConnector inputOutputPos, out RedstoneBase tile)
    {
        //bool result = false;
        if (TileExist(posOfTile, out tile))
        {
            Debug.Log(tile.name + " = " + tile.inputOutputConnector_dict[inputOutputPos]);
            return tile.redstoneTileType == RedstoneTileType.Wire || tile.isSpecial && RedstoneBase.IsInput(tile.inputOutputConnector_dict[inputOutputPos]);
        }
        tile = null;
        return false;
    }


    /// <summary>
    /// Retourne True si la tuile a la position "posOfTile" EST Special, renvoie la "tile" via le out.
    /// Sinon false.
    /// 
    /// Tuile considéré comme special :
    /// répéteur
    /// comparateur
    /// </summary>
    public bool TileIsSpecialAt(Vector2Int posOfTile, out RedstoneBase tile)
    {
        if (TileExist(posOfTile, out tile))
        {
            return tile.isSpecial;
        }
        tile = null;
        return false;
    }

    /// <summary>
    /// Retourne True si la tuile a la position "posOfTile" EST Special au connecteur "outputPos" et est une sortie, renvoie la "tile" via le out.
    /// Sinon false.
    /// 
    /// Tuile considéré comme special :
    /// répéteur
    /// comparateur
    /// </summary>
    public bool TileIsSpecialAndHaveOutputAt(Vector2Int posOfTile, InputOutputConnector outputPos, out RedstoneBase tile)
    {
        if (TileIsSpecialAt(posOfTile, out tile))
        {
            return tile.inputOutputConnector_dict[outputPos] == InputOutputType.Output;
        }
        tile = null;
        return false;
    }

    #endregion

    #region TileIsWire
    public bool TileIsWireOrTorch(Vector2Int posOfTile, out RedstoneBase tile)
    {
        if (TileExist(posOfTile, out tile) && (tile.redstoneTileType == RedstoneTileType.Wire || tile.redstoneTileType == RedstoneTileType.Torch))
        {
            return true;
        }
        tile = null;
        return false;
    }

    public bool TileIsWire(Vector2Int posOfTile, out RedstoneWire tile)
    {
        RedstoneBase redstoneBase;
        if(TileExist(posOfTile, out redstoneBase) && redstoneBase.redstoneTileType == RedstoneTileType.Wire)
        {
            tile = (RedstoneWire)redstoneBase;
            return true;
        }
        tile = null;
        return false; 
    }
    #endregion

    #region TileExist

    public bool TileExist(Vector2Int posOfTile, out RedstoneBase tile)
    {
        tile = TileExist(posOfTile) ? grid[posOfTile.x, posOfTile.y].GetComponent<RedstoneBase>() : null;

        return tile != null && tile.redstoneTileType != RedstoneTileType.Empty && tile.redstoneTileType != RedstoneTileType.NotReplace;
    }

    public bool TileExist(Vector2Int posOfTile)
    {
        return TileIsInGrid(posOfTile) && grid[posOfTile.x, posOfTile.y] != null;
    }
    
    #endregion


    public bool TileIsInGrid(Vector2Int posOfTile)
    {
        return posOfTile.x >= 0 && posOfTile.x < sizeGrid.x && posOfTile.y >= 0 && posOfTile.y < sizeGrid.y;
    }


    
    public bool IsActiveTile(Vector2Int posOfTile)
    {
        RedstoneBase tile;
        return TileExist(posOfTile, out tile) && tile.isActive;
    }

    public void ReloadRedstone()
    {
        /*for (int x = 0; x < sizeGrid.x; x++)
        {
            for (int y = 0; y < sizeGrid.y; y++)
            {
                RedstoneTorch redstoneTorch;
                if (redstoneTorch = grid[x, y].GetComponent<RedstoneTorch>())
                {
                    //redstoneTorch.ReloadAroundRedstone();
                    redstoneTorch.ReloadAroundRedstoneAfterStart();
                }
            }
        }*/
    }

    public GameObject ReplaceTile(RedstoneTileType tileType, RedstoneBase toReplace)
    {
        Transform parent = toReplace.transform.parent;

        GameObject obj;
        switch (tileType)
        {
            case RedstoneTileType.Wire:
                obj = Instantiate(_redstoneWire, parent);
                break;

            case RedstoneTileType.Torch:
                obj = Instantiate(_redstoneTorch, parent);
                break;

            case RedstoneTileType.Repeater:
                obj = Instantiate(_redstoneRepeater, parent);
                break;

            case RedstoneTileType.Lamp:
                obj = Instantiate(_redstoneLamp, parent);
                break;

            default:
                obj = Instantiate(_redstoneEmpty, parent);
                break;
        }

        obj.transform.localPosition = toReplace.transform.localPosition;
        grid[toReplace.posOfTile.x, toReplace.posOfTile.y] = obj;
        obj.GetComponent<Collider2D>().enabled = false;
        obj.GetComponent<Collider2D>().enabled = true;
        //Debug.Log(toReplace.posOfTile);
        Destroy(toReplace.gameObject);

        return obj;
    }
}
