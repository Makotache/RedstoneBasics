using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedstoneNotReplace : RedstoneBase
{
    private new void Start()
    {
        childObjSpriteRenderer = childSpriteObj.GetComponent<SpriteRenderer>();
        tileSpriteOff = childObjSpriteRenderer.sprite;
        tileSpriteOn = childObjSpriteRenderer.sprite;

        redstoneTileType = RedstoneTileType.NotReplace;
        gridManager = GetComponentInParent<GridManager>();

        inputOutputConnector_dict.Add(InputOutputConnector.North, InputOutputType.Input);
        inputOutputConnector_dict.Add(InputOutputConnector.East, InputOutputType.Input);
        inputOutputConnector_dict.Add(InputOutputConnector.South, InputOutputType.Input);
        inputOutputConnector_dict.Add(InputOutputConnector.West, InputOutputType.Input);


        posOfTile.x = (int)transform.position.x;
        posOfTile.y = (int)transform.position.y;

        canBeTurn = false;
    }

    public void ReloadConnectorInput()
    {

    }

    /*public override void ReloadHisRedstone(RedstoneBase itemWhoReloadThis)
    {

    }*/
}
