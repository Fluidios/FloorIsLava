using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player
{
    public class PlayerEntity : NetworkBehaviour
    {
        [Networked]
        public string Name { get; set; }
        public ObservableValue<bool> ReadyToPlay = new ObservableValue<bool>(false);

        private void Start()
        {
            ReadyToPlay.Subscribe((b) => Debug.Log(string.Format("{0} ready: {1}", Name, b)));
        }
    }
}
