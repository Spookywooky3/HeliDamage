﻿//  Copyright 2020 gitub.com/spookywooky3

//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System;
using Oxide.Core.Libraries.Covalence;
using Rust;

namespace Oxide.Plugins
{
    [Info("HeliDamage", "Spooks © 2020", 0.9), Description("Manages damage scaling for helicopters.")]
    class HeliDamage : CovalencePlugin
    {
        private PluginConfig config;
        const int damageTypeMax = (int)DamageType.LAST;

        /* CONFIGURATION CLASS */
        class PluginConfig
        {
            public float minicopterDamageMultiplier { get; set; }
            public float scrapDamageMultiplier { get; set; }
            public float decayMultiplier { get; set; }
            public bool  samDamage { get; set; }
            public bool  heliDecay { get; set; }
        }

        private PluginConfig GetDefaultConfig()
        {
            return new PluginConfig
            {
                minicopterDamageMultiplier = 0.0f,
                scrapDamageMultiplier = 0.0f,
                decayMultiplier = 1.0f,
                samDamage = true,
                heliDecay = true
            };
        }

        protected override void LoadDefaultConfig()
        {
            Config.WriteObject(GetDefaultConfig(), true);
        }

        private void Init()
        {
            config = Config.ReadObject<PluginConfig>();
            permission.RegisterPermission("helidamage.reload", this);
            Puts($"HeliDamage plugin loaded at {DateTime.Now}");
        }

        object OnEntityTakeDamage(BaseCombatEntity entity, HitInfo info)
        {
            if (entity == null)
                return null;

            if (info == null)
                return null;

            switch (entity.ShortPrefabName)
            {
                /* SCRAP HELI */
                case "scraptransporthelicopter":
                    if (!HitScale(info, config.scrapDamageMultiplier))
                        Puts($"Error setting hitscale for {entity.ShortPrefabName}");
                    break;

                /* MINICOPTER */
                case "minicopter.entity":
                    if (!HitScale(info, config.minicopterDamageMultiplier))
                        Puts($"Error setting hitscale for {entity.ShortPrefabName}");
                    break;

                /* ANYTHING ELSE */
                default:
                    // IN FUTURE IF NEEDED CAN ADD SHIT HERE
                    break;
            }
            return null;
        }

        private bool HitScale(HitInfo hitInfo, float multiplier)
        {
            try
            {
                for (var i = 0; i < damageTypeMax; i++)
                {
                    if ((DamageType)i == DamageType.Decay && config.heliDecay == true || (DamageType)i == DamageType.Generic)
                    {
                        hitInfo.damageTypes.Scale((DamageType)i, 1.0f);
                    }
                    else if (hitInfo?.WeaponPrefab?.name == "rocket_sam")
                    {
                        hitInfo.damageTypes.Scale((DamageType)i, 1.0f);
                    }
                    else
                    {
                        if (hitInfo == null)
                            continue;
                        hitInfo.damageTypes.Scale((DamageType)i, multiplier);
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Puts(e.Message + "\n" + e.StackTrace);
                return false;
            }
        }
    }
}
