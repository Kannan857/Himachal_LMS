PM> Scaffold-DbContext "server=localhost;port=3306;user=root;password=!Password1;database=lms" MySql.Data.EntityFrameworkCore -OutputDir lms -f

Scafold only setlected tables

Scaffold-DbContext "server=localhost;port=3306;user=root;password=!Password1;database=lms" MySql.Data.EntityFrameworkCore -OutputDir lms -Tables user -f

https://dev.mysql.com/doc/connector-net/en/connector-net-entityframework-core-scaffold-example.html

Authenticaton & User Registartion : https://jasonwatmore.com/post/2018/06/26/aspnet-core-21-simple-api-for-authentication-registration-and-user-management


1. Super Admin cab create Super Admin
2. Verfication for Admin
3. Roles and Prev as MicroService
4. Holiday schedule for an year.
5. 