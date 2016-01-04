# Alan.Utils
dotNet utils library.

### Install

	Install-Package Alan.Utils 

## Features

### SQL Server Multiple Result Query
`Alan.Utils.Sql.SqlServerExtentions` 和	`Alan.Utils.Sql.DataTableSetExtensions` 提供了一些SQL Server数据库查询和DataTable转换的实用方法.
下面介绍一下将一次查询的多个集合映射到实体:

假设一个下订单的场景, 你需要先查找到用户, 然后再查找到产品和库存, 按照以前的做法需要对数据库进行三次查询. 下面是伪代码:

	bool PlaceOrder(int userId, int productId, int count)
    {
		//以前的写法, 比如使用Dapper.Net
        var user = connection.FindOne<User>("select * from Users where UserId = @id", new{ id = userId });
        if(user == null) return false;
        var product = connection.FindOne<Product>("select * from Products where ProductId = @id", new { id = productId} );
        if(product == null) return false;
        var warehouse = connection.FindOne<Warehouse>("select * from Warehouses where Product = @id" , new { id = productId });
        if(warehouse.Count < count) return false;

        
       //新的写法, 使用Alan.Utils.Sql
		var sql = @"
			select * from Users where UserId = @userId
			select * from Products where ProductId = @productId
			select * from Warehouse where ProductId = @productId
        ";
        var items = connection.ExQuery<User, Products, Warehouse>(sql, new { userId, productId });
        if(items.Item1.Count <= 0 || items.Item2.Count <= 0 || items.Item3.Count <=0 || items.Item4.Count <= count) return false;


		//....接下来的其他操作
    }


嗯, 两种写法看上去都很像 [Dapper.Net](https://github.com/StackExchange/dapper-dot-net), 利用了C#扩展方法. 第二种写法对数据库只做了一次查询操作, 然后返回多个强类型的结果集, 是不是感觉比第一种快捷了很多? [例子](https://github.com/Allen-Wei/Alan.Utils/blob/master/Alan.Utils.Examples/SqlServerExmple.cs)
