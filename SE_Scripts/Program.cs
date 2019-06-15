using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
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
        MyIni ini = new MyIni();
        
        string outputName;
        IMyTextPanel outputPanel;
        List<MyIniKey> types = new List<MyIniKey>();
        Dictionary<string, Decimal> desired = new Dictionary<string, Decimal>();
        Dictionary<string, string> names = new Dictionary<string, string>();
        public Program()
        {
            // The constructor, called only once every session and
            // always before any other method is called. Use it to
            // initialize your script. 
            //     
            // The constructor is optional and can be removed if not
            // needed.
            // 
            // It's recommended to set RuntimeInfo.UpdateFrequency 
            // here, which will allow your script to run itself without a 
            // timer block.
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
            MyIniParseResult result;
            if (!ini.TryParse(Me.CustomData, out result))
                throw new Exception(result.ToString());
            outputName = ini.Get("config", "output").ToString();
            ini.GetKeys("names", types);
            foreach(MyIniKey key in types)
            {
                names[key.Name] = ini.Get("names", key.Name).ToString();
            }

            foreach (MyIniKey key in types)
            {
                desired[key.Name] = ini.Get("amounts", key.Name).ToDecimal();
            }
        }

        private string getPower()
        {
            
            float maxPower = 0;
            float storedPower = 0;
            GridTerminalSystem.GetBlocksOfType(batteries);
            for (int i = 0; i < batteries.Count; i++)
            {
                maxPower += batteries[i].MaxStoredPower;
                storedPower += batteries[i].CurrentStoredPower;
            }
            string prog = Utilities.progressBar(storedPower, maxPower);
            string msg = "{0} {1:000.00} MW/ {2:000.00}MW\n";
            
            return string.Format(msg, prog, storedPower, maxPower);
        }

        Dictionary<string, int> counts = new Dictionary<string, int>();
        private void getCounts(string itemstring)
        {
            int count = 0;
            List<IMyCargoContainer> containers = new List<IMyCargoContainer>();
            GridTerminalSystem.GetBlocksOfType(containers, (IMyCargoContainer block)=>block.HasInventory);
            //Echo("Found " + containers.Count.ToString() + " containers");
            for (int i = 0; i < containers.Count; i++)
            {
                IMyCargoContainer cont = containers[i];
                List<MyInventoryItem> items = new List<MyInventoryItem>();
                if (cont.HasInventory)
                {
                    IMyInventory cont_inv = cont.GetInventory() as IMyInventory;
                    cont_inv.GetItems(items, (MyInventoryItem item)=>item.Type.ToString()==itemstring);
                    //Echo("Found " + items.Count.ToString() + " items in " + cont.Name);
                    for (int j=0; j<items.Count; j++)
                    {
                        var item = items[j];
                        count += item.Amount.ToIntSafe();
                    }
                }
            }
            counts[itemstring] = count;
        }

        private string getValues()
        {
            string rtn = "";
            string msg = "{0} {1} {2:000000} kg/ {3:000000}kg\n";
            string prog;
            foreach (KeyValuePair<string, Decimal> dictionaryEntry in desired)
            {
                string fulltypename = dictionaryEntry.Key;
                Decimal desiredAmount = dictionaryEntry.Value;
                string displayname = names[fulltypename];

                getCounts(fulltypename);
                int actualCount = counts[fulltypename];
                prog = Utilities.progressBar((double)actualCount, (double)desiredAmount);
                rtn += string.Format(msg, prog, Utilities.formatString(displayname), actualCount, desiredAmount);
            }
            return rtn;
        }


        List<IMyBatteryBlock> batteries = new List<IMyBatteryBlock>();
        public void Main(string argument, UpdateType updateSource)
        {
            string textToOutput;
            //Get the display for the power readout
            textToOutput = getPower();
            //Get the iron count formatted for readout
            textToOutput += getValues();
            if (outputPanel == null)
            {
                outputPanel = GridTerminalSystem.GetBlockWithName(outputName) as IMyTextPanel;
            }
            if (outputPanel == null)
            {
                Echo("No output panel");
                return;
            }
            outputPanel.Font = "Monospace";
            outputPanel.WriteText(textToOutput, false);
        }
    }
}