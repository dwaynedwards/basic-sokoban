using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Sokoban
{
    [ShowOdinSerializedPropertiesInInspector]
    public class InputButton : MonoBehaviour
    {
        #region Field and Properties

        #region Public

        public event Action OnButtonPressed = delegate { };

        #endregion

        #region Private

        [SerializeField]
        private Key[] _keys;

        #endregion

        #endregion

        #region Unity Methods

        private void OnDestroy()
        {
            GetComponent<Button>().onClick.RemoveAllListeners();
        }

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(() => OnButtonPressed());
        }

        private void Update()
        {
            foreach (var key in _keys)
            {
                if (!Keyboard.current[key].wasPressedThisFrame)
                {
                    return;
                }

                OnButtonPressed();
            }
        }

        #endregion
    }
}
