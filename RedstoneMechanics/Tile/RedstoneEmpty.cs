using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedstoneEmpty : RedstoneBase
{
    private void Awake()
    {
        redstoneTileType = RedstoneTileType.Empty;
        childObjSpriteRenderer = childSpriteObj.GetComponent<SpriteRenderer>();
        tileSpriteOff = childObjSpriteRenderer.sprite;
        tileSpriteOn = childObjSpriteRenderer.sprite;


        inputOutputConnector_dict.Add(InputOutputConnector.North, InputOutputType.None);
        inputOutputConnector_dict.Add(InputOutputConnector.East, InputOutputType.None);
        inputOutputConnector_dict.Add(InputOutputConnector.South, InputOutputType.None);
        inputOutputConnector_dict.Add(InputOutputConnector.West, InputOutputType.None);

        //gridManager = GetComponentInParent<GridManager>();

        /*posOfTile.x = (int)transform.position.x;
        posOfTile.y = (int)transform.position.y;

        gameObject.name = "Redstone" + redstoneTileType + $"({posOfTile.x}, {posOfTile.y})";

        endStart.Invoke();*/
        //Debug.Log("empty créé");
    }

    


    /*public override void ReloadHisRedstone(RedstoneBase itemWhoReloadThis)
    {

    }*/
}
