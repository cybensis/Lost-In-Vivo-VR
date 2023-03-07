using HarmonyLib;
using Locogame.Propagate;
using UnityEngine;
using UnityEngine.UI;


namespace VivoVR.Input
{
    [HarmonyPatch]
    internal class ProximityTriggerPatches
    {
        // FML. Instead of storing the get button input in some object and accessing that in every use case, the code calls upon Input.GetButton("Use") in pretty much every single OnTriggerStay interaction,
        // so now I have to go and write patches for every single one of these functions :,)
        private static VRInputValues controls = new VRInputValues();
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Act_on_button_scr), "OnTriggerStay")]

        private static void Act_on_button_scr_patch(Act_on_button_scr __instance, object[] __args)
        {
            if (((Component)(object)__args[0]).gameObject.CompareTag("Player"))
            {
                if (controls.interactButtonDown())
                {
                    __instance.v_tar.SetActive(value: true);
                    __instance.gameObject.SetActive(false);
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Activate_sub), "OnTriggerStay")]

        private static void Activate_sub_patch(Activate_sub __instance, object[] __args)

        {
            if (((Component)(object)__args[0]).gameObject.CompareTag("Player"))
            {
                if (controls.interactButtonDown())
                {
                    __instance.Pickup();
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Ammo_pickup), "OnTriggerStay")]

        private static void Ammo_pickup_patch(Ammo_pickup __instance, object[] __args)
        {
            if (!((Component)(object)__args[0]).gameObject.CompareTag("Player"))
            {
                return;
            }
            if (controls.interactButtonDown())
            {
                if (!__instance.No_repeat)
                    PlayerPrefs.SetInt("ammo" + __instance.pickup_number, 1);
                __instance.Pickup();
            }
        }



        [HarmonyPostfix]
        [HarmonyPatch(typeof(Basic_door_scr), "OnTriggerStay")]

        private static void Basic_door_scr_patch(Basic_door_scr __instance, ref float ___t, object[] __args)
        {

            if (___t != 0f || !((Component)(object)__args[0]).gameObject.CompareTag("Player"))
            {
                return;
            }
            if (controls.interactButtonDown())
            {
                if (!__instance.locked)
                {
                    ___t = 50f;
                    __instance.OpenDoor();
                }
                else if (__instance.locked && ___t == 0f)
                {
                    if (PlayerPrefs.GetInt("Key_var") == __instance.keys_needed)
                    {
                        ___t = 30f;
                        __instance.UnlockDoor();
                    }
                    else if (PlayerPrefs.GetInt("Key_var") > __instance.keys_needed)
                    {
                        ___t = 50f;
                        __instance.OpenDoor();
                    }
                    else
                    {
                        ___t = 30f;
                        __instance.LockedDoor();
                    }
                }

            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(choice_scr), "OnTriggerStay")]

        private static void choice_scr_patch(choice_scr __instance, object[] __args, ref float ___t, ref GameObject ___player)
        {
            if (___t <= 0f && ((Component)(object)__args[0]).gameObject.CompareTag("Player"))
                if (controls.interactButtonDown())
                {
                    ___t = 1f;
                    ___player = ((Component)(object)__args[0]).gameObject;
                    __instance.Examine();
                }
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(choice_scr), "Update")]

        private static void choice_scr_update_patch(choice_scr __instance, ref float ___t, bool ___QUES, ref bool ___stage)
        {
            if (!___QUES) {
                return;
            }
            if (___t <= 0f)
                if (controls.interactButtonDown())
                {
                    var reflection = __instance.GetType().GetMethod("Exit", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    reflection.Invoke(__instance, new object[] { });
                }
                else if (controls.pauseButtonDown())
                {
                    ___stage = true;
                    var reflection = __instance.GetType().GetMethod("Exit", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    reflection.Invoke(__instance, new object[] { });
                }
                else if (___t <= 0f && (controls.leftJoyXAxis() > 0.3f || controls.leftJoyXAxis() < -0.3f)) {
                    var reflection = __instance.GetType().GetMethod("change", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    reflection.Invoke(__instance, new object[] { });
                }
        }



        [HarmonyPostfix]
        [HarmonyPatch(typeof(DLC_car_scr), "OnTriggerStay")]

        private static void DLC_car_scr_patch(DLC_car_scr __instance, object[] __args, ref float ___t, ref AudioSource[] ___player_snd)
        {

            if (!(___t <= 0f) || !((Component)(object)__args[0]).gameObject.CompareTag("Player") || __instance.IN)
            {
                return;
            }
            if (controls.interactButtonDown())
            {
                ___t = 2f;
                __instance.IN = true;
                __instance.BODY[0].SetActive(value: false);
                __instance.BODY[1].SetActive(value: true);
                __instance.BODY[6].SetActive(value: true);
                __instance.Player[0].GetComponent<FPSRigidBodyWalker>().enabled = false;
                __instance.Player[0].GetComponent<Rigidbody>().useGravity = false;
                __instance.Player[1].GetComponent<CameraControl>().enabled = false;
                __instance.Player[2].transform.parent = __instance.BODY[4].transform;
                __instance.Player[2].transform.position = __instance.POS[0].transform.position;
                __instance.Player[2].transform.rotation = __instance.POS[0].transform.rotation;
                __instance.Player[2].GetComponent<SmoothMouseLook>().enabled = false;
                __instance.Player[2].GetComponent<SmoothMouseLook_ALT>().enabled = true;
                __instance.snd_src[1].GetComponent<AudioSource>().clip = __instance.SNDS[0];
                __instance.snd_src[1].GetComponent<AudioSource>().Play();
                __instance.snd_src[0].GetComponent<AudioSource>().volume = 0.2f;
                ((Behaviour)(object)__instance.snd_src[0].GetComponent<AudioLowPassFilter>()).enabled = true;
                ___player_snd = __instance.Player[0].GetComponents<AudioSource>();
                AudioSource[] array = ___player_snd;
                foreach (AudioSource val in array)
                {
                    ((Behaviour)(object)val).enabled = false;
                }
            }
        }



        [HarmonyPostfix]
        [HarmonyPatch(typeof(DLC_ENT_AI_scr), "OnTriggerStay")]

        private static void DLC_ENT_AI_scr_patch(DLC_ENT_AI_scr __instance, object[] __args)
        {
            if (!((Component)(object)__args[0]).gameObject.CompareTag("Player"))
            {
                return;
            }
            if (__instance.stage > __instance.done_stage || __instance.T <= 0f && controls.interactButtonDown())
            {
                var reflection = __instance.GetType().GetMethod("ReTalk", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                reflection.Invoke(__instance, new object[] { });

            }
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(DLC_fake_sys_scr), "OnTriggerStay")]

        private static void DLC_fake_sys_scr_patch(DLC_fake_sys_scr __instance, object[] __args, int ___stage, ref float ___t, bool ___started)
        {
            if (!((Component)(object)__args[0]).gameObject.CompareTag("Player") || ___stage == 100)
            {
                return;
            }
            if (controls.interactButtonDown())
            {
                if (!___started && ___stage != 6)
                {
                    ___t = 30f;
                    __instance.Examine();
                }

            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(DLC_pills_scr), "OnTriggerStay")]

        private static void DLC_pills_scr_patch(DLC_pills_scr __instance, object[] __args)
        {
            if (!((Component)(object)__args[0]).gameObject.CompareTag("Player"))
            {
                return;
            }
            if (controls.interactButtonDown())
            {
                Camera.main.GetComponent<DLC_screen_effect_scr>().Increase();
                __instance.Status.GetComponent<AudioSource>().clip = __instance.snd;
                __instance.Status.GetComponent<AudioSource>().Play();
                __instance.v_tar.GetComponent<DLC_tele_port_scr>().Pills();
                __instance.transform.parent.gameObject.SetActive(false);
                __instance.gameObject.SetActive(false);

            }
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(DLC_radio_butt_scr), "OnTriggerStay")]

        private static void DLC_radio_butt_scr_patch(DLC_radio_butt_scr __instance, object[] __args)
        {
            if (!((Component)(object)__args[0]).gameObject.CompareTag("Player"))
            {
                return;
            }
            if (controls.interactButtonDown() && __instance.cool <= 0f)
            {
                if (__instance.stage == 0)
                {
                    __instance.stage = 1;
                    GameObject[] array = __instance.tars;
                    foreach (GameObject gameObject in array)
                    {
                        gameObject.GetComponent<PropagateSound>().masterVolume = 0f;
                    }
                    __instance.cool = 1f;
                    __instance.texty = "TURN ON";
                }
                else
                {
                    __instance.stage = 0;
                    __instance.tars[0].GetComponent<PropagateSound>().masterVolume = 0.3f;
                    __instance.tars[1].GetComponent<PropagateSound>().masterVolume = 0.5f;
                    __instance.tars[2].GetComponent<PropagateSound>().masterVolume = 0.8f;
                    __instance.cool = 1f;
                    __instance.texty = "TURN OFF";
                }

            }
        }



        [HarmonyPostfix]
        [HarmonyPatch(typeof(DW_car_scr), "OnTriggerStay")]

        private static void DW_car_scr_patch(DW_car_scr __instance, object[] __args, ref float ___t, ref AudioSource[] ___player_snd)
        {
            if (!(___t <= 0f) || !((Component)(object)__args[0]).gameObject.CompareTag("Player"))
            {
                return;
            }
            if (controls.interactButtonDown())
            {
                ___t = 2f;
                if (__instance.IN)
                {
                    if (!__instance.POS[1].GetComponent<DW_car_out>().BLOCKED)
                    {
                        __instance.IN = false;
                        __instance.Rain[0].SetActive(value: true);
                        __instance.Rain[1].SetActive(value: false);
                        __instance.DRIVE = false;
                        __instance.speed = 0f;
                        __instance.BODY[3].GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0f, 0f, 0f), (ForceMode)2);
                        __instance.Wheels[0].motorTorque = 0f;
                        __instance.Wheels[1].motorTorque = 0f;
                        __instance.Flash.SetActive(value: true);
                        __instance.BODY[0].SetActive(value: true);
                        __instance.BODY[1].SetActive(value: false);
                        __instance.Player[0].GetComponent<FPSRigidBodyWalker>().enabled = true;
                        __instance.Player[1].GetComponent<CameraControl>().enabled = true;
                        __instance.Player[0].transform.parent = null;
                        __instance.Player[2].transform.parent = null;
                        __instance.Player[0].transform.position = __instance.POS[1].transform.position;
                        __instance.Player[0].GetComponent<Rigidbody>().useGravity = true;
                        __instance.Player[2].transform.position = __instance.POS[1].transform.position;
                        __instance.Player[2].GetComponent<SmoothMouseLook>().enabled = true;
                        __instance.Player[2].GetComponent<SmoothMouseLook_ALT>().enabled = false;
                        __instance.snd_src[1].GetComponent<AudioSource>().clip = __instance.SNDS[1];
                        __instance.snd_src[1].GetComponent<AudioSource>().Play();
                        __instance.snd_src[0].GetComponent<AudioSource>().volume = 0.5f;
                        __instance.snd_src[0].GetComponent<AudioLowPassFilter>().cutoffFrequency = 5000f;
                        ((Behaviour)(object)__instance.snd_src[0].GetComponent<AudioLowPassFilter>()).enabled = false;
                        __instance.snd_src[0].GetComponent<AudioSource>().pitch = 1f;
                        __instance.driving_time = 0f;
                        __instance.BODY[6].SetActive(value: false);
                        __instance.BODY[6].transform.localScale = new Vector3(0f, 0f, 0f);
                        __instance.BODY[9].SetActive(value: true);
                        __instance.BODY[10].SetActive(value: false);
                        __instance.Cruise = false;
                        __instance.Player[0].GetComponent<Footsteps>().enabled = true;
                        ___player_snd = __instance.Player[0].GetComponents<AudioSource>();
                        AudioSource[] array = ___player_snd;
                        foreach (AudioSource val in array)
                        {
                            ((Behaviour)(object)val).enabled = true;
                        }
                    }
                }
                else
                {
                    __instance.IN = true;
                    __instance.Rain[0].SetActive(value: false);
                    __instance.Rain[1].SetActive(value: true);
                    __instance.Flash.SetActive(value: true);
                    __instance.BODY[0].SetActive(value: false);
                    __instance.BODY[1].SetActive(value: true);
                    __instance.Player[0].GetComponent<FPSRigidBodyWalker>().enabled = false;
                    __instance.Player[0].GetComponent<Rigidbody>().useGravity = false;
                    __instance.Player[1].GetComponent<CameraControl>().enabled = false;
                    __instance.Player[2].transform.parent = __instance.BODY[5].transform;
                    __instance.Player[2].transform.position = __instance.POS[0].transform.position;
                    __instance.Player[2].transform.rotation = __instance.POS[0].transform.rotation;
                    __instance.Player[2].GetComponent<SmoothMouseLook>().enabled = false;
                    __instance.Player[2].GetComponent<SmoothMouseLook_ALT>().enabled = true;
                    __instance.snd_src[1].GetComponent<AudioSource>().clip = __instance.SNDS[0];
                    __instance.snd_src[1].GetComponent<AudioSource>().Play();
                    __instance.snd_src[0].GetComponent<AudioSource>().volume = 0.2f;
                    ((Behaviour)(object)__instance.snd_src[0].GetComponent<AudioLowPassFilter>()).enabled = true;
                    ___player_snd = __instance.Player[0].GetComponents<AudioSource>();
                    AudioSource[] array2 = ___player_snd;
                    foreach (AudioSource val2 in array2)
                    {
                        ((Behaviour)(object)val2).enabled = false;
                    }
                }
            }
        }



        [HarmonyPostfix]
        [HarmonyPatch(typeof(Eat_scr), "OnTriggerStay")]

        private static void Eat_scr_patch(Eat_scr __instance, object[] __args)
        {
            if (!((Component)(object)__args[0]).gameObject.CompareTag("Player"))
            {
                return;
            }
            if (controls.interactButtonDown())
            {
                __instance.Pickup();
            }
        }



        [HarmonyPostfix]
        [HarmonyPatch(typeof(Emblem_door_scr), "OnTriggerStay")]

        private static void Emblem_door_scr_patch(Emblem_door_scr __instance, object[] __args, ref bool ___Not_Start, int ___state)
        {
            if (!((Component)(object)__args[0]).gameObject.CompareTag("Player") || !(__instance.cool < 0f))
            {
                return;
            }
            if (controls.interactButtonDown())
            {
                ___Not_Start = true;
                if (___state == 1)
                {
                    __instance.GetComponent<AudioSource>().clip = __instance.snds[0];
                    __instance.GetComponent<AudioSource>().Play();
                    __instance.cool = 1f;
                    var Remove_EM = __instance.GetType().GetMethod("Remove_EM", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (PlayerPrefs.GetInt("PKEY0") == 1)
                    {
                        PlayerPrefs.SetInt("PKEY0", 2);
                        Remove_EM.Invoke(__instance, new object[] { 0 });
                        __instance.Check_EMS();
                    }
                    else if (PlayerPrefs.GetInt("PKEY1") == 1)
                    {
                        PlayerPrefs.SetInt("PKEY1", 2);
                        Remove_EM.Invoke(__instance, new object[] { 1 });
                        __instance.Check_EMS();
                    }
                    else if (PlayerPrefs.GetInt("PKEY2") == 1)
                    {
                        PlayerPrefs.SetInt("PKEY2", 2);
                        Remove_EM.Invoke(__instance, new object[] { 2 });
                        __instance.Check_EMS();
                    }
                }
                else if (___state == 20)
                {
                    __instance.Door_trig.GetComponent<Basic_door_scr>().OpenDoor();
                    __instance.cool = 10f;
                }
                else
                {
                    __instance.Door_trig.GetComponent<Basic_door_scr>().LockedDoor();
                    __instance.cool = 1f;
                }
            }
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(Emblem_pickup), "OnTriggerStay")]

        private static void Emblem_pickup_patch(Emblem_pickup __instance, object[] __args)
        {
            if (!((Component)(object)__args[0]).gameObject.CompareTag("Player"))
            {
                return;
            }
            if (controls.interactButtonDown())
            {
                __instance.Pickup();
            }
        }



        [HarmonyPostfix]
        [HarmonyPatch(typeof(Examine_Detail_scr), "OnTriggerStay")]

        private static void Examine_Detail_scr_patch(Examine_Detail_scr __instance, object[] __args, ref float ___t, bool ___started)
        {
            if (___t != 0f || !((Component)(object)__args[0]).gameObject.CompareTag("Player"))
            {
                return;
            }
            if (controls.interactButtonDown())
            {
                if (!___started && __instance.Status.GetComponent<Sys_scr>().stage != 6)
                {
                    ___t = 30f;
                    __instance.Examine();
                }
            }
        }



        [HarmonyPostfix]
        [HarmonyPatch(typeof(Examine_repeat_scr), "OnTriggerStay")]

        private static void Examine_repeat_scr_patch(Examine_repeat_scr __instance, object[] __args, ref float ___t, ref bool ___started)
        {
            if (___t != 0f || !((Component)(object)__args[0]).gameObject.CompareTag("Player"))
            {
                return;
            }
            if (controls.interactButtonDown())
            {
                if (!___started)
                {
                    ___t = 30f;
                    __instance.Examine();
                    ___started = true;
                }
            }
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(Examine_Scr), "OnTriggerStay")]

        private static void Examine_Scr_patch(Examine_Scr __instance, object[] __args, ref float ___t)
        {
            if (___t != 0f || !((Component)(object)__args[0]).gameObject.CompareTag("Player"))
            {
                return;
            }
            if (controls.interactButtonDown())
            {
                ___t = 30f;
                __instance.Examine();
            }
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(Fake_pickup_scr), "OnTriggerStay")]

        private static void Fake_pickup_scr_patch(Fake_pickup_scr __instance, object[] __args, bool ___ded)
        {
            if (___ded || !((Component)(object)__args[0]).gameObject.CompareTag("Player"))
            {
                return;
            }
            if (controls.interactButtonDown())
            {
                __instance.Pickup();
            }
        }



        [HarmonyPostfix]
        [HarmonyPatch(typeof(FIP_button_scr), "OnTriggerStay")]

        private static void FIP_button_scr_patch(FIP_button_scr __instance, object[] __args, ref float ___t)
        {
            if (___t != 0f || !((Component)(object)__args[0]).gameObject.CompareTag("Player"))
            {
                return;
            }
            if (controls.interactButtonDown())
            {
                ___t = 40f;
                __instance.face.GetComponent<FIP_scr>().Flip();
            }
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(inv_error_scr), "OnTriggerStay")]

        private static void inv_error_scr_patch(inv_error_scr __instance, object[] __args)
        {
            if (__instance.done || !((Component)(object)__args[0]).gameObject.CompareTag("Player"))
            {
                return;
            }
            if (controls.interactButtonDown())
            {
                var reflection = __instance.GetType().GetMethod("Pickup", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                reflection.Invoke(__instance, new object[] { });
                __instance.done = true;
            }
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(Item_pickup), "OnTriggerStay")]

        private static void Item_pickup_patch(Item_pickup __instance, object[] __args)
        {
            if (!((Component)(object)__args[0]).gameObject.CompareTag("Player"))
            {
                return;
            }
            if (controls.interactButtonDown())
            {
                __instance.Pickup();
                if (__instance.play_sounds)
                {
                    __instance.snd.SetActive(value: true);
                }
            }
        }




        [HarmonyPostfix]
        [HarmonyPatch(typeof(Key_pickup_scr), "OnTriggerStay")]

        private static void Key_pickup_scr_patch(Key_pickup_scr __instance, object[] __args)
        {
            if (!((Component)(object)__args[0]).gameObject.CompareTag("Player"))
            {
                return;
            }
            if (controls.interactButtonDown())
            {
                __instance.Pickup();
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Ladder_enter_scr), "OnTriggerStay")]
        private static void Ladder_enter_scr_patch(Ladder_enter_scr __instance, object[] __args, bool ___CoolDown)
        {
            if (!((Component)(object)__args[0]).gameObject.CompareTag("Player") || ___CoolDown)
            {
                return;
            }
            if (controls.interactButtonDown())
            {
                var reflection = __instance.GetType().GetMethod("Pickup", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                reflection.Invoke(__instance, new object[] { });
            }
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(map_pickup), "OnTriggerStay")]
        private static void map_pickup_patch(map_pickup __instance, object[] __args)
        {
            if (!((Component)(object)__args[0]).gameObject.CompareTag("Player"))
            {
                return;
            }
            if (controls.interactButtonDown())
            {
                __instance.Pickup();
            }
        }



        [HarmonyPostfix]
        [HarmonyPatch(typeof(Midite_sword_scr), "OnTriggerStay")]
        private static void Midite_sword_scr_patch(Midite_sword_scr __instance, object[] __args)
        {
            if (!((Component)(object)__args[0]).gameObject.CompareTag("Player"))
            {
                return;
            }
            if (controls.interactButtonDown())
            {
                var reflection = __instance.GetType().GetMethod("Pickup", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                reflection.Invoke(__instance, new object[] { });
                return;
            }
        }




        [HarmonyPostfix]
        [HarmonyPatch(typeof(normal_item_pickup_scr), "OnTriggerStay")]
        private static void normal_item_pickup_scr_patch(normal_item_pickup_scr __instance, object[] __args)
        {
            if (((Component)(object)__args[0]).gameObject.CompareTag("Player") && __instance.wait < 0.1f)
            {
                if (controls.interactButtonDown())
                {
                    PlayerPrefs.SetInt("item" + __instance.pickup_number, 1);
                    __instance.Pickup();
                }
            }
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(Note_pickup_scr), "OnTriggerStay")]
        private static void Note_pickup_scr_patch(Note_pickup_scr __instance, object[] __args, ref float ___t, ref bool ___held, ref GameObject ___texty)
        {
            if (!((Component)(object)__args[0]).gameObject.CompareTag("Player") || !(___t < 1f)) {
                return;
            }
            if (controls.interactButtonDown())
            {
                if (!___held)
                {
                    ___held = true;
                    __instance.GetComponent<AudioSource>().clip = __instance.snds[0];
                    __instance.GetComponent<AudioSource>().Play();
                    ___t = 20f;
                    __instance.HUD_note.SetActive(value: true);
                    __instance.HUD_note.GetComponent<RawImage>().texture = __instance.tex;
                    __instance.HUD_text.GetComponent<UnityEngine.UI.Text>().text = ___texty.GetComponent<TextMesh>().text;
                    __instance.Status.GetComponent<UnityEngine.UI.Text>().text = string.Empty;
                    __instance.gameObject.GetComponent<MeshRenderer>().enabled = false;
                    ___texty.SetActive(value: false);
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Note_pickup_scr), "FixedUpdate")]
        private static void Note_pickup_scr_end_patch(Note_pickup_scr __instance, ref float ___t, ref bool ___held, ref GameObject ___texty)
        {
            if (___t > 0f)
            {
                ___t -= 0.5f;
            }
            if (___t < 1f && ___held && controls.interactButtonDown())
            {
                ___held = false;
                __instance.GetComponent<AudioSource>().clip = __instance.snds[1];
                __instance.GetComponent<AudioSource>().Play();
                ___t = 20f;
                __instance.HUD_note.SetActive(false);
                __instance.gameObject.GetComponent<MeshRenderer>().enabled = true;
                ___texty.SetActive(true);
            }
        }



        [HarmonyPostfix]
        [HarmonyPatch(typeof(Padlock_scr), "OnTriggerStay")]
        private static void Padlock_scr_patch(Padlock_scr __instance, object[] __args, ref float ___t2, ref int ___started, ref float ___amount)
        {
            if (!((Component)(object)__args[0]).gameObject.CompareTag("Player") || !(___t2 < 0f))
            {
                return;
            }

            if (controls.interactButtonHeldDown())
            {
                if (PlayerPrefs.GetInt("Pick_var") > 0)
                {
                    if (___started == 0)
                    {
                        ___started = 1;
                        __instance.Model.GetComponent<Animation>().Play("Padlock_loop");
                        __instance.Model.GetComponent<AudioSource>().clip = __instance.snds[0];
                        __instance.Model.GetComponent<AudioSource>().loop = true;
                        __instance.Model.GetComponent<AudioSource>().Play();
                        __instance.sprite.SetActive(value: true);
                    }
                    else if (___started == 1)
                    {
                        ___amount -= 0.1f;
                        __instance.sprite.transform.Rotate(0f, 0f, -3f);
                        if (___amount < 0f)
                        {
                            __instance.sprite.SetActive(value: false);
                            __instance.Unlock();
                        }
                    }
                }
                else if (PlayerPrefs.GetInt("Pick_var") < 1)
                {
                    ___t2 = 10f;
                    __instance.Sys.GetComponent<UnityEngine.UI.Text>().text = "Locked";
                    __instance.Sys.GetComponent<Sys_scr>().t = 1.5f;
                    __instance.Sys.GetComponent<Sys_scr>().stage = 1;
                    __instance.Model.GetComponent<AudioSource>().clip = __instance.snds[1];
                    __instance.Model.GetComponent<AudioSource>().loop = false;
                    __instance.Model.GetComponent<AudioSource>().Play();
                }
            }
            else if (controls.interactButtonUp())
            {
                __instance.sprite.SetActive(value: false);
                __instance.Model.GetComponent<Animation>().Stop();
                if (___started == 1)
                {
                    var reflection = __instance.GetType().GetMethod("Fail", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    reflection.Invoke(__instance, new object[] { });
                }
            }
        }



        [HarmonyPostfix]
        [HarmonyPatch(typeof(pet_scr), "OnTriggerStay")]
        private static void pet_scr_patch(pet_scr __instance, object[] __args, ref float ___t, ref bool ___pet)
        {
            if (___t <= 0f && !___pet && ((Component)(object)__args[0]).gameObject.CompareTag("Player"))
            {
                if (controls.interactButtonDown())
                {
                    ___t = 2.7f;
                    ___pet = true;
                    __instance.DOGGO.GetComponent<Animation>().CrossFade("dog_lick_pet", 0.2f, (PlayMode)4);
                    __instance.GetComponent<AudioSource>().clip = __instance.pet_snd;
                    __instance.GetComponent<AudioSource>().Play();
                    Camera.main.GetComponent<Animation>().Play("CameraLand");
                }
            }
        }



        [HarmonyPostfix]
        [HarmonyPatch(typeof(Pickup_Acid_scr), "OnTriggerStay")]
        private static void Pickup_Acid_scr_patch(Pickup_Acid_scr __instance, object[] __args)
        {
            if (!((Component)(object)__args[0]).gameObject.CompareTag("Player"))
            {
                return;
            }
            if (controls.interactButtonDown())
            {
                __instance.Pickup();
                return;
            }
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(Pickup_fake_scr), "OnTriggerStay")]
        private static void Pickup_fake_scr_patch(Pickup_fake_scr __instance, object[] __args)
        {
            if (!((Component)(object)__args[0]).gameObject.CompareTag("Player"))
            {
                return;
            }
            if (controls.interactButtonDown())
            {
                __instance.Pickup();
                return;
            }
        }



        [HarmonyPostfix]
        [HarmonyPatch(typeof(Pickup_sword), "OnTriggerStay")]
        private static void Pickup_sword_patch(Pickup_sword __instance, object[] __args, ref int ___done, ref float ___t)
        {
            if (!((Component)(object)__args[0]).gameObject.CompareTag("Player"))
            {
                return;
            }
            if (controls.interactButtonDown() && ___done == 0)
            {
                __instance.Altar.GetComponent<AudioSource>().Play();
                __instance.Sword.GetComponent<Animation>().Play();
                ___t = 22f;
                ___done = 1;
            }
        }



        [HarmonyPostfix]
        [HarmonyPatch(typeof(power_switch_brain_scr), "OnTriggerStay")]
        private static void power_switch_brain_scr_patch(power_switch_brain_scr __instance, object[] __args)
        {
            if (((Component)(object)__args[0]).gameObject.CompareTag("Player") && !__instance.power && __instance.cooldown <= 0f) { 
                if (controls.interactButtonDown())
                {
                    var reflection = __instance.GetType().GetMethod("ON", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    reflection.Invoke(__instance, new object[] { });
                    __instance.sub_cool = 0;
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Run_anim_scr), "OnTriggerStay")]
        private static void Run_anim_scr_patch(Run_anim_scr __instance, object[] __args, ref float ___t)
        {
            if (___t == 0f && ((Component)(object)__args[0]).gameObject.CompareTag("Player"))
            {
                if (controls.interactButtonDown())
                {

                    ___t = __instance.reset_time;
                    var reflection = __instance.GetType().GetMethod("Run", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    reflection.Invoke(__instance, new object[] { });
                }
            }
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(Save_door_scr), "OnTriggerStay")]
        private static void Save_door_scr_patch(Save_door_scr __instance, object[] __args, ref float ___t)
        {
            if (___t == 0f && ((Component)(object)__args[0]).gameObject.CompareTag("Player"))
            {
                if (controls.interactButtonDown())
                {

                    ___t = 50f;
                    __instance.OpenDoor();
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Save_door2_scr), "OnTriggerStay")]
        private static void Save_door2_scr_patch(Save_door2_scr __instance, object[] __args, ref float ___t)
        {
            if (___t == 0f && ((Component)(object)__args[0]).gameObject.CompareTag("Player"))
            {
                if (controls.interactButtonDown())
                {

                    ___t = 50f;
                    __instance.OpenDoor();
                }
            }
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(Save_trig_scr), "OnTriggerStay")]
        private static void Save_trig_scr_patch(Save_trig_scr __instance, object[] __args, ref int ___stage, ref float ___t)
        {
            if (___stage == 0 && ((Component)(object)__args[0]).gameObject.CompareTag("Player"))
            {
                if (controls.interactButtonDown())
                {

                    ___t = 0f;
                    __instance.Door.SetActive(value: false);
                    var reflection = __instance.GetType().GetMethod("SAVE", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    reflection.Invoke(__instance, new object[] { });
                }
            }
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(Simple_door_scr), "OnTriggerStay")]
        private static void Simple_door_scr_patch(Simple_door_scr __instance, object[] __args, ref float ___t)
        {
            if (___t != 0 || !((Component)(object)__args[0]).gameObject.CompareTag("Player")) {
                return;
            }
            {
                if (controls.interactButtonDown())
                {
                    if (!__instance.locked)
                    {
                        if (!__instance.open)
                        {
                            ___t = 35f;
                            __instance.OpenDoor();
                        }
                        else if (__instance.open)
                        {
                            if (__instance.Hold_Item && __instance.Item.active)
                            {
                                ___t = 35f;
                                return;
                            }
                            ___t = 35f;
                            __instance.CloseDoor();
                        }
                    }
                    else if (__instance.locked && ___t == 0f)
                    {
                        ___t = 35f;
                        __instance.LockedDoor();
                    }
                    ___t = 0f;
                }
            }
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(sub_doors_scr), "OnTriggerStay")]
        private static void sub_doors_scr_patch(sub_doors_scr __instance, object[] __args)
        {
            if (!((Component)(object)__args[0]).gameObject.CompareTag("Player")) {
                return;
            }
            if (controls.interactButtonDown())
            {
                if (!__instance.locked && !__instance.open)
                {
                    /*var reflection = __instance.GetType().GetMethod("Change", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    reflection.Invoke(__instance, new object[] { true });*/
                    __instance.Change(true);
                }
            }
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(Switch_flip_trig), "OnTriggerStay")]
        private static void Switch_flip_trig_patch(Switch_flip_trig __instance, object[] __args, ref float ___t)
        {
            if (___t != 0f || !((Component)(object)__args[0]).gameObject.CompareTag("Player"))
            {
                return;
            }
            if (controls.interactButtonDown())
            {

                if (!__instance.done)
                {
                    ___t = 60f;
                    /*var reflection = __instance.GetType().GetMethod("Switch", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    reflection.Invoke(__instance, new object[] { });*/
                    __instance.Switch(); 
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Tape_button_scr), "OnTriggerStay")]
        private static void Tape_button_scr_patch(Tape_button_scr __instance, object[] __args, ref float ___t)
        {
            if (__instance.t <= 0f && ((Component)(object)__args[0]).gameObject.CompareTag("Player"))
            {
                if (controls.interactButtonDown())
                {
                    __instance.menu.SetActive(true);
                    __instance.menu.GetComponent<Tape_screen>().BEGIN();
                    __instance.t = 2f;
                }
            }
        }



        [HarmonyPostfix]
        [HarmonyPatch(typeof(Tape_door_scr), "OnTriggerStay")]
        private static void Tape_door_scr_patch(Tape_door_scr __instance, object[] __args)
        {
            if (Time.timeSinceLevelLoad > __instance.wait && ((Component)(object)__args[0]).gameObject.CompareTag("Player"))
            {
                if (controls.interactButtonDown())
                {
                    ((Component)(object)__args[0]).transform.position = __instance.POS.transform.position;
                    __instance.Overlay.GetComponent<Auto_Kill_scr>().life = 30f;
                    __instance.Overlay.SetActive(true);
                    var reflection = __instance.GetType().GetMethod("OpenDoor", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    reflection.Invoke(__instance, new object[] { });
                }
            }
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(valve_scr), "OnTriggerStay")]
        private static void valve_scr_patch(valve_scr __instance, object[] __args, int ___stage, ref float ___t)
        {
            if (___stage < 4 && ((Component)(object)__args[0]).gameObject.CompareTag("Player"))
            {
                if (controls.interactButtonDown() && ___t <= 0f)
                {
                    ___t = 2.4f;
                    var reflection = __instance.GetType().GetMethod("Turn", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    reflection.Invoke(__instance, new object[] { });
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Vice_scr), "OnTriggerStay")]
        private static void Vice_scr_patch(Vice_scr __instance, object[] __args, int ___stage, ref float ___t)
        {
            if (___stage < 4 && ((Component)(object)__args[0]).gameObject.CompareTag("Player"))
            {
                if (controls.interactButtonDown() && ___t <= 0f)
                {
                    ___t = 2.4f;
                    var reflection = __instance.GetType().GetMethod("Turn", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    reflection.Invoke(__instance, new object[] { });
                }
            }
        }
    }
}
