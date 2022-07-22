using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedstoneLamp : RedstoneBase
{
    private void Awake()
    {
        redstoneTileType = RedstoneTileType.Lamp;

        inputOutputConnector_dict.Add(InputOutputConnector.North, InputOutputType.Input);
        inputOutputConnector_dict.Add(InputOutputConnector.East, InputOutputType.Input);
        inputOutputConnector_dict.Add(InputOutputConnector.South, InputOutputType.Input);
        inputOutputConnector_dict.Add(InputOutputConnector.West, InputOutputType.Input);
    }

    

    public override void ReloadHisRedstone(RedstoneBase itemWhoReloadThis, List<RedstoneBase> redstoneBases)
    {
        InputOutputConnector[] inputs = GetAllInput(this);

        RedstoneBase otherTile;

        int maxStrengthSignal = 0;
        for (int i = 0; i < inputs.Length; i++)//special_lock.Length = 2
        {
            if (gridManager.TileHaveOutputAt(GetPosByDirection(posOfTile, inputs[i]), GetInvertConnector(inputs[i]), out otherTile) && otherTile.strengthSignal > maxStrengthSignal)
            {
                maxStrengthSignal = otherTile.strengthSignal;
                break;
            }
        }
        this.strengthSignal = maxStrengthSignal;
    }
}
