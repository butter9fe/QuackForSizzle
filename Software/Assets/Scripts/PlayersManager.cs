using UnityEngine;
using CookOrBeCooked.Systems.EventSystem;
using System.Collections.Generic;
using System.Linq;

namespace QuackForSizzle.Player
{
    public class PlayersManager : CookOrBeCooked.Utility.Singleton<PlayersManager>
    {
        private Dictionary<PlayerNumber, Controller> _controllers = new();
        public Dictionary<PlayerNumber, Controller> Controllers => _controllers;

        private void OnEnable()
        {
            _controllers = FindObjectsByType<Controller>(FindObjectsSortMode.None).ToDictionary(x => x.PlayerNumber);
        }
    }
}
