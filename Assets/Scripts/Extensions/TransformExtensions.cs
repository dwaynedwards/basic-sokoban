using System;
using UnityEngine;

namespace Sokoban.Extensions
{
    public static class TransformExtensions
    {
        public static void ForEveryChild(this Transform parent, Action<Transform> action)
        {
            for (var i = parent.childCount - 1; i >= 0; i--)
            {
                action(parent.GetChild(i));
            }
        }
    }
}
