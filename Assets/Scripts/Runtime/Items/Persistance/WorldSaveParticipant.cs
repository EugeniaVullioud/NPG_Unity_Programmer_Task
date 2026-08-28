using Game.Items;
using Game.SaveSystem;
using System;
using System.Collections.Generic;

public sealed class WorldSaveParticipant : ISaveParticipant
{
    const int CurrentVersion = 1;

    readonly IReadOnlyList<WorldItemPickup> pickups;

    public string Id => "world";

    public WorldSaveParticipant(
        IReadOnlyList<WorldItemPickup> pickups)
    {
        this.pickups = pickups ?? throw new ArgumentNullException(nameof(pickups));
    }

    public void Capture(SaveData saveData)
    {
        if (saveData == null) throw new ArgumentNullException(nameof(saveData));

        WorldSaveData data = new WorldSaveData
        {
            Version = CurrentVersion
        };

        for (int i = 0; i < pickups.Count; i++)
        {
            WorldItemPickup pickup = pickups[i];

            if (pickup == null) continue;

            if (string.IsNullOrWhiteSpace(pickup.PickupId)) continue;

            if (pickup.State != WorldItemPickup.PickupState.Consumed) continue;

            data.Pickups.Add(new WorldPickupSaveData
            {
                PickupId = pickup.PickupId,
                Consumed = true
            });
        }

        saveData.World = data;
    }

    public bool Restore(SaveData saveData)
    {
        if (saveData == null) throw new ArgumentNullException(nameof(saveData));

        if (saveData.World == null) return true;

        if (saveData.World.Version > CurrentVersion) return false;

        for (int i = 0; i < saveData.World.Pickups.Count; i++)
        {
            WorldPickupSaveData savedPickup = saveData.World.Pickups[i];

            if (savedPickup == null)
                continue;

            if (!savedPickup.Consumed) continue;

            WorldItemPickup pickup = FindPickup(savedPickup.PickupId);

            if (pickup == null) continue;

            pickup.RestoreConsumed();
        }

        return true;
    }

    WorldItemPickup FindPickup(string pickupId)
    {
        for (int i = 0; i < pickups.Count; i++)
        {
            WorldItemPickup pickup = pickups[i];

            if (pickup == null) continue;

            if (pickup.PickupId == pickupId) return pickup;
        }

        return null;
    }
}
