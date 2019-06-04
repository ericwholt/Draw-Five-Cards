using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeckOfCardsAPI.Models
{
    public class Card
    {
        public string ImageUrl { get; set; }
        public string Value { get; set; }
        public string Suit { get; set; }
        public string Code { get; set; }
    }
}