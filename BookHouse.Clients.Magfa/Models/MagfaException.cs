using System;
using System.Collections.Generic;
using System.Text;

namespace BookHouse.Clients.Magfa.Models
{
    /// <summary>
    /// خطاهای هنگام کار با 
    /// </summary>
    public class MagfaException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="code">کد خطای دریافت شده از مگفا</param>
        public MagfaException(int code) :
            base(Resources.MagfaStatus.ResourceManager.GetString(code.ToString()))
        {
            Code = code;
        }


        /// <summary>
        /// کد خطا
        /// </summary>
        public int Code { get; private set; }
    }
}
