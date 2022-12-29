using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Model
{
    public static class Utils
    {

        public static bool CheckChance(float chance)
        {
            return Random.Range(0f, 1f) <= chance;
        }

        public static Vector2 GetRandomVector2(float variation = 1)
        {
            return new Vector2(Random.Range(-variation, variation), Random.Range(-variation, variation));
        }

        public static Vector2 GetVectorRotated90(Vector2 vector, int sign)
        {
            return new Vector2(vector.y, -vector.x) * sign;
        }

        public static bool CompareNumsApproximately(float first, float second, float allowedDifference)
        {
            var d = first - second;

            return Mathf.Abs(d) < allowedDifference;
        }

        public static bool CompareVectors(Vector2 me, Vector2 other, float allowedDifference = 0.01f)
        {
            var dx = me.x - other.x;
            if (Mathf.Abs(dx) > allowedDifference)
                return false;

            var dy = me.y - other.y;
            if (Mathf.Abs(dy) > allowedDifference)
                return false;

            return true;
        }


        public static IEnumerator StopTime(float time)
        {
            Time.timeScale = 0;
            yield return new WaitForSecondsRealtime(time);
            Time.timeScale = 1;
        }
    }
}
