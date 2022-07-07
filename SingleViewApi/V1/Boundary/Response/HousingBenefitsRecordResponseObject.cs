namespace SingleViewApi.V1.Boundary.Response;

public class HousingBenefitsRecordResponseObject
{
    public int AccountReference { get; set; }

    public string AccountCheckDigit { get; set; }

    public string Title { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string PaymentMethod { get; set; }

    public decimal AccountBalance { get; set; }


    public Address PropertyAddress { get; set; }

#nullable enable

    public Address? ForwardingAddress { get; set; }

#nullable disable
}


