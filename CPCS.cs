using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CPCS : MonoBehaviour
{
    public static bool AddIfNotExist<T>(List<T> list, T item)
    {
        if (!list.Contains(item))
        {
            list.Add(item);
            return true;
        }
        return false;
    }

    public static bool ApproximatelyCoef(int ia, int ib, int coef)
    {
        bool add = ib <= ia + coef;
        bool remove = ia - coef <= ib;
        return add && remove;
    }

    /// <summary>
    /// Permet de savoir si le contenue des lites d'une liste <paramref name="list"/> contient <paramref name="obj"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="obj"></param>
    /// <returns>True si c'est le cas sinon false</returns>
    public static bool ListOfListContains<T>(List<List<T>> list, T obj)
    {
        return list.Where(lst => lst.Contains(obj)).Count() > 0;
    }
}