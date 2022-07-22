using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RedstoneTileType
{
    Empty,
    Wire,
    Repeater,
    Torch,
    Lamp,
    Block,
    NotReplace
}

public enum InputOutputConnector
{
    North,
    East,
    South,
    West
}

public enum InputOutputType
{
    Input,
    Output,
    InputOutput,
    Special,
    None
}
