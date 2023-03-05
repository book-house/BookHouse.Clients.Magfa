using BookHouse.Clients.Magfa.Models;
using BookHouse.Clients.Magfa.Resources;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serializers.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace BookHouse.Clients.Magfa
{
    /// <summary>
    /// سرويس ‍HTTP v2 بهينه‌سازی‌شده‌ی سرويس HTTP v1 با متدهای REST و به صورت امن HTTPS، 
    /// طراحی شده و به راحتی در همه‌ی زبان‌های برنامه‌نويسی قابل استفاده است. به دلیل سربار کم این سرويس،
    /// پهنای باند مصرفی در آن بسیار اندک خواهد بود. شایان ذکر است در این سرويس نیز تنها از UTF-8 پشتيبانی می‌شود.
    /// افزون بر امکانات نسخه‌ی v1 امکان در‌یافت پيامک‌های ورودی نيز در اين سرويس فراهم شده است.
    /// همانند نسخه v1، در اين سرويس نيز سامانه به ازای هر پيامک ارسالی، شناسه‌ای یکتا جهت پيگیری وضعيت ارایه می‌نمايد.همچنین برای افزايش سرعت ارسال می‌توانيد از فراخوانی‌های همزمان استفاده نماييد.
    /// </summary>
    /// <seealso href="https://messaging.magfa.com/ui/?public/wiki/api/http_v2">آدرس داکیومنت مگفا</seealso>
    public class MagfaV2Client : IMagfaV2Client
    {
        private readonly MagfaClientOptions _options;
        private readonly RestClient _restClient;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public MagfaV2Client(MagfaClientOptions options)
        {

            if (options == null) throw new ArgumentNullException(nameof(options));
            if (options.Username == null) throw new ArgumentNullException(nameof(options.Username));
            if (options.Password == null) throw new ArgumentNullException(nameof(options.Password));

            _options = options;

            _restClient = new RestClient("https://sms.magfa.com/api/http/sms/v2")
            {
                Authenticator = new HttpBasicAuthenticator($"{options.Username}/{options.Domain}", options.Password),
            };

            _restClient.UseSystemTextJson(new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });

            _restClient.AddDefaultHeader("cache-control", "no-cache");
            _restClient.AddDefaultHeader("accept", "application/json");
        }


        /// <summary>
        /// دريافت مانده اعتبار حساب
        /// </summary>
        /// <seealso href="https://messaging.magfa.com/ui/?public/wiki/api/http_v2#balance">
        /// لینک مستندات
        /// </seealso>
        /// <returns>میزان موجودی باقی مانده</returns>
        /// <exception cref="Exception">خطا در</exception>
        /// <exception cref="MagfaException">حاوی پیام خطا</exception>
        public virtual async Task<long> BalanceAsync()
        {
            var request = new RestRequest("balance", Method.Get);

            var result = await _restClient.ExecuteAsync<MagfaBalanceResult>(request);

            if (result.IsSuccessful)
            {
                if (result.Data.Status != 0)
                {
                    throw new MagfaException(result.Data.Status);
                }

                return result.Data.Balance;
            }
            else
            {
                throw result.ErrorException ?? new Exception(result.ErrorMessage);
            }
        }


        /// <summary>
        /// ارسال پيامک به يک گيرنده
        /// </summary>
        /// <seealso href="https://messaging.magfa.com/ui/?public/wiki/api/http_v2#send">
        /// لینک مستندات
        /// </seealso>
        /// <param name="message">متن پیام</param>
        /// <param name="recipients">شماره گیرنده</param>
        /// <returns><seealso cref="MagfaMessage"/></returns>
        public virtual async Task<MagfaMessage> SendAsync(string message, string recipients)
        {
            var res = await SendAsync(new[] { message }, new[] { recipients });
            return res.First();
        }

        /// <summary>
        /// ارسال پيامک به يک یا چند گيرنده (حداکثر يک‌صد (۱۰۰) عدد)
        /// </summary>
        /// <seealso href="https://messaging.magfa.com/ui/?public/wiki/api/http_v2#send">
        /// لینک مستندات
        /// </seealso>
        /// <param name="messages">آرايه‌ای از پيام</param>
        /// <param name="recipients">آرايه‌ای از گيرنده</param>
        /// <param name="uids">آرايه‌ای از شناسه‌ی یکتای کاربر</param>
        /// <param name="senders">آرايه‌ای از فرستنده</param>
        /// <returns>آرایه ای از <seealso cref="MagfaMessage"/></returns>
        /// <exception cref="Exception">خطای ارتباط با سرویس</exception>
        /// <exception cref="MagfaException">در صورت وجود خطا متن آن در Message خواهد داشت</exception>
        public virtual async Task<MagfaMessage[]> SendAsync(string[] messages, string[] recipients, long[] uids = null, string[] senders = null)
        {
            if (senders == null)
            {
                senders = _options.SenderNumber;
            }

            var request = new RestRequest("send", Method.Post)
            {
                RequestFormat = DataFormat.Json
            };

            request.AddBody(new
            {
                senders,
                recipients,
                messages,
                uids
            });

            var result = await _restClient.ExecuteAsync<MagfaSendResult>(request);

            if (result.IsSuccessful)
            {
                if (result.Data.Status != 0)
                {
                    throw new MagfaException(result.Data.Status);
                }

                return result.Data.Messages;
            }
            else
            {
                throw result.ErrorException ?? new Exception(result.ErrorMessage);
            }
        }
    }
}
