using System;
using BattleTech;
using Harmony;
using UnityEngine;

namespace VehicleEvasivePipsGainiedAdditionalFix
{
    [HarmonyPatch(typeof(Vehicle), "GetEvasivePipsResult", 0)]
    public static class VehiclePatch
    {
        public static bool Prefix(float distMoved, bool isJump, bool isSprint, bool isMelee, Vehicle __instance, ref int __result, StatCollection ___statCollection)
        {
            int targetSpeedIndex = __instance.Combat.ToHit.GetTargetSpeedIndex(distMoved);
            int num = (targetSpeedIndex <= -1) ? 0 : __instance.Combat.Constants.ToHit.EvasivePipsMovingTarget[targetSpeedIndex];
            if (isJump)
            {
                num++;
            }

            if ((double)distMoved > 0.01)
            {
                num += ___statCollection.GetValue<int>("EvasivePipsGainedAdditional");
            }

            num = Mathf.Min(num, ___statCollection.GetValue<int>("MaxEvasivePips"));

            __result = (int)((float)num * __instance.Combat.Constants.ResolutionConstants.VehicleEvasiveResultMultiplier);

           
            return false;

        }
    }
}
