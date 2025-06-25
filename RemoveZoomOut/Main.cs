using BepInEx;
using BepInEx.Configuration;
using RoR2;
using RoR2.ContentManagement;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RemoveZoomOut
{
  [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
  public class Main : BaseUnityPlugin
  {
    public const string PluginGUID = PluginAuthor + "." + PluginName;
    public const string PluginAuthor = "Nuxlar";
    public const string PluginName = "RemoveZoomOut";
    public const string PluginVersion = "1.0.0";

    internal static Main Instance { get; private set; }
    public static string PluginDirectory { get; private set; }

    public static ConfigEntry<bool> changeSeekerUtil;
    public static ConfigEntry<bool> changeSeekerSec;
    public static ConfigEntry<bool> changeFSUtil;
    public static ConfigEntry<bool> changeEgg;
    private static ConfigFile RZOConfig { get; set; }

    public void Awake()
    {
      Instance = this;

      Log.Init(Logger);

      RZOConfig = new ConfigFile(Paths.ConfigPath + "\\com.Nuxlar.RemoveZoomOut.cfg", true);
      changeEgg = RZOConfig.Bind<bool>("General", "Change Egg Zoom", true, "Reduces camera zoom out on the Volcanic Egg equipment.");
      changeFSUtil = RZOConfig.Bind<bool>("General", "Change False Son Zoom", true, "Removes camera zoom out when aiming False Son's alt utility.");
      changeSeekerSec = RZOConfig.Bind<bool>("General", "Change Seeker Sec Zoom", true, "Removes camera zoom out when aiming Seeker's secondary Unseen Hand.");
      changeSeekerUtil = RZOConfig.Bind<bool>("General", "Change Seeker Util Zoom", true, "Removes camera zoom out when using Seeker's utility (both).");

      LoadAssets();

      if (changeSeekerSec.Value)
        TweakEntityState(RoR2BepInExPack.GameAssetPaths.RoR2_DLC2_Seeker_EntityStates_Seeker.UnseenHand_asset, "abilityAimType", "3");
      if (changeFSUtil.Value)
        TweakEntityState(RoR2BepInExPack.GameAssetPaths.RoR2_DLC2_FalseSon_EntityStates_FalseSon.AimMeridiansWill_asset, "cameraTeleportPositionOffset", "0 0 -3");
    }

    private void TweakEntityState(string path, string fieldName, string value)
    {
      AssetReferenceT<EntityStateConfiguration> escRef = new AssetReferenceT<EntityStateConfiguration>(path);
      AssetAsyncReferenceManager<EntityStateConfiguration>.LoadAsset(escRef).Completed += (x) =>
      {
        EntityStateConfiguration esc = x.Result;
        for (int i = 0; i < esc.serializedFieldsCollection.serializedFields.Length; i++)
        {
          if (esc.serializedFieldsCollection.serializedFields[i].fieldName == fieldName)
          {
            esc.serializedFieldsCollection.serializedFields[i].fieldValue.stringValue = value;
          }
        }
      };
    }

    private static void LoadAssets()
    {
      if (changeEgg.Value)
      {
        AssetReferenceT<GameObject> eggRef = new AssetReferenceT<GameObject>(RoR2BepInExPack.GameAssetPaths.RoR2_Base_FireBallDash.FireballVehicle_prefab);
        AssetAsyncReferenceManager<GameObject>.LoadAsset(eggRef).Completed += (x) =>
        {
          GameObject vehicle = x.Result;
          CameraTargetParams camParams = vehicle.GetComponent<CameraTargetParams>();
          camParams.cameraParams.data.idealLocalCameraPos = new Vector3(0f, 2f, -15f);
        };
      }

      if (changeSeekerUtil.Value)
      {
        AssetReferenceT<GameObject> soujournRef = new AssetReferenceT<GameObject>(RoR2BepInExPack.GameAssetPaths.RoR2_DLC2_Seeker_SojournVehicle.SojournVehicle_prefab);
        AssetAsyncReferenceManager<GameObject>.LoadAsset(soujournRef).Completed += (x) =>
        {
          GameObject vehicle = x.Result;
          CameraTargetParams camParams = vehicle.GetComponent<CameraTargetParams>();
          camParams.cameraParams.data.idealLocalCameraPos = new Vector3(0f, 0f, -15f); // 0 -2.12 -20.78
        };
      }
    }
  }
}