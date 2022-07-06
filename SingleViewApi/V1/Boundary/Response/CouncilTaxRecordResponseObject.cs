namespace SingleViewApi.V1.Boundary.Response;

public class CouncilTaxRecordResponseObject
{
    public int AccountReference { get; set; }

    public string AccountCheckDigit { get; set; }

    public string Title { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string PaymentMethod { get; set; }

    public decimal AccountBalance { get; set; }


    public AcademyAddress PropertyAddress { get; set; }

#nullable enable

    public AcademyAddress? ForwardingAddress { get; set; }

#nullable disable
}


