using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player
{
    public struct NetworkInputData : INetworkInput
    {
        public const byte MouseButton0 = 0x01;
        public const byte MouseButton1 = 0x02;

        public byte buttons;
        public Vector3 direction;
    }
}
