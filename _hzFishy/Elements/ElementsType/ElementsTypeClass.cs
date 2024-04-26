public enum ElementsType : byte
{
    /// <summary> Player isnt overlapping any element </summary>
    None,
    /// <summary> on the floor </summary>
    FloorBox,
    /// <summary> in the air </summary>
    AirBox,
    /// <summary> middle air </summary>
    DownSlideBox,
    /// <summary> on the floor and air </summary>
    UpSlideBox,
    /// <summary> ladder/rope to climb up </summary>
    Ladder,
    /// <summary> zipline cable </summary>
    Zipline,
    RopeSwing,
    Panel,
    WallRun
}
