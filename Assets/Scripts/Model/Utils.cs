using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Model
{
    public static class Utils
    {

        public static bool CheckRandom(float chance)
        {
            return Random.Range(0, 1) <= chance;
        }

        public static Vector2 GetRandomVector2()
        {
            return new Vector2(Random.Range(-1, 1), Random.Range(-1, 1));
        }
    }
}
