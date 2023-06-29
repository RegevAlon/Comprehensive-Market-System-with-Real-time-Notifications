namespace Market.DomainLayer
{
    public class PendingAgreementEvent : Event
    {
        private Member _appointer;
        private Member _appointee;
        private Shop _shop;
        public PendingAgreementEvent(Shop shop, Member appointer, Member appointee) : base("Pending Agreement Event")
        {
            _shop = shop;
            _appointer = appointer;
            _appointee = appointee;
        }

        public override string GenerateMsg()
        {
            return $"{Name}: Member: \'{_appointer.UserName}\' wish to add \'{_appointee.UserName}\'" +
                $"appointment: Owner " +
                $"to the shop: {_shop.Name}. " +
                "The appointment agreement is waiting for your approval in the shop's agreements section.";
        }
    }
}
