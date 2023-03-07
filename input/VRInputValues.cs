using Valve.VR;

namespace VivoVR.Input
{
    internal class VRInputValues
    {
        public bool interactButtonDown() {
            return SteamVR_Actions._default.Interact.GetStateDown(SteamVR_Input_Sources.Any);
        }
        public bool interactButtonHeldDown()
        {
            return SteamVR_Actions._default.Interact.GetState(SteamVR_Input_Sources.Any);
        }
        public bool interactButtonUp()
        {
            return SteamVR_Actions._default.Interact.GetStateUp(SteamVR_Input_Sources.Any);
        }

        public bool flashlightButtonDown()
        {
            return SteamVR_Actions._default.Flashlight.GetStateDown(SteamVR_Input_Sources.Any);
        }
        public bool mapButtonDown()
        {
            return SteamVR_Actions._default.Map.GetStateDown(SteamVR_Input_Sources.Any);
        }
        public bool sprintButtonHold()
        {
            return SteamVR_Actions._default.Sprint.GetState(SteamVR_Input_Sources.Any);
        }
        public bool reloadButtonDown()
        {
            return SteamVR_Actions._default.Reload.GetStateDown(SteamVR_Input_Sources.Any);
        }
        public bool invButtonDown()
        {
            return SteamVR_Actions._default.OpenInv.GetStateDown(SteamVR_Input_Sources.Any);
        }
        public bool pauseButtonDown()
        {
            return SteamVR_Actions._default.OpenMenu.GetStateDown(SteamVR_Input_Sources.Any);
        }
        public bool whistleButtonDown()
        {
            return SteamVR_Actions._default.Whistle.GetStateDown(SteamVR_Input_Sources.Any);
        }


        public float leftJoyXAxis() {
            return SteamVR_Actions._default.LeftThumbStick.GetAxis(SteamVR_Input_Sources.Any).x;
        }
        public float leftJoyYAxis()
        {
            return SteamVR_Actions._default.LeftThumbStick.GetAxis(SteamVR_Input_Sources.Any).y;
        }
        public float rightJoyXAxis()
        {
            return SteamVR_Actions._default.RightThumbStick.GetAxis(SteamVR_Input_Sources.Any).x;
        }
        public float rightJoyYAxis()
        {
            return SteamVR_Actions._default.RightThumbStick.GetAxis(SteamVR_Input_Sources.Any).y;
        }
        public bool rightTriggerDown()
        {
            return SteamVR_Actions._default.RightTrigger.GetStateDown(SteamVR_Input_Sources.Any);
        }
        public bool leftTriggerDown()
        {
            return SteamVR_Actions._default.RightTrigger.GetStateDown(SteamVR_Input_Sources.Any);
        }
    }
}
