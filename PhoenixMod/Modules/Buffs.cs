﻿using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace PhoenixWright.Modules
{
    public static class Buffs
    {
        // armor buff gained during roll
        internal static BuffDef armorBuff;
        internal static BuffDef turnaboutBuff;

        internal static List<BuffDef> buffDefs = new List<BuffDef>();

        internal static void RegisterBuffs()
        {
            armorBuff = AddNewBuff("Armor Buff", Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texArmourBuff"), Color.white, false, false);
            turnaboutBuff = AddNewBuff("Turnabout", Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texBuffIconNoBackground"), Color.white, true, false);
        }

        // simple helper method
        internal static BuffDef AddNewBuff(string buffName, Sprite buffIcon, Color buffColor, bool canStack, bool isDebuff)
        {
            BuffDef buffDef = ScriptableObject.CreateInstance<BuffDef>();
            buffDef.name = buffName;
            buffDef.buffColor = buffColor;
            buffDef.canStack = canStack;
            buffDef.isDebuff = isDebuff;
            buffDef.eliteDef = null;
            buffDef.iconSprite = buffIcon;

            buffDefs.Add(buffDef);

            return buffDef;
        }
    }
}