using Game.SaveSystem;
using System;
using UnityEngine;

namespace Game.Player
{
    /// <summary>
    /// Persists player state as part of the global save system.
    /// </summary>
    public sealed class PlayerSaveParticipant : ISaveParticipant
    {
        readonly Transform playerTransform;

        /// <summary>
        /// Creates a player save participant.
        /// </summary>
        public PlayerSaveParticipant(Transform playerTransform)
        {
            this.playerTransform = playerTransform;
        }

        /// <summary>
        /// Captures the player's position and rotation.
        /// </summary>
        public void Capture(SaveData saveData)
        {
            Vector3 position = playerTransform.position;
            Quaternion rotation = playerTransform.rotation;

            saveData.Player.PositionX = position.x;
            saveData.Player.PositionY = position.y;
            saveData.Player.PositionZ = position.z;

            saveData.Player.RotationX = rotation.x;
            saveData.Player.RotationY = rotation.y;
            saveData.Player.RotationZ = rotation.z;
            saveData.Player.RotationW = rotation.w;
        }

        /// <summary>
        /// Restores the player's position and rotation.
        /// </summary>
        public bool Restore(SaveData saveData)
        {
            if (saveData == null || saveData.Player == null)
            {
                throw new ArgumentNullException(nameof(saveData));
            }

            PlayerSaveData data = saveData.Player;


            playerTransform.SetPositionAndRotation(
                new Vector3(data.PositionX, data.PositionY, data.PositionZ),
                new Quaternion(data.RotationX, data.RotationY, data.RotationZ, data.RotationW));

            return true;
        }
    }
}