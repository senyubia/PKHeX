using System;
using System.Diagnostics.CodeAnalysis;
using static PKHeX.Core.LearnMethod;
using static PKHeX.Core.LearnEnvironment;
using static PKHeX.Core.LearnSource5;

namespace PKHeX.Core;

/// <summary>
/// Exposes information about how moves are learned in <see cref="B2W2"/>.
/// </summary>
public sealed class LearnSource5B2W2 : ILearnSource, IEggSource
{
    public static readonly LearnSource5B2W2 Instance = new();
    private static readonly PersonalTable5B2W2 Personal = PersonalTable.B2W2;
    private static readonly Learnset[] Learnsets = Legal.LevelUpB2W2;
    private static readonly EggMoves6[] EggMoves = Legal.EggMovesBW; // same
    private const int MaxSpecies = Legal.MaxSpeciesID_5;
    private const LearnEnvironment Game = B2W2;

    public Learnset GetLearnset(ushort species, byte form) => Learnsets[Personal.GetFormIndex(species, form)];

    public bool TryGetPersonal(ushort species, byte form, [NotNullWhen(true)] out PersonalInfo? pi)
    {
        pi = null;
        if (species > MaxSpecies)
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
        if (types.HasFlagFast(MoveSourceType.LevelUp))
        {
            var learn = GetLearnset(evo.Species, evo.Form);
            var level = learn.GetLevelLearnMove(move);
            if (level != -1 && level <= evo.LevelMax)
                return new(LevelUp, Game, (byte)level);
        }

        if (types.HasFlagFast(MoveSourceType.Machine) && GetIsTM(pi, move))
            return new(TMHM, Game);

        if (types.HasFlagFast(MoveSourceType.TypeTutor) && GetIsTypeTutor(pi, move))
            return new(Tutor, Game);

        if (types.HasFlagFast(MoveSourceType.SpecialTutor) && GetIsSpecialTutor(pi, move))
            return new(Tutor, Game);

        if (types.HasFlagFast(MoveSourceType.EnhancedTutor) && GetIsEnhancedTutor(evo, pk, move, option))
            return new(Tutor, Game);

        return default;
    }

    private static bool GetIsSpecialTutor(PersonalInfo pi, ushort move)
    {
        var tutors = Tutors_B2W2;
        for (int i = 0; i < tutors.Length; i++)
        {
            var tutor = Array.IndexOf(tutors[i], move);
            if (tutor == -1)
                continue;
            if (pi.SpecialTutors[i][tutor])
                return true;
            break;
        }
        return false;
    }

    private static bool GetIsEnhancedTutor(EvoCriteria evo, ISpeciesForm current, ushort move, LearnOption option) => evo.Species switch
    {
        (int)Species.Keldeo => move is (int)Move.SecretSword,
        (int)Species.Meloetta => move is (int)Move.RelicSong,
        (int)Species.Rotom => move switch
        {
            (int)Move.Overheat  => option == LearnOption.AtAnyTime || current.Form == 1,
            (int)Move.HydroPump => option == LearnOption.AtAnyTime || current.Form == 2,
            (int)Move.Blizzard  => option == LearnOption.AtAnyTime || current.Form == 3,
            (int)Move.AirSlash  => option == LearnOption.AtAnyTime || current.Form == 4,
            (int)Move.LeafStorm => option == LearnOption.AtAnyTime || current.Form == 5,
            _ => false,
        },
        _ => false,
    };

    private static bool GetIsTypeTutor(PersonalInfo pi, ushort move)
    {
        var index = Array.IndexOf(TypeTutor567, move);
        if (index == -1)
            return false;
        return pi.TypeTutors[index];
    }

    private static bool GetIsTM(PersonalInfo info, ushort move)
    {
        var index = Array.IndexOf(TMHM_BW, move);
        if (index == -1)
            return false;
        return info.TMHM[index];
    }

    public void GetAllMoves(Span<bool> result, PKM pk, EvoCriteria evo, MoveSourceType types = MoveSourceType.All)
    {
        if (!TryGetPersonal(evo.Species, evo.Form, out var pi))
            return;

        if (types.HasFlagFast(MoveSourceType.LevelUp))
        {
            var learn = GetLearnset(evo.Species, evo.Form);
            (bool hasMoves, int start, int end) = learn.GetMoveRange(evo.LevelMax);
            if (hasMoves)
            {
                var moves = learn.Moves;
                for (int i = end; i >= start; i--)
                    result[moves[i]] = true;
            }
        }

        if (types.HasFlagFast(MoveSourceType.Machine))
        {
            var flags = pi.TMHM;
            var moves = TMHM_BW;
            for (int i = 0; i < moves.Length; i++)
            {
                if (flags[i])
                    result[moves[i]] = true;
            }
        }

        if (types.HasFlagFast(MoveSourceType.TypeTutor))
        {
            var flags = pi.TypeTutors;
            var moves = TypeTutor567;
            for (int i = 0; i < moves.Length; i++)
            {
                if (flags[i])
                    result[moves[i]] = true;
            }
        }

        if (types.HasFlagFast(MoveSourceType.SpecialTutor))
        {
            // B2W2 Tutors
            var tutors = Tutors_B2W2;
            for (int i = 0; i < tutors.Length; i++)
            {
                var flags = pi.SpecialTutors[i];
                var moves = tutors[i];
                for (int m = 0; m < moves.Length; m++)
                {
                    if (flags[m])
                        result[moves[m]] = true;
                }
            }
        }

        if (types.HasFlagFast(MoveSourceType.EnhancedTutor))
        {
            var species = evo.Species;
            if (species is (int)Species.Rotom && evo.Form is not 0)
                result[MoveTutor.GetRotomFormMove(evo.Form)] = true;
            else if (species is (int)Species.Keldeo)
                result[(int)Move.SecretSword] = true;
            else if (species is (int)Species.Meloetta)
                result[(int)Move.RelicSong] = true;
        }
    }

    internal static readonly ushort[][] Tutors_B2W2 =
    {
        new ushort[] { 450, 343, 162, 530, 324, 442, 402, 529, 340, 067, 441, 253, 009, 007, 008 },           // Driftveil City
        new ushort[] { 277, 335, 414, 492, 356, 393, 334, 387, 276, 527, 196, 401, 399, 428, 406, 304, 231 }, // Lentimas Town
        new ushort[] { 020, 173, 282, 235, 257, 272, 215, 366, 143, 220, 202, 409, 355 },                     // Humilau City
        new ushort[] { 380, 388, 180, 495, 270, 271, 478, 472, 283, 200, 278, 289, 446, 214, 285 },           // Nacrene City
    };
}
