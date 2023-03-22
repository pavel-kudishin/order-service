using Dapper;
using Npgsql;
using Ozon.Route256.Five.OrderService.Core.Repository;
using Ozon.Route256.Five.OrderService.Core.Repository.Dto;

namespace Ozon.Route256.Five.OrderService.Db.Repositories;

public sealed class OrderDbRepository : IOrderRepository
{
    private readonly IConnectionFactory _connectionFactory;

    public OrderDbRepository(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<OrderDto?> Find(long orderId, CancellationToken token)
    {
        await using NpgsqlConnection connection = await _connectionFactory.GetConnection();

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
            orders
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

        return order;
    }

    public async Task Insert(OrderDto order, CancellationToken token)
    {
        await using NpgsqlConnection connection = await _connectionFactory.GetConnection();

        const string SQL = @"
        INSERT INTO
            public.orders
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
        );";

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
            order.CustomerId,
            order.Address?.City,
            order.Address?.Street,
            order.Address?.Building,
            order.Address?.Apartment,
            order.Address?.Region,
            order.Address?.Latitude,
            order.Address?.Longitude,
        };

        await connection.ExecuteAsync(SQL, queryArguments);
    }

    public async Task Update(OrderDto order, CancellationToken token)
    {
        await using NpgsqlConnection connection = await _connectionFactory.GetConnection();

        const string SQL = @"
        UPDATE
            public.orders
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
            order.CustomerId,
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

    public async Task<OrderDto[]> FindByCustomer(
        int customerId, DateTime? startDate, DateTime? endDate,
        int pageNumber, int itemsPerPage, CancellationToken token)
    {
        await using NpgsqlConnection connection = await _connectionFactory.GetConnection();

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
            orders
        WHERE
            customer_id = @customerId
            AND
            (@startDate IS NULL OR @startDate <= date_created)
            AND
            (@endDate IS NULL OR @endDate >= date_created)
        ORDER BY id
        LIMIT @limit
        OFFSET @offset";

        var queryArguments = new
        {
            customerId,
            startDate,
            endDate,
            offset = pageNumber * itemsPerPage,
            limit = itemsPerPage,
        };

        IEnumerable<OrderDto> orders = await connection.QueryAsync<OrderDto, AddressDto, OrderDto>(
            SQL,
            (orderDto, addressDto) =>
            {
                orderDto.Address = addressDto;
                return orderDto;
            },
            queryArguments,
            splitOn: "City");

        return orders.ToArray();
    }

    public async Task<OrderDto[]> Filter(string[]? regions, OrderSourceDto[]? sources,
        int pageNumber, int itemsPerPage,
        OrderingDirectionDto orderingDirection, CancellationToken token)
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

        if (regions != null && regions.Length > 0)
        {
            builder.Where("address_region = ANY(@regions)");
        }
        if (sources != null && sources.Length > 0)
        {
            builder.Where("order_source = ANY(@sources)");
        }

        builder.OrderBy($"region {orderingDirection.ToString()}");

        SqlBuilder.Template? builderTemplate =
            builder.AddTemplate("SELECT /**select**/ FROM orders /**where**/ /**orderby**/ LIMIT @limit OFFSET @offset");
        string rawSql = builderTemplate.RawSql;

        var queryArguments = new
        {
            regions,
            sources = sources?.Select(s => (int)s).ToArray(),
            offset = pageNumber * itemsPerPage,
            limit = itemsPerPage,
        };

        await using NpgsqlConnection connection = await _connectionFactory.GetConnection();
        IEnumerable<OrderDto> orders = await connection.QueryAsync<OrderDto, AddressDto, OrderDto>(
            rawSql,
            (orderDto, addressDto) =>
            {
                orderDto.Address = addressDto;
                return orderDto;
            },
            queryArguments,
            splitOn: "City");

        return orders.ToArray();
    }

    public async Task<AggregateOrdersDto[]> AggregateOrders(string[]? regions, DateTime startDate,
        DateTime? endDate, CancellationToken token)
    {
        SqlBuilder builder = new();
        builder.Where("date_created >= @startDate");
        if (regions != null && regions.Length > 0)
        {
            builder.Where("address_region = ANY(@regions)");
        }
        if (endDate.HasValue)
        {
            builder.Where("date_created <= @endDate");
        }

        SqlBuilder.Template? builderTemplate =
            builder.AddTemplate("SELECT * FROM orders /**where**/");
        string innerSql = builderTemplate.RawSql;

        const string AGGREGATION_SQL = @"
         SELECT
            address_region AS Region,
            SUM(price) AS TotalOrdersPrice,
            SUM(weight) AS TotalWeight,
            COUNT(*) AS OrdersCount,
            COUNT(DISTINCT customer_id) AS CustomersCount
        FROM
            (
                {0}
            ) T
        GROUP BY address_region
        ";

        string sql = string.Format(AGGREGATION_SQL, innerSql);

        var queryArguments = new
        {
            regions,
            startDate,
            endDate,
        };

        await using NpgsqlConnection connection = await _connectionFactory.GetConnection();
        IEnumerable<AggregateOrdersDto> orders =
            await connection.QueryAsync<AggregateOrdersDto>(sql, queryArguments);

        return orders.ToArray();
    }
}