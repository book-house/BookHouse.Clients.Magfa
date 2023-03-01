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
    public class MagfaV2Client
    {
        private readonly MagfaClientOptions _options;
        private readonly RestClient _restClient;
        private readonly ResourceManager _resourceManager;

        public MagfaV2Client(MagfaClientOptions options)
        {

            _resourceManager = new ResourceManager(typeof(MagfaStatus));


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

        public class BalanceResult
        {
            public int Status { get; set; }
            public long Balance { get; set; }
        }

        public virtual async Task<long> BalanceAsync()
        {
            var request = new RestRequest("balance", Method.Get);

            var result = await _restClient.ExecuteAsync<BalanceResult>(request);

            if (result.IsSuccessful)
            {
                if (result.Data.Status != 0)
                {
                    throw new Exception(_resourceManager.GetString(result.Data.Status.ToString()));
                }

                return result.Data.Balance;
            }
            else
            {
                throw result.ErrorException ?? new Exception(result.ErrorMessage);
            }
        }


        /// <summary>
        /// ارسال پیام به گیرنده
        /// <seealso href="https://messaging.magfa.com/ui/?public/wiki/api/http_v2#send">لینک مستندات</seealso>
        /// </summary>
        /// <param name="message">متن پیام</param>
        /// <param name="recipients">شماره گیرنده</param>
        /// <returns><seealso cref="MagfaMessage"/></returns>
        public virtual async Task<MagfaMessage> SendAsync(string message, string recipients)
        {
            var res = await SendAsync(new[] { message }, new[] { recipients });
            return res.First();
        }

        /// <summary>
        ///
        /// <seealso href="https://messaging.magfa.com/ui/?public/wiki/api/http_v2#send">لینک مستندات</seealso>
        /// </summary>
        /// <param name="messages">آرايه‌ای از پيام</param>
        /// <param name="recipients">آرايه‌ای از گيرنده</param>
        /// <param name="uids">آرايه‌ای از شناسه‌ی یکتای کاربر</param>
        /// <param name="senders">آرايه‌ای از فرستنده</param>
        /// <returns>آرایه ای از <seealso cref="MagfaMessage"/></returns>
        /// <exception cref="Exception">در صورت وجود خطا متن آن در Message خواهد داشت</exception>
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
                    throw new Exception(result.Data.StatusDescription);
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
