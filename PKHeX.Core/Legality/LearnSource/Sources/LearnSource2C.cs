using System;
using System.Diagnostics.CodeAnalysis;
using static PKHeX.Core.LearnMethod;
using static PKHeX.Core.LearnEnvironment;
using static PKHeX.Core.LearnSource2;

namespace PKHeX.Core;

/// <summary>
/// Exposes information about how moves are learned in <see cref="C"/>.
/// </summary>
public sealed class LearnSource2C : ILearnSource, IEggSource
{
    public static readonly LearnSource2C Instance = new();
    private static readonly PersonalTable2 Personal = PersonalTable.C;
    private static readonly EggMoves2[] EggMoves = Legal.EggMovesC;
    private static readonly Learnset[] Learnsets = Legal.LevelUpC;
    private const int MaxSpecies = Legal.MaxSpeciesID_2;
    private const LearnEnvironment Game = C;
    private const int CountTMHM = 57;

    public Learnset GetLearnset(ushort species, byte form) => Learnsets[species];

    public bool TryGetPersonal(ushort species, byte form, [NotNullWhen(true)] out PersonalInfo? pi)
    {
        pi = null;
        if (species > Legal.MaxSpeciesID_2)
            return false;
        pi = Personal[species];
        return true;
    }

    public bool GetIsEggMove(ushort species, byte form, ushort move)
    {
        if (species > MaxSpecies)
            return false;
        var moves = EggMoves[species];
        return moves.GetHasEggMove(move);
    }

    public ReadOnlySpan<ushort> GetEggMoves(ushort species, byte form)
    {
        if (species > MaxSpecies)
            return ReadOnlySpan<ushort>.Empty;
        return EggMoves[species].Moves;
    }

    public MoveLearnInfo GetCanLearn(PKM pk, PersonalInfo pi, EvoCriteria evo, ushort move, MoveSourceType types = MoveSourceType.All, LearnOption option = LearnOption.Current)
    {
        if (types.HasFlagFast(MoveSourceType.Machine) && GetIsTM(pi, move))
            return new(TMHM, Game);

        if (types.HasFlagFast(MoveSourceType.SpecialTutor) && GetIsSpecialTutor(pk, evo.Species, move))
            return new(Tutor, Game);

        if (types.HasFlagFast(MoveSourceType.LevelUp))
        {
            var learn = GetLearnset(evo.Species, evo.Form);
            var level = learn.GetLevelLearnMove(move);
            if (level != -1 && evo.LevelMin <= level && level <= evo.LevelMax)
                return new(LevelUp, Game, (byte)level);
        }

        return default;
    }

    private static bool GetIsSpecialTutor(PKM pk, ushort species, ushort move)
    {
        if (!ParseSettings.AllowGen2Crystal(pk))
            return false;
        var tutor = Array.IndexOf(Tutors_GSC, move);
        if (tutor == -1)
            return false;
        var info = PersonalTable.C[species];
        return info.TMHM[CountTMHM + tutor];
    }

    private static bool GetIsTM(PersonalInfo info, ushort move)
    {
        var index = Array.IndexOf(TMHM_GSC, move);
        if (index == -1)
            return false;
        return info.TMHM[index];
    }

    public void GetAllMoves(Span<bool> result, PKM pk, EvoCriteria evo, MoveSourceType types = MoveSourceType.All)
    {
        if (!TryGetPersonal(evo.Species, evo.Form, out var pi))
            return;

        bool removeVC = pk.Format == 1 || pk.VC1;
        if (types.HasFlagFast(MoveSourceType.LevelUp))
        {
            var learn = GetLearnset(evo.Species, evo.Form);
            var min = ParseSettings.AllowGen2MoveReminder(pk) ? 1 : evo.LevelMin;
            (bool hasMoves, int start, int end) = learn.GetMoveRange(evo.LevelMax, min);
            if (hasMoves)
            {
                var moves = learn.Moves;
                for (int i = end; i >= start; i--)
                {
                    var move = moves[i];
                    if (!removeVC || move <= Legal.MaxMoveID_1)
                        result[move] = true;
                }
            }
        }

        if (types.HasFlagFast(MoveSourceType.Machine))
        {
            var flags = pi.TMHM;
            var moves = TMHM_GSC;
            for (int i = 0; i < moves.Length; i++)
            {
                if (flags[i])
                {
                    var move = moves[i];
                    if (!removeVC || move <= Legal.MaxMoveID_1)
                        result[move] = true;
                }
            }
        }

        if (types.HasFlagFast(MoveSourceType.SpecialTutor))
        {
            var flags = pi.TMHM;
            for (int i = CountTMHM; i < flags.Length; i++)
            {
                if (flags[i])
                    result[Tutors_GSC[i - CountTMHM]] = true;
            }
        }
    }

    public static void GetEncounterMoves(IEncounterTemplate enc, Span<ushort> init)
    {
        var species = enc.Species;
        var learn = Learnsets[species];
        learn.SetEncounterMoves(enc.LevelMin, init);
    }
}
