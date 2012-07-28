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

        /// <summary>
        /// The RenderLayer and Hostility-based ColorScheme used to render the GameObject.
        /// </summary>
        public ColorScheme<Hostility> ColorScheme { get; protected set; }
        /// <summary>
        /// The GameEventManager to send events to.
        /// A singleton is not used, in case some groups of objects broadcast general
        /// messages that would be misinterpreted by supersets of their object group
        /// </summary>
        public GameEventManager GameEventManager { get; protected set; }
        /// <summary>
        /// The PhysicsComponent which controls the GameObject's motion
        /// </summary>
        public PhysicsComponent PhysicsComponent { get; protected set; }
        /// <summary>
        /// Any behaviors the GameObject has (not necessarily active or for which the GameObject meets all criteria)
        /// </summary>
        public IList<IBehavior> Behaviors { get; protected set; }
        /// <summary>
        /// Whether or not the GameObject is active.  Non-active objects are not necessarily dead, however)
        /// </summary>
        public bool Active { get; protected set; }
        /// <summary>
        /// The ratio of elapsed time for this unit compared to the time delta it is given.
        /// </summary>
        /// /// <example>
        /// For a unit with Timescale 2.0, things happen twice as fast - energy regen, attack rate, etc
        /// </example>
        public float Timescale { get; set; }
        /// <summary>
        /// The unit's health.  &lt;= 0 does not necessarily indicate dead or non-active
        /// </summary>
        public int Health { get; set; }

        #endregion

        #region Initialization

        /// <summary>
        /// Create a deactived GameObject with 0 health, and fire a GameObjectCreatedEvent
        /// </summary>
        /// <param name="manager">The manager which will fire the GameObjectCreatedEvent</param>
        public GameObject(GameEventManager manager) : this(manager, 0, false, true) { }
        /// <summary>
        /// Create a GameObject with a certain amount of health, possible active, and
        /// can fire a GameObjectCreatedEvent
        /// </summary>
        /// <param name="manager">The manager which will fire the GameObjectCreatedEvent</param>
        /// <param name="health"></param>
        /// <param name="active"></param>
        /// <param name="fireOnCreateEvent"></param>
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

        /// <summary>
        /// Copy another Gameobject.  Optionally fire a GameObjectCreatedEvent
        /// </summary>
        /// <param name="other"></param>
        /// <param name="fireOnCreateEvent"></param>
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

        /// <summary>
        /// Adds a behavior to the GameObject
        /// </summary>
        /// <param name="behavior"></param>
        public virtual void AddBehavior(IBehavior behavior)
        {
            Behaviors.Add(behavior);
        }

        /// <summary>
        /// Removes a behavior from the GameObject
        /// </summary>
        /// <param name="behavior"></param>
        public virtual void RemoveBehavior(IBehavior behavior)
        {
            Behaviors.Remove(behavior);
        }

        /// <summary>
        /// Update all behaviors the GameObject has,
        /// and applies those behaviors to this object
        /// if it meets the behavior's criteria
        /// </summary>
        /// <param name="dt"></param>
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

        /// <summary>
        /// Handle a GameEvent from a GameObject (can be self)
        /// </summary>
        /// <param name="src"></param>
        /// <param name="args"></param>
        public virtual void OnGameEvent(GameObject src, GameEventArgs args)
        {
        }

        #endregion

        /// <summary>
        /// Update any logic and behaviors
        /// </summary>
        /// <param name="dt"></param>
        public virtual void Update(float dt)
        {
            UpdateBehaviors(dt);
        }

        /// <summary>
        /// Draw a certain pass of the GameObject using a given SpriteBatch
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="pass"></param>
        public virtual void Draw(SpriteBatch batch, RenderPass pass) { }

        /// <summary>
        /// Touch the GameObject with another
        /// </summary>
        /// <param name="other"></param>
        /// <returns>True if the objects interact</returns>
        public virtual bool Touch(GameObject other)
        {
            return false;
        }

        /// <summary>
        /// Destroy the GameObject.  Can be used in cleanup and can fire GameObjectDestroyedEvents
        /// </summary>
        /// <param name="isCleanup">This object is being destroyed as part of a cleanup step</param>
        /// <param name="fireOnDestroyEvent">The GameObject should signal that is is being destroyed</param>
        public virtual void Destroy(bool isCleanup=true, bool fireOnDestroyEvent=false)
        {
            Active = false;
            if (fireOnDestroyEvent && GameEventManager != null)
                GameEventManager.AddEvent(new GameObjectDestroyedEvent(this));
        }
    }
}
