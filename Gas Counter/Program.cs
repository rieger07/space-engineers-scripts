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
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRage;
using VRageMath;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        // This file contains your actual script.
        //
        // You can either keep all your code here, or you can create separate
        // code files to make your program easier to navigate while coding.
        //
        // In order to add a new utility class, right-click on your project, 
        // select 'New' then 'Add Item...'. Now find the 'Space Engineers'
        // category under 'Visual C# Items' on the left hand side, and select
        // 'Utility Class' in the main area. Name it in the box below, and
        // press OK. This utility class will be merged in with your code when
        // deploying your final script.
        //
        // You can also simply create a new utility class manually, you don't
        // have to use the template if you don't want to. Just do so the first
        // time to see what a utility class looks like.
        // 
        // Go to:
        // https://github.com/malware-dev/MDK-SE/wiki/Quick-Introduction-to-Space-Engineers-Ingame-Scripts
        //
        // to learn more about ingame scripts.
        IMyTextPanel displayPanel;
        MyIni ini = new MyIni();
        public Program()
        {
            // The constructor, called only once every session and
            // always before any other method is called. Use it to
            // initialize your script. 
            //     
            // The constructor is optional and can be removed if not
            // needed.
            // 
            // It's recommended to set Runtime.UpdateFrequency 
            // here, which will allow your script to run itself without a 
            // timer block.
            Runtime.UpdateFrequency = UpdateFrequency.Update100;
            MyIniParseResult result;
            if (!ini.TryParse(Me.CustomData, out result))
                throw new Exception(result.ToString());
            string displayName = ini.Get("config", "Display").ToString();
            displayPanel = GridTerminalSystem.GetBlockWithName(displayName) as IMyTextPanel;
            Utilities.setupDisplay(displayPanel);
            displayPanel.Alignment = TextAlignment.LEFT;
            displayPanel.FontSize = (float)1.0;
            displayPanel.Font = "Monospace";
        }

        string parseTanks(List<IMyGasTank> tanks, string title)
        {
            string rtn = "";
            float capacity = 0;
            double filled = 0;
            for (int i = 0; i < tanks.Count; i++)
            {

                IMyGasTank tank = tanks[i];
                capacity += tank.Capacity;
                filled += tank.FilledRatio*tank.Capacity;
            }
            string msg = "{0}: \n{3} {1:00.00}/{2:00.00} ML\n";
            string prog = Utilities.progressBar(filled, capacity);
            rtn += string.Format(msg, title, filled / 1e6, capacity / 1e6, prog);
            return rtn;
        }


        List<IMyGasTank> hydrotanks = new List<IMyGasTank>();
        List<IMyGasTank> oxytanks = new List<IMyGasTank>();
        public void Main(string argument, UpdateType updateSource)
        {
            // The main entry point of the script, invoked every time
            // one of the programmable block's Run actions are invoked,
            // or the script updates itself. The updateSource argument
            // describes where the update came from. Be aware that the
            // updateSource is a  bitfield  and might contain more than 
            // one update type.
            // 
            // The method itself is required, but the arguments above
            // can be removed if not needed.

            GridTerminalSystem.GetBlocksOfType<IMyGasTank>(hydrotanks, (IMyGasTank tank)=>tank.BlockDefinition.SubtypeId.Contains("Hydro"));
            GridTerminalSystem.GetBlocksOfType<IMyGasTank>(oxytanks, (IMyGasTank tank) => !tank.BlockDefinition.SubtypeId.Contains("Hydro"));

            string echo = "";
            echo += parseTanks(hydrotanks, "Hydrogen");
            echo += parseTanks(oxytanks, "Oxygen");
            displayPanel.WriteText(echo, false);
            
        }
    }
}
