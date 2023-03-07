using HarmonyLib;
using UnityEngine;
using UnityStandardAssets.ImageEffects;
using Valve.VR;

namespace VivoVR.Input
{
    [HarmonyPatch]
    internal class VRInputPatches
    {
        private static VRInputValues controls = new VRInputValues();

        // This handles controls for most of the buttons
        [HarmonyPostfix]
        [HarmonyPatch(typeof(My_FPS), "Update")]
        private static void ButtonControlHandler(My_FPS __instance, ref float ___Wait, bool ___Light_on)
        {
            if (__instance.Cut_Scene_player)
            {
                return;
            }

            if ((controls.mapButtonDown() && Time.timeScale == 1f && PlayerPrefs.GetInt("MAP") > 0) && ___Wait < 0.1f && Time.timeScale > 0f)
            {
                __instance.Mappy.SetActive(true);
                __instance.Mappy.GetComponent<AudioSource>().Play();
                Time.timeScale = 0f;
                ___Wait = 0.5f;
            }
            else if (controls.pauseButtonDown() && PlayerPrefs.GetInt("MAIN_1") != 1 && (PlayerPrefs.GetInt("MAP") <= 0 || !__instance.Mappy.active))
            {
                Camera.main.GetComponent<BlurOptimized>().enabled = true;
                __instance.Pause_Menu[0].SetActive(true);
                __instance.Pause_Menu[1].SetActive(true);
                Time.timeScale = 0f;
            }
            else if (controls.flashlightButtonDown() && Time.timeScale == 1f && ___Wait < 0.1f)
            {
                if (!___Light_on)
                {
                    PlayerPrefs.SetInt("Light", 0);
                    __instance.Flashlight_obj.SetActive(true);
                    __instance.Flashlight2_obj.SetActive(true);
                    Traverse.Create(__instance).Field("Light_on").SetValue(true);
                    __instance.Light_snd1.GetComponent<AudioSource>().Play();
                }
                else
                {
                    PlayerPrefs.SetInt("Light", 1);
                    __instance.Flashlight_obj.SetActive(false);
                    __instance.Flashlight2_obj.SetActive(false);
                    Traverse.Create(__instance).Field("Light_on").SetValue(false);
                    __instance.Light_snd2.GetComponent<AudioSource>().Play();
                }
            }
            else if (controls.invButtonDown() && Time.timeScale == 1f)
            {
                __instance.Player_Control.SetActive(false);
                __instance.INV_Menu[0].SetActive(true);
                __instance.INV_Menu[1].SetActive(true);
                __instance.INV_Menu[2].GetComponent<Inv_main_scr>().Open_INV();
                Time.timeScale = 0f;
            }
        }




        // Closes the map screen when it's open
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Map_control), "Update")]
        private static void CloseMap(Map_control __instance, float ___wait)
        {
            if (controls.mapButtonDown() && ___wait < 0.1f)
            {
                Camera.main.GetComponent<BlurOptimized>().enabled = false;
                Time.timeScale = 1f;
                __instance.gameObject.SetActive(false);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(InputControl), "Update")]
        private static void handleVariousInputs(InputControl __instance)
        {
            __instance.fireHold = controls.rightTriggerDown();
            __instance.usePress = controls.interactButtonDown();
            __instance.usePressUp = controls.interactButtonDown();
            __instance.useHold = controls.interactButtonDown();
            __instance.zoomPress = controls.leftTriggerDown();
            __instance.moveX = controls.leftJoyXAxis();
            __instance.moveY = controls.leftJoyYAxis();
            __instance.lookX = controls.rightJoyXAxis();
            __instance.lookY = controls.rightJoyYAxis();
            __instance.sprintHold = controls.sprintButtonHold();
            __instance.reloadPress = controls.reloadButtonDown();
            // Make crouching and next weapon axis range pretty high so user doesn't keep accidentally crouching 
            __instance.crouchHold = (__instance.lookY < -0.9f) ? true : false;
            __instance.selectNextPress = (__instance.lookY > 0.9f) ? true : false; // This is for changing weapons


        }


        private static float cooldown = 0;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Whistle), "Update")]
        private static bool handleWhistle(Whistle __instance)
        {
            if (controls.whistleButtonDown() && cooldown <= Time.time)
            {
                MonoBehaviour.print((object)"Doggy where you at?");
                cooldown = Time.time + 8f;
                var reflection = __instance.GetType().GetMethod("PlayAudios", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                reflection.Invoke(__instance, new object[] { });
            }
            return false;
        }
    }
}
