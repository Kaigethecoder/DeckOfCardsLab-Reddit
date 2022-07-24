using DeckOfCardsLab.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DeckOfCardsLab.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;


        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult DisplayDeckOfCards()
        {
            HttpClient httpClient = _httpClientFactory.CreateClient();
            //https://deckofcardsapi.com/api/deck/new/shuffle/?deck_count=1
            //For any API, we have 3 steps: 1. figure out what url to call
            //2. figure out what we have to send it (nothing here)
            //3. figure out what it returns and model that as a class
            //API Response
            //
            /*{
            "success": true,
             "deck_id": "3p40paa87x90",
             "shuffled": true,
             "remaining": 52
            }*/
            const string createDeckOfCardsApiUrl = "https://deckofcardsapi.com/api/deck/new/shuffle/?deck_count=1";

            //call the API
            //The ending part, .GetAwaiter().GetResult(), is necessary for async api's like this
            //another way to do this is to make your method async and use the await keyword instead.
            //however, that is something you need to KNOW how to do before you do it, or you can break a lot of stuff!
            var apiResponse = httpClient.GetFromJsonAsync<DeckOfCards_Create>(createDeckOfCardsApiUrl).GetAwaiter().GetResult();


            //draw a few cards
            string deckId = apiResponse.deck_id;
            int noCardsToDraw = 5;
            string drawDeckOfCardsAPiFormat = $"https://deckofcardsapi.com/api/deck/{deckId}/draw/?count={noCardsToDraw}";


            var drawCardsResponse = httpClient.GetFromJsonAsync<DeckOfCards_Draw>(drawDeckOfCardsAPiFormat).GetAwaiter().GetResult();
            var displayCardsModel = new DisplayResultsModel();
            displayCardsModel.createResult = apiResponse;
            displayCardsModel.drawResult = drawCardsResponse;
            return View(displayCardsModel);


        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult DisplayReddit()
        {
            HttpClient httpClient = _httpClientFactory.CreateClient();
            const string redditApiUrl = "https://www.reddit.com/r/aww/.json";
            var apiResponse = httpClient.GetFromJsonAsync<RedditSimpleResponse>(redditApiUrl).GetAwaiter().GetResult();
            return View(apiResponse);
        }
        
    }
    //lab 2: reddit
    public class RedditSimpleResponse
    {
        public string kind { get; set; }
        public RedditSimpleResponse_Data data { get; set; }
    }
    public class RedditSimpleResponse_Data
    {
        public string after { get; set; }
        public RedditSimpleResponse_Data_Child[] children { get; set; }
    }
    public class RedditSimpleResponse_Data_Child
    {
        public string kind { get; set; }
        public RedditSimpleResponse_Data_Child_Data data { get; set; }
    }
    public class RedditSimpleResponse_Data_Child_Data
    {
        public string title { get; set; }
        public RedditSimpleResponse_Data_Child_Data_LinkFlairRichText[] link_flair_richtext { get; set; }
    }
    public class RedditSimpleResponse_Data_Child_Data_LinkFlairRichText
    {
        public string a { get; set; }
        public string e { get; set; }
        public string u { get; set; }
    }

    public class DeckOfCards_Draw
    {
        public bool                     success         { get; set; }
        public string                   deck_id         { get; set; }
        public int                      remaining       { get; set; }
        public DeckOfCards_Draw_Card[]  cards           { get; set; }
    }
    public class DisplayResultsModel
    {
        public DeckOfCards_Create createResult { get; set; }
        public DeckOfCards_Draw drawResult { get; set; }

    }
    public class DeckOfCards_Draw_Card
    {
        public string       image     { get; set; }
        public string       value   { get; set; }
        public string       suit    { get; set; }
        public string       code    { get; set; }
    }
    public class DeckOfCards_Create
    {
        public bool         success     { get; set; }
        public string       deck_id     { get; set; }
        public bool         shuffled    { get; set; }
        public int          remaining   { get; set; }
    }
}