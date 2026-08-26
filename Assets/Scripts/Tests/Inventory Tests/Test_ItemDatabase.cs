#if UNITY_EDITOR

using Game.Inventory;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;

public class Test_ItemDatabase : MonoBehaviour
{
    readonly List<Object> createdObjects = new();

    ItemDefinition CreateDefinition(string id)
    {
        ItemDefinition definition = ScriptableObject.CreateInstance<ItemDefinition>();

        definition.SetIdForTesting(id);
        createdObjects.Add(definition);

        return definition;
    }

    ItemDatabase CreateDatabase(params ItemDefinition[] definitions)
    {
        ItemDatabase database = ScriptableObject.CreateInstance<ItemDatabase>();

        database.SetDefinitionsForTesting(definitions);
        createdObjects.Add(database);

        return database;
    }

    [TearDown]
    public void TearDown()
    {
        for (int i = 0; i < createdObjects.Count; i++)
        {
            if (createdObjects[i] != null)
            {
                Object.DestroyImmediate(createdObjects[i]);
            }
        }

        createdObjects.Clear();
    }

    /// <summary>
    /// Verifies that a valid ItemDefinition is added to the database and can be retrieved using its unique ID.
    /// </summary>
    [Test]
    public void Initialize_RegistersValidDefinition()
    {
        ItemDefinition definition = CreateDefinition("sword_iron");

        ItemDatabase database = CreateDatabase(definition);

        database.Initialize();

        bool found = database.TryGet("sword_iron", out ItemDefinition result);

        Assert.IsTrue(found);
        Assert.AreSame(definition, result);
    }

    /// <summary>
    /// Verifies that multiple valid ItemDefinitions are independently
    /// registered and can each be retrieved using their IDs.
    /// </summary>
    [Test]
    public void Initialize_RegistersMultipleDefinitions()
    {
        ItemDefinition sword = CreateDefinition("sword_iron");

        ItemDefinition potion = CreateDefinition("potion_health");

        ItemDefinition armor = CreateDefinition("armor_iron");

        ItemDatabase database = CreateDatabase(sword, potion, armor);

        database.Initialize();

        Assert.IsTrue(database.TryGet("sword_iron", out ItemDefinition swordResult));

        Assert.IsTrue(database.TryGet("potion_health", out ItemDefinition potionResult));

        Assert.IsTrue(database.TryGet("armor_iron", out ItemDefinition armorResult));

        Assert.AreSame(sword, swordResult);
        Assert.AreSame(potion, potionResult);
        Assert.AreSame(armor, armorResult);
    }

    /// <summary>
    /// Verifies that requesting an ID that does not exist in the database returns false and does not return an ItemDefinition.
    /// </summary>
    [Test]
    public void TryGet_ReturnsFalseForUnknownId()
    {
        ItemDefinition definition = CreateDefinition("sword_iron");

        ItemDatabase database = CreateDatabase(definition);

        database.Initialize();

        bool found = database.TryGet("does_not_exist", out ItemDefinition result);

        Assert.IsFalse(found);
        Assert.IsNull(result);
    }

    /// <summary>
    /// Verifies that null entries in the definitions list are ignored and do not prevent valid definitions from being registered.
    /// </summary>
    [Test]
    public void Initialize_IgnoresNullDefinitions()
    {
        ItemDefinition sword = CreateDefinition("sword_iron");

        ItemDatabase database = CreateDatabase(null, sword, null);

        database.Initialize();

        Assert.IsTrue(database.TryGet("sword_iron", out ItemDefinition result));

        Assert.AreSame(sword, result);
    }

    /// <summary>
    /// Verifies that an ItemDefinition with an empty ID is ignored and cannot be retrieved from the database.
    /// </summary>
    [Test]
    public void Initialize_IgnoresDefinitionWithEmptyId()
    {
        ItemDefinition invalid = CreateDefinition("");

        ItemDatabase database = CreateDatabase(invalid);

        LogAssert.Expect(LogType.Error, $"Item '{invalid.name}' has no ID.");

        database.Initialize();

        Assert.IsFalse(database.TryGet("", out _));
    }

    /// <summary>
    /// Verifies that an ItemDefinition containing only whitespace in its ID is ignored by the database.
    /// </summary>
    [Test]
    public void Initialize_IgnoresDefinitionWithWhitespaceId()
    {
        ItemDefinition invalid = CreateDefinition("   ");

        ItemDatabase database = CreateDatabase(invalid);

        LogAssert.Expect(LogType.Error, $"Item '{invalid.name}' has no ID.");

        database.Initialize();

        Assert.IsFalse(database.TryGet("   ", out _));
    }

    /// <summary>
    /// Verifies that duplicate item IDs are detected and that the
    /// first definition remains registered instead of being replaced by the duplicate definition.
    /// </summary>
    [Test]
    public void Initialize_DuplicateIds_KeepsFirstDefinition()
    {
        ItemDefinition first = CreateDefinition("sword_iron");

        ItemDefinition duplicate = CreateDefinition("sword_iron");

        ItemDatabase database = CreateDatabase(first, duplicate);

        LogAssert.Expect(LogType.Error, "Duplicate item ID 'sword_iron'."
        );

        database.Initialize();

        Assert.IsTrue(database.TryGet("sword_iron", out ItemDefinition result));

        Assert.AreSame(first, result);
        Assert.AreNotSame(duplicate, result);
    }

    /// <summary>
    /// Verifies that item IDs use ordinal case-sensitive comparison, meaning IDs that differ only by letter casing are treated as different identifiers.
    /// </summary>
    [Test]
    public void TryGet_IsCaseSensitive()
    {
        ItemDefinition lowerCase = CreateDefinition("sword_iron");

        ItemDefinition upperCase = CreateDefinition("SWORD_IRON");

        ItemDatabase database = CreateDatabase(lowerCase, upperCase);

        database.Initialize();

        Assert.IsTrue(database.TryGet("sword_iron", out ItemDefinition lowerResult));

        Assert.IsTrue(database.TryGet("SWORD_IRON", out ItemDefinition upperResult));

        Assert.AreSame(lowerCase, lowerResult);
        Assert.AreSame(upperCase, upperResult);
    }

    /// <summary>
    /// Verifies that TryGet automatically initializes the database when the runtime lookup table has not yet been created.
    /// </summary>
    [Test]
    public void TryGet_InitializesDatabaseWhenNeeded()
    {
        ItemDefinition sword = CreateDefinition("sword_iron");

        ItemDatabase database = CreateDatabase(sword);

        // Intentionally do not call Initialize().

        bool found = database.TryGet("sword_iron", out ItemDefinition result);

        Assert.IsTrue(found);
        Assert.AreSame(sword, result);
    }

    /// <summary>
    /// Verifies that an empty ItemDatabase initializes successfully and returns false when an item is requested.
    /// </summary>
    [Test]
    public void Initialize_EmptyDatabase_ReturnsFalseForLookup()
    {
        ItemDatabase database = CreateDatabase();

        database.Initialize();

        Assert.IsFalse(database.TryGet("anything", out _));
    }

    /// <summary>
    /// Verifies that the database can be initialized multiple times without losing or duplicating valid definitions.
    /// </summary>
    [Test]
    public void Initialize_CanBeCalledMultipleTimes()
    {
        ItemDefinition sword = CreateDefinition("sword_iron");

        ItemDatabase database = CreateDatabase(sword);

        database.Initialize();
        database.Initialize();

        Assert.IsTrue(database.TryGet("sword_iron", out ItemDefinition result));

        Assert.AreSame(sword, result);
    }
}
#endif