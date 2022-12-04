using System;
using System.Collections.Generic;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core;

public static class EncounterTradeGenerator
{
    public static IEnumerable<EncounterTrade> GetPossible(PKM pk, EvoCriteria[] chain, GameVersion gameSource)
    {
        if (pk.Format <= 2 || pk.VC)
            return GetPossibleVC(chain, gameSource);
        return GetPossible(chain, gameSource);
    }

    private static IEnumerable<EncounterTradeGB> GetPossibleVC(EvoCriteria[] chain, GameVersion game)
    {
        var table = GetTableVC(game);
        foreach (var enc in table)
        {
            foreach (var evo in chain)
            {
                if (evo.Species != enc.Species)
                    continue;
                if (evo.Form != 0)
                    break;
                yield return enc;
                break;
            }
        }
    }

    private static IEnumerable<EncounterTrade> GetPossible(EvoCriteria[] chain, GameVersion game)
    {
        var table = GetTable(game);
        foreach (var enc in table)
        {
            foreach (var evo in chain)
            {
                if (evo.Species != enc.Species)
                    continue;
                yield return enc;
                break;
            }
        }
    }

    public static IEnumerable<EncounterTradeGB> GetValidEncounterTradesVC(PKM pk, EvoCriteria[] chain, GameVersion game)
    {
        var table = GetTableVC(game);
        foreach (var p in table)
        {
            foreach (var evo in chain)
            {
                if (evo.Species != p.Species || evo.Form != 0)
                    continue;
                if (p.IsMatchExact(pk, evo))
                    yield return p;
                break;
            }
        }
    }

    public static IEnumerable<EncounterTrade> GetValidEncounterTrades(PKM pk, EvoCriteria[] chain, GameVersion game)
    {
        // Pre-filter for some language scenarios
        int lang = pk.Language;
        if (lang == (int)LanguageID.UNUSED_6) // invalid language
            return Array.Empty<EncounterTrade>();
        if (lang == (int)LanguageID.Hacked && !EncounterTrade5PID.IsValidMissingLanguage(pk)) // Japanese trades in BW have no language ID
            return Array.Empty<EncounterTrade>();

        var table = GetTable(game);
        return GetValidEncounterTrades(pk, chain, table);
    }

    private static IEnumerable<EncounterTrade> GetValidEncounterTrades(PKM pk, EvoCriteria[] chain, EncounterTrade[] poss)
    {
        foreach (var p in poss)
        {
            foreach (var evo in chain)
            {
                if (evo.Species != p.Species)
                    continue;
                if (p.IsMatchExact(pk, evo))
                    yield return p;
                break;
            }
        }
    }

    private static IEnumerable<EncounterTradeGB> GetTableVC(GameVersion game)
    {
        if (RBY.Contains(game))
            return Encounters1.TradeGift_RBY;
        if (GSC.Contains(game))
            return Encounters2.TradeGift_GSC;
        return Array.Empty<EncounterTradeGB>();
    }

    private static EncounterTrade[] GetTable(GameVersion game) => game switch
    {
        R or S or E => Encounters3RSE.TradeGift_RSE,
        FR or LG => Encounters3FRLG.TradeGift_FRLG,
        D or P or Pt => Encounters4DPPt.TradeGift_DPPt,
        HG or SS => Encounters4HGSS.TradeGift_HGSS,
        B or W => Encounters5BW.TradeGift_BW,
        B2 or W2 => Encounters5B2W2.TradeGift_B2W2,
        X or Y => Encounters6XY.TradeGift_XY,
        AS or OR => Encounters6AO.TradeGift_AO,
        SN or MN => Encounters7SM.TradeGift_SM,
        US or UM => Encounters7USUM.TradeGift_USUM,
        GP or GE => Encounters7GG.TradeGift_GG,
        SW or SH => Encounters8.TradeGift_SWSH,
        BD or SP => Encounters8b.TradeGift_BDSP,
        SL or VL => Encounters9.TradeGift_SV,
        _ => Array.Empty<EncounterTrade>(),
    };
}
