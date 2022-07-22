using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IReloadRedstone
{
    public void ReloadHisRedstone(RedstoneBase itemToNotReload, List<RedstoneBase> parentSource);

    public void ReloadStrengthSignal();
}