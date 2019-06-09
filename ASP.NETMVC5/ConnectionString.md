## Web.config

* Connection with trusted security
```xml
 <connectionStrings>
    <add connectionString="Data Source=BS-062;Initial Catalog=DbName;Integrated Security=True;MultipleActiveResultSets=true;" 
         name="DbConnection" providerName="System.Data.SqlClient"/>
</connectionStrings>
```
* Connection with standard security
```xml
 <connectionStrings>
   <add name="DbConnection" connectionString="Data Source=172.16.229.241;Initial Catalog=DbName;User ID=sa; Password=123456;MultipleActiveResultSets=true;" providerName="System.Data.SqlClient" />
</connectionStrings>
```
* Connection to a sqlserver instance
```xml
 <connectionStrings>
   <add name="DbConnection" connectionString="Data Source=myServerName\myInstanceName;Initial Catalog=DbName;User ID=sa; Password=123456;MultipleActiveResultSets=true;" providerName="System.Data.SqlClient" />
</connectionStrings>
```
* Default connection for localDb
```xml
<connectionStrings>
    <add name="DefaultConnection" connectionString="Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=DbName;Integrated Security=True;" providerName="System.Data.SqlClient" />
</connectionStrings>
```