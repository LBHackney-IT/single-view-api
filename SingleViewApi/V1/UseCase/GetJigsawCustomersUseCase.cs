using System;
using System.Threading.Tasks;
using Hackney.Core.Logging;
using SingleViewApi.V1.Gateways;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.V1.UseCase
{
    public class GetJigsawCustomersUseCase : IGetJigsawCustomersUseCase
    {
        private IJigsawGateway _jigsawGateway;

        public GetJigsawCustomersUseCase(IJigsawGateway jigsawGateway)
        {
            _jigsawGateway = jigsawGateway;
        }

        [LogCall]

        public async Task<dynamic> Execute(string firstName, string lastName)
        {
            var results = await _jigsawGateway.GetCustomers(firstName, lastName);

            return results;
        }
    }
}
