using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRage;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        public class Utilities
        {
            public static int Clamp(int value, int min, int max)
            {
                if (value < min)
                {
                    return min;
                }
                else if (value > max)
                {
                    return max;
                }
                else
                {
                    return value;
                }
            }

            public static string formatString(string s, int length = 15)
            {
                string rtn = s;
                if (s.Length < length)
                {
                    for (int i = 0; i < length - s.Length; i++)
                    {
                        rtn += " ";
                    }
                }
                return rtn;
            }

            public static string progressBar(double current, double max)
            {
                int barLength = 10;
                string rtn = "[";
                int numbars = (int)Math.Round(Math.Round(current / max, 2) * barLength, 0);
                numbars = Clamp(numbars, 0, barLength);
                for (int i = 0; i < numbars; i++)
                {
                    rtn += "|";
                }
                if (numbars < barLength)
                {
                    for (int j = 0; j < (barLength - numbars); j++)
                    {
                        rtn += ".";
                    }
                }
                rtn += "]";
                return rtn;
            }

            public static void setupDisplay(IMyTextPanel panel)
            {
                panel.ContentType = ContentType.TEXT_AND_IMAGE;
                panel.FontSize = 2;
                panel.FontColor = Color.Blue;
                panel.Alignment = TextAlignment.CENTER;
            }
        }
    }
}
