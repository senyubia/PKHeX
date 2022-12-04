using System;

namespace PKHeX.Core;

/// <summary>
/// Exposes info about the Technical Machine / Technical Record (TR) compatibility and history of use.
/// </summary>
public interface ITechRecord
{
    /// <summary>
    /// Individual accessed indexes indicate if they can be learned.
    /// </summary>
    ReadOnlySpan<bool> TechRecordPermitFlags { get; }

    /// <summary>
    /// Individual accessed move IDs that a given TR teaches.
    /// </summary>
    ReadOnlySpan<ushort> TechRecordPermitIndexes { get; }

    /// <summary>
    /// Maximum count of record flags that are available.
    /// </summary>
    int RecordCountTotal { get; }

    /// <summary>
    /// Maximum count of record flags that are used.
    /// </summary>
    int RecordCountUsed { get; }

    /// <summary>
    /// Indicates if the TR has been previously used on this entity to learn the move.
    /// </summary>
    /// <param name="index">TR index</param>
    /// <returns>True if learned</returns>
    bool GetMoveRecordFlag(int index);

    /// <summary>
    /// Sets the flag indicating that the TR has been learned.
    /// </summary>
    /// <param name="index">TR index</param>
    /// <param name="value">True if learned</param>
    void SetMoveRecordFlag(int index, bool value = true);

    /// <summary>
    /// Indicates if any move has been learned via TR.
    /// </summary>
    bool GetMoveRecordFlagAny();
}
