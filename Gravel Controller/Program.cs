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
        MyIni ini = new MyIni();
        string gravel_name = "MyObjectBuilder_Ingot/Stone";
        int gravel_limit;
        string gravel_group_name;
        IMyBlockGroup gravel_shitter_group;
        List<IMyTerminalBlock> gravel_shitters = new List<IMyTerminalBlock>();
        IMyTextSurface panel;
        int amountBeforeDrop = 0;
        int amountAfterDrop = 0;
        bool justDropped = false;
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
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
            MyIniParseResult result;
            if (!ini.TryParse(Me.CustomData, out result))
                throw new Exception(result.ToString());
            string displayName = ini.Get("config", "DisplayName").ToString();
            gravel_group_name = ini.Get("config", "GroupName").ToString();
            gravel_limit = ini.Get("config", "GravelLimit").ToInt32();

            panel = GridTerminalSystem.GetBlockWithName(displayName) as IMyTextPanel;
            Utilities.setupDisplay(panel as IMyTextPanel);
            panel.WriteText("FUCKING SHITTY LCD", false);
            Echo(gravel_group_name);
            gravel_shitter_group = GridTerminalSystem.GetBlockGroupWithName(gravel_group_name);
            gravel_shitter_group.GetBlocks(gravel_shitters);
            
        }

        Dictionary<string, int> counts = new Dictionary<string, int>();
        private void getCounts(string itemstring)
        {
            int count = 0;
            List<IMyTerminalBlock> containers = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType(containers, (IMyTerminalBlock block) => block.HasInventory);
            //Echo("Found " + containers.Count.ToString() + " containers");
            for (int i = 0; i < containers.Count; i++)
            {
                IMyTerminalBlock cont = containers[i];
                List<MyInventoryItem> items = new List<MyInventoryItem>();
                IMyInventory cont_inv = cont.GetInventory() as IMyInventory;
                if (cont_inv.ItemCount == 0)
                {
                    continue;
                }

                cont_inv.GetItems(items, (MyInventoryItem item) => item.Type.ToString() == itemstring);
                //Echo("Found " + items.Count.ToString() + " items in " + cont.Name);
                for (int j = 0; j < items.Count; j++)
                {
                    var item = items[j];
                    count += item.Amount.ToIntSafe();
                }
                
            }
            counts[itemstring] = count;
        }
        
        void turnOffShitters()
        {
            for(int i=0; i<gravel_shitters.Count; i++)
            {
                IMyShipConnector temp = gravel_shitters[i] as IMyShipConnector;
                if (temp != null)
                {
                    temp.Enabled = false;
                }
            }
        }

        void turnOnShitters()
        {
            for (int i = 0; i < gravel_shitters.Count; i++)
            {
                IMyShipConnector temp = gravel_shitters[i] as IMyShipConnector;
                if(temp != null)
                {
                    temp.Enabled = true;
                }
                
            }
        }

        string getRidOfStupidGravel()
        {
            string rtn = "";
            if (counts[gravel_name] > gravel_limit)
            {
                amountBeforeDrop = counts[gravel_name];
                justDropped = true;
                turnOnShitters();
                rtn += "Enabled";
            }
            else
            {
                if (justDropped)
                {
                    amountAfterDrop = counts[gravel_name];
                    justDropped = false;
                    turnOffShitters();
                }
                rtn += "Disabled";
            }
            return rtn;
        }

        public void Main(string argument, UpdateType updateSource)
        {

            getCounts(gravel_name);

            string message = 
                "Runtime: {0:0.00}ms\n" +
                "Gravel: {1:0000000}\n" +
                "Limit: {2:0000000}\n" +
                "Shitters: {3}\n" +
                "Dropped: {4:0000000}\n";
            string enabled = getRidOfStupidGravel();
            string msg = string.Format(message, Runtime.LastRunTimeMs, counts[gravel_name], gravel_limit, enabled, amountBeforeDrop-amountAfterDrop);
            panel.WriteText(msg, false);

            
        }
    }
}
