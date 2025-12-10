using Ecommerce.Application.Features.Products.Queries.Vms;
using Ecommerce.Domain;
using MediatR;

namespace Ecommerce.Application.Features.Products.Queries.GetProductList;

public class GetProductListQuery : IRequest<IReadOnlyList<ProductVm>>
{
}