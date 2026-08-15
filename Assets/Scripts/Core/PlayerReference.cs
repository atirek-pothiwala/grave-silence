using UnityEngine;

namespace GraveSilence.Core
{
    /// <summary>
    /// Cached reference to the player, avoiding repeated FindGameObjectWithTag calls.
    /// </summary>
    public class PlayerReference : MonoBehaviour
    {
        public static Transform Transform { get; private set; }
        public static GameObject GameObject => Transform != null ? Transform.gameObject : null;

        private void Awake()
        {
            if (!CompareTag(GameConstants.PlayerTag))
            {
                Debug.LogWarning("PlayerReference should be on the Player object.");
                return;
            }

            Transform = transform;
        }

        private void OnDestroy()
        {
            if (Transform == transform)
                Transform = null;
        }
    }
}
