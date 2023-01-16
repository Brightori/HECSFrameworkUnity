﻿using System;
using System.Linq;
using HECSFramework.Core;
using Sirenix.OdinInspector;
using Systems;
using UnityEngine;

namespace HECSFramework.Unity
{
    [DefaultExecutionOrder(-100)]
    public partial class Actor : MonoBehaviour, IActor
    {
        [SerializeField, BoxGroup("Init")] private ActorInitModule actorInitModule = new ActorInitModule();
        [SerializeField, BoxGroup("Init")] private ActorContainer actorContainer;

        private Entity entity;
        public GameObject GameObject => gameObject;

        public string ContainerID => entity.ContainerID;
        public ActorContainer ActorContainer => actorContainer;

        public IEntity Entity => entity;

        public void Command<T>(T command) where T : struct, ICommand => entity.Command(command);

        private void Awake()
        {
            entity = new Entity(EntityManager.Worlds.Data[actorInitModule.WorldIndex], gameObject.name);
            entity.SetGuid(actorInitModule.Guid);

            if (actorInitModule.InitActorMode == InitActorMode.InitOnStart)
            {
                if (actorContainer != null && !entity.IsInited)
                    actorContainer.Init(this.entity);
            }
        }

        public void InitWithContainer()
        {
            Awake();
            Init();
        }

        public void Init(World world = null)
        {
            entity.Init(world);
        }

        public void InitWithContainer(ActorContainer entityContainer)
        {
            actorContainer = entityContainer;
            InitWithContainer();
        }

        protected virtual void Start()
        {
            if (actorInitModule.InitActorMode == InitActorMode.InitOnStart)
                Init();
        }

        public void Dispose()
        {
            entity.HecsDestroy();
        }

        public bool Equals(IEntity other) => entity.Equals(other);

        public void GenerateGuid()
        {
            entity.GenerateGuid();
            actorInitModule.SetGuid(entity.GUID);
        }

        public void HecsDestroy()
        {
            entity.World.GetSingleSystem<DestroyEntityWorldSystem>().ProcessDeathOfActor(this);
        }

        private void OnDestroy()
        {
            if (entity.IsAlive)
                entity.HecsDestroy();
        }

        public bool TryGetComponent<T>(out T component, bool lookInChildsToo = false)
        {
            if (!entity.IsAlive())
            {
                component = default;
                return false;
            }

            if (lookInChildsToo)
            {
                component = GetComponentsInChildren<T>(true).FirstOrDefault();
                return component != null;
            }

            component = GetComponent<T>();
            return component != null && component.ToString() != "null";
        }

        public bool TryGetComponents<T>(out T[] components)
        {
            components = GetComponentsInChildren<T>(true);
            return components != null && components.Length > 0;
        }

       
        public bool ContainsMask<T>() where T : IComponent => entity.ContainsMask<T>();

        public override int GetHashCode()
        {
            return entity != null ? entity.GetHashCode() : gameObject.GetHashCode();
        }

        public override bool Equals(object other)
        {
            return entity.Equals(other);
        }

        public void InjectContainer(EntityContainer container, bool isAdditive = false)
        {
            var components = container.GetComponentsInstances();
            var systems = container.GetSystemsInstances();

            entity.Inject(components, systems, isAdditive);
        }

        public void InjectContainer(EntityContainer container, bool isAdditive = false, params IComponent[] additionalComponents)
        {
            var components = container.GetComponentsInstances();
            var systems = container.GetSystemsInstances();

            foreach (var additional in additionalComponents)
                components.Add(additional);

            entity.Inject(components, systems, isAdditive);
        }

        protected virtual void Reset()
        {
            actorInitModule.SetID(gameObject.name);
            actorInitModule.SetGuid(Guid.NewGuid());
        }
    }
}