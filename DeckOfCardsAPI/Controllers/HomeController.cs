using DeckOfCardsAPI.Models;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace DeckOfCardsAPI.Controllers
{
    public class HomeController : Controller
    {
        public Deck GetDeck()
        {
            //https://deckofcardsapi.com/api/deck/new/shuffle/?deck_count=1

            string url = $"https://deckofcardsapi.com/api/deck/new/shuffle/?deck_count=1";

            HttpWebRequest request = WebRequest.CreateHttp(url);
            //If we needed it, special setup would go here
            //Examples: Add Headers, Secret Key, Add User Agent(What app making call, what browser, what server generally handled by .Net)
            //          Responses Formats
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader rd = new StreamReader(response.GetResponseStream());

            string APIText = rd.ReadToEnd();
            Deck deck = ConvertToDeck(APIText);
            return deck;
        }

        public Deck ConvertToDeck(string APIText)
        {
            JToken t = JToken.Parse(APIText);
            Deck deck = new Deck();
            deck.Id = t["deck_id"].ToString();
            deck.Remaining = int.Parse(t["remaining"].ToString());
            Session["Deck"] = deck;
            return deck;
        }

        public List<Card> GetCards(Deck deck, int count = 5)
        {
            List<Card> cards = new List<Card>();

            string url = $"https://deckofcardsapi.com/api/deck/{deck.Id}/draw/?count={count}";

            HttpWebRequest request = WebRequest.CreateHttp(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader rd = new StreamReader(response.GetResponseStream());
            string APIText = rd.ReadToEnd();

            JToken t = JToken.Parse(APIText);

            bool success = t["success"].ToString() == "true" ? true : false;
            deck.Remaining = int.Parse(t["remaining"].ToString());
            Session["Deck"] = deck;
            List<JToken> jtokens = t["cards"].ToList();
            foreach (JToken jtoken in jtokens)
            {
                Card card = ConvertToCard(jtoken);
                cards.Add(card);
            }

            return cards;
        }

        public Card ConvertToCard(JToken jToken)
        {

            Card card = new Card
            {
                ImageUrl = jToken["image"].ToString(),
                Value = jToken["value"].ToString(),
                Suit = jToken["suit"].ToString(),
                Code = jToken["code"].ToString()
            };

            return card;
        }

        public ActionResult Index()
        {
            Deck deck = GetDeck();
            List<Card> cards = GetCards(deck);
            Session["CardsInHand"] = cards;

            return View(cards);
        }

        [HttpPost]
        public ActionResult Index(int count, string Card1, string Card2, string Card3, string Card4, string Card5)
        {
            Deck deck = (Deck)Session["Deck"];
            List<Card> cardsInHand = new List<Card>((List<Card>)Session["CardsInHand"]);
            List<string> cardsToKeep = new List<string>() { Card1, Card2, Card3, Card4, Card5 };
            List<Card> NewHand = new List<Card>();

            for (int i = 0; i < cardsInHand.Count; i++)
            {
                for (int j = 0; j < cardsToKeep.Count; j++)
                {
                    if (cardsInHand[i].Code == cardsToKeep[j])
                    {
                        NewHand.Add(cardsInHand[i]);
                    }
                }

            }

            count -= NewHand.Count;

            //List<Card> newCards = GetCards(deck, count);
            GetCards(deck, count).ForEach(x => NewHand.Add(x));

            Session["CardsInHand"] = NewHand;
            return View(NewHand);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}