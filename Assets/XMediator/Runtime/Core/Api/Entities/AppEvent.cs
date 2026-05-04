using JetBrains.Annotations;

namespace XMediator.Api
{
    /// <summary>
    /// Represents a user app event to be tracked for analytics, segmentation, and ad network targeting.
    /// </summary>
    /// <remarks>
    /// App events capture meaningful user actions, such as completing a level, making a purchase, or signing
    /// up. They are also forwarded to all registered mediation adapters that support it.
    /// <para>
    /// Use the provided factory methods to create instances:
    /// <list type="bullet">
    /// <item><description><see cref="Standard"/> for well-known events defined in <see cref="AppEventName"/>.</description></item>
    /// <item><description><see cref="Custom"/> for app-specific event names not covered by the standard catalog.</description></item>
    /// </list>
    /// </para>
    /// <para>Track events using <see cref="XMediator.Api.EventTracker.Track(AppEvent)"/>.</para>
    /// </remarks>
    public class AppEvent
    {
        internal string Name { get; }
        [CanBeNull] internal CustomProperties Properties { get; }

        private AppEvent(string name, [CanBeNull] CustomProperties properties)
        {
            Name = name;
            Properties = properties;
        }

        /// <summary>
        /// Creates a standard app event using a predefined name from <see cref="AppEventName"/>.
        /// </summary>
        /// <remarks>
        /// Use this factory for well-known event names defined in <see cref="AppEventName"/>
        /// (e.g., <see cref="AppEventName.LevelComplete"/>, <see cref="AppEventName.Purchase"/>).
        /// </remarks>
        /// <param name="name">A predefined event name from <see cref="AppEventName"/>.</param>
        /// <param name="properties">Optional key-value properties providing additional context. Defaults to <c>null</c>.</param>
        /// <returns>An <see cref="AppEvent"/> with the resolved name string and given properties.</returns>
        public static AppEvent Standard(AppEventName name, [CanBeNull] CustomProperties properties = null)
        {
            return new AppEvent(name.ToRawValue(), properties);
        }

        /// <summary>
        /// Creates a custom app event with a free-form name string.
        /// </summary>
        /// <remarks>
        /// Use this factory when your event is not covered by the predefined names in <see cref="AppEventName"/>.
        /// <para>
        /// As a safeguard against unexpectedly large payloads, event content may be truncated before
        /// being sent. This limit is not expected to affect typical usage — it only acts
        /// as a protection for edge cases.
        /// </para>
        /// </remarks>
        /// <param name="name">A string identifier for the event. Use descriptive, consistent naming (e.g., <c>"item_unlocked"</c>).</param>
        /// <param name="properties">Optional key-value properties providing additional context. Defaults to <c>null</c>.</param>
        /// <returns>An <see cref="AppEvent"/> with the given name and properties.</returns>
        public static AppEvent Custom(string name, [CanBeNull] CustomProperties properties = null)
        {
            return new AppEvent(name, properties);
        }

        public override string ToString()
        {
            return $"{nameof(Name)}: {Name}, {nameof(Properties)}: {Properties}";
        }
    }
}
