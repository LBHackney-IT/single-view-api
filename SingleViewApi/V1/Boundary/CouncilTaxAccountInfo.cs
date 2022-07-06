namespace SingleViewApi.V1.Boundary;

public class CouncilTaxAccountInfo
{
    public int AccountReference { get; set; }

    public string AccountCheckDigit { get; set; }

    public string PaymentMethod { get; set; }

    public decimal AccountBalance { get; set; }

    public Address PropertyAddress { get; set; }

#nullable enable
    public Address? ForwardingAddress { get; set; }

#nullable disable
}
