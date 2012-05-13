using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Entities.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Engine.Entities.Behaviors;
using Engine.Events;
using Engine.Events.GameObjectEvents;
using Engine.Rendering;

namespace Engine.Entities
{
    /// <summary>
    /// An object that exists in the game.
    /// Consists of components which are managed by component systems.
    /// Most interaction should be handled by passing and receiving appropriate messages,
    ///     but can be handled directly using the Touch(GameObject other) method.
    /// </summary>
    public class GameObject
    {
        #region Fields

        public ColorScheme<Hostility> ColorScheme { get; protected set; }
        public GameEventManager GameEventManager { get; protected set; }
        public PhysicsComponent PhysicsComponent { get; protected set; }
        public List<IBehavior> Behaviors { get; protected set; }
        public bool Active { get; protected set; }
        public float Timescale { get; set; }
        public int Health { get; set; }

        #endregion

        #region Initialization

        public GameObject(GameEventManager manager) : this(manager, 0, false, true) { }
        public GameObject(GameEventManager manager, int health, bool active, bool fireOnCreateEvent)
        {
            ColorScheme = new ColorScheme<Hostility>();
            PhysicsComponent = new PhysicsComponent();
            Behaviors = new List<IBehavior>();
            Active = active;
            Timescale = 1;
            Health = health;
            if (fireOnCreateEvent && GameEventManager != null)
                GameEventManager.AddEvent(new GameObjectCreatedEvent(this));
        }

        public GameObject(GameObject other, bool fireOnCreateEvent=true)
        {
            ColorScheme = new ColorScheme<Hostility>(other.ColorScheme);
            GameEventManager = other.GameEventManager;
            PhysicsComponent = new PhysicsComponent(other.PhysicsComponent);
            Behaviors = new List<IBehavior>(other.Behaviors);

            Active = other.Active;
            Timescale = other.Timescale;
            Health = other.Health;
            
            if (fireOnCreateEvent && GameEventManager != null)
                GameEventManager.AddEvent(new GameObjectCreatedEvent(this));
        }

        #endregion

        #region Behaviors

        public virtual void AddBehavior(IBehavior behavior)
        {
            Behaviors.Add(behavior);
        }

        public virtual void RemoveBehavior(IBehavior behavior)
        {
            Behaviors.Remove(behavior);
        }

        protected virtual void UpdateBehaviors(float dt)
        {
            foreach (var behavior in Behaviors)
            {
                behavior.Update(dt);
                if (behavior.MeetsCriteria(this))
                    behavior.Apply(this);
            }
        }

        #endregion

        #region GameEvents

        public virtual void OnGameEvent(GameObject src, GameEventArgs args)
        {
        }

        #endregion

        public virtual void Update(float dt)
        {
            UpdateBehaviors(dt);
        }

        public virtual void Draw(SpriteBatch batch, RenderPass pass) { }

        public virtual bool Touch(GameObject other)
        {
            return false;
        }

        public virtual void Destroy(bool isCleanup=true, bool fireOnDestroyEvent=true)
        {
            Active = false;
            if (fireOnDestroyEvent && GameEventManager != null)
                GameEventManager.AddEvent(new GameObjectDestroyedEvent(this));
        }
    }
}
