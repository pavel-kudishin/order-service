﻿using Ozon.Route256.Five.OrderService.Domain.BusinessObjects;

namespace Ozon.Route256.Five.OrderService.Core.Handlers.CustomersGet;

public interface ICustomersGettingHandler: IHandlerWithoutRequest<CustomerBo[]>
{
}