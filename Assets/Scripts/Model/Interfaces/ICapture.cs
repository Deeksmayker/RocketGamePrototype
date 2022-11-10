using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Model.Interfaces
{
    public interface ICapture
    {
        public IEnumerator Capture(GameObject target);
    }
}
