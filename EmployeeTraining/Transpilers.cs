using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace EmployeeTraining
{
    [HarmonyPatch(typeof(SaveManager))]
    [HarmonyPatch(nameof(SaveManager.Save))]
    public class SaveManager_Save_Patcher
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            // Plugin.LogDebug("Applying SaveManager_Save_Patcher");
            var code = new List<CodeInstruction>(instructions);

            int insertionIndex = -1;
            for (int i = 0; i < code.Count - 1; i++)
            {
                /* Search following code:
                 " <--- INSERT HERE --->
                 *  ldloc.0 |
                 *     call | Void StoreCachedFile(System.String)
                 */ 
                if (code[i].opcode == OpCodes.Ldloc_0 && code[i + 1].opcode == OpCodes.Call)
                {
                    var method = (MethodInfo)code[i + 1].operand;
                    if (method.DeclaringType == typeof(ES3) && method.Name == nameof(ES3.StoreCachedFile))
                    {
                        insertionIndex = i;
                        break;
                    }
                }
            }

            var instructionsToInsert = new List<CodeInstruction>
                {
                    new CodeInstruction(OpCodes.Ldloc_0), // filePath
                    // new CodeInstruction(OpCodes.Ldarg_0),
                    // new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(SaveManager), "m_ES3Settings")),
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(
                            typeof(ETSaveManager), nameof(ETSaveManager.Save),
                            new Type[]{typeof(string)/*filePath*/})
                    ),
                };

            if (insertionIndex != -1)
            {
                code.InsertRange(insertionIndex, instructionsToInsert);
            }
            // for (int i = 0; i < code.Count; i++)
            // {
            //     Plugin.LogDebug($"-- {i,3} | {code[i].opcode,8} | {code[i].operand}");
            // }
            return code;
        }
    }

/*
    [HarmonyPatch(typeof(Restocker), "PlaceProducts", MethodType.Enumerator)]
    [HarmonyDebug]
    public class Restocker_PlaceProducts_Patcher
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            Plugin.LogDebug("Applying Restocker_PlaceProducts_Patcher");
            var code = new List<CodeInstruction>(instructions);

            var restockerLocal = il.DeclareLocal(typeof(Restocker));
            code.Insert(0, new CodeInstruction(OpCodes.Ldarg_0)); // this (Restockerインスタンス) をロード
            code.Insert(1, new CodeInstruction(OpCodes.Stloc_3, restockerLocal)); // ローカル変数に保存


            int insertionIndex = -1;
            for (int i = 1; i < code.Count; i++)
            {
                // Plugin.LogDebug($"-- {i:###} | {code[i].opcode,8} | {code[i].operand}");
                if (code[i].opcode == OpCodes.Ldc_R4 && (float)code[i].operand == 0.2f
                        && code[i + 1].opcode == OpCodes.Newobj && code[i + 1].operand.ToString().IndexOf("Void .ctor(Single)") > -1)
                {
                    // Plugin.LogDebug($"----> operand: {code[i].operand.GetType()}");
                    insertionIndex = i;
                    break;
                }
            }

            var nestedType = typeof(Restocker).GetNestedType("<PlaceProducts>d__40", BindingFlags.NonPublic);
            var targetField = nestedType.GetField("<>2__current", BindingFlags.NonPublic | BindingFlags.Instance);
            var newLocal = il.DeclareLocal(typeof(UnityEngine.WaitForSeconds));

            var instructionsToInsert = new List<CodeInstruction>
                {
                    new CodeInstruction(OpCodes.Nop),
                    new CodeInstruction(OpCodes.Ldloc_3, restockerLocal),
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(
                            typeof(RestockerSkill), nameof(RestockerSkill.GetIntervalOnPlacingProducts),
                            new Type[]{typeof(Restocker)})),
                    new CodeInstruction(OpCodes.Stloc_S, newLocal.LocalIndex),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldloc_S, newLocal.LocalIndex),
                    new CodeInstruction(OpCodes.Stfld, targetField)
                };

            if (insertionIndex != -1)
            {
                code.RemoveRange(insertionIndex - 1, 4);
                code.InsertRange(insertionIndex - 1, instructionsToInsert);
            }
            return code;
        }
    }
*/

}