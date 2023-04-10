using System.Data;
using System.Data.Common;
using Dapper;
using Ozon.Route256.Five.OrderService.Core.ClientBalancing;
using Ozon.Route256.Five.OrderService.Core.Exceptions;
using Ozon.Route256.Five.OrderService.Db.Dto;
using Ozon.Route256.Five.OrderService.Db.Extensions;
using Ozon.Route256.Five.OrderService.Db.Repositories.Harness;
using Ozon.Route256.Five.OrderService.Domain.BusinessObjects;
using Ozon.Route256.Five.OrderService.Domain.Repository;

namespace Ozon.Route256.Five.OrderService.Db.Repositories;

internal sealed class OrderDbRepository : IOrderRepository
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly IShardingRule<long> _longShardingRule;
    private readonly IShardingRule<string> _stringShardingRule;
    private readonly IDbStore _dbStore;
    private readonly ICustomerRepository _customerRepository;

    public OrderDbRepository(
        IConnectionFactory connectionFactory,
        IShardingRule<long> longShardingRule,
        IShardingRule<string> stringShardingRule,
        IDbStore dbStore,
        ICustomerRepository customerRepository)
    {
        _connectionFactory = connectionFactory;
        _longShardingRule = longShardingRule;
        _stringShardingRule = stringShardingRule;
        _dbStore = dbStore;
        _customerRepository = customerRepository;
    }

    public async Task<OrderBo?> Find(long orderId, CancellationToken token)
    {
        await using DbConnection connection = _connectionFactory.GetConnectionByKey(orderId);

        const string SQL = @"
        SELECT
            id AS Id,
            goods_count AS GoodsCount,
            price AS TotalPrice,
            weight AS TotalWeight,
            order_source AS Source,
            date_created AS DateCreated,
            state AS State,
            phone AS Phone,
            customer_id AS CustomerId,
            address_city AS City,
            address_street AS Street,
            address_building AS Building,
            address_apartment AS Apartment,
            address_latitude AS Latitude,
            address_longitude AS Longitude,
            address_region AS Region
        FROM
            __bucket__.orders
        WHERE
            id = @orderId
        LIMIT 1";

        IEnumerable<OrderDto> orders = await connection.QueryAsync<OrderDto, AddressDto, OrderDto>(
            SQL,
            (orderDto, addressDto) =>
            {
                orderDto.Address = addressDto;
                return orderDto;
            },
            new { orderId },
            splitOn: "City");

        OrderDto? order = orders.FirstOrDefault();

        if (order == null)
        {
            return null;
        }

        CustomerBo? customer = await _customerRepository.Find(order.CustomerId, token);

        if (customer == null)
        {
            throw new RepositoryException($"Customer #{order.CustomerId} not found");
        }

        return order?.ToOrderBo(customer);
    }

    public async Task Insert(OrderBo order, CancellationToken token)
    {
        await using DbConnection connection = _connectionFactory.GetConnectionByKey(order.Id);

        const string SQL = @"
        INSERT INTO
            __bucket__.orders
        (
            id, goods_count, price, weight,
            order_source, date_created,
            state, phone, customer_id, address_city,
            address_street, address_building,
            address_apartment, address_region,
            address_latitude, address_longitude
        )
        VALUES
        (
            @Id, @GoodsCount, @TotalPrice, @TotalWeight,
            @Source, @DateCreated,
            @State, @Phone, @CustomerId, @City,
            @Street, @Building,
            @Apartment, @Region,
            @Latitude, @Longitude
        )";

        var queryArguments = new
        {
            order.Id,
            order.GoodsCount,
            order.TotalPrice,
            order.TotalWeight,
            order.Source,
            order.DateCreated,
            order.State,
            order.Phone,
            CustomerId = order.Customer.Id,
            order.Address?.City,
            order.Address?.Street,
            order.Address?.Building,
            order.Address?.Apartment,
            order.Address?.Region,
            order.Address?.Latitude,
            order.Address?.Longitude,
        };

        await connection.ExecuteAsync(SQL, queryArguments);

        await InsertIndexOnCustomerId(order);
        await InsertIndexOnRegionName(order);
    }

    private async Task InsertIndexOnRegionName(OrderBo order)
    {
        const string SQL = @"
        INSERT INTO
            __bucket__.index_order_id_region_name
        (
            order_id,
            region_name,
            date_created,
            order_source
        )
        VALUES
        (
            @order_id,
            @region_name,
            @date_created,
            @order_source
        )";

        DynamicParameters param = new();
        param.Add("@order_id", order.Id);
        param.Add("@region_name", order.Address?.Region);
        param.Add("@date_created", order.DateCreated);
        param.Add("@order_source", order.Source);

        await using DbConnection connection = _connectionFactory.GetConnectionByKey(order.Address!.Region);
        await connection.ExecuteAsync(SQL, param);
    }

    private async Task InsertIndexOnCustomerId(OrderBo order)
    {
        const string SQL = @"
        INSERT INTO
            __bucket__.index_order_id_customer_id
        (
            order_id,
            customer_id,
            date_created
        )
        VALUES
        (
            @order_id,
            @customer_id,
            @date_created
        )";

        DynamicParameters param = new();
        param.Add("@order_id", order.Id);
        param.Add("@customer_id", order.Customer.Id);
        param.Add("@date_created", order.DateCreated);

        await using DbConnection connection = _connectionFactory.GetConnectionByKey(order.Customer.Id);
        await connection.ExecuteAsync(SQL, param);
    }

    public async Task Update(OrderBo order, CancellationToken token)
    {
        await using DbConnection connection = _connectionFactory.GetConnectionByKey(order.Id);

        const string SQL = @"
        UPDATE
            __bucket__.orders
        SET
            goods_count=@GoodsCount,
            price=@TotalPrice,
            weight=@TotalWeight,
            order_source=@Source,
            date_created=@DateCreated,
            state=@State,
            phone=@Phone,
            customer_id=@CustomerId,
            address_city=@City,
            address_street=@Street,
            address_building=@Building,
            address_apartment=@Apartment,
            address_region=@Region,
            address_latitude=@Latitude,
            address_longitude=@Longitude
        WHERE
            id = @Id";

        var queryArguments = new
        {
            order.Id,
            order.GoodsCount,
            order.TotalPrice,
            order.TotalWeight,
            order.Source,
            order.DateCreated,
            order.State,
            order.Phone,
            CustomerId = order.Customer.Id,
            order.Address?.City,
            order.Address?.Street,
            order.Address?.Building,
            order.Address?.Apartment,
            order.Address?.Region,
            order.Address?.Latitude,
            order.Address?.Longitude,
        };

        int count = await connection.ExecuteAsync(SQL, queryArguments);
    }

    public async Task<OrderBo[]> FindByCustomer(int customerId, DateTime? startDate, DateTime? endDate,
        int pageNumber, int itemsPerPage, CancellationToken token)
    {
        IEnumerable<long> orderIds = await GetOrderIdsFromIndex(customerId, startDate, endDate, pageNumber, itemsPerPage);
        OrderBo[] orders = await GetOrdersByIds(orderIds, token);
        return orders.OrderBy(o => o.Id).ToArray();
    }

    private async Task<OrderBo[]> GetOrdersByIds(IEnumerable<long> orderIds, CancellationToken token)
    {
        SqlBuilder builder = new();
        builder.Select("id AS Id");
        builder.Select("goods_count AS GoodsCount");
        builder.Select("price AS TotalPrice");
        builder.Select("weight AS TotalWeight");
        builder.Select("order_source AS Source");
        builder.Select("date_created AS DateCreated");
        builder.Select("state AS State");
        builder.Select("phone AS Phone");
        builder.Select("customer_id AS CustomerId");
        builder.Select("address_city AS City");
        builder.Select("address_street AS Street");
        builder.Select("address_building AS Building");
        builder.Select("address_apartment AS Apartment");
        builder.Select("address_latitude AS Latitude");
        builder.Select("address_longitude AS Longitude");
        builder.Select("address_region AS Region");
        builder.Where("id = ANY(@order_ids)");

        SqlBuilder.Template? builderTemplate =
            builder.AddTemplate(@"
            SELECT
                /**select**/
            FROM __bucket__.orders
                /**where**/");
        string rawSql = builderTemplate.RawSql;

        List<OrderDto> result = new();
        Dictionary<int, long[]>? bucketToOrderIdsMap = GetBucketToOrderIdsMap(orderIds);
        if (bucketToOrderIdsMap != null)
        {
            foreach ((int bucketId, long[] idsInBucket) in bucketToOrderIdsMap)
            {
                DynamicParameters orderParams = new();
                orderParams.Add("@order_ids", idsInBucket);

                await using DbConnection connection = _connectionFactory.GetConnectionByBucket(bucketId);
                IEnumerable<OrderDto> orders = await connection.QueryAsync<OrderDto, AddressDto, OrderDto>(
                    rawSql,
                    (orderDto, addressDto) =>
                    {
                        orderDto.Address = addressDto;
                        return orderDto;
                    },
                    orderParams,
                    splitOn: "City");

                result.AddRange(orders);
            }
        }

        int[] customerIds = result.Select(o => o.CustomerId).Distinct().ToArray();
        CustomerBo[] customers = await _customerRepository.FindMany(customerIds, token);

        return result.ToOrdersBo(customers);
    }

    private Dictionary<int, long[]> GetBucketToOrderIdsMap(IEnumerable<long> orderIds)
    {
        Dictionary<int, long[]> bucketToOrderIdsMap =
            orderIds
                .Select(orderId =>
                    (BucketId: _longShardingRule.GetBucketId(orderId, _dbStore.BucketsCount), OrderId: orderId))
                .GroupBy(x => x.BucketId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.OrderId).ToArray());

        return bucketToOrderIdsMap;
    }

    private async Task<IEnumerable<long>> GetOrderIdsFromIndex(
        int customerId,
        DateTime? startDate,
        DateTime? endDate,
        int pageNumber,
        int itemsPerPage)
    {
        const string SQL = @"
        SELECT
            order_id
        FROM
            __bucket__.index_order_id_customer_id
        WHERE
            customer_id = @customer_id
            AND
            (@start_date IS NULL OR @start_date <= date_created)
            AND
            (@end_date IS NULL OR @end_date >= date_created)
        ORDER BY order_id
        LIMIT @limit
        OFFSET @offset";

        DynamicParameters indexParams = new();
        indexParams.Add("@customer_id", customerId);
        indexParams.Add("@start_date", startDate, DbType.DateTime);
        indexParams.Add("@end_date", endDate, DbType.DateTime);
        indexParams.Add("@limit", itemsPerPage);
        indexParams.Add("@offset", pageNumber * itemsPerPage);

        await using DbConnection connection = _connectionFactory.GetConnectionByKey(customerId);
        IEnumerable<long>? orderIds = await connection.QueryAsync<long>(SQL, indexParams);
        return orderIds;
    }

    public async Task<OrderBo[]> Filter(string[]? regions,
        OrderSourceBo[]? sources,
        int pageNumber,
        int itemsPerPage,
        OrderingDirectionBo orderingDirection,
        CancellationToken token)
    {
        if (regions == null || regions.Length == 0)
        {
            return Array.Empty<OrderBo>();
        }

        long[] orderIds = await GetOrderIdsFromIndex(regions, sources, pageNumber, itemsPerPage, orderingDirection);
        OrderBo[] orders = await GetOrdersByIds(orderIds, token);

        return orders.OrderBy(o => o.Address?.Region).ToArray();
    }

    private async Task<long[]> GetOrderIdsFromIndex(
        string[] regions,
        OrderSourceBo[]? sources,
        int pageNumber,
        int itemsPerPage,
        OrderingDirectionBo orderingDirection)
    {
        IOrderedEnumerable<string> orderedRegions =
            orderingDirection == OrderingDirectionBo.Asc
                ? regions.OrderBy(r => r)
                : regions.OrderByDescending(r => r);

        List<long> orderIds = new(itemsPerPage);
        int skippedOrdersCount = 0; // Считаем сколько заказов уже пропущено для пейджинга
        foreach (string region in orderedRegions)
        {
            // По каждому региону добавляем заказы пока не наберем itemsPerPage
            if (orderIds.Count >= itemsPerPage)
            {
                break;
            }

            SqlBuilder builder = new();
            builder.Select("order_id");
            builder.Where("region_name = @region_name");
            if (sources != null && sources.Length > 0)
            {
                builder.Where("order_source = ANY(@sources)");
            }
            SqlBuilder.Template? builderTemplate =
                builder.AddTemplate(@"
            SELECT
                /**select**/
            FROM __bucket__.index_order_id_region_name
                /**where**/
                /**orderby**/
            LIMIT @limit");
            string rawSql = builderTemplate.RawSql;

            // Осталось найти столько элементов
            int limit = (pageNumber + 1) * itemsPerPage - skippedOrdersCount - orderIds.Count;

            DynamicParameters indexParams = new();
            indexParams.Add("@region_name", region);
            indexParams.Add("@sources", sources?.Select(s => (int)s).ToArray());
            indexParams.Add("@limit", limit);

            await using DbConnection connection = _connectionFactory.GetConnectionByKey(region);
            IEnumerable<long>? ids = await connection.QueryAsync<long>(rawSql, indexParams);
            long[] regionOrderIds = ids.ToArray();

            if (skippedOrdersCount + regionOrderIds.Length <= pageNumber * itemsPerPage)
            {
                // Если еще не дошли до нужной страницы заказов
                skippedOrdersCount += regionOrderIds.Length;
            }
            else
            {
                // Дошли до нужной страницы
                IEnumerable<long> restOfOrders = regionOrderIds.Skip(pageNumber * itemsPerPage - skippedOrdersCount);
                orderIds.AddRange(restOfOrders);
                skippedOrdersCount = pageNumber * itemsPerPage;
            }
        }

        return orderIds.ToArray();
    }

    public async Task<AggregatedOrdersBo[]> AggregateOrders(string[]? regions,
        DateTime startDate,
        DateTime? endDate,
        CancellationToken token)
    {
        if (regions == null || regions.Length == 0)
        {
            return Array.Empty<AggregatedOrdersBo>();
        }

        const string SQL = @"
        SELECT
            address_region AS Region,
            SUM(price) AS TotalOrdersPrice,
            SUM(weight) AS TotalWeight,
            COUNT(*) AS OrdersCount
        FROM
            (
                SELECT * FROM __bucket__.orders WHERE id = ANY(@order_ids)
            ) T
        GROUP BY address_region
        ";

        const string SQL_CUSTOMERS = @"
        SELECT DISTINCT
            customer_id AS CustomerId,
            address_region AS Region
        FROM
            __bucket__.orders
        WHERE
            id = ANY(@order_ids)
        ";

        long[] orderIds = await GetOrderIdsFromIndex(regions, startDate, endDate, token);

        Dictionary<int, long[]> bucketToOrderIdsMap = GetBucketToOrderIdsMap(orderIds);
        List<AggregateOrdersDto> result = new(regions.Length * bucketToOrderIdsMap.Count);
        List<(int CustomerId, string Region)> customersByRegion = new List<(int CustomerId, string Region)>();

        foreach ((int bucketId, long[] idsInBucket) in bucketToOrderIdsMap)
        {
            DynamicParameters orderParams = new();
            orderParams.Add("@order_ids", idsInBucket);

            await using DbConnection connection = _connectionFactory.GetConnectionByBucket(bucketId);
            IEnumerable<AggregateOrdersDto> orders =
                await connection.QueryAsync<AggregateOrdersDto>(SQL, orderParams);

            result.AddRange(orders);

            IEnumerable<(int CustomerId, string Region)> customers =
                await connection.QueryAsync<(int CustomerId, string Region)>(SQL_CUSTOMERS, orderParams);

            customersByRegion.AddRange(customers);
        }

        ILookup<string, int> lookup = customersByRegion
            .ToLookup(tuple => tuple.Region, tuple => tuple.CustomerId);

        AggregateOrdersDto[] aggregateOrders = result
            .GroupBy(o => o.Region)
            .Select(g =>
            {
                AggregateOrdersDto[] orderDtos = g.ToArray();
                return new AggregateOrdersDto()
                {
                    Region = g.Key,
                    OrdersCount = orderDtos.Sum(a => a.OrdersCount),
                    TotalWeight = orderDtos.Sum(a => a.TotalWeight),
                    CustomersCount = lookup[g.Key].Distinct().Count(),
                    TotalOrdersPrice = orderDtos.Sum(a => a.TotalOrdersPrice),
                };
            })
            .ToArray();

        return aggregateOrders.ToAggregatedOrdersBo();
    }

    private async Task<long[]> GetOrderIdsFromIndex(
        string[] regions,
        DateTime startDate,
        DateTime? endDate,
        CancellationToken token)
    {
        Dictionary<int, string[]> bucketToRegionNamesMap =
            regions
                .Select(regionName =>
                    (BucketId: _stringShardingRule.GetBucketId(regionName, _dbStore.BucketsCount), RegionName: regionName))
                .GroupBy(x => x.BucketId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.RegionName).ToArray());

        const string SQL = @"
        SELECT
            order_id
        FROM
            __bucket__.index_order_id_region_name
        WHERE
            region_name = ANY(@region_names)
            AND
            (@start_date <= date_created)
            AND
            (@end_date IS NULL OR @end_date >= date_created)
        ";

        List<long> result = new();

        foreach ((int bucketId, string[] namesInBucket) in bucketToRegionNamesMap)
        {
            DynamicParameters indexParams = new();
            indexParams.Add("@region_names", namesInBucket);
            indexParams.Add("@start_date", startDate);
            indexParams.Add("@end_date", endDate, DbType.DateTime);

            await using DbConnection connection = _connectionFactory.GetConnectionByBucket(bucketId);
            IEnumerable<long>? orderIds = await connection.QueryAsync<long>(SQL, indexParams);

            result.AddRange(orderIds);
        }

        return result.ToArray();
    }
}