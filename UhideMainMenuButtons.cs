using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;
using UnclaimedWorldInjector;
using UWGame.ClientSide.Interface;
using WindowSystem;
using GameStateManagement;
using Microsoft.Xna.Framework;
using UWGame.SimSide.Items;
using UWGame.SimSide.Entities;
using Core;

//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Input;

namespace UncWorldMod_Unhidden
{
    [HarmonyPatch]
    class UhideMainMenuButtons
    {
        public static MainMenuPanel menu;

        [HarmonyPatch(typeof(MainMenuPanel), "InitButtons")]//nameof(MainMenuPanel.InitButtons)]
        [HarmonyPostfix]
        private static void MainMenuPostfix(ref MainMenuPanel __instance)
        {
            menu = __instance;
    //        MethodInfo dynMethod1 = __instance.GetType().GetMethod("btEditMap_Click",
    //BindingFlags.NonPublic | BindingFlags.Instance);
    //        dynMethod1.Invoke(__instance, new object[] { UIComponent sender, EventArgs e });

    //        MethodInfo dynMethod2 = __instance.GetType().GetMethod("btTestMap_Click",
    //BindingFlags.NonPublic | BindingFlags.Instance);

            TextButton textButton6 = new TextButton(__instance.Interface.gui);
            __instance.Window.Add(textButton6);
            textButton6.Init(TextButton.TextButtonType.White);
            textButton6.Position = new Microsoft.Xna.Framework.Point(142, 76);
            textButton6.Text = "EDIT";
            textButton6.Click += btEditMap_Click;
            textButton6.Width = 59;//106;
            TextButton textButton7 = new TextButton(__instance.Interface.gui);
            __instance.Window.Add(textButton7);
            textButton7.Init(TextButton.TextButtonType.White);
            textButton7.Position = new Microsoft.Xna.Framework.Point(198, 76);//(146, 45);
            textButton7.Text = "TEST";
            textButton7.Click += btTestMap_Click;
            textButton7.Width = 59;//106;
        }

        // Token: 0x06000D96 RID: 3478 RVA: 0x00009EED File Offset: 0x000080ED
        private static void btEditMap_Click(UIComponent sender, EventArgs e)
        {
            ((MainMenuInterface)menu.Interface).mainMenuScreen.EditMap();
        }

        private static void btTestMap_Click(UIComponent sender, EventArgs e)
        {
            ((MainMenuInterface)menu.Interface).mainMenuScreen.TestMap();
        }
    }
    
}
