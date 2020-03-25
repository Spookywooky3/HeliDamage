using System;
using System.Collections.Generic;
using System.Text;

namespace Oxide.Plugins
{
    [Info("PatrolControl", "Spooks © 2020", 1.0), Description("Controls the patrol.")]
    class PatrolControl : CovalencePlugin 
    {
        private PluginConfiguration config;

        class PluginConfiguration
        {
            public float bulletAccuracy { get; set; }
            public float patrolDamage { get; set; }
            public float patrolHealth { get; set; }
            public float rotorHealth { get; set; }
            public float tailRotorHealth { get; set; }
            public int bulletSpeed { get; set; }
            public int maxLifeTime { get; set; }
        }

        private PluginConfiguration GetDefaultConfig()
        {
            return new PluginConfiguration
            {
                bulletAccuracy = 1.1f,
                patrolDamage = 22.0f,
                patrolHealth = 10000.0f,
                rotorHealth = 1000.0f,
                tailRotorHealth = 1000.0f,
                bulletSpeed = 250,
                maxLifeTime = 15
            };
        }

        protected override void LoadDefaultConfig()
        {
            Config.WriteObject(GetDefaultConfig(), true);
        }

        private void Init()
        {
            config = Config.ReadObject<PluginConfiguration>();

            ConVar.PatrolHelicopter.bulletAccuracy = config.bulletAccuracy; /* BULLET ACCURACY */
            ConVar.PatrolHelicopter.lifetimeMinutes = config.maxLifeTime;   /* LIFE TIME (MINUTES) */
        }

        void OnEntitySpawned(BaseNetworkable entity)
        {
            if (entity.ShortPrefabName == "patrolhelicopter")
            {
                BaseHelicopter patrol = entity as BaseHelicopter;
                var weakSpots = patrol.weakspots;

                /* HEALTH */
                weakSpots[0].health = config.rotorHealth;
                weakSpots[1].health = config.tailRotorHealth;
                patrol.health = config.patrolHealth;
                /* BULLET VARIABLES */
                patrol.bulletDamage = config.patrolDamage;
                patrol.bulletSpeed = config.bulletSpeed;

                patrol.Update();
                patrol.SendNetworkUpdateImmediate();
            }
        }
    }
}
