using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DijnetHelper.Logic.Model;
using DijnetHelper.Model;
using Newtonsoft.Json;
using Xamarin.Forms;
using Bill = DijnetHelper.Logic.Model.Bill;

namespace DijnetHelper.Logic
{
    // browser wrapper used to manipulate dijnet page
    public class DijnetBrowser : IDijnetBrowser, IDisposable
    {
        private const string MainUrlBase = "https://www.dijnet.hu";

        private static readonly Regex PriceRegex = new Regex("^(?<value>(\\d|\\s)+)(?<currency>\\D+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant);
        private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(30);

        private readonly WebView webView;
        private readonly string mainUrl;
        private readonly AsyncResult<WebNavigationResult> webNavigationResult;

        public DijnetBrowser(WebView webView, string mainUrlPath)
        {
            if (mainUrlPath == null) throw new ArgumentNullException(nameof(mainUrlPath));
            this.webView = webView ?? throw new ArgumentNullException(nameof(webView));

            mainUrl = $"{MainUrlBase}{mainUrlPath}";
            webNavigationResult = new AsyncResult<WebNavigationResult>();

            webView.Navigated += OnWebViewNavigated;
        }

        public async Task<List<Bill>> GetBillsAsync()
        {
            bool success = await NavigateHandler(async () => await OpenUrlAndWait(mainUrl));
            if (!success)
            {
                return null;
            }

            return await GetBillsAsync(webView);
        }

        public async Task<List<Card>> SelectBillAndGetCardsAsync(Bill bill)
        {
            string script = $"$('tr#r_{bill.ElementId} > td.fizet > div > div > a > span').click()";

            bool success = await NavigateHandler(async () => await InvokeAndWait(webView, script));
            if (!success)
            {
                return null;
            }

            return await GetCardsAsync(webView);
        }

        public async Task<PayResult> PayBillAsync(Card card)
        {
            await SelectCardAsync(webView, card);
            return await PayBillAsync(webView);
        }

        public async Task<bool> LogoutAsync()
        {
            string checkScript = "$('button:contains(\"Kilépés\")').length > 0";
            bool exists = await EvaluateAndDeserializeAsync<bool>(webView, checkScript);
            if (!exists)
            {
                return true;
            }

            string script = "$('button:contains(\"Kilépés\")').click()";

            bool success = await NavigateHandler(async () => await InvokeAndWait(webView, script));
            if (!success)
            {
                return false;
            }

            return true;
        }

        private async Task SelectCardAsync(WebView target, Card card)
        {
            string script = $"$('input[value=\"{card.Id}\"]').click()";
            await EvaluateAsync(target, script);
        }

        private async Task<PayResult> PayBillAsync(WebView target)
        {
            PayResult result = new PayResult();

            string pivot = "Regisztrált kártyák:";
            string script = $"$('div.xt_field:contains(\"{pivot}\")').closest('div > table > tbody > tr:nth-child(1)').children('td:nth-child(3)').find('input.button').click()";

            bool success = await NavigateHandler(async () => await InvokeAndWait(target, script));
            if (!success)
            {
                result.Error = "Payment failed (com error)!";
                return result;
            }

            // TODO check for error, and return detail

            string checkPivot = "A tranzakció sikeres volt.";
            string checkScript = $"$('h2:contains(\"{checkPivot}\")').length > 0";
            bool resultSuccess = await EvaluateAndDeserializeAsync<bool>(target, checkScript);

            result.Success = resultSuccess;
            if (!resultSuccess)
            {
                // TODO log

                result.Error = "Payment failed!";
            }

            return result;
        }

        private async Task<bool> NavigateHandler(Func<Task<bool>> navigateAsync)
        {
            bool success = await navigateAsync();
            if (!success || !webNavigationResult.HasResult || webNavigationResult.Result != WebNavigationResult.Success)
            {
                // TODO log, could use CallerMemberName

                return false;
            }

            return true;
        }

        private async Task<List<Card>> GetCardsAsync(WebView source)
        {
            string pivot = "Regisztrált kártyák:";
            string script =
                "$.makeArray(" +
                    $"$('div.xt_field:contains(\"{pivot}\")')" +
                    ".closest('div > table > tbody > tr:nth-child(1)')" +
                    ".children('td:nth-child(2)')" +
                    ".find('span.xt_radio_item')" +
                    ".map(function() { var self = $(this); return { 'id': self.prev().attr('value'), 'name': self.text().trim(), 'default': self.prev().attr('checked') === 'checked' }; }))";

            List<Card> cards = await EvaluateAndDeserializeAsync<List<Card>>(source, script);

            return cards ?? new List<Card>(0);
        }

        private async Task<bool> InvokeAndWait(WebView target, string script)
        {
            target.Eval(script);
            return await webNavigationResult.WaitAsync(Timeout);
        }

        private async Task<bool> OpenUrlAndWait(string url)
        {
            if (webView.Source is UrlWebViewSource source && source.Url == url)
            {
                webView.Reload();
            }
            else
            {
                webView.Source = mainUrl;
            }

            return await webNavigationResult.WaitAsync(Timeout);
        }

        private void OnWebViewNavigated(object sender, WebNavigatedEventArgs args)
        {
            webNavigationResult.Set(args.Result);
        }

        private async Task<List<Bill>> GetBillsAsync(WebView source)
        {
            // get TR ids
            List<int> trIds = await EvaluateAndDeserializeAsync<List<int>>(source, "typeof(szlist) !== 'undefined' ? szlist.map(function(e) { return e.rid; }) : []");
            if (trIds == null || trIds.Count == 0)
            {
                return new List<Bill>(0);
            }

            // get each TR data by id
            var getBillTasks = trIds
                .Select(async trId => new
                {
                    Id = trId,
                    Data = await EvaluateAndDeserializeAsync<List<string>>(source, $"$.makeArray($('tr#r_{trId} > td:not(.fizet)').map(function() {{ return $(this).text().trim(); }}))"),
                    Target = await EvaluateAsync(source, $"$('tr#r_{trId} > td.fizet > div > div > a').attr('href')")
                });

            var allBillData = await Task.WhenAll(getBillTasks);
            List<Bill> bills = allBillData?.Select(x => ConvertToBill(x.Id, x.Data, x.Target)).Where(x => x != null).ToList();

            return bills ?? new List<Bill>(0);
        }

        private Price ConvertToPrice(string s)
        {
            var match = PriceRegex.Match(s);
            if (!match.Success)
            {
                return null;
            }

            return new Price
            {
                Value = int.Parse(Regex.Replace(match.Groups["value"].Value, "\\s+", "")),
                Currency = match.Groups["currency"].Value
            };
        }

        private DateTime ConvertToDate(string s)
        {
            return DateTime.ParseExact(s, "yyyy.MM.dd", CultureInfo.InvariantCulture);
        }

        private Bill ConvertToBill(int id, List<string> data, string target)
        {
            // ["Szolgáltató","Számlakibocsátói azonosító","Számlaszám\nBizonylatszám","Kiállítás dátuma","Számla/Bizonylat végösszege","Fizetési határidő","Fizetendő"]

            if (data == null)
            {
                return null;
            }

            return new Bill
            {
                Provider = data[0],
                ProviderId = data[1],
                Id = data[2], // TODO more than one number can be here?
                IssueDate = ConvertToDate(data[3]),
                TotalPrice = ConvertToPrice(data[4]),
                DueDate = ConvertToDate(data[5]),
                PriceToPay = ConvertToPrice(data[6]),
                Target = target,
                ElementId = id,
                Status = BillStatus.Unpaid // TODO cannot map from data, would require more effort
            };
        }

        private async Task<string> EvaluateAsync(WebView source, string script)
        {
            try
            {
                return await source.EvaluateJavaScriptAsync(script);
            }
            catch (Exception ex)
            {
                // TODO log
                return null;
            }
        }

        private async Task<TResult> EvaluateAndDeserializeAsync<TResult>(WebView source, string script)
        {
            string result = await EvaluateAsync(source, script);
            return result == null ? default : JsonConvert.DeserializeObject<TResult>(result);
        }

        public void Dispose()
        {
            webNavigationResult.Dispose();
            webView.Navigated -= OnWebViewNavigated;
        }
    }
}
