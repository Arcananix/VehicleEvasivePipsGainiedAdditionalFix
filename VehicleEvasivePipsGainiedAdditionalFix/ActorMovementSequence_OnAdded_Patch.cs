using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BattleTech;
using Harmony;
using UnityEngine;
using System.Reflection.Emit;


namespace VehicleEvasivePipsGainiedAdditionalFix
{
    [HarmonyPatch(typeof(ActorMovementSequence), "OnAdded", 0)]
    public static class ActorMovementSequence_OnAdded_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator iL)
        {
            var codes = new List<CodeInstruction>(instructions);
            int startIndex = -1;
            bool foundCheck = false;
            bool foundAcePilot = false;

            Label jumpToPoint = iL.DefineLabel();

            for (int i = 0; i < codes.Count; i++)
            {
                //look for the brfalse opcode to jump
                if(codes[i].opcode == OpCodes.Brfalse)
                {
                    startIndex = i;
                    foundCheck = true;            
                }

                if(codes[i].opcode == OpCodes.Ldstr && codes[i].operand.ToString() == "ACE PILOT")
                {
                    foundAcePilot = true;                   
                }

                //once ace pilot if found the next this reference ldarg.0 is our jump to point we need to label
                if(foundAcePilot && codes[i].opcode == OpCodes.Ldarg_0)
                {
                    codes[i].labels.Add(jumpToPoint);
                    break;
                }
            }

            if(foundCheck && foundAcePilot)
            {
                //insert the unconditional jump to bypass the code if this.owningActor.HasFiredThisRound is true since we are replacing that in the prefix
                codes.Insert(startIndex + 1, new CodeInstruction(OpCodes.Br, jumpToPoint));
            }
            return codes.AsEnumerable();
        }

        public static bool Prefix(ActorMovementSequence __instance)
        { 
            if (__instance.owningActor.HasFiredThisRound)
            {
                if (__instance.OwningVehicle != null)
                {
                    __instance.OwningVehicle.Combat.MessageCenter.PublishMessage(new FloatieMessage(__instance.owningActor.GUID, __instance.owningActor.GUID, "ACE PILOT", FloatieMessage.MessageNature.Buff));
                }
                else if(__instance.OwningMech != null)
                {
                    __instance.OwningMech.Combat.MessageCenter.PublishMessage(new FloatieMessage(__instance.owningActor.GUID, __instance.owningActor.GUID, "ACE PILOT", FloatieMessage.MessageNature.Buff));
                }
            }

            return true;
        }
    }
}
