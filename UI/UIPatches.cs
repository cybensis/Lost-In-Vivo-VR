using HarmonyLib;
using UnityEngine;
using UnityEngine.PostProcessing;
using UnityEngine.UI;

namespace VivoVR.UI
{
    [HarmonyPatch]
    internal class UIPatches
    {
        public static Canvas hud;


        // When loading a new area there is usually a black screen, I'm guessing to make sure the game is loaded in
        // properly before the user can see anything, but ima disable it :)
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Sys_scr), "Start")]
        private static bool DisableBlackScreen(Sys_scr __instance)
        {
            __instance.real_stage = 9;
            __instance.t2 = 2f;
            return false;
        }


        // Ironsights keeps trying to set FOV which is throwing errors and is annoying, so block it for now
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Ironsights), "Update")]
        private static bool BlockIronSights(Ironsights __instance)
        {
            return false;
        }


        // i forgor what this does but it does something useful
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ReconfigurePrefab), "Start")]
        private static void FixSomething(ReconfigurePrefab __instance)
        {
            __instance.transform.localScale = Vector3.zero;

        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(SmoothMouseLook), "Start")]
        private static void ConfigureMouseMovement(SmoothMouseLook __instance)
        {
            // Block mouse movement on the Y axis
            __instance.maximumY = 0;
            __instance.minimumY = 0;
            // The normal camera rect settings makes the world bend when looking around in VR, this fixes that
            Camera.main.rect = new Rect(0, 0, 1, 1);
        }


        // This uses the SmoothMouseLook update function to track the HUD to the headset
        [HarmonyPostfix]
        [HarmonyPatch(typeof(SmoothMouseLook), "Update")]
        private static void TrackHUDToHeadset(SmoothMouseLook __instance)
        {
            // SmoothMouseLook is disabled when inv is open so use isActiveAndEnabled to prevent the inventory UI tracking with headset movement
            if (hud != null && hud.name == "HUD" && __instance.isActiveAndEnabled)
            {
                hud.renderMode = RenderMode.WorldSpace;
                Vector3 menuPosition = Camera.main.transform.position;
                // Move the camera forward and to the left a little to position it "perfectly"
                menuPosition = Camera.main.transform.position + Camera.main.transform.forward * 0.4f + Camera.main.transform.right * -0.03f;
                menuPosition.y = Camera.main.transform.position.y;
                // Set the HUDS rotation and position to the modified cameras values
                hud.transform.rotation = Camera.main.transform.rotation;
                hud.transform.position = menuPosition;
                // I found this scale for the HUD, coupled with the menuPosition above works nicely
                hud.transform.localScale = new Vector3(0.0005f, 0.0005f, 0.0005f); ;

            }

        }


        // When a Canvas is made, change it to WorldSpace otherwise it wont show on the headset
        [HarmonyPostfix]
        [HarmonyPatch(typeof(CanvasScaler), "OnEnable")]
        private static void MoveCanvasesToWorldSpace(CanvasScaler __instance)
        {
            var canvas = __instance.GetComponent<Canvas>();
            if (canvas.GetComponent<Menu_Run>())
            {
                canvas.renderMode = RenderMode.WorldSpace;
                canvas.transform.localScale = Vector3.one * 0.11f;

            }
            else if (canvas != null && canvas.name == "HUD")
            {
                hud = canvas;
                // I don't think I need this localScale here since TrackHUDToHeadset() manages scale, but whatevs
                hud.transform.localScale = new Vector3(0.002f, 0.002f, 0.001f);
                RawImage[] rawImageObjs = hud.GetComponentsInChildren<RawImage>();
                // The HUD has a slight black background to it, so get rid of that by disabling the coloured element
                for (int i = 0; i < rawImageObjs.Length; i++)
                {
                    if (rawImageObjs[i].name == "overlay")
                    {
                        rawImageObjs[i].enabled = false;
                    }
                }
                // The normal camera rect settings makes the world bend when looking around in VR, this fixes that
                Camera.main.rect = new Rect(0, 0, 1, 1);
                // I don't remember what this is for but the game works and I don't wanna break anything so it can stay for now
                Camera.main.transform.localPosition = new Vector3(-0.1f, 0, 0);
                // Post processing has screen space reflections which render reflections differently in each lens,
                // so I need to turn off SSR but can't figure out how so I'll just turn everything off for now :)
                Camera.main.GetComponent<PostProcessingBehaviour>().enabled = false;

            }
        }


        // Sets the inventory HUD and inventory items to a good position
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Inv_main_scr), "Open_INV")]
        private static void SetInvPosition(Inv_main_scr __instance)
        {
            if (hud != null && hud.name == "HUD")
            {
                hud.renderMode = RenderMode.WorldSpace;
                // I know these positions are really specific but they work so I'll leave it for now
                hud.transform.localScale = new Vector3(0.002f, 0.002f, 0.001f);
                hud.transform.position = new Vector3(0.6917f, 201.4259f, -0.4414f);
                hud.transform.rotation = new Quaternion(0, 0, 0, 0);
                __instance.transform.position = new Vector3(0.4917f, 201.2495f, 0.7586f);

            }
        }


        // Change the position part later, using update for this is a waste, it should only need to be done once
        // when the weapon is pulled out but rotation is fine, keep that here
        [HarmonyPostfix]
        [HarmonyPatch(typeof(WeaponBehavior), "Update")]
        private static void MoveWeaponWithHeadset(WeaponBehavior __instance)
        {
            // Maps the rotation of the weapon to the headset 
            __instance.transform.position = Camera.main.transform.position;
            __instance.transform.localPosition = new Vector3(0, 0, 0);
            __instance.transform.rotation = Camera.main.transform.rotation;
        }


        // The weapons had this annoying behaviour of appearing further down on the Y axis than it should've, this fixes that
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerWeapons), "SelectWeapon")]
        private static void SelectWeapon(PlayerWeapons __instance, object[] __args)
        {
            var index = __args[0];
            WeaponBehavior selectedWeapon = __instance.weaponOrder[int.Parse(index.ToString())].GetComponent<WeaponBehavior>();
            selectedWeapon.transform.position = Camera.main.transform.position;
            selectedWeapon.transform.localPosition = new Vector3(0, 0, 0);

        }


        // This tracks the weapon Y axis rotation to the camera
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FPSRigidBodyWalker), "FixedUpdate")]
        private static void FPSRigidBodyWalkerww(FPSRigidBodyWalker __instance)
        {
            Quaternion bodyRotation = __instance.transform.rotation;
            bodyRotation.y = Camera.main.transform.rotation.y;
            __instance.transform.rotation = Camera.main.transform.rotation;
        }


        // This changes the game manual controls to match oculus controls
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Inv_Item), "Spawn")]
        private static void ChangeManualControls(Inv_Item __instance)
        {
            if (__instance.item == "Game Manual")
            {
                __instance.des = "Move - Left Stick\nCrouch - Right Stick Down\nSprint - Left Stick In\nUse - A\nWhistle - Right Stick In\nFlashlight - X\nReload - Right Grip\nCheck Map - Left Grip\nChange weapon\n- Right Stick Up\nAttack - RT\nBlock - LT\nInventory - B\nPause - Y";
            }

        }
    }
}