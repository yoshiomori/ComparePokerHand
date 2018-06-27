using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparePokerHand
{
    class CardEqualityComparer : IEqualityComparer<Card>
    {
        public bool Equals(Card x, Card y)
        {
            return x.value == y.value;
        }

        public int GetHashCode(Card obj)
        {
            return obj.value;
        }
    }

    class Card : IComparable
    {
        public readonly int value;
        public readonly char suit;

        public Card(string card)
        {
            var value = card.ElementAt(0);
            var suit = card.ElementAt(1);
            char[] suits = { 'C', 'D', 'H', 'S' };
            char[] values = { '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'J', 'Q', 'K', 'A' };
            if (card.Length != 2 || !suits.Contains(suit) || !values.Contains(value))
                throw new Exception("Must be a card");
            this.suit = suit;
            this.value = GetValue(value);
        }

        public int CompareTo(object obj)
        {
            if (obj is Card card)
                return this.value > card.value ? 1 : this.value < card.value ? -1 : 0;
            throw new NotImplementedException();
        }

        private static int GetValue(char c)
        {
            if (c == '2')
                return 0;
            else if (c == '3')
                return 1;
            else if (c == '4')
                return 2;
            else if (c == '5')
                return 3;
            else if (c == '6')
                return 4;
            else if (c == '7')
                return 5;
            else if (c == '8')
                return 6;
            else if (c == '9')
                return 7;
            else if (c == 'T')
                return 8;
            else if (c == 'J')
                return 9;
            else if (c == 'Q')
                return 10;
            else if (c == 'K')
                return 11;
            else
                return 12;
        }

        public bool IsNext(Card card) => (this.value + 1) % 13 == card.value;

        public bool IsAses() => this.value == 12;

        public bool IsTwo() => this.value == 0;
    }

    class HandStrength : IComparable
    {
        /* hand */
        private readonly int value;

        /* kiker */
        private readonly int k1 = 0;
        private readonly int k2 = 0;
        private readonly int k3 = 0;
        private readonly int k4 = 0;
        private readonly int k5 = 0;

        public HandStrength(List<Card> hand)
        {
            hand.Sort();
            var count = Count(hand);
            var keys = count.Keys.OrderBy(key => key.value).OrderBy(key => count[key]).ToArray(); ;
            if (IsStraight(hand) && IsFlush(hand))
            { // Straight Flush
                this.value = 8;
                this.k1 = hand.Last().value;
            }
            else if (keys.Length == 2 && count[keys.Last()] == 4)
            { // Four of Kind
                this.value = 7;
                this.k1 = keys.Last().value;
            }
            else if (keys.Length == 2 && count[keys.Last()] == 3)
            { // Full house
                this.value = 6;
                this.k1 = keys.Last().value;
            }
            else if (IsFlush(hand))
            {
                this.value = 5;
                this.k1 = hand[4].value;
                this.k2 = hand[3].value;
                this.k3 = hand[2].value;
                this.k4 = hand[1].value;
                this.k5 = hand[0].value;
            }
            else if (IsStraight(hand))
            {
                this.value = 4;
                this.k1 = hand.Last().value;
            }
            else if (keys.Length == 3 && count[keys.Last()] == 3)
              // Three of a Kind
                this.value = 3;
            else if (keys.Length == 3 && count[keys.Last()] == 2)
            { // Two Pairs
                this.value = 2;
                this.k1 = keys.Last().value;
                this.k2 = keys[1].value;
                this.k3 = keys.First().value;
            }
            else if (keys.Length == 4 && count[keys.Last()] == 2)
            { // Pair
                this.value = 1;
                this.k1 = keys.Last().value;
                this.k2 = keys[2].value;
                this.k3 = keys[1].value;
                this.k4 = keys.First().value;
            }
            else
            {
                this.value = 0;
                this.k1 = keys.Last().value;
                this.k2 = keys[3].value;
                this.k3 = keys[2].value;
                this.k4 = keys[1].value;
                this.k4 = keys.First().value;
            }
        }

        static bool IsStraight(List<Card> hand)
            => hand.Last().IsAses() && hand.First().IsTwo() ? IsSequenceUntil(hand, 3) : IsSequenceUntil(hand, 4);

        static bool IsSequenceUntil(List<Card> hand, int count)
            => Enumerable.Range(0, count).Aggregate(true, (s, i) => s && hand[i].IsNext(hand[i + 1]));

        static bool IsFlush(List<Card> hand)
            => Enumerable.Range(0, 4).Aggregate(true, (s, i) => s && hand[i].suit == hand[i + 1].suit);

        static Dictionary<Card, int> Count(List<Card> hand)
        {
            var response = new Dictionary<Card, int>(new CardEqualityComparer());
            foreach (Card card in hand)
                if (response.ContainsKey(card))
                    response[card]++;
                else
                    response[card] = 1;
            return response;
        }

        public int CompareTo(object obj)
        {
            if (obj is HandStrength handStrength)
            {
                if (this.value > handStrength.value)
                    return 1;
                else if (this.value < handStrength.value)
                    return -1;
                else if (this.k1 > handStrength.k1)
                    return 1;
                else if (this.k1 < handStrength.k1)
                    return -1;
                else if (this.k2 > handStrength.k2)
                    return 1;
                else if (this.k2 < handStrength.k2)
                    return -1;
                else if (this.k3 > handStrength.k3)
                    return 1;
                else if (this.k3 < handStrength.k3)
                    return -1;
                else if (this.k4 > handStrength.k4)
                    return 1;
                else if (this.k4 < handStrength.k4)
                    return -1;
                else if (this.k5 > handStrength.k5)
                    return 1;
                else if (this.k5 < handStrength.k5)
                    return -1;
                else
                    return 0;
            }
            else
                throw new NotImplementedException();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var s = Console.ReadLine();
            List<string> responses = new List<string>();
            while (s.Length > 0)
            {
                var hands = s.Split(' ').Select(card => new Card(card)).ToList();
                var handStrength1 = new HandStrength(hands.GetRange(0, 5));
                var handStrength2 = new HandStrength(hands.GetRange(5, 5));
                responses.Add(
                    handStrength1.CompareTo(handStrength2) > 0 ? "Black"
                    : handStrength1.CompareTo(handStrength2) < 0 ? "White" : "Tie"
                );
                s = Console.ReadLine();
            }
            foreach (var response in responses)
                Console.WriteLine(response);
            Console.ReadKey();
        }
    }
}
