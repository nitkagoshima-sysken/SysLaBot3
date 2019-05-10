using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysLaBot
{
    /// <summary>
    /// 繰り返しを定義します
    /// </summary>
    public enum Repetition
    {
        /// <summary>
        /// 毎年、同じ月日に繰り返します
        /// </summary>
        EveryYear,
        /// <summary>
        /// 毎月、同じ日に繰り返します
        /// </summary>
        EveryMonth,
        /// <summary>
        /// 毎週、同じ曜日に繰り返します
        /// </summary>
        EveryWeek,
        /// <summary>
        /// 毎日、同じ時刻に繰り返します
        /// </summary>
        EveryDay,
        /// <summary>
        /// 一度きりです、まるで人生のように
        /// </summary>
        OnceOnly,
    }
}
