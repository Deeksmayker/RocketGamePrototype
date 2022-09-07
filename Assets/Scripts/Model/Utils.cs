using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Model
{
    public static class Utils
    {
        public static bool CheckRandom(float chance)
        {
            Random random = new Random();
            return random.NextDouble() <= chance;
        }
    }
}
