using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


namespace VivoVR.Input
{
    [HarmonyPatch]
    internal class MenuPatches
    {
        private static VRInputValues controls = new VRInputValues();

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Menu_tunnel), "Start")]
        private static void moveMenuTunnel(Menu_tunnel __instance)
        {
            Vector3 menuTunnelPos = __instance.transform.position;
            menuTunnelPos.y = menuTunnelPos.y + 0.4f;
            __instance.transform.position = menuTunnelPos;
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(Gameover_menu_scr), "FixedUpdate")]
        private static void GameOverMenuControls(Gameover_menu_scr __instance, ref float ___t, ref int ___press, ref int ___press2)
        {

            if (___t < 1f)
            {
                if (controls.leftJoyXAxis() > 0.3f || controls.leftJoyXAxis() < -0.3f)
                {
                    ___press = 0;
                    ___press2 = 0;
                    ___t = 2f;
                    if (__instance.slot == 0)
                    {
                        __instance.slot = 1;
                    }
                    else
                    {
                        __instance.slot = 0;
                    }
                    var reflection = __instance.GetType().GetMethod("Select", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    reflection.Invoke(__instance, new object[] { });
                }
                else if (controls.interactButtonDown())
                {
                    ___t = 2f;
                    var reflection = __instance.GetType().GetMethod("Hit", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    reflection.Invoke(__instance, new object[] { });
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(menu_fade_script), "FixedUpdate")]
        private static void MenuFadeScriptControls(menu_fade_script __instance, ref float ___menu)
        {
            if (___menu < 8f && controls.interactButtonDown())
            {
                var reflection = __instance.GetType().GetMethod("Skip", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                reflection.Invoke(__instance, new object[] { });
            }

        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(Tape_screen), "Update")]
        private static void TapeScreenControls(Tape_screen __instance, ref float ___t, bool ___has1, bool ___has2, bool ___has3, bool ___has4)
        {
            if (controls.leftJoyYAxis() > 0.3f)
            {
                if (__instance.select == 0)
                {
                    __instance.select = 4;
                }
                else
                {
                    __instance.select--;
                }
                var reflection = __instance.GetType().GetMethod("change", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                reflection.Invoke(__instance, new object[] { });
            }
            if (controls.leftJoyYAxis() < -0.3f)
            {
                if (__instance.select == 4)
                {
                    __instance.select = 0;
                }
                else
                {
                    __instance.select++;
                }
                var reflection = __instance.GetType().GetMethod("change", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                reflection.Invoke(__instance, new object[] { });
            }
            if (controls.interactButtonDown() && ___t <= 0f)
            {
                ___t = 1f;
                __instance.GetComponent<AudioSource>().clip = __instance.snds[1];
                if (__instance.select == 4)
                {
                    var reflection = __instance.GetType().GetMethod("Leave", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    reflection.Invoke(__instance, new object[] { });
                }
                else if (__instance.select == 0 && ___has1)
                {
                    var reflection = __instance.GetType().GetMethod("GO", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    reflection.Invoke(__instance, new object[] { });
                }
                else if (__instance.select == 1 && ___has2)
                {
                    var reflection = __instance.GetType().GetMethod("GO", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    reflection.Invoke(__instance, new object[] { });
                }
                else if (__instance.select == 2 && ___has3)
                {
                    var reflection = __instance.GetType().GetMethod("GO", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    reflection.Invoke(__instance, new object[] { });
                }
                else if (__instance.select == 3 && ___has4)
                {
                    var reflection = __instance.GetType().GetMethod("GO", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    reflection.Invoke(__instance, new object[] { });
                }
                else
                {
                    __instance.GetComponent<AudioSource>().clip = __instance.snds[2];
                }
                if (__instance.select != 4)
                {
                    __instance.GetComponent<AudioSource>().Play();
                }
            }
            if ((controls.interactButtonDown() || controls.rightJoyYAxis() < -0.8f) && ___t <= 0f)
            {
                var reflection = __instance.GetType().GetMethod("Leave", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                reflection.Invoke(__instance, new object[] { });
            }
        }



        // This patch is meant to use a ton of coroutines but they aint gunna work with reflections and I can't be assed figuring out how they work atm, but this might not work
        // at all so better to use keyboard buttons for the moment.
        [HarmonyPostfix]
        [HarmonyPatch(typeof(DLC_tape_menu_scr), "Update")]
        private static void DLCTapeMenuControls(DLC_tape_menu_scr __instance)
        {

            if (__instance.locked)
            {
                return;
            }
            if (controls.interactButtonDown())
            {
                if (__instance.Yslot == 1)
                {
                    var reflection = __instance.GetType().GetMethod("GO", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    reflection.Invoke(__instance, new object[] { });
                    /*StartCoroutine(GO());*/
                }
                else if (__instance.Yslot == 0 || (__instance.Yslot == 2 && __instance.screen == 1))
                {
                    if (__instance.slot < 2)
                    {
                        if (__instance.screen == 0)
                        {
                            /*StartCoroutine(Open());*/
                            var reflection = __instance.GetType().GetMethod("Open", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                            reflection.Invoke(__instance, new object[] { });
                        }
                        else
                        {
                            /* StartCoroutine(Close());*/
                            var reflection = __instance.GetType().GetMethod("Close", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                            reflection.Invoke(__instance, new object[] { });
                        }
                    }
                    else
                    {
                        __instance.GetComponent<AudioSource>().clip = __instance.snds[2];
                        __instance.GetComponent<AudioSource>().Play();
                    }
                }
                else
                {
                    SceneManager.LoadScene("menu");
                }
            }
            if (controls.leftJoyXAxis() > 0.3f)
            {
                if (__instance.screen == 0)
                {
                    __instance.slot++;
                    if (__instance.slot > 4)
                    {
                        __instance.slot = 0;
                        var reflection = __instance.GetType().GetMethod("Change", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        reflection.Invoke(__instance, new object[] { 10 });
                    }
                    else
                    {
                        var reflection = __instance.GetType().GetMethod("Change", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        reflection.Invoke(__instance, new object[] { 1 });
                    }
                }
                else
                {
                    var reflection = __instance.GetType().GetMethod("Close", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    reflection.Invoke(__instance, new object[] { });
                }
            }
            if (controls.leftJoyXAxis() < -0.3f)
            {
                if (__instance.screen == 0)
                {
                    __instance.slot--;
                    if (__instance.slot < 0)
                    {
                        __instance.slot = 4;
                        var reflection = __instance.GetType().GetMethod("Change", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        reflection.Invoke(__instance, new object[] { 20 });
                    }
                    else
                    {
                        var reflection = __instance.GetType().GetMethod("Change", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        reflection.Invoke(__instance, new object[] { -1 });
                    }
                }
                else
                {
                    /* StartCoroutine(Close());*/
                    var reflection = __instance.GetType().GetMethod("Close", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    reflection.Invoke(__instance, new object[] { });
                }
            }
            if (controls.leftJoyYAxis() > 0.3f)
            {
                __instance.Yslot--;
                if (__instance.Yslot < 0)
                {
                    __instance.Yslot = 2;
                    var reflection = __instance.GetType().GetMethod("Change_Y", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    reflection.Invoke(__instance, new object[] { 10 });
                }
                else
                {
                    var reflection = __instance.GetType().GetMethod("Change_Y", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    reflection.Invoke(__instance, new object[] { 1 });
                }
            }
            if (controls.leftJoyYAxis() > -0.3f)
            {
                __instance.Yslot++;
                if (__instance.Yslot > 2)
                {
                    __instance.Yslot = 0;
                    var reflection = __instance.GetType().GetMethod("Change_Y", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    reflection.Invoke(__instance, new object[] { 10 });
                }
                else
                {

                    var reflection = __instance.GetType().GetMethod("Change_Y", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    reflection.Invoke(__instance, new object[] { 1 });
                }
            }
        }

        private static int pauseMenuControlTimer = 7;


        [HarmonyPrefix]
        [HarmonyPatch(typeof(New_PAUSE), "Update")]
        private static void pauseMenuControls(New_PAUSE __instance)
        {
            float yAxis = controls.leftJoyYAxis();
            var reflection = __instance.GetType().GetMethod("change", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (pauseMenuControlTimer <= 0)
            {
                if (yAxis < -0.3f)
                {
                    reflection.Invoke(__instance, new object[] { 1 });
                }
                else if (yAxis > 0.3f)
                {
                    reflection.Invoke(__instance, new object[] { -1 });

                }
                pauseMenuControlTimer = 7;
            }
            else
            {
                pauseMenuControlTimer--;
            }
            if (controls.interactButtonDown())
            {
                if (__instance.new_stage == 1)
                {
                    reflection = __instance.GetType().GetMethod("Exit", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    reflection.Invoke(__instance, new object[] { });
                }
                else
                {
                    reflection = __instance.GetType().GetMethod("Cont", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    reflection.Invoke(__instance, new object[] { });
                }
            }
            else if (controls.pauseButtonDown())
            {
                reflection = __instance.GetType().GetMethod("Cont", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                reflection.Invoke(__instance, new object[] { });
            }
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(Menu_Item_scr), "FixedUpdate")]
        private static void MainMenuControls(Menu_Item_scr __instance, ref float ___t, float ___delay, ref int ___rand_game)
        {
            if (___t < 1f)
            {
                if (__instance.screen == 2)
                {
                    if (controls.leftJoyYAxis() > 0.3f)
                    {
                        ___t = 2f;
                        if (__instance.slot == 1)
                        {
                            __instance.slot = 3;
                        }
                        else
                        {
                            __instance.slot--;
                        }
                        var reflection = __instance.GetType().GetMethod("Select", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        reflection.Invoke(__instance, new object[] { });
                    }
                    if (controls.leftJoyYAxis() < -0.3f)
                    {
                        ___t = 2f;
                        if (__instance.slot == 3)
                        {
                            __instance.slot = 1;
                        }
                        else
                        {
                            __instance.slot++;
                        }
                        var reflection = __instance.GetType().GetMethod("Select", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        reflection.Invoke(__instance, new object[] { });

                    }
                    if (controls.interactButtonDown() && ___delay < 0.1f)
                    {
                        ___t = 2f;
                        var reflection = __instance.GetType().GetMethod("Hit", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        reflection.Invoke(__instance, new object[] { });
                    }
                    if (controls.leftJoyXAxis() < -0.3f && __instance.slot == 1)
                    {
                        __instance.slider2.GetComponent<UnityEngine.UI.Slider>().value -= 0.01f;
                        __instance.SetGAM(__instance.slider2.GetComponent<UnityEngine.UI.Slider>().value);

                    }
                    if (controls.leftJoyXAxis() > 0.3f && __instance.slot == 1)
                    {
                        __instance.slider2.GetComponent<UnityEngine.UI.Slider>().value += 0.01f;
                        __instance.SetGAM(__instance.slider2.GetComponent<UnityEngine.UI.Slider>().value);
                    }
                    return;
                }
                if (controls.leftJoyYAxis() > 0.3f)
                {

                    ___t = 2f;
                    __instance.menu1_obj[0].GetComponent<Text>().text = "NEW GAME";
                    __instance.menu4_obj[3].GetComponent<Text>().text = "DELETE DATA";
                    if (PlayerPrefs.GetInt("PLUS") > 0)
                    {
                        __instance.menu1_obj[5].SetActive(value: true);
                        if (___rand_game == 1)
                        {
                            __instance.menu1_obj[5].GetComponent<Text>().text = "+" + PlayerPrefs.GetInt("PLUS") + " ???";
                        }
                        else
                        {
                            __instance.menu1_obj[5].GetComponent<Text>().text = "+" + PlayerPrefs.GetInt("PLUS");
                        }
                    }
                    else
                    {
                        __instance.menu1_obj[5].SetActive(value: false);
                    }
                    if (__instance.slot == 1)
                    {
                        if (__instance.screen != 1 && __instance.screen != 4 && __instance.menu1_obj[3].GetComponent<Text>().text == "EXIT")
                        {
                            __instance.slot = 4;
                        }
                        else
                        {
                            __instance.slot = 5;
                        }
                    }
                    else
                    {
                        __instance.slot--;
                    }
                    var reflection = __instance.GetType().GetMethod("Select", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    reflection.Invoke(__instance, new object[] { });
                }
                if (controls.leftJoyYAxis() < -0.3f)
                {

                    ___t = 2f;
                    __instance.menu1_obj[0].GetComponent<Text>().text = "NEW GAME";
                    __instance.menu4_obj[3].GetComponent<Text>().text = "DELETE DATA";
                    if (PlayerPrefs.GetInt("PLUS") > 0)
                    {
                        __instance.menu1_obj[5].SetActive(value: true);
                        if (___rand_game == 1)
                        {
                            __instance.menu1_obj[5].GetComponent<Text>().text = "+" + PlayerPrefs.GetInt("PLUS") + " ???";
                        }
                        else
                        {
                            __instance.menu1_obj[5].GetComponent<Text>().text = "+" + PlayerPrefs.GetInt("PLUS");
                        }
                    }
                    else
                    {
                        __instance.menu1_obj[5].SetActive(value: false);
                    }
                    if (__instance.screen != 1 && __instance.screen != 4 && __instance.menu1_obj[3].GetComponent<Text>().text == "EXIT")
                    {
                        if (__instance.slot == 4)
                        {
                            __instance.slot = 1;
                        }
                        else
                        {
                            __instance.slot++;
                        }
                    }
                    else if (__instance.slot == 5)
                    {
                        __instance.slot = 1;
                    }
                    else
                    {
                        __instance.slot++;
                    }
                    var reflection = __instance.GetType().GetMethod("Select", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    reflection.Invoke(__instance, new object[] { });
                }
                if (controls.interactButtonDown() && ___delay < 0.1f)
                {
                    ___t = 2f;
                    var reflection = __instance.GetType().GetMethod("Hit", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    reflection.Invoke(__instance, new object[] { });
                }
                if (__instance.screen == 1)
                {
                    if (controls.leftJoyXAxis() < -0.3f)
                    {
                        __instance.menu1_obj[0].GetComponent<Text>().text = "NEW GAME";
                        __instance.menu4_obj[3].GetComponent<Text>().text = "DELETE DATA";
                        if (PlayerPrefs.GetInt("PLUS") > 0)
                        {
                            __instance.menu1_obj[5].SetActive(value: true);
                            if (___rand_game == 1)
                            {
                                __instance.menu1_obj[5].GetComponent<Text>().text = "+" + PlayerPrefs.GetInt("PLUS") + " ???";
                            }
                            else
                            {
                                __instance.menu1_obj[5].GetComponent<Text>().text = "+" + PlayerPrefs.GetInt("PLUS");
                            }
                        }
                        else
                        {
                            __instance.menu1_obj[5].SetActive(value: false);
                        }
                        if (__instance.slot == 1)
                        {
                            __instance.slider.GetComponent<UnityEngine.UI.Slider>().value -= 0.01f;
                            __instance.SetVOL(__instance.slider.GetComponent<UnityEngine.UI.Slider>().value);
                        }
                    }
                    if (controls.leftJoyXAxis() > 0.3f)
                    {
                        __instance.menu1_obj[0].GetComponent<Text>().text = "NEW GAME";
                        __instance.menu4_obj[3].GetComponent<Text>().text = "DELETE DATA";
                        if (PlayerPrefs.GetInt("PLUS") > 0)
                        {
                            __instance.menu1_obj[5].SetActive(value: true);
                            if (___rand_game == 1)
                            {
                                __instance.menu1_obj[5].GetComponent<Text>().text = "+" + PlayerPrefs.GetInt("PLUS") + " ???";
                            }
                            else
                            {
                                __instance.menu1_obj[5].GetComponent<Text>().text = "+" + PlayerPrefs.GetInt("PLUS");
                            }
                        }
                        else
                        {
                            __instance.menu1_obj[5].SetActive(value: false);
                        }
                        if (__instance.slot == 1)
                        {
                            __instance.slider.GetComponent<UnityEngine.UI.Slider>().value += 0.01f;
                            __instance.SetVOL(__instance.slider.GetComponent<UnityEngine.UI.Slider>().value);
                        }
                    }
                }
                if (__instance.screen == 4)
                {
                    if (controls.leftJoyXAxis() < -0.3f && __instance.slot == 3)
                    {
                        __instance.slider3.GetComponent<UnityEngine.UI.Slider>().value -= 0.1f;
                        __instance.SetFOV(Mathf.RoundToInt(__instance.slider3.GetComponent<UnityEngine.UI.Slider>().value));
                    }
                    if (controls.leftJoyXAxis() > 0.3f && __instance.slot == 3)
                    {
                        __instance.slider3.GetComponent<UnityEngine.UI.Slider>().value += 0.1f;
                        __instance.SetFOV(Mathf.RoundToInt(__instance.slider3.GetComponent<UnityEngine.UI.Slider>().value));
                    }
                }
            }
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(NEW_MENU_SCR), "Update")]
        private static void NewMenuControls(NEW_MENU_SCR __instance, ref float ___t)
        {
            if (___t <= 0f)
            {
                if (controls.leftJoyYAxis() > 0.3f)
                {
                    ___t = 3f;
                    __instance.vid_delay = __instance.vid;
                    var reflection = __instance.GetType().GetMethod("Select", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    reflection.Invoke(__instance, new object[] { -1 });
                }
                else if (controls.leftJoyYAxis() < -0.3f)
                {
                    ___t = 3f;
                    __instance.vid_delay = __instance.vid;
                    var reflection = __instance.GetType().GetMethod("Select", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    reflection.Invoke(__instance, new object[] { 1 });
                }
                else if (controls.interactButtonDown())
                {
                    ___t = 3f;
                    __instance.vid_delay = __instance.vid;
                    var reflection = __instance.GetType().GetMethod("Hit", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    reflection.Invoke(__instance, new object[] { });
                }
                else if (controls.leftJoyXAxis() > 0.3f)
                {
                    __instance.vid_delay = __instance.vid;
                    var reflection = __instance.GetType().GetMethod("Slide", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    reflection.Invoke(__instance, new object[] { 1f });
                }
                else if (controls.leftJoyXAxis() < -0.3f)
                {
                    __instance.vid_delay = __instance.vid;
                    var reflection = __instance.GetType().GetMethod("Slide", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    reflection.Invoke(__instance, new object[] { -1f });
                }
            }
            ___t -= 0.25f;
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(Skip_menu_scr), "FixedUpdate")]
        private static void SkipMenuControls(Skip_menu_scr __instance, ref float ___cool)
        {
            if (controls.interactButtonDown() && ___cool <= 0f)
            {
                ___cool = 0.5f;
                if (__instance.slot == 1)
                {
                    __instance.slot = 0;
                }
                else
                {
                    __instance.slot = 1;
                }
                __instance.menu_obj[0].SetActive(false);
                __instance.menu_obj[1].SetActive(false);
                __instance.menu_obj[__instance.slot].SetActive(true);
                __instance.GetComponent<AudioSource>().Play();
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(midnite_menu_scr), "FixedUpdate")]
        private static void MidniteMenuControls(midnite_menu_scr __instance, ref float ___t)
        {
            if (___t < 1f)
            {
                if (controls.leftJoyYAxis() > 0.3f)
                {
                    ___t = 2f;
                    if (__instance.slot == 0)
                    {
                        __instance.slot = 2;
                    }
                    else if (__instance.slot == 1)
                    {
                        __instance.slot = 0;
                    }
                    else if (__instance.slot == 2)
                    {
                        __instance.slot = 1;
                    }
                    var reflection = __instance.GetType().GetMethod("Select", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    reflection.Invoke(__instance, new object[] { });
                }
                else if (controls.leftJoyYAxis() < -0.3f)
                {
                    ___t = 2f;
                    if (__instance.slot == 0)
                    {
                        __instance.slot = 1;
                    }
                    else if (__instance.slot == 1)
                    {
                        __instance.slot = 2;
                    }
                    else if (__instance.slot == 2)
                    {
                        __instance.slot = 0;
                    }
                    var reflection = __instance.GetType().GetMethod("Select", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    reflection.Invoke(__instance, new object[] { });
                }
                if (controls.interactButtonDown())
                {
                    ___t = 2f;
                    var reflection = __instance.GetType().GetMethod("Hit", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    reflection.Invoke(__instance, new object[] { });
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Inv_main_scr), "Update")]
        private static void inventoryControls(Inv_main_scr __instance, ref float ___t, ref string ___item_name, ref int ___skin1, ref int ___skin2, ref int ___skin3, ref string ___item_des)
        {
            if (controls.pauseButtonDown() || controls.invButtonDown())
            {
                if (___t < 1f)
                {
                    __instance.Y_var = 0f;
                    __instance.slot_number = 0;
                    ___t = 6f;
                    ___item_name = __instance.Slots[__instance.slot_number].GetComponent<Inv_Item>().item;
                    ___item_des = __instance.Slots[__instance.slot_number].GetComponent<Inv_Item>().des;
                    __instance.item_text[0].GetComponent<Text>().text = ___item_name;
                    __instance.item_text[1].GetComponent<Text>().text = ___item_des;
                    __instance.INV_Menu[0].SetActive(false);
                    __instance.INV_Menu[1].SetActive(false);
                    __instance.Player_Control.SetActive(true);
                    Time.timeScale = 1f;
                    ___t = 6f;
                    __instance.Mixy[0].TransitionTo(0f);
                    __instance.Status.GetComponent<AudioSource>().clip = __instance.snds[2];
                    __instance.Status.GetComponent<AudioSource>().Play();

                }
            }

            if (controls.leftJoyXAxis() > 0.5f && __instance.SLOT_COOL <= 0f)
            {
                __instance.Y_var += 22.5f;
                if (__instance.Y_var > 340f)
                {
                    __instance.Y_var = 0f;
                }
                __instance.Snap();
            }
            if (controls.leftJoyXAxis() < -0.5f && __instance.SLOT_COOL <= 0f)
            {
                __instance.Y_var -= 22.5f;
                if (__instance.Y_var < 0f)
                {
                    __instance.Y_var += 360f;
                }
                __instance.Snap();
            }
            if (controls.leftJoyYAxis() > 0.5f && __instance.SLOT_COOLY <= 0f)
            {
                if (___item_name == "Sledgehammer" && PlayerPrefs.GetInt("PLUS") > 0)
                {
                    ___skin1 = PlayerPrefs.GetInt("SKIN1") + 1;
                    PlayerPrefs.SetInt("SKIN1", ___skin1);
                }
                else if (___item_name == "Shotgun" && PlayerPrefs.GetInt("PLUS") > 0)
                {
                    ___skin2 = PlayerPrefs.GetInt("SKIN2") + 1;
                    PlayerPrefs.SetInt("SKIN2", ___skin2);
                }
                else if (___item_name == "Old Kitchen Knife" && PlayerPrefs.GetInt("PLUS") > 0)
                {
                    ___skin3 = PlayerPrefs.GetInt("SKIN3") + 1;
                    PlayerPrefs.SetInt("SKIN3", ___skin3);
                }
                else
                {
                    return;
                }
                __instance.SLOT_COOLY = 1f;
                __instance.GetComponent<AudioSource>().PlayOneShot(__instance.snds[0]);
            }
            if (controls.leftJoyYAxis() < -0.5f && __instance.SLOT_COOLY <= 0f)
            {
                if (___item_name == "Sledgehammer" && PlayerPrefs.GetInt("PLUS") > 0)
                {
                    ___skin1 = PlayerPrefs.GetInt("SKIN1") - 1;
                    PlayerPrefs.SetInt("SKIN1", ___skin1);
                }
                else if (___item_name == "Shotgun" && PlayerPrefs.GetInt("PLUS") > 0)
                {
                    ___skin2 = PlayerPrefs.GetInt("SKIN2") - 1;
                    PlayerPrefs.SetInt("SKIN2", ___skin2);
                }
                else if (___item_name == "Old Kitchen Knife" && PlayerPrefs.GetInt("PLUS") > 0)
                {
                    ___skin3 = PlayerPrefs.GetInt("SKIN3") - 1;
                    PlayerPrefs.SetInt("SKIN3", ___skin3);
                }
                else
                {
                    return;
                }
                __instance.SLOT_COOLY = 1f;
                __instance.GetComponent<AudioSource>().PlayOneShot(__instance.snds[0]);
            }
        }
    }
}
