namespace F1DirtDrift
{
    /// <summary>
    /// Defines the different states of a race session
    /// </summary>
    public enum RaceState
    {
        None,           // No race active
        Countdown,      // Pre-race countdown (3-2-1-GO)
        Racing,         // Race in progress
        Finished,       // Race completed, showing results
        Paused          // Race paused
    }

    /// <summary>
    /// Defines car start positions in staggered time trial
    /// </summary>
    public enum StartPosition
    {
        First = 0,      // Player starts first
        Second = 1,     // Player starts when first car completes lap 1
        Third = 2       // Player starts when second car completes lap 1
    }

    /// <summary>
    /// Defines input control scheme being used
    /// </summary>
    public enum InputMode
    {
        KeyboardMouse,  // Desktop controls (WASD/Arrows)
        Touch,          // Mobile controls (buttons + steering wheel)
        Gamepad         // Controller support (future)
    }
}
