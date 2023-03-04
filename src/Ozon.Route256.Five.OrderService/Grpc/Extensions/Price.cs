namespace Ozon.Route256.Five.Orders.Grpc
{
    public partial class Price
    {
        public static implicit operator decimal(Price price)
            => price.Rubles + price.Kopecks / 100m;

        public static implicit operator Price(decimal value)
        {
            int rubles = decimal.ToInt32(value);
            int kopecks = decimal.ToInt32((value - rubles) * 100m);

            return new Price()
            {
                Rubles = rubles,
                Kopecks = kopecks,
            };
        }
    }
}
