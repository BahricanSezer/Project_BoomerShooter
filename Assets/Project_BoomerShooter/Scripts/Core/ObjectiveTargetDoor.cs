using UnityEngine;
using System.Collections.Generic;

namespace BoomerShooter.Gameplay
{
    public class ObjectiveTargetDoor : MonoBehaviour
    {
        [Header("Target Settings")]
        [SerializeField] private List<GameObject> targetObjects = new List<GameObject>();

        private void Update()
        {
            for (int i = targetObjects.Count - 1; i >= 0; i--)
            {
                if (targetObjects[i] == null)
                {
                    targetObjects.RemoveAt(i);
                }
            }

            if (targetObjects.Count == 0)
            {
                OpenDoor();
            }
        }

        private void OpenDoor()
        {
            Destroy(gameObject);
        }
    }
}