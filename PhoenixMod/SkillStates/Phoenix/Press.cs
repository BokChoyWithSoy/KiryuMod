﻿using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using PhoenixWright.Modules.Survivors;

namespace PhoenixWright.SkillStates
{
    public class Press : BaseSkillState
    {
        public static float damageCoefficient = 6f;
        public static float procCoefficient = 1f;
        public static float duration = 3f;
        public Vector3 rayPosition;
        public static bool hasDamaged;


        private bool hasFired;
        private float stopwatch;
        private Animator animator;

        protected string hitboxName = "press";
        protected BlastAttack blastAttack;
        protected float attackStartTime = 0.01f * duration;
        protected float attackEndTime = 1f *duration;

        public override void OnEnter()
        {
            base.OnEnter();
            Ray aimRay = base.GetAimRay();
            this.hasFired = false;

            base.StartAimMode(duration, true);
            base.PlayAnimation("FullBody, Override", "Point", "ShootGun.playbackRate", (Press.duration  / Press.duration));

            rayPosition = aimRay.origin + 20 * aimRay.direction;

            EffectManager.SpawnEffect(Modules.Assets.pressEffect, new EffectData
            {
                origin = rayPosition,
                scale = 1f,
                rotation = Quaternion.LookRotation(aimRay.direction)

            }, false);

            blastAttack = new BlastAttack();
            blastAttack.radius = 10f;
            blastAttack.procCoefficient = 0.2f;
            blastAttack.position = rayPosition;
            blastAttack.attacker = base.gameObject;
            blastAttack.crit = Util.CheckRoll(base.characterBody.crit, base.characterBody.master);
            blastAttack.baseDamage = this.damageStat * damageCoefficient;
            blastAttack.falloffModel = BlastAttack.FalloffModel.None;
            blastAttack.teamIndex = TeamComponent.GetObjectTeam(blastAttack.attacker);
            blastAttack.damageType = DamageType.Stun1s;
            blastAttack.attackerFiltering = AttackerFiltering.Default;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            Ray aimRay = base.GetAimRay();
            rayPosition = aimRay.origin + 20 * aimRay.direction;

            stopwatch += Time.fixedDeltaTime;
            if (this.stopwatch >= attackStartTime && this.stopwatch <= attackEndTime )
            {
                FireAttack();
            }

            if(PhoenixPlugin.currentStacks >= PhoenixController.maxStacks)
            {
                base.skillLocator.primary.UnsetSkillOverride(base.skillLocator.primary, Phoenix.primaryBottle, GenericSkill.SkillOverridePriority.Contextual);
                base.skillLocator.primary.UnsetSkillOverride(base.skillLocator.primary, Phoenix.primaryKnife, GenericSkill.SkillOverridePriority.Contextual);
                base.skillLocator.primary.UnsetSkillOverride(base.skillLocator.primary, Phoenix.primaryPhone, GenericSkill.SkillOverridePriority.Contextual);
                base.skillLocator.primary.UnsetSkillOverride(base.skillLocator.primary, Phoenix.primaryServbot, GenericSkill.SkillOverridePriority.Contextual);
                base.skillLocator.primary.SetSkillOverride(base.skillLocator.primary, Phoenix.primaryArm, GenericSkill.SkillOverridePriority.Contextual);

                base.skillLocator.secondary.SetSkillOverride(base.skillLocator.secondary, Phoenix.secondaryPressStrong, GenericSkill.SkillOverridePriority.Contextual);
            }

        }

        public override void OnExit()
        {
            base.OnExit();
            base.characterMotor.disableAirControlUntilCollision = false;
        }

        private void FireAttack()
        {
            if (!this.hasFired)
            {
                this.hasFired = true;
                blastAttack.Fire();
                OnHitEnemyAuthority();
            }
        }

        protected virtual void OnHitEnemyAuthority()
        {
            if (PhoenixController.GetEvidenceType() && hasDamaged)
            {
                base.characterBody.AddBuff(Modules.Buffs.turnaboutBuff);
                PhoenixPlugin.currentStacks++;
                hasDamaged = false;
            }
        }


    }
}