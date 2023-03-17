using System.Text;
using Dapper;
using Npgsql;
using Ozon.Route256.Five.OrderService.Core.Repository;
using Ozon.Route256.Five.OrderService.Core.Repository.Dto;

namespace Ozon.Route256.Five.OrderService.Db.Repositories;

public sealed class OrderDbRepository : IOrderRepository
{
    private readonly IConnectionCreator _connectionCreator;

    public OrderDbRepository(IConnectionCreator connectionCreator)
    {
        _connectionCreator = connectionCreator;
    }

    public async Task<OrderDto?> Find(long orderId, CancellationToken token)
    {
        await using NpgsqlConnection connection = await _connectionCreator.GetConnection();

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
        await using NpgsqlConnection connection = await _connectionCreator.GetConnection();

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
        await using NpgsqlConnection connection = await _connectionCreator.GetConnection();

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
        await using NpgsqlConnection connection = await _connectionCreator.GetConnection();

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
        const string SQL_START = @"
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
        ";

        List<string> whereConditions = new List<string>(2);
        if (regions != null && regions.Length > 0)
        {
            whereConditions.Add("address_region = ANY(@regions)");
        }
        if (sources != null && sources.Length > 0)
        {
            whereConditions.Add("order_source = ANY(@sources)");
        }

        StringBuilder sql = new(SQL_START);
        if (whereConditions.Count > 0)
        {
            sql.AppendLine("WHERE");
        }

        for (int i = 0; i < whereConditions.Count; i++)
        {
            if (i > 0)
            {
                sql.AppendLine("AND");
            }
            sql.AppendLine(whereConditions[i]);
        }

        sql.Append("ORDER BY region ");
        sql.AppendLine(orderingDirection.ToString());

        sql.AppendLine("LIMIT @limit OFFSET @offset");

        var queryArguments = new
        {
            regions,
            sources = sources?.Select(s => (int)s).ToArray(),
            offset = pageNumber * itemsPerPage,
            limit = itemsPerPage,
        };

        await using NpgsqlConnection connection = await _connectionCreator.GetConnection();
        IEnumerable<OrderDto> orders = await connection.QueryAsync<OrderDto, AddressDto, OrderDto>(
            sql.ToString(),
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
        const string SQL_START = @"
         SELECT
            address_region AS Region,
            SUM(price) AS TotalOrdersPrice,
            SUM(weight) AS TotalWeight,
            COUNT(*) AS OrdersCount,
            COUNT(DISTINCT customer_id) AS CustomersCount
        FROM
            (
            SELECT
                *
            FROM
                orders
            WHERE
                date_created >= @startDate
                ";

        StringBuilder sql = new(SQL_START);

        if (regions != null && regions.Length > 0)
        {
            sql.AppendLine("AND address_region = ANY(@regions)");
        }
        if (endDate.HasValue)
        {
            sql.AppendLine("AND date_created <= @endDate");
        }

        sql.AppendLine(@"
            ) T
        GROUP BY address_region
        ");

        var queryArguments = new
        {
            regions,
            startDate,
            endDate,
        };

        await using NpgsqlConnection connection = await _connectionCreator.GetConnection();
        IEnumerable<AggregateOrdersDto> orders =
            await connection.QueryAsync<AggregateOrdersDto>(sql.ToString(), queryArguments);

        return orders.ToArray();
    }
}