using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime;

using HarmonyLib;
using UnclaimedWorldInjector;
using UWGame.ClientSide.Interface;
using UWGame.ClientSide;
using WindowSystem;
using GameStateManagement;
using Microsoft.Xna.Framework;
using UWGame.SimSide;
using UWGame.SimSide.Entities;
using Core;
using Kensei.Dev;
using Microsoft.Xna.Framework.Input;
using InputEventSystem;
using UWGame.ClientSide.Interface.HUD_Windows;
using UWGame;
using UWGame.SimSide.Items;

namespace UWorld_UnhiddenMod
{
    [HarmonyPatch]
    class MoreControls
    {
        public static bool ShadowsDisabled = false;

        [HarmonyPatch(typeof(Client), nameof(Client.HandleInput))]//nameof(MainMenuPanel.InitButtons)]
        [HarmonyPostfix]
        private static void ShadPostfix(ref Client __instance, ref InputData ___inputData)
        {
            if (__instance.EnableKeyboardShortcuts)
            {
                if (___inputData.IsKeyTapped(Keys.O))
                {
                    ShadowsDisabled = !ShadowsDisabled;
                }
            }
            stateOld = Mouse.GetState();
        }

        [HarmonyPatch(typeof(DateAndTime), nameof(DateAndTime.ComputeSunAndLight))]//nameof(MainMenuPanel.InitButtons)]
        [HarmonyPostfix]
        private static void SunPostfix(ref DateAndTime __instance)
        {
            if (ShadowsDisabled)
            {
                if (__instance.SunIsUp)
                {
                    __instance.SunIsUp = false;
                }
            }
        }

        //public static DataSheet datash;
        public static MouseState stateOld;
        public static MouseState stateNew;

        [HarmonyPatch(typeof(DataSheet), nameof(DataSheet.UpdateScrolling))]//nameof(MainMenuPanel.InitButtons)]
        [HarmonyPostfix]
        private static void ScrollPostfix(ref DataSheet __instance, ref Grid ___currentlyShownExpandedGrid, ref UIComponent ___expandedContentViewPort)//, ref GameTime elapsed)
        {
            //InputData inp = The.InGameUI.gui.InputData; //.MouseWheelMove;
            int diff = 0;

            if (stateOld != null) stateNew = Mouse.GetState();

            if (stateOld != null && stateNew != null && stateOld != stateNew)
            {
                diff = stateNew.ScrollWheelValue - stateOld.ScrollWheelValue;
                stateOld = stateNew;

                if (diff < 0)
                {
                    MoreControls.MScrollDownGrid(__instance, ___currentlyShownExpandedGrid, ___expandedContentViewPort);
                    return;
                }
                if (diff > 0)
                {
                    MoreControls.MScrollUpGrid(__instance, ___currentlyShownExpandedGrid);
                }
            }
        }

        static MethodInfo dynMethod, dynMethod2;

        //// some unsafe stuff instead of reflection. faster and can change readonly, but what reference it need to work?
        //public class Caller
        //{
        //    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "MethodName")]
        //    public static extern string GetMethod(MyClassName obj);
        //}
        //var method = Caller.GetMethod(this);

        static void MScrollDownGrid(DataSheet datash, Grid grid, UIComponent vPort)
        {
            //MethodInfo methodInfo = typeof(DataSheet).GetMethod("HideScrollDownArea", BindingFlags.NonPublic | BindingFlags.Instance);
            //var parameters = new object[] { map, extraCellValidator };
            //__result = (IntVec3)methodInfo.Invoke(null, parameters);
            dynMethod = typeof(DataSheet).GetMethod("HideScrollDownArea", BindingFlags.NonPublic | BindingFlags.Instance);
            dynMethod2 = typeof(DataSheet).GetMethod("ShowScrollUpArea", BindingFlags.NonPublic | BindingFlags.Instance);


            grid.Y -= 30;//(int)(elapsed.ElapsedGameTime.TotalSeconds * (double)DataSheet.scrollSpeedPerSecond);
            if (grid.Bottom <= vPort.Height)//; IsAtBottom(grid, vPort))
            {
                grid.Y = vPort.Height - grid.Height;
                dynMethod.Invoke(datash, new object[] { });//datash.HideScrollDownArea(); //private method call not work REFLECT
            }
            dynMethod2.Invoke(datash, new object[] { }); //datash.ShowScrollUpArea(); //private method call not work
        }

        static void MScrollUpGrid(DataSheet datash, Grid grid)
        {
            dynMethod = typeof(DataSheet).GetMethod("HideScrollUpArea", BindingFlags.NonPublic | BindingFlags.Instance);
            dynMethod2 = typeof(DataSheet).GetMethod("ShowScrollDownArea", BindingFlags.NonPublic | BindingFlags.Instance);
            //datash.GetType()
            grid.Y += 30;// (int)(elapsed.ElapsedGameTime.TotalSeconds * (double)DataSheet.scrollSpeedPerSecond);
            if (grid.Y >= 0)//IsAtTop(grid))
            {
                grid.Y = 0;
                dynMethod.Invoke(datash, new object[] { });//datash.HideScrollUpArea(); //private method call not work
            }
            dynMethod2.Invoke(datash, new object[] { }); //datash.ShowScrollDownArea(); //private method call not work
        }

        //static bool IsAtTop(Grid grid)
        //{
        //    return grid.Y >= 0;
        //}

        //// Token: 0x0600413A RID: 16698 RVA: 0x002B9B46 File Offset: 0x002B7D46
        //static bool IsAtBottom(Grid grid, UIComponent vport)
        //{
        //    return grid.Bottom <= vport.Height;
        //}
    }
}
