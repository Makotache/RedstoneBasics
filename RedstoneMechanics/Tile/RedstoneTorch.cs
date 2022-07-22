using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedstoneTorch : RedstoneBase
{

    private void Awake()
    {
        strengthSignal = strengthSignal;
        redstoneTileType = RedstoneTileType.Torch;

        inputOutputConnector_dict.Add(InputOutputConnector.North, InputOutputType.Output);
        inputOutputConnector_dict.Add(InputOutputConnector.East, InputOutputType.Output);
        inputOutputConnector_dict.Add(InputOutputConnector.South, InputOutputType.Output);
        inputOutputConnector_dict.Add(InputOutputConnector.West, InputOutputType.Output);

        //endStart.AddListener(ReloadHisRedstone);
    }

    /*public void ReloadHisRedstone(RedstoneBase itemToNotReload, List<RedstoneBase> parentSource)
    {
        
    }*/
    public void ReloadStrengthSignal()
    {
        //throw new System.NotImplementedException();
    }
}
