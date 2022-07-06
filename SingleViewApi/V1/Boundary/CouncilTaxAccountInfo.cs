namespace SingleViewApi.V1.Boundary;

public class CouncilTaxAccountInfo
{
    public int AccountReference { get; set; }

    public string AccountCheckDigit { get; set; }

    public string PaymentMethod { get; set; }

    public decimal AccountBalance { get; set; }

    public AcademyAddress PropertyAddress { get; set; }

    public AcademyAddress ForwardingAddress { get; set; }
}
