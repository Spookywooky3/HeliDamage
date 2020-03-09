using System;
using System.Collections.Generic;
using System.Linq;
using Oxide.Core;
using Oxide.Core.Libraries.Covalence;
using Oxide.Game.Rust.Cui;
using System.Reflection;
using Oxide.Core.Plugins;
using Oxide.Core.CSharp;
using UnityEngine.UI;
using System.Collections;
using Oxide.Game.Rust.Cui;
using Oxide.Core.Configuration;
using Oxide.Core.Libraries.Covalence;
using Rust;
using System.IO;
using UnityEngine;

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
                minicopterDamageMultiplier = 1.0f,
                scrapDamageMultiplier = 1.0f,
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

        [Command("helidamage.reload", "hd.reload")]
        void ConfigReload(IPlayer player, string command, string[] args)
        {
            if (player.HasPermission("helidamage.reload") || player.IsAdmin)
            {
                config = Config.ReadObject<PluginConfig>();
            }
            else
            {
                player.Reply($"You don't have the permission helidamage.reload");
            }
        }
    }
}
