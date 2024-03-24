using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using System.IO;
using System.Reflection;
using Objforming;
using Objforming.Serialization.Json;
using Roguegard;
using Roguegard.CharacterCreation;

namespace RoguegardUnity.Tests
{
    public class EquipmentAndVehicleTest : ScriptableObject
    {
        [SerializeField] private RoguegardSettingsData _settings = null;

        [SerializeField] private ScriptableCharacterCreationData _player = null;
        [SerializeField] private ScriptableCharacterCreationData _vehicle = null;
        [SerializeField] private ScriptableCharacterCreationData _equipment = null;

        [Test]
        public void RideTest()
        {
            _settings.TestLoad();
            ObjformingLogger.Primary = new RoguegardObjformingLogger();
            RogueRandom.Primary = new RogueRandom(0);
            StaticID.Next();

            var random = RogueRandom.Primary;
            var player = _player.CreateObj(null, Vector2Int.zero, random);
            var vehicle = _vehicle.CreateObj(player, Vector2Int.zero, random);

            var vehicleInfo = VehicleInfo.Get(vehicle);
            if (!vehicleInfo.TryOpen(vehicle, player))
            {
                Debug.LogError($"{player} は {vehicle} に騎乗できませんでした。");
                return;
            }
            Debug.Log($"{player} は {vehicle} に騎乗しました。");

            if (vehicleInfo.Rider == null)
            {
                Debug.LogError($"{nameof(vehicleInfo.Rider)} == {null}");
                return;
            }
            Debug.Log($"{nameof(vehicleInfo.Rider)} != {null}");

            var rideVehicle = RideRogueEffect.GetVehicle(player);
            if (rideVehicle != vehicle)
            {
                Debug.LogError($"{nameof(RideRogueEffect.GetVehicle)} != {nameof(vehicle)}");
                return;
            }
            Debug.Log($"{nameof(RideRogueEffect.GetVehicle)} == {nameof(vehicle)}");



            var jPlayer = SerialCopy(player);

            var jVehicle = jPlayer.Space.Objs[jPlayer.Space.Objs.Count - 1];
            if (jVehicle.GetName() != vehicle.GetName())
            {
                Debug.LogError($"{jVehicle} != {vehicle}");
                return;
            }
            Debug.Log($"{jVehicle} == {vehicle}");

            var jVehicleInfo = VehicleInfo.Get(jVehicle);
            if (jVehicleInfo.Rider == null)
            {
                Debug.LogError($"シリアル化で騎乗が解除されました。");
                return;
            }
            Debug.Log($"シリアル化で騎乗が解除されませんでした。");

            var jRideVehicle = RideRogueEffect.GetVehicle(jPlayer);
            if (jRideVehicle != jVehicle)
            {
                Debug.LogError($"逆シリアル化: {nameof(RideRogueEffect.GetVehicle)} != {nameof(jVehicle)}");
                return;
            }
            Debug.Log($"逆シリアル化: {nameof(RideRogueEffect.GetVehicle)} == {nameof(jVehicle)}");
        }

        [Test]
        public void EquipTest()
        {
            _settings.TestLoad();
            ObjformingLogger.Primary = new RoguegardObjformingLogger();
            RogueRandom.Primary = new RogueRandom(0);
            StaticID.Next();

            var random = RogueRandom.Primary;
            var player = _player.CreateObj(null, Vector2Int.zero, random);
            var equipment = _equipment.CreateObj(player, Vector2Int.zero, random);

            var equipmentInfo = equipment.Main.GetEquipmentInfo(equipment);
            if (!equipmentInfo.TryOpen(equipment, 0))
            {
                Debug.LogError($"{player} は {equipment} を装備できませんでした。");
                return;
            }
            Debug.Log($"{player} は {equipment} を装備しました。");

            if (equipmentInfo.EquipIndex == -1)
            {
                Debug.LogError($"{nameof(equipmentInfo.EquipIndex)} == {-1}");
                return;
            }
            Debug.Log($"{nameof(equipmentInfo.EquipIndex)} != {-1}");

            var equipmentState = player.Main.GetEquipmentState(player);
            var equipEquipment = equipmentState.GetEquipment(EquipKw.Bottoms, 0);
            if (equipEquipment != equipment)
            {
                Debug.LogError($"{nameof(equipmentState.GetEquipment)} != {nameof(equipment)}");
                return;
            }
            Debug.Log($"{nameof(equipmentState.GetEquipment)} == {nameof(equipment)}");



            var jPlayer = SerialCopy(player);

            var jEquipment = jPlayer.Space.Objs[jPlayer.Space.Objs.Count - 1];
            if (jEquipment.GetName() != equipment.GetName())
            {
                Debug.LogError($"{jEquipment} != {equipment}");
                return;
            }
            Debug.Log($"{jEquipment} == {equipment}");

            var jEquipmentInfo = jEquipment.Main.GetEquipmentInfo(jEquipment);
            if (jEquipmentInfo.EquipIndex == -1)
            {
                Debug.LogError($"シリアル化で装備が解除されました。");
                return;
            }
            Debug.Log($"シリアル化で装備が解除されませんでした。");

            var jEquipEquipment = jPlayer.Main.GetEquipmentState(jPlayer).GetEquipment(EquipKw.Bottoms, 0);
            if (jEquipEquipment != jEquipment)
            {
                Debug.LogError($"逆シリアル化: {nameof(RideRogueEffect.GetVehicle)} != {nameof(jEquipment)}");
                return;
            }
            Debug.Log($"逆シリアル化: {nameof(RideRogueEffect.GetVehicle)} == {nameof(jEquipment)}");
        }

        private RogueObj SerialCopy(RogueObj player)
        {
            var config = StandardRogueDeviceSave.GetJsonSerializationConfig();
            using var memoryStream = new MemoryStream();
            config.Serialize(memoryStream, player);
            memoryStream.Position = 0;
            return config.Deserialize<RogueObj>(memoryStream);
        }
    }
}
