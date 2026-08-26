using System;

namespace Game.SaveSystem
{
    /// <summary>
    /// Serializable representation of persistent player state.
    /// </summary>
    [Serializable]
    public sealed class PlayerSaveData
    {
        public float PositionX;
        public float PositionY;
        public float PositionZ;

        public float RotationX;
        public float RotationY;
        public float RotationZ;
        public float RotationW;
    }
}