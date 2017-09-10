

using System;
using System.Net.Http;
using net.sxtrader.muk.eventargs;
using net.sxtrader.muk.interfaces;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using net.sxtrader.pinnacleif.pinnacledata;
using net.sxtrader.bftradingstrategies.SXALInterfaces;
using System.Collections;
using net.sxtrader.pinnacleif.PinnacleData;
using System.Collections.Generic;
using System.Threading;

namespace net.sxtrader.pinnacleif
{
    public sealed partial class PinnacleKom : IBFTSCommon
    {
        public event EventHandler<SXExceptionMessageEventArgs> ExceptionMessageEvent;
        public event EventHandler<SXWMessageEventArgs> MessageEvent;

        private static volatile PinnacleKom instance;
        private static object syncRoot = new Object();

        private string _usr = String.Empty;
        private string _pwd = String.Empty;

        private HttpClient _httpClient;

        private SXALEventType[] _events;
        private Dictionary<long, IList<League>> _leagues;
        private List<Fixtures> _fixtures;
        private long _lastFixtures = 0;
        private DateTime _lastFixturesDts = DateTime.MinValue;

        private long _lastOdds = 0;
        private DateTime _lastOddsDts = DateTime.MinValue;

        private const string BaseAddress = "https://api.pinbet88.com/";

        public static PinnacleKom Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new PinnacleKom();
                    }
                }

                return instance;
            }
        }

        private PinnacleKom()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(BaseAddress) };
            _leagues = new Dictionary<long, IList<League>>();
            _fixtures = new List<Fixtures>();
        }
        

        public bool login()
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(
                        Encoding.ASCII.GetBytes(string.Format("{0}:{1}", _usr, _pwd))));

            double availBalance, currentBalance, creditLimit;
            availBalance = currentBalance = creditLimit = 0;

            getAccounFounds(out availBalance, out currentBalance, out creditLimit);
            return true;
        }


        private SXALEventType[] internalLoadEvents()
        {
            lock (syncRoot)
            {
                ArrayList alEvents = new ArrayList();

                const string uri = "v2/sports";

                Event evt = GetJson<Event>(uri);

                foreach (Sports s in evt.Sports)
                {
                    SXALEventType e = new SXALEventType();
                    e.Id = s.Id;
                    e.Name = s.Name;
                    alEvents.Add(e);
                }

                _events = (SXALEventType[])alEvents.ToArray(typeof(SXALEventType));

                _leagues.Clear();
                const string uriLeagues = "v2/leagues?sportid={0}";

                foreach(SXALEventType e in _events)
                {
                    try
                    {                        
                        GetLeague l = GetJson<GetLeague>(string.Format(uriLeagues, e.Id));
                        _leagues.Add(e.Id, l.Leagues);
                        
                    }
                    catch(Exception exc)
                    {
                        Console.WriteLine("{0}: {1} - {2}", e.Id, e.Name, exc.Message);
                    }
                }

                return _events;
            }
        }

        private SXALMarket[] internalGetAllMarkets(int?[] eventids, DateTime fromDate, DateTime toDate)
        {
            lock (syncRoot)
            {

                if(DateTime.Now.Subtract(_lastFixturesDts).TotalSeconds <= 5)
                {
                    Thread.Sleep(5000);
                }
                _lastFixturesDts = DateTime.Now;
                ArrayList alMarkets = new ArrayList();
                SXALMarket m = new SXALMarket();
                const string URLFIXTURE1 = "/v1/fixtures?sportId={0}&since={1}";
                const string URLFIXTURE2 = "/v1/fixtures?sportId={0}";
                ///v1/fixtures?sportId={sportid} &leagueIds=[{leagueId1},{leagueId2},…]&since={since}&IsLive={islive}
                foreach (int eventId in eventids)
                {
                    Fixtures f = null;
                    if (_lastFixtures == 0)
                    {
                        f = GetJson<Fixtures>(string.Format(URLFIXTURE2, eventId));
                        _lastFixtures = f.Last;
                    }
                    else
                    {
                        f = GetJson<Fixtures>(string.Format(URLFIXTURE1, eventId, _lastFixtures));
                        _lastFixtures = f.Last;
                    }

                    _fixtures.Add(f);
                    internalGetOdds(eventId);

                    SXALMarket market = new SXALMarket();
                    
                }
                return (SXALMarket[])alMarkets.ToArray(typeof(SXALMarket));
            }
        }

        private void internalGetOdds(long sportId)
        {
            const string URLODDS1 = "v1/odds?sportid={0}&oddsFormat=DECIMAL";
            const string URLODDS2 = "v1/odds?sportid={0}&since={1}&oddsFormat=DECIMAL";

            lock(syncRoot)
            {
                if (DateTime.Now.Subtract(_lastOddsDts).TotalSeconds > 5)
                {
                    Odds o = null;
                  if(_lastOdds <= 0)
                    {
                        o = GetJson<Odds>(string.Format(URLODDS1, sportId));
                    }
                  else
                    {
                        o = GetJson<Odds>(string.Format(URLODDS2, sportId, _lastOdds));
                    }

                    _lastOdds = o.Last;
                    _lastOddsDts = DateTime.Now;

                    
                }
            }

        }

        private void internalGetLeagues()
        {
            _leagues.Clear();
            const string uriLeagues = "v2/leagues?sportid={0}";

            foreach (SXALEventType e in _events)
            {
                try
                {
                    GetLeague l = GetJson<GetLeague>(string.Format(uriLeagues, e.Id));
                    _leagues.Add(e.Id, l.Leagues);

                }
                catch (Exception exc)
                {
                    Console.WriteLine("{0}: {1} - {2}", e.Id, e.Name, exc.Message);
                }
            }
        }

        private SXALMUBet[] internalGetBets(DateTime dts)
        {
            ArrayList alBets = new ArrayList();

            return (SXALMUBet[])alBets.ToArray(typeof(SXALMUBet));
        }

        private void internalGetAccounFounds(out double availBalance, out double currentBalance, out double creditLimit, out string currency)
        {
            lock (syncRoot)
            {
                availBalance = currentBalance = creditLimit = 0.0;
                const string uri = "v1/client/balance";

                ClientBalance balance = GetJson<ClientBalance>(uri);

                availBalance = (double)balance.AvailableBalance;
                availBalance = (double)(balance.AvailableBalance + balance.OutstandingTransactions);
                creditLimit = (double)balance.GivenCredit;
                currency = balance.Currency;
            }
        }

        private T GetJson<T>(string requestType, params object[] values)
        {
            /*
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestType)
            {
                Version = HttpVersion.Version10
                
            };


           httpRequestMessage.Content = HttpContent.
            
            using (var response = await _httpClient.GetAsync(string.Format(requestType, values)).ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();

                // deserialise json async
                return await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<T>(json));
            }
            */

            var response =  _httpClient.GetAsync(string.Format(requestType, values)).Result; //.ConfigureAwait(false);
            

            response.EnsureSuccessStatusCode();         // throw if web request failed

            

            var json = response.Content.ReadAsStringAsync().Result;

            // deserialise json async
            return JsonConvert.DeserializeObject<T>(json);

        }

    }
}