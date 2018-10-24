using System;
using System.Reflection;
using Harmony;

namespace VehicleEvasivePipsGainiedAdditionalFix
{
    public class Main
    {
        public static void Init(string _directory, string settingsJson)
        {
            HarmonyInstance.Create("Arcananix.VehicleEvasivePipsGainedAdditionalFix").PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
